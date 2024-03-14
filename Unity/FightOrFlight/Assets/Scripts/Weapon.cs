using Assets.Scripts;
using UnityEngine;

/// <summary>
/// ������, ��������� � ������� ������, �������� �� ������, ����� ������ ������ ������� � ��������
/// </summary>
public class Weapon : MonoBehaviour
{
    #region �������� � ����

    public PlayerScript player; //����� �� ��������� � �������
    public GameObject DamageHitboxPrefub;

    /// <summary>
    /// �������� ����� ������ ��� ������� � �����
    /// </summary>
    internal Item InventoryWeapon
    { //������ � �������
        set
        {
            Debug.Log($"player {player.view.Owner.ActorNumber} has picked {inventoryWeapon}");

            inventoryWeapon = value.itemStats;
            inventoryWeaponType = value.itemType;
            ammo = value.count;

            initByType(value.itemType);
            //��������� ��� ��������� ������� ������ � ������
            //...


        }
    }
    internal void setWeaponOnPlayerInit(ItemStats.ItemTypes itemType)
    {
        ItemStats itemStats = ItemStats.ItemsStats[itemType];

        inventoryWeapon = itemStats;
        inventoryWeaponType = itemType;
        ammo = itemStats.start_ammo;

        initByType(itemType);
    }
    private ItemStats inventoryWeapon = ItemStats.ItemsStats[ItemStats.ItemTypes.pick]; //��� ������
    private int ammo; //�������� ��������

    internal ItemStats.ItemTypes inventoryWeaponType;
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
        atackDelegate = new AtackDelegate(() => { });
    }

    /// <summary>
    /// ������������� ������� ������ �������� ��������������� ������������ ��������
    /// </summary>
    /// <param name="type"></param>
    void initByType(ItemStats.ItemTypes item)
    {
        Debug.Log("Init by type");
        atackDelegate = new AtackDelegate(()=>{ });
        switch(item)
        {
            case ItemStats.ItemTypes.pick:          atackDelegate += pick_use;  break;
            case ItemStats.ItemTypes.chainsaw:      atackDelegate += chainsaw_use; break;
            case ItemStats.ItemTypes.pistol:        atackDelegate += pistol_use; break;
            case ItemStats.ItemTypes.reagents:      atackDelegate += reagents_use; break;
            case ItemStats.ItemTypes.knife:         atackDelegate += knife_use; break;
            case ItemStats.ItemTypes.machine_gun:   atackDelegate += machine_gun_use; break;
            case ItemStats.ItemTypes.flamethrower:  atackDelegate += flamethrower_use; break;
            case ItemStats.ItemTypes.sprayer:       atackDelegate += sprayer_use; break;
            case ItemStats.ItemTypes.plasma_cutter: atackDelegate += plasma_cutter_use; break;
            case ItemStats.ItemTypes.goo_absorber:  atackDelegate += goo_absorber_use; break;
            case ItemStats.ItemTypes.bite:          atackDelegate += bite_use; break;
            case ItemStats.ItemTypes.tongue:        atackDelegate += tongue_use; break;
        }
    }

    /// <summary>
    /// ������ �������� ��� ���� ����� ������. ���������� ��� ���������� ������ �� ����������� �������.
    /// </summary>
    public void shoot()
    {
        TimerCanAtackOnlyAfterZero = inventoryWeapon.regarge_seconds;
        Debug.Log("ATACK!");

        if (player.weapon.ammo > 0)
            ammo--;
        else
        {
            //��� ������?
            return;
        }

        if (player == EventsManager.currentPlayer)
        {
            atackDelegate.Invoke();
        }
        
    }

    #endregion

    private delegate void AtackDelegate();
    private AtackDelegate atackDelegate; //���������� ��� ���������� ������ ��� ����-��� ���������� ��������
    
    #region �������, �� �������� �������� ��� ������ ����� ������ ��� �������������

    void pick_use()
    {
        Debug.Log("Pick Used");
        var hitbox = Instantiate(
                DamageHitboxPrefub,
                this.transform
                ).GetComponent<DamageHitbox>();
        hitbox.init(
            inventoryWeapon.damage_type,
            inventoryWeapon.damage,
            player.gameObject,
            !player.playerStats.IsMonster);
    }

    void chainsaw_use()
    {
        Debug.Log("Chainsaw Used");
        var hitbox = Instantiate(
                DamageHitboxPrefub,
                this.transform
                ).GetComponent<DamageHitbox>();
        hitbox.init(
            inventoryWeapon.damage_type,
            inventoryWeapon.damage,
            player.gameObject,
            !player.playerStats.IsMonster);
    }

    void knife_use()
    {
        chainsaw_use();
    }

    void pistol_use() { chainsaw_use(); }

    void reagents_use() { chainsaw_use(); }

    void machine_gun_use() { chainsaw_use(); }

    void flamethrower_use() { chainsaw_use(); }

    void sprayer_use() { chainsaw_use(); }

    void plasma_cutter_use() { chainsaw_use(); }

    void goo_absorber_use() { chainsaw_use(); }

    void bite_use() { chainsaw_use(); }

    void tongue_use() { chainsaw_use(); }

    #endregion
}
