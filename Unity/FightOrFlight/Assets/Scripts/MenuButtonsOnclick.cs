using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

/// <summary>
/// Для обработки нажатия кнопок из меню (ТОЛЬКО ИЗ МЕНЮ)
/// </summary>
public class MenuButtonsOnclick : MonoBehaviourPunCallbacks
{

    public InputField inputField; //Поле с именем комнаты

    //Настройки игры для каждой комнаты
    private RoomOptions roomOptions = new RoomOptions
    {
        MaxPlayers = 8,
        IsVisible = true,
        IsOpen = true
    };

    //
    public void Start()
    {
        roomNumber = 0;
        needRetryCreating = false;
        isJoinedRoom = false;
    }

    /// <summary>
    /// Создание или переход в новую комнату (автоматчмейкинг)
    /// </summary>
    public void OnQuickGameClicked()
    {
        Debug.Log("btnOnclick");
        roomNumber = 0;
        TryJoinOrCreateRoom(); //Запуск попыток подключения
    }


    #region MatchMaking

    private int roomNumber;  //Номер комнаты, к которой игрок пытается подключиться в данный момент
    private bool needRetryCreating;

    //Попытка подключиться к комнате с планами попытаться снова, но для другой комнаты
    private void TryJoinOrCreateRoom()
    {
        needRetryCreating = true;
        string roomName = "Room_" + roomNumber;
        PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions,TypedLobby.Default, null);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        if (!needRetryCreating) //Не планируем искать другую конату? Увы, не вышло(
            return;

        //Пытемся подключиться к другой комнате
        Debug.Log("OnCreateRoomFailed");
        if (roomNumber < 5)
        {
            roomNumber++;
            TryJoinOrCreateRoom();
        }
    }

    #endregion

    /// <summary>
    /// Подключение к конкретной комнате, имя которой указывают игроки
    /// </summary>
    public void OnJoinConcreteRoom()
    {
        Debug.Log("btnOnclick");
        roomNumber = int.MaxValue;
        PhotonNetwork.JoinOrCreateRoom(inputField.text, roomOptions, TypedLobby.Default, null);
    }

    //Подключились к комнате? Идём на сцену с игрой! Но осторожно, чтобы не перейти дважды - синхронизируем потоки!
    private static bool isJoinedRoom = false;
    private static object lockObject = new object();
    public override void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom(): " + PhotonNetwork.CurrentRoom.Name);
        lock (lockObject)
        {
            if (!isJoinedRoom)
            {
                isJoinedRoom = true;             
                PhotonNetwork.LoadLevel("SceneGame");
            }
        }
    }
    public override void OnJoinedLobby()
    {
        OnJoinedRoom();
    }
}
