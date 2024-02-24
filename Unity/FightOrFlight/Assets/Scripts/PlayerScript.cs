using Assets.Scripts;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������ ������ (������ ������)
/// </summary>
public class PlayerScript : MonoBehaviour
{
    //�� getcomponent
    internal PhotonView view;
    FloatingJoystick joystick;
    Rigidbody2D rigidbody;

    //���� ������ ������
    public float current_health = 100;
    internal PlayerStats playerStats = new PlayerStats { };


    #region ������ �����

    void Start()
    {
        view = GetComponent<PhotonView>();
        joystick = FindFirstObjectByType<FloatingJoystick>();
        rigidbody = GetComponent<Rigidbody2D>();

        updateForCurrentPlayerClass = new UpdateForCurrentPlayerClass(empty_void);

        if (view.IsMine)
        {
            EventsManager.currentPlayer = this;
            CameraFollow.target = this.transform;
        }
    }

    void Update()
    {
        if (!view.IsMine)
            return;

        var v = new Vector2(joystick.Horizontal, joystick.Vertical);
        rigidbody.velocity = v * playerStats.speed;

        if (v != Vector2.zero)
        {
            float angle = Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        updateForCurrentPlayerClass.Invoke();
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Item"))
            EventsManager.THIS.btnInteract.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Item"))
            EventsManager.THIS.btnInteract.SetActive(false);
    }

    #endregion


    #region ������� ������ ���� ��� ������� ������

    /// <summary>
    /// ���������� ����� ������� ���� ��� ������� ������. ������ ������� ����,
    /// �������, ������� �� ����� ������, ��������� ���������.
    /// </summary>
    /// <param name="role">��� ������</param>
    /// <param name="new_position">����� ������</param>
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

    //���������� � Update
    delegate void UpdateForCurrentPlayerClass();
    UpdateForCurrentPlayerClass updateForCurrentPlayerClass;


    void empty_void() {}

    #endregion
}