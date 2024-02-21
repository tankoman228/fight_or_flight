using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

/// <summary>
/// Инициализация библиотеки "Фотон", подключение к серверу 
/// и выход с экрана загрузки при успешном подключении
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
