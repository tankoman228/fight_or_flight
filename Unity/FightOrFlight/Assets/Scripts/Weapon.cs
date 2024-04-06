using Assets.Scripts;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Объект, находится в префабе игрока, отвечает за оружие, атаку игрока других игроков и анимации
/// </summary>
public class Weapon : MonoBehaviour
{
    #region Свойства и поля

    public PlayerScript player; //Задан из редактора в префабе
    public GameObject DamageHitboxPrefub;

    /// <summary>
    /// Задавать новое оружие при подборе с карты
    /// </summary>
    internal Item InventoryWeapon
    { //Ячейка с оружием
        set
        {
            Debug.Log($"player {player.view.Owner.ActorNumber} has picked {inventoryWeapon}");

            inventoryWeapon = value.itemStats;
            inventoryWeaponType = value.itemType;
            ammo = value.count;
            if (player.view.IsMine) EventsManager.THIS.textCountWeapon.text = ammo.ToString();

            initByType(value.itemType);

            //Обработка для изменения спрайта оружия у игрока (инвентарь)
            if (player.view.IsMine)
                TextureLoadingManager.loadSprite(value.itemType, EventsManager.THIS.imageInv2);

            //Изменение оружия (спрайт)
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
    private ItemStats inventoryWeapon = ItemStats.ItemsStats[ItemStats.ItemTypes.pick]; //Тип оружия
    private int ammo; //Осталось патронов

    internal ItemStats.ItemTypes inventoryWeaponType;
    public ItemStats InventoryWeaponStats { get { return inventoryWeapon; } }
    public int Ammo { 
        set { ammo = value; if (player.view.IsMine) { EventsManager.THIS.textCountWeapon.text = value.ToString(); } }
        get { return ammo; } 
    }

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
        atackDelegate = new AtackDelegate(() => { });
    }

    /// <summary>
    /// Инициализация взятого оружия согласно характеристикам подобранного предмета
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
            case ItemStats.ItemTypes.claws:         atackDelegate += claws_use; break;
            case ItemStats.ItemTypes.tongue:        atackDelegate += tongue_use; break;
        }

