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
