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
/// Накинут на пустышку (сцена с игрой). Использован для реализации связи игроков друг 
/// между другом, пользовательского интерфейса (UI) и глобальных событий Photon
/// 
/// </summary>
public class EventsManager : MonoBehaviourPunCallbacks
{
    public static PlayerScript currentPlayer; //Задавать в OnCreate игрока для view.isMine
    public static EventsManager THIS; //Он всё равно 1 на карте
    float timerStartGameWaiter = float.MaxValue; //Таймер до старта игры
    bool game_awaiting = false; //Запущен ли таймер?

    //Прикрепить в редакторе
    public GameObject btnAtack, btnUse, btnInteract, btnStartGame, btnLeaveRoom;
    public Image imageInv1, imageInv2;
    public Text textStartOrCancel; //Из кнопки

    //Глобальные переменные
    public static int seed = -1;

    #region Пользовательский Интерфейс (UI)

    //Задание интерфейчу начального положения
    void Start()
    {
        THIS = this;
        btnInteract.SetActive(false);

        try
        {
            Sprite sprite = Resources.Load<Sprite>("otval"); //Загрузка еткстуры из папки Resources

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
        SendPhotonEvent(0, timerStartGameWaiter >= 5);
    }

    // Кнопка выхода из комнаты
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

    //Обновление таймера ожидания старта игры
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


    #region PhotonEventsHandler (работа с СОБЫТИЯМИ)

    void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent; //Простая работа с делегатами
        PhotonNetwork.AddCallbackTarget(this);
    }

    void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent; //Простая работа с делегатами
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    /// <summary>
    /// Отправка и создание глобального события всем игрокам с соответствующим кодом и содержанием
    /// </summary>
    /// <param name="code"> Код события </param>
    /// <param name="put"> Содержание события (тело запроса) </param>
    public void SendPhotonEvent(byte code, object put)
    {
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(code, put, raiseEventOptions, SendOptions.SendReliable);
    }

    /// <summary>
    /// Вызывается, когда произошло глобальное событие. 
    /// Появится у каждого клиента после вызова SendPhotonEvent
    /// Обработка глобальных событий соответстсвенно их коду
    /// </summary>
    /// <param name="photonEvent"> объект события </param>
    /// <exception cref="Exception"> произойдёт, если игрок на сцене не найден </exception>
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

    int playersSend = 0; //Игроки, отправившие уведомление о том, что у них остановился таймер
    /// <summary>
    /// Используется в обработке события начала игры (код 1)
    /// Перепроверяется число игроков и последний отправивший определяет seed
    /// З.Ы. Отдалённо похоже на перекличку
    /// </summary>
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

            //Приоритеты ролей, если игроков меньше 8
            PlayerStats.PlayerStatsType[] roles = { 
                PlayerStats.PlayerStatsType.miner,      //Если человек 1, он всегда шахтёр
                PlayerStats.PlayerStatsType.hypnotoad,
                PlayerStats.PlayerStatsType.guard,
                PlayerStats.PlayerStatsType.slither,
                PlayerStats.PlayerStatsType.scientist,
                PlayerStats.PlayerStatsType.megarat,
                PlayerStats.PlayerStatsType.enginier,
                PlayerStats.PlayerStatsType.black_goo   //Черная слизь появится только при полном наборе игроков
            };

            //Выбор ролей игрокам, инициализация игроков, выбор стартовых позиций
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

            //Выбор предметов с карты, их перераспределение по возможным точкам появления
            var allItems = GameObject.FindGameObjectsWithTag("Item");
            var allItemsSpawnpoints = new List<GameObject> 
                (GameObject.FindGameObjectsWithTag("ItemSpawnpoint"));
            System.Random rand = new System.Random(seed);
            int j = 0;
            while(j < allItems.Length) 
            {
                //Пока список предметов не опустеет, кидаем на рандомную позицию,
                //Вычёркиваем из списка возможных использованную позицию
                i = rand.Next() % allItemsSpawnpoints.Count;
                allItems[j].transform.position = allItemsSpawnpoints[i].transform.position;      
                allItemsSpawnpoints.RemoveAt(i);
                j++;
            }        
        }
    }

    #endregion
}
