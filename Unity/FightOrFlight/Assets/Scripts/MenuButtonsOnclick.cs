using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

/// <summary>
/// ��� ��������� ������� ������ �� ���� (������ �� ����)
/// </summary>
public class MenuButtonsOnclick : MonoBehaviourPunCallbacks
{
    /// <summary>
    /// �������� ��� ������� � ����� ������� (���������������)
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
        PhotonNetwork.LoadLevel("SceneGame"); // �������� ����� � �����
    }
}
