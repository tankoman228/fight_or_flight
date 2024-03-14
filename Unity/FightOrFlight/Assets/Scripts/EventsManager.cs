using Assets.Scripts;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ������� �� �������� (����� � �����). ����������� ��� ���������� ����� ������� ���� 
/// ����� ������, ����������������� ���������� (UI) � ���������� ������� Photon
/// ���������, �������� ��� (� ���� �� ���� ��������� ������ ����������� ����� ��������)
/// </summary>
public class EventsManager : MonoBehaviourPunCallbacks
{
    public static PlayerScript currentPlayer; //�������� � OnCreate ������ ��� view.isMine
    public static EventsManager THIS; //�� �� ����� 1 �� �����
    float timerStartGameWaiter = float.MaxValue; //������ �� ������ ����
    bool game_awaiting = false; //������� �� ������?

    //���������� � ���������
    public GameObject btnAtack, btnUse, btnInteract, btnStartGame, btnLeaveRoom, textEndgame;
    public Image imageInv1, imageInv2;
    public Text textStartOrCancel; //�� ������

    //���������� ����������
    public static int seed = -1;
    public static int monsters_total = 0, people_total = 0, people_escaped = 0;

    #region ���������������� ��������� (UI)

    //������� ���������� ���������� ���������
    void Start()
    {
        //��������� ����������
        monsters_total = 0; 
        people_total = 0; 
        people_escaped = 0;
        game_awaiting = false;

        THIS = this;
        btnInteract.SetActive(false);
        textEndgame.SetActive(false);

        try
        {
            Sprite sprite = Resources.Load<Sprite>("otval"); //�������� �������� �� ����� Resources

            imageInv1.sprite = sprite;
            imageInv2.sprite = sprite;
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    #region Onclick Listeners

    //������� �� ������ ������ ��� ������ ������� ������� ����� ������� ����
    public void Onclick_btnStartGame()
    {
        SendPhotonEvent(EventCodes.TimerReset, timerStartGameWaiter >= 5);
    }

    // ������ ������ �� �������
    public void Onckick_btnLeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    } 
    public override void OnLeftRoom()
    {
        PhotonNetwork.Disconnect();
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        PhotonNetwork.LoadLevel(0);
    }


    //������ �������������� (�����)
    public void Onclick_btnInteract()
    {
        if (PlayerScript.selectedItem != null)
        {
            var item = PlayerScript.selectedItem.GetComponent<Item>();
            int itemID = item.itemID;

            SendPhotonEvent(EventCodes.ItemFound, itemID);
        }
    }

    //������ ����� (�������)
    public void Onclick_btnShoot()
    {
        if (currentPlayer.weapon.canAtack)
            SendPhotonEvent(EventCodes.PlayerAtack, null);
    }

    //������ ������������� �������� (������)
    public void Onclick_btnUse()
    {
        if (currentPlayer.inventoryToolCount > 0)    
            SendPhotonEvent(EventCodes.InstrumentUsed, null);   
    }

    #endregion

    //���������� ������� �������� ������ ����
    private void Update()
    {
        if (game_awaiting && timerStartGameWaiter <= 5)
        {
            textStartOrCancel.text = $"Cancel ({(int)(timerStartGameWaiter * 5)})";

            if (timerStartGameWaiter < 0)
            {
                Destroy(btnStartGame);
                Destroy(btnLeaveRoom);
                
                game_awaiting = false;
                SendPhotonEvent(EventCodes.GameStarted, new System.Random().Next(0, int.MaxValue - 1));
            }
            timerStartGameWaiter -= Time.deltaTime;
        }
    }

    #endregion


    #region PhotonEventsHandler (������ � ���������)

    void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent; //������� ������ � ����������
        PhotonNetwork.AddCallbackTarget(this);
    }

    void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent; //������� ������ � ����������
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    /// <summary>
    /// �������� � �������� ����������� ������� ���� ������� � ��������������� ����� � �����������
    /// </summary>
    /// <param name="code"> ��� ������� </param>
    /// <param name="put"> ���������� ������� (���� �������) </param>
    public void SendPhotonEvent(byte code, object put)
    {
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(code, put, raiseEventOptions, SendOptions.SendReliable);
    }

