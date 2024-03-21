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
