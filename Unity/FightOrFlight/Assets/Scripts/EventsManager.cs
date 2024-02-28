using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using ExitGames.Client.Photon;
using Photon.Realtime;
using Assets.Scripts;
using UnityEngine.SceneManagement;

/// <summary>
/// ������� �� �������� (����� � �����). ����������� ��� ���������� ����� ������� ���� 
/// ����� ������, ����������������� ���������� (UI) � ���������� ������� Photon
/// 
/// </summary>
public class EventsManager : MonoBehaviourPunCallbacks
{
    public static PlayerScript currentPlayer; //�������� � OnCreate ������ ��� view.isMine
    public static EventsManager THIS; //�� �� ����� 1 �� �����
    float timerStartGameWaiter = float.MaxValue; //������ �� ������ ����
    bool game_awaiting = false; //������� �� ������?

    //���������� � ���������
    public GameObject btnAtack, btnUse, btnInteract, btnStartGame, btnLeaveRoom;
    public Image imageInv1, imageInv2;
    public Text textStartOrCancel; //�� ������

    //���������� ����������
    public static int seed = -1;

    #region ���������������� ��������� (UI)

    //������� ���������� ���������� ���������
    void Start()
    {
        THIS = this;
        btnInteract.SetActive(false);

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
        SendPhotonEvent(0, timerStartGameWaiter >= 5);
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

            SendPhotonEvent(2, itemID);
        }
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
                SendPhotonEvent(1, new System.Random().Next(0, int.MaxValue - 1));
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
        if (photonEvent.Code == 0) //Timer reset
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
        else if (photonEvent.Code == 1) //Game started
        {
            seed = (int)photonEvent.CustomData;
            gameStartRolCall();
            PhotonNetwork.CurrentRoom.IsOpen = false;
        }
        else if (photonEvent.Code == 2) //Item found
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
                        player.InventoryWeapon = item.itemStats;
                    else
                        player.InventoryTool = item.itemStats;

                    Destroy(item.gameObject);

                    return;
                }
            }

            throw new Exception("Player not found");
        }
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
                }
                else
                {
                    player.InitMatchStarted(roles[i],
                        allEnemySpawnpoints[((seed / 22 - 255) + i) %
                        allEnemySpawnpoints.Length].transform.position);
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

    #endregion
}
