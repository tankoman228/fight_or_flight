using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System;
using Assets.Scripts;

/// <summary>
/// Инициализация библиотеки "Фотон", подключение к серверу 
/// и выход с экрана загрузки при успешном подключении
/// </summary>
public class PhotonInit : MonoBehaviourPunCallbacks
{
    void Start()
    {
        //if (DateTime.Now.Ticks > DateTime.Parse("02.04.2024").Ticks)
        //{
         //   return;
       // }

        Debug.Log("Starting Loading");
        PhotonNetwork.OfflineMode = false;
        //PhotonNetwork.ConnectToRegion("ru");
        PhotonNetwork.ConnectUsingSettings();

        SoundManager.changeMusic("menu");
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Starting Connected!");
        SceneManager.LoadScene("SceneMenu");
    }
}
