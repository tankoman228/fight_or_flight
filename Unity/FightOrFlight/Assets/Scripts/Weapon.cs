using Assets.Scripts;
using UnityEngine;

/// <summary>
/// ������, ��������� � ������� ������, �������� �� ������, ����� ������ ������ ������� � ��������
/// </summary>
public class Weapon : MonoBehaviour
{
    #region �������� � ����


    public PlayerScript player; //����� �� ��������� � �������

    /// <summary>
    /// �������� ����� ������ ��� ������� � �����
    /// </summary>
    internal Item InventoryWeapon
    { //������ � �������
        set
        {
            Debug.Log($"player {player.view.Owner.ActorNumber} has picked {inventoryWeapon}");

            inventoryWeapon = value.itemStats;
            ammo = value.count;

            initByType(value);
            //��������� ��� ��������� ������� ������ � ������
            //...


        }
    }
    private ItemStats inventoryWeapon = ItemStats.ItemsStats[ItemStats.ItemTypes.pick]; //��� ������
    private int ammo; //�������� ��������

    public ItemStats InventoryWeaponStats { get { return inventoryWeapon; } }
    public int Ammo { get { return ammo; } }

    public bool canAtack { get { return TimerCanAtackOnlyAfterZero < 0; } }
    private float TimerCanAtackOnlyAfterZero = 0;
    #endregion


    #region ����� ������

    //������ �����������
    private void Update()
    {
        TimerCanAtackOnlyAfterZero -= Time.deltaTime;
    }

    private void Start()
    {
        atackDelegate = new AtackDelegate(shoot);
    }

    /// <summary>
    /// ������������� ������� ������ �������� ��������������� ������������ ��������
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
    /// ������ �������� ��� ���� ����� ������. ���������� ��� ���������� ������ �� ����������� �������.
    /// </summary>
    private void shoot()
    {
        TimerCanAtackOnlyAfterZero = inventoryWeapon.regarge_seconds;
        Debug.Log("ATACK!");
    }

    #endregion

    internal delegate void AtackDelegate();
    internal AtackDelegate atackDelegate; //���������� ��� ���������� ������ ��� ����-��� ���������� ��������
    
    #region �������, �� �������� �������� ��� ������ ����� ������ ��� �������������

    void pick_use()
    {
        Debug.Log("Pick used");
    }

    #endregion
}
