using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

/// <summary>
/// ��� ��������� ������� ������ �� ���� (������ �� ����)
/// </summary>
public class MenuButtonsOnclick : MonoBehaviourPunCallbacks
{

    public InputField inputField; //���� � ������ �������

    //��������� ���� ��� ������ �������
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
    /// �������� ��� ������� � ����� ������� (���������������)
    /// </summary>
    public void OnQuickGameClicked()
    {
        Debug.Log("btnOnclick");
        roomNumber = 0;
        TryJoinOrCreateRoom(); //������ ������� �����������
    }


    #region MatchMaking

    private int roomNumber;  //����� �������, � ������� ����� �������� ������������ � ������ ������
    private bool needRetryCreating;

    //������� ������������ � ������� � ������� ���������� �����, �� ��� ������ �������
    private void TryJoinOrCreateRoom()
    {
        needRetryCreating = true;
        string roomName = "Room_" + roomNumber;
        PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions,TypedLobby.Default, null);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        if (!needRetryCreating) //�� ��������� ������ ������ ������? ���, �� �����(
            return;

        //������� ������������ � ������ �������
        Debug.Log("OnCreateRoomFailed");
        if (roomNumber < 5)
        {
            roomNumber++;
            TryJoinOrCreateRoom();
        }
    }

    #endregion

    /// <summary>
    /// ����������� � ���������� �������, ��� ������� ��������� ������
    /// </summary>
    public void OnJoinConcreteRoom()
    {
        Debug.Log("btnOnclick");
        roomNumber = int.MaxValue;
        PhotonNetwork.JoinOrCreateRoom(inputField.text, roomOptions, TypedLobby.Default, null);
    }

    //������������ � �������? ��� �� ����� � �����! �� ���������, ����� �� ������� ������ - �������������� ������!
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
