using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;

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

    /// <summary>
    /// ������������� ����� ���������� �������� �� �����
    /// </summary>
    /// <param name="DamageType">��� �����, ��������� ������� � ������ ������</param>
    /// <param name="DamageSize">������ �����</param>
    /// <param name="instantinatedBy">��������� �����</param>
    /// <param name="monsterUnderAtack">��������� �������</param>
    internal void init(DamageManager.DamageTypes DamageType, float DamageSize, GameObject instantinatedBy, bool monsterUnderAtack)
    {
        this.DamageType = DamageType;
        this.DamageSize = DamageSize;
        this.instantinatedBy = instantinatedBy;
        this.IsMonsterAtacked = monsterUnderAtack;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Player"))
        {
            if (collision.gameObject != instantinatedBy)
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
