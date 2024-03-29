using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;
using System.ComponentModel;

/// <summary>
/// Создаёт хитбокс. Объект уничтожается и наносит урон при контакте с игроком
/// Указнной в init команде
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
    /// Инициализация после добавления хитбокса на сцену
    /// </summary>
    /// <param name="DamageType">Тип урона, наносимый игроком в данный момент</param>
    /// <param name="DamageSize">Размер урона</param>
    /// <param name="instantinatedBy">Атакующий игрок</param>
    /// <param name="monsterUnderAtack">Атакуемая команда</param>
    /// <param name="lifetime">Сколько секунд пройдёт перед уничтожением объекта</param>
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
    ///  Инициализация после добавления хитбокса на сцену как снаряда,
    ///  вызывать только совместно с обычным init
    /// </summary>
    /// <param name="speed">Скорость снаряда</param>
    internal void init_as_bullet(float speed)
    {
        Vector3 direction = instantinatedBy.transform.right;
        Quaternion rotation = Quaternion.LookRotation(direction);
        transform.rotation = rotation;
        IsBullet = true;
        bulletSpeed = speed;
    }

    /// <summary>
    /// Движение снаряда
    /// </summary>
    private void Update()
    {
        if (IsBullet)
            transform.Translate(instantinatedBy.transform.right * bulletSpeed * Time.deltaTime);
    }

    //Нанесение урона игроку из противоположной команды
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