        if (player.view.IsMine)
        {
            TextureLoadingManager.loadSprite(inventoryWeaponType, EventsManager.THIS.imageInv2);
        }
    }

    /// <summary>
    /// Логика стрельбы для всех типов оружия. Вызывается для атакующего игрока по глобальному событию.
    /// </summary>
    public void shoot()
    {
        TimerCanAtackOnlyAfterZero = inventoryWeapon.regarge_seconds;
        Debug.Log("ATACK!");

        if (ammo > 0)
            Ammo--;
        else
        {
            //Нет патрон?
            return;
        }

        switch (inventoryWeaponType)
        {
            case ItemStats.ItemTypes.pick: 
                SoundManager.PlaySound(gameObject, "PickUse"); break;

            case ItemStats.ItemTypes.chainsaw: 
                SoundManager.PlaySound(gameObject, "Lazer"); break;

            case ItemStats.ItemTypes.pistol:
                SoundManager.PlaySound(gameObject, "Shoot"); break;

            case ItemStats.ItemTypes.reagents:
                SoundManager.PlaySound(gameObject, "Spray"); break;

            case ItemStats.ItemTypes.knife:
                SoundManager.PlaySound(gameObject, "PickUse"); break;

            case ItemStats.ItemTypes.machine_gun:
                SoundManager.PlaySound(gameObject, "ShootAk47"); break;

            case ItemStats.ItemTypes.flamethrower: break;

            case ItemStats.ItemTypes.sprayer:
                SoundManager.PlaySound(gameObject, "Spray"); break;

            case ItemStats.ItemTypes.plasma_cutter:
                SoundManager.PlaySound(gameObject, "lazer"); break;

            case ItemStats.ItemTypes.goo_absorber:
                SoundManager.PlaySound(gameObject, "GoOo"); break;

            case ItemStats.ItemTypes.bite:
                SoundManager.PlaySound(gameObject, "GoOo"); break;

            case ItemStats.ItemTypes.claws:
                SoundManager.PlaySound(gameObject, "MouseAtack"); break;

            case ItemStats.ItemTypes.tongue:
                SoundManager.PlaySound(gameObject, "WOA"); break;

        }
        if (player == EventsManager.currentPlayer)
        {
            atackDelegate.Invoke();
        }
        
    }

    #endregion

    private delegate void AtackDelegate();
    private AtackDelegate atackDelegate; //Вызывается для атакующего игрока при соот-щем глобальном сообытии
    
    #region Функции, их задавать делегату для разных типов оружия при инициализации

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

        // Применяем разброс к направлению снаряда
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

        // Применяем разброс к направлению снаряда
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
            float spreadAngle = Random.Range(-25f, 25f); // Генерируем случайный угол разброса
            Quaternion spreadRotation = Quaternion.Euler(0f, 0f, spreadAngle); // Создаем кватернион поворота для угла разброса

            var hitbox = Instantiate(
                DamageHitboxPrefub,
                this.transform
            ).GetComponent<DamageHitbox>();

            hitbox.transform.localScale = new Vector3(0.5f, 0.1f);

            // Применяем разброс к направлению снаряда
            Vector3 bulletDirection = spreadRotation * Vector3.up;

            hitbox.init(
                inventoryWeapon.damage_type,
                inventoryWeapon.damage,
                player.gameObject,
                !player.playerStats.IsMonster,
                0.4f);
            hitbox.init_as_bullet(51.3f);

            hitbox.transform.up = bulletDirection; // Устанавливаем направление снаряда с учетом разброса
        }       
    }

    void flamethrower_use() {

        for (int i = 0; i < 10; i++)
        {
            float spreadAngle = Random.Range(-13f, 13f); // Генерируем случайный угол разброса от -10 до 10 градусов
            Quaternion spreadRotation = Quaternion.Euler(0f, 0f, spreadAngle); // Создаем кватернион поворота для угла разброса

            var hitbox = Instantiate(
                DamageHitboxPrefub,
                this.transform
            ).GetComponent<DamageHitbox>();

            hitbox.transform.localScale = new Vector3(0.5f, 0.1f);

            // Применяем разброс к направлению снаряда
            Vector3 bulletDirection = spreadRotation * Vector3.up;

            hitbox.init(
                inventoryWeapon.damage_type,
                inventoryWeapon.damage,
                player.gameObject,
                !player.playerStats.IsMonster,
                0.5f);
            hitbox.init_as_bullet(18.3f);

            hitbox.transform.up = bulletDirection; // Устанавливаем направление снаряда с учетом разброса
        }
    }

    void sprayer_use() {
        for (int i = 0; i < 10; i++)
        {
            float spreadAngle = Random.Range(-13f, 13f); // Генерируем случайный угол разброса от -10 до 10 градусов
            Quaternion spreadRotation = Quaternion.Euler(0f, 0f, spreadAngle); // Создаем кватернион поворота для угла разброса

            var hitbox = Instantiate(
                DamageHitboxPrefub,
                this.transform
            ).GetComponent<DamageHitbox>();

            hitbox.transform.localScale = new Vector3(0.5f, 0.1f);

            // Применяем разброс к направлению снаряда
            Vector3 bulletDirection = spreadRotation * Vector3.up;

            hitbox.init(
                inventoryWeapon.damage_type,
                inventoryWeapon.damage,
                player.gameObject,
                !player.playerStats.IsMonster,
                0.5f);
            hitbox.init_as_bullet(12.3f);

            hitbox.transform.up = bulletDirection; // Устанавливаем направление снаряда с учетом разброса
        }
    }

    void plasma_cutter_use() {
        var hitbox = Instantiate(
                        DamageHitboxPrefub,
                        this.transform
                    ).GetComponent<DamageHitbox>();

        hitbox.transform.localScale = new Vector3(0.7f, 0.3f);
        // Применяем разброс к направлению снаряда
        Vector3 bulletDirection = Vector3.up;

        hitbox.init(
            inventoryWeapon.damage_type,
            inventoryWeapon.damage,
            player.gameObject,
            !player.playerStats.IsMonster,
            0.6f);
        hitbox.init_as_bullet(51.3f);

        hitbox.transform.up = bulletDirection;
    }

    void goo_absorber_use() {
        var hitbox = Instantiate(
        DamageHitboxPrefub,
        this.transform
        ).GetComponent<DamageHitbox>();

        hitbox.transform.localScale = new Vector3(2f, 2f);
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
            0.7f);
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
}
