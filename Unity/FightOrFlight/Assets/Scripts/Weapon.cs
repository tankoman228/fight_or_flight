using Assets.Scripts;
using UnityEngine;
using UnityEngine.UI;

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
            if (player.view.IsMine) EventsManager.THIS.textCountWeapon.text = ammo.ToString();

            initByType(value.itemType);

            //��������� ��� ��������� ������� ������ � ������ (���������)
            if (player.view.IsMine)
                TextureLoadingManager.loadSprite(value.itemType, EventsManager.THIS.imageInv2);

            //��������� ������ (������)
            if (inventoryWeapon.spriteWeaponName != null)
            {
                try
                {
                    Debug.Log("weapon for sprite " + player.graphicsCurrent.name + player.view.ViewID);
                    player.graphicsCurrent.GetComponent<SpriteRenderer>().sprite =
                        TextureLoadingManager.spritesForCharacters[player.playerStats.rolename.Replace(" ", "")                    
                        + inventoryWeapon.spriteWeaponName];
                }
                catch
                {
                    Debug.LogError("ERROR weapon sprite not found " + player.playerStats.rolename.Replace(" ", "")
                        + inventoryWeapon.spriteWeaponName);
                }
            }
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
    public int Ammo { 
        set { ammo = value; if (player.view.IsMine) { EventsManager.THIS.textCountWeapon.text = value.ToString(); } }
        get { return ammo; } 
    }

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
        atackAnimationDelegate = new AtackDelegate(() => { });
    }

    /// <summary>
    /// ������������� ������� ������ �������� ��������������� ������������ ��������
    /// </summary>
    /// <param name="type"></param>
    void initByType(ItemStats.ItemTypes item)
    {
        Debug.Log("Init by type");
        atackDelegate = new AtackDelegate(()=>{ });
        atackAnimationDelegate = new AtackDelegate(()=>{ });
        switch(item)
        {
            case ItemStats.ItemTypes.pick:          
                atackDelegate += pick_use; atackAnimationDelegate += animate_pick_use; break;
            case ItemStats.ItemTypes.chainsaw:      
                atackDelegate += chainsaw_use; atackAnimationDelegate += animate_chainsaw_use; break;
            case ItemStats.ItemTypes.pistol:        
                atackDelegate += pistol_use; atackAnimationDelegate += animate_pistol_use; break;
            case ItemStats.ItemTypes.reagents:      
                atackDelegate += reagents_use; atackAnimationDelegate += animate_reagents_use; break;
            case ItemStats.ItemTypes.knife:         
                atackDelegate += knife_use; atackAnimationDelegate += animate_knife_use; break;
            case ItemStats.ItemTypes.machine_gun:   
                atackDelegate += machine_gun_use; atackAnimationDelegate += animate_machine_gun_use; break;
            case ItemStats.ItemTypes.flamethrower:  
                atackDelegate += flamethrower_use; atackAnimationDelegate += animate_flamethrower_use; break;
            case ItemStats.ItemTypes.sprayer:       
                atackDelegate += sprayer_use; atackAnimationDelegate += animate_sprayer_use; break;
            case ItemStats.ItemTypes.plasma_cutter: 
                atackDelegate += plasma_cutter_use; atackAnimationDelegate += animate_plasma_cutter_use; break;
            case ItemStats.ItemTypes.goo_absorber:  
                atackDelegate += goo_absorber_use; atackAnimationDelegate += animate_goo_absorber_use; break;
            case ItemStats.ItemTypes.bite:          
                atackDelegate += bite_use; atackAnimationDelegate += animate_bite_use; break;
            case ItemStats.ItemTypes.claws:         
                atackDelegate += claws_use; atackAnimationDelegate += animate_claws_use; break;
            case ItemStats.ItemTypes.tongue:        
                atackDelegate += tongue_use; atackAnimationDelegate += animate_tongue_use; break;
        }

        if (player.view.IsMine)
        {
            TextureLoadingManager.loadSprite(inventoryWeaponType, EventsManager.THIS.imageInv2);
        }
    }

    /// <summary>
    /// ������ �������� ��� ���� ����� ������. ���������� ��� ���������� ������ �� ����������� �������.
    /// </summary>
    public void shoot()
    {
        TimerCanAtackOnlyAfterZero = inventoryWeapon.regarge_seconds;
        Debug.Log("ATACK!");

        if (ammo > 0)
            Ammo--;
        else
        {
            //��� ������?
            return;
        }

        
        if (player == EventsManager.currentPlayer)
        {
            atackDelegate.Invoke();
        }
        atackAnimationDelegate.Invoke();
        
    }

    #endregion

    private delegate void AtackDelegate();
    private AtackDelegate atackDelegate, atackAnimationDelegate; //���������� ��� ���������� ������ ��� ����-��� ���������� ��������
    
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

    void pistol_use() {
        var hitbox = Instantiate(
                        DamageHitboxPrefub,
                        this.transform
                    ).GetComponent<DamageHitbox>();

        hitbox.transform.localScale = new Vector3(0.7f, 0.3f);

        // ��������� ������� � ����������� �������
        Vector3 bulletDirection = Vector3.up;

        hitbox.init(
            inventoryWeapon.damage_type,
            inventoryWeapon.damage,
            player.gameObject,
            !player.playerStats.IsMonster,
            0.4f);
        hitbox.init_as_bullet(41.3f);

        hitbox.transform.up = bulletDirection;
    }

    void reagents_use() {
        var hitbox = Instantiate(
                        DamageHitboxPrefub,
                        this.transform
                    ).GetComponent<DamageHitbox>();

        hitbox.transform.localScale = new Vector3(0.5f, 0.1f);

        // ��������� ������� � ����������� �������
        Vector3 bulletDirection = Vector3.up;

        hitbox.init(
            inventoryWeapon.damage_type,
            inventoryWeapon.damage,
            player.gameObject,
            !player.playerStats.IsMonster,
            0.8f);
        hitbox.init_as_bullet(31.3f);

        hitbox.transform.up = bulletDirection;
    }

    void machine_gun_use() {
        for (int i = 0; i < 5 && ammo > 0; i++, Ammo--)
        {
            float spreadAngle = Random.Range(-25f, 25f); // ���������� ��������� ���� ��������
            Quaternion spreadRotation = Quaternion.Euler(0f, 0f, spreadAngle); // ������� ���������� �������� ��� ���� ��������

            var hitbox = Instantiate(
                DamageHitboxPrefub,
                this.transform
            ).GetComponent<DamageHitbox>();

            hitbox.transform.localScale = new Vector3(0.5f, 0.1f);

            // ��������� ������� � ����������� �������
            Vector3 bulletDirection = spreadRotation * Vector3.up;

            hitbox.init(
                inventoryWeapon.damage_type,
                inventoryWeapon.damage,
                player.gameObject,
                !player.playerStats.IsMonster,
                0.4f);
            hitbox.init_as_bullet(51.3f);

            hitbox.transform.up = bulletDirection; // ������������� ����������� ������� � ������ ��������
        }       
    }

    void flamethrower_use() {

        for (int i = 0; i < 10; i++)
        {
            float spreadAngle = Random.Range(-13f, 13f); // ���������� ��������� ���� �������� �� -10 �� 10 ��������
            Quaternion spreadRotation = Quaternion.Euler(0f, 0f, spreadAngle); // ������� ���������� �������� ��� ���� ��������

            var hitbox = Instantiate(
                DamageHitboxPrefub,
                this.transform
            ).GetComponent<DamageHitbox>();

            hitbox.transform.localScale = new Vector3(0.5f, 0.1f);

            // ��������� ������� � ����������� �������
            Vector3 bulletDirection = spreadRotation * Vector3.up;

            hitbox.init(
                inventoryWeapon.damage_type,
                inventoryWeapon.damage,
                player.gameObject,
                !player.playerStats.IsMonster,
                0.5f);
            hitbox.init_as_bullet(18.3f);

            hitbox.transform.up = bulletDirection; // ������������� ����������� ������� � ������ ��������
        }
    }

    void sprayer_use() {
        for (int i = 0; i < 10; i++)
        {
            float spreadAngle = Random.Range(-13f, 13f); // ���������� ��������� ���� �������� �� -10 �� 10 ��������
            Quaternion spreadRotation = Quaternion.Euler(0f, 0f, spreadAngle); // ������� ���������� �������� ��� ���� ��������

            var hitbox = Instantiate(
                DamageHitboxPrefub,
                this.transform
            ).GetComponent<DamageHitbox>();

            hitbox.transform.localScale = new Vector3(0.5f, 0.1f);

            // ��������� ������� � ����������� �������
            Vector3 bulletDirection = spreadRotation * Vector3.up;

            hitbox.init(
                inventoryWeapon.damage_type,
                inventoryWeapon.damage,
                player.gameObject,
                !player.playerStats.IsMonster,
                0.5f);
            hitbox.init_as_bullet(12.3f);

            hitbox.transform.up = bulletDirection; // ������������� ����������� ������� � ������ ��������
        }
    }

    void plasma_cutter_use() {
        var hitbox = Instantiate(
                        DamageHitboxPrefub,
                        this.transform
                    ).GetComponent<DamageHitbox>();

        hitbox.transform.localScale = new Vector3(1.3f, 1.3f);
        // ��������� ������� � ����������� �������
        Vector3 bulletDirection = Vector3.up;

        hitbox.init(
            inventoryWeapon.damage_type,
            inventoryWeapon.damage,
            player.gameObject,
            !player.playerStats.IsMonster,
            0.7f);
        hitbox.init_as_bullet(51.3f);

        hitbox.transform.up = bulletDirection;
    }

    void goo_absorber_use() {
        var hitbox = Instantiate(
        DamageHitboxPrefub,
        this.transform
        ).GetComponent<DamageHitbox>();

        hitbox.transform.localScale = new Vector3(1f, 1f);
        hitbox.init(
            inventoryWeapon.damage_type,
            inventoryWeapon.damage,
            player.gameObject,
            !player.playerStats.IsMonster,
            0.2f);
        SoundManager.PlaySound(gameObject, "GoOo");
    }

    void bite_use() {
        var hitbox = Instantiate(
        DamageHitboxPrefub,
        this.transform
    ).GetComponent<DamageHitbox>();

        hitbox.transform.localScale = new Vector3(2, 2);
        hitbox.init(
            inventoryWeapon.damage_type,
            inventoryWeapon.damage,
            player.gameObject,
            !player.playerStats.IsMonster,
            0.3f);
        
    }
    void claws_use()
    {
        var hitbox = Instantiate(
        DamageHitboxPrefub,
        this.transform
    ).GetComponent<DamageHitbox>();

        hitbox.transform.localScale = new Vector3(1.3f, 1.3f);
        hitbox.init(
            inventoryWeapon.damage_type,
            inventoryWeapon.damage,
            player.gameObject,
            !player.playerStats.IsMonster,
            0.4f);
    }

    void tongue_use() {
        var hitbox = Instantiate(
        DamageHitboxPrefub,
        this.transform
        ).GetComponent<DamageHitbox>();

        hitbox.transform.localScale = new Vector3(4f, 0.2f);
        hitbox.init(
            inventoryWeapon.damage_type,
            inventoryWeapon.damage,
            player.gameObject,
            !player.playerStats.IsMonster,
            0.2f);
        
    }

    #endregion


    #region �������, �������� �������� ��� �������� ��� �����

    void animate_pick_use()
    {
        SoundManager.PlaySound(gameObject, "PickUse");
    }

    void animate_chainsaw_use()
    {
        SoundManager.PlaySound(gameObject, "Lazer");
    }

    void animate_knife_use()
    {
        SoundManager.PlaySound(gameObject, "PickUse");
    }

    void animate_pistol_use()
    {
        SoundManager.PlaySound(gameObject, "Shoot");
    }

    void animate_reagents_use()
    {
        SoundManager.PlaySound(gameObject, "Spray");
    }

    void animate_machine_gun_use()
    {
        SoundManager.PlaySound(gameObject, "ShootAk47");
    }

    void animate_flamethrower_use()
    {

    }

    void animate_sprayer_use()
    {
        SoundManager.PlaySound(gameObject, "Spray");
    }

    void animate_plasma_cutter_use()
    {
        SoundManager.PlaySound(gameObject, "lazer");
    }

    void animate_goo_absorber_use()
    {
        SoundManager.PlaySound(gameObject, "GoOo");
    }

    void animate_bite_use()
    {
        SoundManager.PlaySound(gameObject, "GoOo");
    }
    void animate_claws_use()
    {
        SoundManager.PlaySound(gameObject, "MouseAtack");
    }

    void animate_tongue_use()
    {
        SoundManager.PlaySound(gameObject, "WOA"); 
    }

    #endregion
}
