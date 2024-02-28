using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using ExitGames.Client.Photon;
using Photon.Realtime;
using Assets.Scripts;

/// <summary>
/// Накинут на пустышку (сцена с игрой). Использован для реализации связи игроков друг 
/// между другом, пользовательского интерфейса и глобальных событий
/// 
/// </summary>
public class EventsManager : MonoBehaviour
{
    public static PlayerScript currentPlayer; //Задавать в OnCreate игрока для view.isMine
    public static EventsManager THIS;
    float timer = float.MaxValue;
    bool game_awaiting;

    //Прикрепить в редакторе
    public GameObject btnAtack, btnUse, btnInteract, btnStartGame;
    public Image imageInv1, imageInv2;
    public Text textStartOrCancel;

    //Глобальные переменные
    public static int seed = -1;

    #region Интерфейс

    //Задание интерфейчу начального положения
    void Start()
    {
        THIS = this;
        btnInteract.SetActive(false);

        try
        {
            Sprite sprite = Resources.Load<Sprite>("otval");

            imageInv1.sprite = sprite;
            imageInv2.sprite = sprite;
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    #region Onclick Listeners

    //Нажатие на кнопку начала или отмены запуска таймера перед началом игры
    public void Onclick_btnStartGame()
    {
        SendPhotonEvent(0, timer >= 5);
    }

    //Кнопка взаимодействия (синяя)
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

    //Обновление таймера
    private void Update()
    {
        if (game_awaiting && timer < 5)
        {
            textStartOrCancel.text = $"Cancel ({(int)(timer * 5)})";

            if (timer < 0)
            {
                btnStartGame.SetActive(false);
                btnStartGame.transform.position = new Vector3(9999,99999,999999);
                Destroy(btnStartGame);
                
                game_awaiting = false;
                SendPhotonEvent(1, new System.Random().Next(0, int.MaxValue - 1));
            }
        }
        timer -= Time.deltaTime;
    }

    #endregion

    #region PhotonEventsHandler (работа с СОБЫТИЯМИ)

    void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }

    void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }

    public void SendPhotonEvent(byte code, object put)
    {
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(code, put, raiseEventOptions, SendOptions.SendReliable);
    }

    void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == 0) //Timer reset
        {
            bool activate_timer = (bool)photonEvent.CustomData;
            if (activate_timer)
            {
                textStartOrCancel.text = "Cancel";
                timer = 5;
                game_awaiting = true;
            }
            else
            {
                textStartOrCancel.text = "Start match";
                timer = float.MaxValue;
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
      
            //Поиск предмета с карты
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

            //Поиск игрока, который поднял предмет
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

    int playersSend = 0;
    void gameStartRolCall()
    {
        playersSend++;
        if (playersSend >= PhotonNetwork.CurrentRoom.Players.Count)
        {
            //Инициализация карты согласно выбранному ключу seed


            var allPlayers = FindObjectsOfType<PlayerScript>();
            Array.Sort(allPlayers, (x, y) => x.view.ViewID - y.view.ViewID);

            var allSpawnpoints = GameObject.FindGameObjectsWithTag("PlayerSpawnpoint");
            var allEnemySpawnpoints = GameObject.FindGameObjectsWithTag("EnemySpawnpoint");

            PlayerStats.PlayerStatsType[] roles = { 
                PlayerStats.PlayerStatsType.miner,
                PlayerStats.PlayerStatsType.hypnotoad,
                PlayerStats.PlayerStatsType.guard,
                PlayerStats.PlayerStatsType.slither,
                PlayerStats.PlayerStatsType.scientist,
                PlayerStats.PlayerStatsType.megarat,
                PlayerStats.PlayerStatsType.enginier,
                PlayerStats.PlayerStatsType.black_goo
            };
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

            var allItems = GameObject.FindGameObjectsWithTag("Item");
            var allItemsSpawnpoints = new List<GameObject> 
                (GameObject.FindGameObjectsWithTag("ItemSpawnpoint"));

            System.Random rand = new System.Random(seed);
            int j = 0;

            while(j < allItems.Length)
            {
                i = rand.Next() % allItemsSpawnpoints.Count;
                allItems[j].transform.position = allItemsSpawnpoints[i].transform.position;      
                allItemsSpawnpoints.RemoveAt(i);
                j++;
            }
            
        }
    }

    #endregion
}
