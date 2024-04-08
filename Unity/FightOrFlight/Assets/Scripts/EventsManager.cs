using Assets.Scripts;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

/// <summary>
/// Накинут на пустышку (сцена с игрой). Использован для реализации связи игроков друг 
/// между другом, пользовательского интерфейса (UI) и глобальных событий Photon
/// Также хранит данные о карте, её состояние
/// Осторожно, спагетти код (я пока не умею правильно делать архитектуру таких проектов)
/// </summary>
public class EventsManager : MonoBehaviourPunCallbacks
{
    public static PlayerScript currentPlayer; //Задавать в OnCreate игрока для view.isMine
    public static EventsManager THIS; //Он всё равно 1 на карте
    float timerStartGameWaiter = float.MaxValue; //Таймер до старта игры
    bool game_awaiting = false; //Запущен ли таймер?
    public static bool generator_activated = false;
    public static bool generator_checkbox_overlapped = false;
    public static bool lift_checkbox_overlapped = false;
    internal static bool spectatorMode = false;

    //Прикрепить в редакторе
    public GameObject btnAtack, btnUse, btnInteract, btnStartGame, btnLeaveRoom, textEndgame;
    public Image imageInv1, imageInv2;
    public Text textStartOrCancel; //Из кнопки
    public GameObject generator;
    public SpriteRenderer generatorSprite;
    public Text textTimeOut;
    public Light2D lightGlobal;

    //Глобальные переменные
    public static int seed = -1;
    public static int people_escaped = 0;
    public Text textCountWeapon, textCountItem;

    //Игроки в текущей сессии (инициализируется только в момент начала игры)
    internal PlayerScript[] players;

    #region Пользовательский Интерфейс (UI)

