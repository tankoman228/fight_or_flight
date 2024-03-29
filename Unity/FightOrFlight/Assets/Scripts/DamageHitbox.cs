using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;
using System.ComponentModel;

/// <summary>
/// ������ �������. ������ ������������ � ������� ���� ��� �������� � �������
/// �������� � init �������
/// </summary>
public class DamageHitbox : MonoBehaviour
{
    public DamageManager.DamageTypes DamageType;
    public float DamageSize;
    public GameObject instantinatedBy; 
    public bool IsMonsterAtacked;
    public bool IsBullet = false;
    public float bulletSpeed = 0;

    /// <summary>
    /// ������������� ����� ���������� �������� �� �����
    /// </summary>
    /// <param name="DamageType">��� �����, ��������� ������� � ������ ������</param>
    /// <param name="DamageSize">������ �����</param>
    /// <param name="instantinatedBy">��������� �����</param>
    /// <param name="monsterUnderAtack">��������� �������</param>
    /// <param name="lifetime">������� ������ ������ ����� ������������ �������</param>
    internal void init(DamageManager.DamageTypes DamageType, float DamageSize, GameObject instantinatedBy, bool monsterUnderAtack,
        float lifetime = 1)
    {
        this.DamageType = DamageType;
        this.DamageSize = DamageSize;
        this.instantinatedBy = instantinatedBy;
        this.IsMonsterAtacked = monsterUnderAtack;

        Destroy(this.gameObject, lifetime);
    }

    /// <summary>
    ///  ������������� ����� ���������� �������� �� ����� ��� �������,
    ///  �������� ������ ��������� � ������� init
    /// </summary>
    /// <param name="speed">�������� �������</param>
    internal void init_as_bullet(float speed)
    {
        Vector3 direction = instantinatedBy.transform.right;
        Quaternion rotation = Quaternion.LookRotation(direction);
        transform.rotation = rotation;
        IsBullet = true;
        bulletSpeed = speed;
    }

    /// <summary>
    /// �������� �������
    /// </summary>
    private void Update()
    {
        if (IsBullet)
            transform.Translate(instantinatedBy.transform.right * bulletSpeed * Time.deltaTime);
    }

    //��������� ����� ������ �� ��������������� �������
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject != instantinatedBy)
        {
            if (collision.gameObject.tag.Equals("Player"))
            {
                Debug.Log("Damage hitbox touched!");

                var player = collision.gameObject.GetComponent<PlayerScript>();

                if (player.playerStats.IsMonster == IsMonsterAtacked)
                {
                    Debug.Log("damaged other player!");
                    DamageManager.apply_damage_to_player(player, DamageType, DamageSize);
                    Destroy(this.gameObject);
                }              
            }
        }
    }
}
