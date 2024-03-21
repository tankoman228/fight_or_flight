using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

/// <summary>
/// ������������� ���������� "�����", ����������� � ������� 
/// � ����� � ������ �������� ��� �������� �����������
/// </summary>
public class PhotonInit : MonoBehaviourPunCallbacks
{
    void Start()
    {
        Debug.Log("Starting Loading");
        PhotonNetwork.OfflineMode = false;
        //PhotonNetwork.ConnectToRegion("ru");
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Starting Connected!");
        SceneManager.LoadScene("SceneMenu");
    }
}