    /// <summary>
    /// ����������, ����� ��������� ���������� �������. 
    /// �������� � ������� ������� ����� ������ SendPhotonEvent
    /// ��������� ���������� ������� ��������������� �� ����
    /// </summary>
    /// <param name="photonEvent"> ������ ������� </param>
    /// <exception cref="Exception"> ���������, ���� ����� �� ����� �� ������ </exception>
    void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == EventCodes.TimerReset) //Timer reset
        {
            bool activate_timer = (bool)photonEvent.CustomData;
            if (activate_timer)
            {
                textStartOrCancel.text = "Cancel";
                timerStartGameWaiter = 5;
                game_awaiting = true;
            }
            else
            {
                textStartOrCancel.text = "Start match";
                timerStartGameWaiter = float.MaxValue;
                game_awaiting = false;
            }
        }
        else if (photonEvent.Code == EventCodes.GameStarted) //Game started
        {
            seed = (int)photonEvent.CustomData;
            gameStartRolCall();
            PhotonNetwork.CurrentRoom.IsOpen = false;
        }
        else if (photonEvent.Code == EventCodes.ItemFound) //Item found
        {
            int id_item = (int)photonEvent.CustomData;

            //����� �������� � �����
            Item item = null;
            foreach (Item i in FindObjectsOfType<Item>())
            {
                //Debug.Log($"{item.itemID} == {id}");
                if (i.itemID == id_item)
                {
                    item = i;
                    break;
                }
            }

            //����� ������, ������� ������ �������
            foreach (var player in FindObjectsOfType<PlayerScript>())
            {
                if (player.view.Owner.ActorNumber == photonEvent.Sender)
                {
                    Debug.Log($"Player {photonEvent.Sender} picked {item.itemType}");


                    if (item.itemStats.isWeapon)
                    {
                        var iss = player.weapon.InventoryWeaponStats;
                        var c = player.weapon.Ammo;
                        var t = player.weapon.inventoryWeaponType;

                        player.weapon.InventoryWeapon = item;

                        item.itemType = t;
                        item.itemStats = iss;
                        item.count = c;
                        
                        item.restart();
                    }
                    else
                    {
                        var iss = player.InventoryTool;
                        var c = player.inventoryToolCount;
                        var t = player.InventoryToolType;

                        player.InventoryTool = item.itemStats;
                        player.InventoryToolType = item.itemType;
                        player.inventoryToolCount = item.count;

                        item.itemStats = iss;
                        item.count = c;
                        item.itemType = t;

                        item.restart();
                    }

                    //Destroy(item.gameObject);


                    return;
                }
            }

            throw new Exception("Player not found");
        }
        else if (photonEvent.Code == EventCodes.PlayerAtack)
        {
            //����� ������, ������� ��������
            foreach (var player in FindObjectsOfType<PlayerScript>())
            {
                if (player.view.Owner.ActorNumber == photonEvent.Sender)
                {
                    player.weapon.shoot();
                    //player.weapon.atackDelegate.Invoke();

                    return;
                }
            }
        }
        else if (photonEvent.Code == EventCodes.PlayerAtacked)
        {
            var data = DamageManager.DeserializeDamageMessage((byte[])photonEvent.CustomData);

            if (currentPlayer.view.Owner.ActorNumber != data.atacked_id)
                return;

            DamageManager.recieve_damage(data.damage_type, data.damage);
        }
        else if (photonEvent.Code == EventCodes.InstrumentUsed)
        {
            Debug.Log("Used instrumen!");
            //����� ������, ������� ����������� �������
            foreach (var player in FindObjectsOfType<PlayerScript>())
            {
                if (player.view.Owner.ActorNumber == photonEvent.Sender)
                {
                    player.UseInstrument();
                    return;
                }
            }
        }
        
    }
    internal static class EventCodes
    {
        internal const byte TimerReset = 0, GameStarted = 1, ItemFound = 2, PlayerAtack = 3;
        internal const byte PlayerAtacked = 4, InstrumentUsed = 5;
    }

    int playersSend = 0; //������, ����������� ����������� � ���, ��� � ��� ����������� ������

    /// <summary>
    /// ������������ � ��������� ������� ������ ���� (��� 1)
    /// ��������������� ����� ������� � ��������� ����������� ���������� seed
    /// �.�. �������� ������ �� ����������
    /// </summary>
    void gameStartRolCall()
    {
        playersSend++;
        if (playersSend >= PhotonNetwork.CurrentRoom.Players.Count)
        {
            //������������� ����� �������� ���������� ����� seed


            var allPlayers = FindObjectsOfType<PlayerScript>();
            Array.Sort(allPlayers, (x, y) => x.view.ViewID - y.view.ViewID);

            var allSpawnpoints = GameObject.FindGameObjectsWithTag("PlayerSpawnpoint");
            var allEnemySpawnpoints = GameObject.FindGameObjectsWithTag("EnemySpawnpoint");

            //���������� �����, ���� ������� ������ 8
            PlayerStats.PlayerStatsType[] roles = { 
                PlayerStats.PlayerStatsType.miner,      //���� ������� 1, �� ������ �����
                PlayerStats.PlayerStatsType.hypnotoad,
                PlayerStats.PlayerStatsType.guard,
                PlayerStats.PlayerStatsType.slither,
                PlayerStats.PlayerStatsType.scientist,
                PlayerStats.PlayerStatsType.megarat,
                PlayerStats.PlayerStatsType.enginier,
                PlayerStats.PlayerStatsType.black_goo   //������ ����� �������� ������ ��� ������ ������ �������
            };

            //����� ����� �������, ������������� �������, ����� ��������� �������
            int i = 0;
            int position_id_start = ((int)seed / 29 - 300) % allSpawnpoints.Length;
            foreach (var player in allPlayers)
            {
                if (i % 2 == 0)
                {
                    player.InitMatchStarted(roles[i],
                        allSpawnpoints[position_id_start].transform.position);
                    people_total++;
                }
                else
                {
                    player.InitMatchStarted(roles[i],
                        allEnemySpawnpoints[((seed / 22 - 255) + i) %
                        allEnemySpawnpoints.Length].transform.position);
                    monsters_total++;
                }
                i++;
            }

            //����� ��������� � �����, �� ����������������� �� ��������� ������ ���������
            var allItems = GameObject.FindGameObjectsWithTag("Item");
            var allItemsSpawnpoints = new List<GameObject> 
                (GameObject.FindGameObjectsWithTag("ItemSpawnpoint"));
            System.Random rand = new System.Random(seed);
            int j = 0;
            while(j < allItems.Length) 
            {
                //���� ������ ��������� �� ��������, ������ �� ��������� �������,
                //����������� �� ������ ��������� �������������� �������
                i = rand.Next() % allItemsSpawnpoints.Count;
                allItems[j].transform.position = allItemsSpawnpoints[i].transform.position;      
                allItemsSpawnpoints.RemoveAt(i);
                j++;
            }        
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log("Player left room");
        var players = GameObject.FindObjectsOfType<PlayerScript>();
        foreach (var player in players)
        {
            if (player.view.Owner == otherPlayer)
            {
                if (!player.isAlive)
                    return;

                if (player.playerStats.IsMonster)
                    monsters_total--;
                else
                    people_total--;

                return;
            }
        }
    }

    /// <summary>
    /// ���������� ��� ��������, ��������� �� ���� � ���� ��, ���������� ����� endgame
    /// </summary>
    internal void check_game_status()
    {
        if (monsters_total == 0)
            endgame();
        if (people_total == 0)
            endgame();
    }

    /// <summary>
    /// ������ ���� ������� �� ��������� ����
    /// </summary>
    private void endgame()
    {
        textEndgame.SetActive(true);
        string text;
        
        float score = (float)people_total / monsters_total;

        if (score >= 0.4)
        {
            text = $"Humans win. {monsters_total} monsters stayed alive. Escaped humans: {people_total}";
            textEndgame.GetComponent<Text>().color = Color.green;
        }
        else if (score <= 0.2)
        {
            text = $"Monsters win, their number is {monsters_total}. And {people_total} humans escaped";
            textEndgame.GetComponent<Text>().color = Color.red;
        }
        else
        {
            text = $"Draw. {people_total} humans escaped, but {monsters_total} monsters stayed alive";
        }
        textEndgame.GetComponent<Text>().text = text;
    }

    #endregion
}
