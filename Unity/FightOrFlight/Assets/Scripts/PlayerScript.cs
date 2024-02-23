using Assets.Scripts;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    PhotonView view;
    FloatingJoystick joystick;

    public float current_health = 100;
    internal PlayerStats playerStats;

    void Start()
    {
        view = GetComponent<PhotonView>();
        joystick = FindFirstObjectByType<FloatingJoystick>();

        updateForCurrentPlayerClass = new UpdateForCurrentPlayerClass(empty_void);
    }

    void Update()
    {
        if (!view.IsMine)
            return;

        this.transform.position += new Vector3
            (joystick.Horizontal, joystick.Vertical, 0) * Time.deltaTime;

        updateForCurrentPlayerClass.Invoke();
    }

    #region Отличия логики игры для каждого класса

    /// <summary>
    /// Вызывается когда начался матч для каждого игрока. Игроку задаётся роль,
    /// команда, позиция на точке спавна, стартовый инвентарь.
    /// </summary>
    /// <param name="role">Тип игрока</param>
    /// <param name="new_position">Точка спавна</param>
    internal void InitMatchStarted(PlayerStats.PlayerStatsType role, Vector3 new_position)
    {
        playerStats = PlayerStats.Stats[role];
        this.transform.position = new_position;

        current_health = playerStats.max_health;

        switch(role)
        {
            case PlayerStats.PlayerStatsType.scientist:
                break;
            case PlayerStats.PlayerStatsType.miner:
                break;
            case PlayerStats.PlayerStatsType.guard:
                break;
            case PlayerStats.PlayerStatsType.enginier:
                break;

            case PlayerStats.PlayerStatsType.black_goo:
                break;
            case PlayerStats.PlayerStatsType.slither:
                break;
            case PlayerStats.PlayerStatsType.megarat:
                break;
            case PlayerStats.PlayerStatsType.hypnotoad:
                break;
            default:
                break;
        }
    }

    //Вызывается в Update
    delegate void UpdateForCurrentPlayerClass();
    UpdateForCurrentPlayerClass updateForCurrentPlayerClass;


    void empty_void() {}

    #endregion
}