using Assets.Scripts;
using UnityEngine;

/// <summary>
/// Объект, находится в префабе игрока, отвечает за оружие, атаку игрока других игроков и анимации
/// </summary>
public class Weapon : MonoBehaviour
{
    #region Свойства и поля


    public PlayerScript player; //Задан из редактора в префабе

    /// <summary>
    /// Задавать новое оружие при подборе с карты
    /// </summary>
    internal Item InventoryWeapon
    { //Ячейка с оружием
        set
        {
            Debug.Log($"player {player.view.Owner.ActorNumber} has picked {inventoryWeapon}");

            inventoryWeapon = value.itemStats;
            ammo = value.count;

            initByType(value);
            //Обработка для изменения спрайта оружия у игрока
            //...


        }
    }
    private ItemStats inventoryWeapon = ItemStats.ItemsStats[ItemStats.ItemTypes.pick]; //Тип оружия
    private int ammo; //Осталось патронов

    public ItemStats InventoryWeaponStats { get { return inventoryWeapon; } }
    public int Ammo { get { return ammo; } }

    public bool canAtack { get { return TimerCanAtackOnlyAfterZero < 0; } }
    private float TimerCanAtackOnlyAfterZero = 0;
    #endregion


    #region Общая логика

    //Таймер перезарядки
    private void Update()
    {
        TimerCanAtackOnlyAfterZero -= Time.deltaTime;
    }

    private void Start()
    {
        atackDelegate = new AtackDelegate(shoot);
    }

    /// <summary>
    /// Инициализация взятого оружия согласно характеристикам подобранного предмета
    /// </summary>
    /// <param name="type"></param>
    void initByType(Item item)
    {
        atackDelegate = new AtackDelegate(shoot);
        switch(item.itemType)
        {
            case ItemStats.ItemTypes.pick:      atackDelegate += pick_use;  break;
            case ItemStats.ItemTypes.chainsaw: break;
            case ItemStats.ItemTypes.pistol: break;
            case ItemStats.ItemTypes.reagents: break;
            case ItemStats.ItemTypes.machine_gun: break;
            case ItemStats.ItemTypes.flamethrower: break;
            case ItemStats.ItemTypes.sprayer: break;
            case ItemStats.ItemTypes.plasma_cutter: break;
            case ItemStats.ItemTypes.goo_absorber: break;
            case ItemStats.ItemTypes.bite: break;
            case ItemStats.ItemTypes.tongue: break;
        }
    }

    /// <summary>
    /// Логика стрельбы для всех типов оружия. Вызывается для атакующего игрока по глобальному событию.
    /// </summary>
    private void shoot()
    {
        TimerCanAtackOnlyAfterZero = inventoryWeapon.regarge_seconds;
        Debug.Log("ATACK!");
    }

    #endregion

    internal delegate void AtackDelegate();
    internal AtackDelegate atackDelegate; //Вызывается для атакующего игрока при соот-щем глобальном сообытии
    
    #region Функции, их задавать делегату для разных типов оружия при инициализации

    void pick_use()
    {
        Debug.Log("Pick used");
    }

    #endregion
}
