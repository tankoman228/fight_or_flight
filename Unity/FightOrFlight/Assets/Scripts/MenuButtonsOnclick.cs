using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

/// <summary>
/// Для обработки нажатия кнопок из меню (ТОЛЬКО ИЗ МЕНЮ)
/// </summary>
public class MenuButtonsOnclick : MonoBehaviourPunCallbacks
{
    /// <summary>
    /// Создание или переход в новую комнату (автоматчмейкинг)
    /// </summary>
    public void OnCreateRoomClicked()
    {
        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = 8,
            IsVisible = true,
            IsOpen = true
        };

        PhotonNetwork.JoinOrCreateRoom("default_name", roomOptions, TypedLobby.Default, null);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Successfully joined room!");
        PhotonNetwork.LoadLevel("SceneGame"); // Загрузка сцены с игрой
    }
}