    //Задание интерфейчу начального положения
    void Start()
    {
        //Обнуление переменных
        people_escaped = 0;
        game_awaiting = false;

        THIS = this;
        btnInteract.SetActive(false);
        textEndgame.SetActive(false);
        lightGlobal.color = Color.black;

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

    #region Spectator mode
    private int id_spectated = 0;
    private void switchSpectaded(int c)
    {
        int attempts = 0;

    findAnother:

        attempts++;
        id_spectated += c;
        if (id_spectated > players.Length)
            id_spectated = 0;
        if (id_spectated < 0)
            id_spectated = players.Length - 1;

        if (attempts > 8)
        {
            CameraFollow.target = generator.transform; return;
        }

        if (players[id_spectated].escaped || !players[id_spectated].isAlive)
        {
            goto findAnother;
        }
        else
        {
            CameraFollow.target = players[id_spectated].transform;
        }
    }
    #endregion

    //Нажатие на кнопку начала или отмены запуска таймера перед началом игры
    public void Onclick_btnStartGame()
    {
        SendPhotonEvent(EventCodes.TimerReset, timerStartGameWaiter >= 15);
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
        if (PlayerScript.selectedItem != null) //Подбор предмета с земли
        {
            var item = PlayerScript.selectedItem.GetComponent<Item>();
            int itemID = item.itemID;

            SendPhotonEvent(EventCodes.ItemFound, itemID);
        }
        if (generator_checkbox_overlapped && !generator_activated) //Активация генератора
        {
            SendPhotonEvent(EventCodes.GeneratorActivate, null);
            btnInteract.SetActive(false);
        }
        if (lift_checkbox_overlapped) //Человек сбегает
        {
            if (generator_activated)
            {
                SendPhotonEvent(EventCodes.HumanEscaped, null); 
                btnInteract.SetActive(false);
            }
        }
    }

    //Кнопка атаки (красная)
    public void Onclick_btnShoot()
    {
        if (spectatorMode)
        {
            switchSpectaded(1); return;
        }

        if (currentPlayer.weapon.canAtack)
            SendPhotonEvent(EventCodes.PlayerAtack, null);
    }

    //Кнопка использования предмета (зелёная)
    public void Onclick_btnUse()
    {
        if (spectatorMode)
        {
            switchSpectaded(-1); return;
        }

        if (currentPlayer.InventoryToolCount > 0)    
            SendPhotonEvent(EventCodes.InstrumentUsed, null);   
    }

    #endregion

    //Обновление таймера ожидания старта игры
    private void Update()
    {
        if (game_awaiting && timerStartGameWaiter <= 15)
        {
            textStartOrCancel.text = $"Cancel ({(int)(timerStartGameWaiter)})";
            timerStartGameWaiter -= Time.deltaTime;

            if (timerStartGameWaiter < 0)
            {
                Destroy(btnStartGame);
                Destroy(btnLeaveRoom);
                
                game_awaiting = false;
                SendPhotonEvent(EventCodes.GameStarted, new System.Random((int)(Time.time * 10000f * Time.deltaTime) + Time.frameCount).Next(0, int.MaxValue - 1));
            }          
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
        if (photonEvent.Code == EventCodes.TimerReset) //Timer reset
        {
            bool activate_timer = (bool)photonEvent.CustomData;
            if (activate_timer)
            {
                textStartOrCancel.text = "Cancel";
                timerStartGameWaiter = 15;
                game_awaiting = true;
                SoundManager.changeMusic("timer");
            }
            else
            {
                textStartOrCancel.text = "Start match";
                timerStartGameWaiter = float.MaxValue;
                game_awaiting = false;
                SoundManager.changeMusic("lobby");
            }
            return;
        }
        else if (photonEvent.Code == EventCodes.GameStarted) //Game started
        {
            seed = (int)photonEvent.CustomData;
            gameStartRolCall();
            PhotonNetwork.CurrentRoom.IsOpen = false;
            SoundManager.changeMusic("game");
            return;
        }
        else if (photonEvent.Code == EventCodes.ItemFound) //Item found
        {
            int id_item = (int)photonEvent.CustomData;

            //Поиск предмета с карты
            Item item = null;
            foreach (Item i in FindObjectsOfType<Item>())
            {
                //Debug.Log($"{item.itemID} == {id}");
                if (i.itemID == id_item)
                {
                    SoundManager.PlaySound(i.gameObject, "Took");
                    item = i;
                    break;
                }
            }

            //Поиск игрока, который поднял предмет
            foreach (var player in players)
            {
                if (player == null) continue;
                if (player.view.Owner.ActorNumber == photonEvent.Sender)
                {
                    //Запихиваем игроку в инвентарь новый предмет, старый оставляем на карте
                    if (item.itemStats.isWeapon)
                    {
                        Debug.Log($"Player {photonEvent.Sender} picked weapon {item.itemType}");

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
                        Debug.Log($"Player {photonEvent.Sender} picked item {item.itemType}");

                        //Запихиваем игроку в инвентарь новый предмет, старый оставляем на карте
                        var iss = player.InventoryTool;
                        var c = player.InventoryToolCount;
                        var t = player.InventoryToolType;

                        player.InventoryTool = item.itemStats;
                        player.InventoryToolType = item.itemType;
                        player.InventoryToolCount = item.count;

                        item.itemStats = iss;
                        item.count = c;
                        item.itemType = t;

                        //Для подбора нового предмета нужно снять бронник
                        if (player.armorUsedFlag)
                        {
                            player.resistanceMultiplyer /= 0.3f;
                            player.speedMultiplyer *= 2f;
                            player.armorUsedFlag = false;
                        }

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
            //Поиск игрока, который атаковал
            foreach (var player in players)
            {
                if (player == null) continue;
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

            foreach (var player in players)
            {
                if (player != null && player.view.Owner.ActorNumber == data.atacked_id)
                {
                    StartCoroutine(damageFlash(player));
                }
            }

            if (currentPlayer.view.Owner.ActorNumber != data.atacked_id)
                return;

            DamageManager.recieve_damage(data.damage_type, data.damage);
            return;
        }
        else if (photonEvent.Code == EventCodes.InstrumentUsed)
        {
            Debug.Log("Used instrument!");
            //Поиск игрока, который использовал предмет
            foreach (var player in players)
            {
                if (player == null) continue;
                if (player.view.Owner.ActorNumber == photonEvent.Sender)
                {
                    player.UseInstrument();
                    return;
                }
            }
        }
        else if (photonEvent.Code == EventCodes.GeneratorActivate)
        {
            generator_activated = true; Debug.Log("Generator activated!");
            generatorSprite.sprite = Resources.LoadAll<Sprite>("Objects").Where(x => x.name == "Generator").First();
            generatorSprite.color = Color.white;
            lightGlobal.color = Color.white;
            SoundManager.PlaySound(generator, "GeneratorOn");
        }
        else if (photonEvent.Code == EventCodes.HumanEscaped)
        {
            //Поиск игрока, который сбежал
            foreach (var player in players)
            {
                if (player == null) continue;
                if (player.view.Owner.ActorNumber == photonEvent.Sender)
                {
                    player.Escape();
                    people_escaped++;
                    THIS.check_game_status();
                    SoundManager.PlaySound(player.gameObject, "GeneratorOn");

                    return;
                }
            }
        }
        else if (photonEvent.Code == EventCodes.PlayerDied)
        {
            //Поиск игрока, который умер
            foreach (var player in players)
            {
                if (player == null) continue;
                if (player.view.Owner.ActorNumber == photonEvent.Sender)
                {
                    check_game_status();
                    if (!player.isAlive)
                        return;

                    player.isAlive = false;
                    player.transform.position = new Vector3(99, 999, 999);

                    if (player.playerStats.IsMonster)
                    {
                        SoundManager.PlaySound(player.gameObject, "GoOo");
                    }
                    else
                    {
                        SoundManager.PlaySound(player.gameObject, "Spray");
                    }

                    return;
                }
            }
        }
        Debug.LogWarning("No return statment used in OnEvent, maybe player or object not found");
        Debug.LogWarning("Error code " + photonEvent.Code);

    }
    internal static class EventCodes
    {
        internal const byte TimerReset = 0, GameStarted = 1, ItemFound = 2, PlayerAtack = 3;
        internal const byte PlayerAtacked = 4, InstrumentUsed = 5, GeneratorActivate = 6;
        internal const byte HumanEscaped = 7, PlayerDied = 8;
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
            generator_activated = false;
            people_escaped = 0;
            //Инициализация карты согласно выбранному ключу seed


            var allPlayers = FindObjectsOfType<PlayerScript>();
            Array.Sort(allPlayers, (x, y) => x.view.ViewID - y.view.ViewID);

            int kek = seed;
            for (int k = 0; k < allPlayers.Length; k++)
            {           
                for (int kk = k + 1; kk < allPlayers.Length; kk++)
                {
                    if (kek % 10 >= 5)
                    {
                        var tmp = allPlayers[kk];
                        allPlayers[kk] = allPlayers[k];
                        allPlayers[k] = tmp;
                    }
                }
                kek /= 10;
            }

            var allSpawnpoints = GameObject.FindGameObjectsWithTag("PlayerSpawnpoint");
            var allEnemySpawnpoints = GameObject.FindGameObjectsWithTag("EnemySpawnpoint");

            //Приоритеты ролей, если игроков меньше 8
            PlayerStats.PlayerStatsType[] roles;
            switch (seed / 1000 % 4)
            {
                case 0:
                    roles = new PlayerStats.PlayerStatsType[] {
                    PlayerStats.PlayerStatsType.guard,
                    PlayerStats.PlayerStatsType.megarat,
                    PlayerStats.PlayerStatsType.miner,
                    PlayerStats.PlayerStatsType.hypnotoad,
                    PlayerStats.PlayerStatsType.scientist,
                    PlayerStats.PlayerStatsType.slither,
                    PlayerStats.PlayerStatsType.enginier,
                    PlayerStats.PlayerStatsType.black_goo   //Черная слизь появится только при полном наборе игроков
                };
                    break;
                case 1:
                    roles = new PlayerStats.PlayerStatsType[] {
                    PlayerStats.PlayerStatsType.miner,      //Если человек 1, он всегда шахтёр
                    PlayerStats.PlayerStatsType.hypnotoad,
                    PlayerStats.PlayerStatsType.guard,
                    PlayerStats.PlayerStatsType.slither,
                    PlayerStats.PlayerStatsType.scientist,
                    PlayerStats.PlayerStatsType.megarat,
                    PlayerStats.PlayerStatsType.enginier,
                    PlayerStats.PlayerStatsType.black_goo   //Черная слизь появится только при полном наборе игроков
                };
                    break;
                case 2:
                    roles = new PlayerStats.PlayerStatsType[] {
                    PlayerStats.PlayerStatsType.guard,    //Если человек 1, он всегда шахтёр
                    PlayerStats.PlayerStatsType.hypnotoad,
                    PlayerStats.PlayerStatsType.miner,
                    PlayerStats.PlayerStatsType.slither,
                    PlayerStats.PlayerStatsType.scientist,
                    PlayerStats.PlayerStatsType.megarat,
                    PlayerStats.PlayerStatsType.enginier,
                    PlayerStats.PlayerStatsType.black_goo   //Черная слизь появится только при полном наборе игроков
                };
                    break;
                default:
                    roles = new PlayerStats.PlayerStatsType[] {
                    PlayerStats.PlayerStatsType.enginier,
                    PlayerStats.PlayerStatsType.black_goo,
                    PlayerStats.PlayerStatsType.slither,
                    PlayerStats.PlayerStatsType.guard,
                    PlayerStats.PlayerStatsType.megarat,            
                    PlayerStats.PlayerStatsType.miner,
                    PlayerStats.PlayerStatsType.hypnotoad,
                    PlayerStats.PlayerStatsType.scientist,
                    };
                    break;
            }

            //Выбор ролей игрокам, инициализация игроков, выбор стартовых позиций
            int i = 0;
            int position_id_start = ((int)seed / 29 - 300) % allSpawnpoints.Length;
            players = new PlayerScript[allPlayers.Length];
            foreach (var player in allPlayers)
            {
                if (!PlayerStats.Stats[roles[i]].IsMonster)
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
                players[i] = player;
                i++;
            }
      
            //Выбор предметов с карты, их перераспределение по возможным точкам появления
            var allItems = GameObject.FindGameObjectsWithTag("Item");
            var allItemsSpawnpoints = new List<GameObject> 
                (GameObject.FindGameObjectsWithTag("ItemSpawnpoint"));
            System.Random rand = new System.Random(seed);
            int j = 0;
            while(j < allItems.Length && allItemsSpawnpoints.Count > 0) 
            {
                //Пока список предметов не опустеет, кидаем на рандомную позицию,
                //Вычёркиваем из списка возможных использованную позицию
                i = rand.Next() % allItemsSpawnpoints.Count;
                allItems[j].transform.position = allItemsSpawnpoints[i].transform.position;      
                allItemsSpawnpoints.RemoveAt(i);
                j++;
            }

            //Выбор точек спавна для генератора с карты, установка генератора на позицию
            var allGeneratorSpawnpoints = new List<GameObject>
                (GameObject.FindGameObjectsWithTag("GeneratorSpawnpoint"));
            generator.transform.position = 
                allGeneratorSpawnpoints[seed / 1000 % allGeneratorSpawnpoints.Count].gameObject.transform.position;

            //Запуск таймера до окончания матча
            timeLeft = 360;
            textTimeOut.CrossFadeColor(Color.red, timeLeft, false, false);
            StartCoroutine(StartTimer());
            StartCoroutine(checkerGameStatus());
        }
    }

    private int timeLeft = 360;
    IEnumerator StartTimer()
    {
        while (timeLeft > 0)
        {
            yield return new WaitForSeconds(1);
            timeLeft--;

            textTimeOut.text = $"{timeLeft}s";
        }
        endgame(2);
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

                PhotonNetwork.Destroy(player.gameObject);
                check_game_status();

                return;
            }
        }
    }

    private int monsters_alive = 0, humans_notescaped = 0;
    /// <summary>
    /// Вызывается для проверки, закончена ли игра и если да, вызывается метод endgame
    /// </summary>
    internal void check_game_status()
    {
        humans_notescaped = 0;
        monsters_alive = 0;

        foreach (var player in players)
        {
            if (player != null && player.isAlive && !player.escaped)
            {
                if (player.playerStats.IsMonster)
                    monsters_alive++;
                else
                    humans_notescaped++;
            }
        }

        if (humans_notescaped > 0 && monsters_alive != 0)
            return;

        if (monsters_alive == 0)
            people_escaped += humans_notescaped; humans_notescaped = 0;

        endgame((float)monsters_alive / ((float)people_escaped + 0.0001f));
    }
    IEnumerator checkerGameStatus()
    {
        while (true)
        {
            yield return new WaitForSeconds(6);
            check_game_status();
        }
    }

    /// <summary>
    /// Должно быть вызвано по окончанию игры
    /// </summary>
    private void endgame(float score)
    {
        textEndgame.SetActive(true);
        string text;
        

        if (score <= 1.5)
        {
            text = $"Humans win";
            textEndgame.GetComponent<Text>().color = Color.green;
        }
        else if (score > 4.0)
        {
            text = $"Monsters win";
            textEndgame.GetComponent<Text>().color = Color.red;
        }
        else
        {
            text = $"Draw";
        }
        textEndgame.GetComponent<Text>().text = text;

        StartCoroutine(ExitRoomAfterDelay(5f));
    }
    private IEnumerator<WaitForSeconds> ExitRoomAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Выход из комнаты Photon
        PhotonNetwork.LeaveRoom();
    }
    private IEnumerator damageFlash(PlayerScript player)
    {
        for (int i = 0; i < 15; i++)
        {
            yield return new WaitForSeconds(0.075f);
            player.graphics.SetActive(i % 2 == 1);
        }
        player.graphics.SetActive(true);
    }

    #endregion
}
