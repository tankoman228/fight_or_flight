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
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        SceneManager.LoadScene("SceneMenu");
    }
}
