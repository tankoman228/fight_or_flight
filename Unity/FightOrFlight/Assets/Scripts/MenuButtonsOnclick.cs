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

    public InputField inputField;
    private bool fuck = false;


    //Настройки игры для каждой комнаты
    private RoomOptions roomOptions = new RoomOptions
    {
        MaxPlayers = 2,
        IsVisible = true,
        IsOpen = true
    };

    /// <summary>
    /// Создание или переход в новую комнату (автоматчмейкинг)
    /// </summary>
    public void OnQuickGameClicked()
    {
        Debug.Log("btnOnclick");
        roomNumber = 0;
        TryJoinOrCreateRoom();
    }


    #region MatchMaking

    private int roomNumber = 0;  //Номер комнаты, к которой игрок пытается подключиться в данный момент

    private void TryJoinOrCreateRoom()
    {
        fuck = true;
        string roomName = "Room_" + roomNumber;
        PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions,TypedLobby.Default, null);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        if (!fuck)
            return;

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


    private static bool isJoinedRoom = false;
    private static object lockObject = new object();
    public override void OnJoinedRoom()
    {
        lock (lockObject)
        {
            if (!isJoinedRoom)
            {
                isJoinedRoom = true;
                Debug.Log("OnJoinedRoom(): " + PhotonNetwork.CurrentRoom.Name);
                PhotonNetwork.LoadLevel("SceneGame");
            }
        }
    }
}
