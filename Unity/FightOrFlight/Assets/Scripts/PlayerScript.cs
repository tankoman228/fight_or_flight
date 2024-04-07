using Assets.Scripts;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Скрипт игрока (именно логика), поведение объекта игрока
/// </summary>
public class PlayerScript : MonoBehaviour
{
    //Из getcomponent
    internal PhotonView view;
    FloatingJoystick joystick;
    Rigidbody2D rigidbody;
    Text textHealth;
    internal Animator animator;

    //Из редактора
    public GameObject graphicsMiner, graphicsGuard, graphicsEnginier, graphicsNikita, graphicsCherv, graphicsMouse, graphicsFrog, graphicsGoo;
    public GameObject graphics;

    internal GameObject graphicsCurrent
    {
        get {
            switch (playerStats.rolename) {
                case "Miner": return graphicsMiner;
                case "Guard": return graphicsGuard;
                case "Scientist": return graphicsNikita;
                case "Enginier": return graphicsEnginier;
                case "Black goo": return graphicsGoo;
                case "Slither": return graphicsCherv;
                case "Megarat": return graphicsMouse;
                case "Hypnotoad": return graphicsFrog;
                default: return graphicsEnginier;
            }
        }
    }

    #region Поля и свйоства

    //Здоровье игрока. Обновляет шкалу здоровья при изменении
    public float Current_health { 
        
        get { return current_health; } 
        set {
            if (view.IsMine)
                textHealth.text = ((int)value).ToString();

            if (value <= playerStats.max_health)
                current_health = value;
            else
                current_health = playerStats.max_health;

            //Сюда прописать смерть игрока
            if (current_health <= 0 && view.IsMine)
            {
                EventsManager.THIS.SendPhotonEvent(EventsManager.EventCodes.PlayerDied, null);
                textHealth.text = "You are dead :(";
                EventsManager.spectatorMode = true;
                CameraFollow.target = EventsManager.THIS.generator.transform;
            }
        } 
    }
    private float current_health; //Текущее здоровье

    internal PlayerStats playerStats = PlayerStats.Stats[PlayerStats.PlayerStatsType.basic];
    internal bool isAlive = true;
    internal static GameObject selectedItem = null; //Предмет, являющийся триггером, с которым возможно вз-вие
    internal static PlayerScript THIS; //Текущий игрок
    internal bool escaped = false;

    //Значения, изменяющиеся в ходе игры (в результате использования предметов)
    internal float speedMultiplyer = 1, resistanceMultiplyer = 1;
    internal bool armorUsedFlag = false;

    #endregion

    #region Инвентарь

    public Weapon weapon; //Скрипт оружия, которое держит в руках игрок
    internal ItemStats InventoryTool { get; set; }  //Ячейка с инструментом
    public ItemStats.ItemTypes InventoryToolType { get { return inventoryToolType; } set
        {
            inventoryToolType = value;
            if (view.IsMine)
            {
                TextureLoadingManager.loadSprite(value, EventsManager.THIS.imageInv1);
                if (InventoryToolType == ItemStats.ItemTypes.armor)
                {
                    textHealth.text = $"{current_health} (use item to take armor on)";
                }
            }
        }}
    private ItemStats.ItemTypes inventoryToolType;

    public int InventoryToolCount
    {
        get { return inventoryToolCount; }
        set { 
            inventoryToolCount = value; 
            if (view.IsMine)
                EventsManager.THIS.textCountItem.text = value.ToString();
        }
    }
    private int inventoryToolCount = 0;
    #endregion

    #region Методы Юнити Start() Update() OnTrigger()

    //Инициализация полей
    void Start()
    {
        view = GetComponent<PhotonView>();
        joystick = FindFirstObjectByType<FloatingJoystick>();
        rigidbody = GetComponent<Rigidbody2D>();
        textHealth = GameObject.Find("tHealth").GetComponent<Text>();

        //Прячем графику
        graphicsNikita.SetActive(false);
        graphicsMiner.SetActive(false);
        graphicsGuard.SetActive(false);
        graphicsEnginier.SetActive(true);
        graphicsGoo.SetActive(false);
        graphicsCherv.SetActive(false);
        graphicsMouse.SetActive(false);
        graphicsFrog.SetActive(false);

        updateForCurrentPlayerClass = new UpdateForCurrentPlayerClass(() => { });

        if (view.IsMine)
        {
            THIS = this; //продубрировал для удобства
            EventsManager.currentPlayer = this;
            CameraFollow.target = this.transform;

            GameObject.Find("tHealth").GetComponent<Text>().text = PhotonNetwork.CurrentRoom.Name;
        }

        EventsManager.spectatorMode = false;
    }

    //Управление, вращение спрайтом
    void Update()
    {
        if (!view.IsMine)
            return;

        updateForCurrentPlayerClass.Invoke(); //Поведение и логика конкретного игрока

        var v = new Vector2(joystick.Horizontal, joystick.Vertical);
        rigidbody.velocity = v * playerStats.speed * speedMultiplyer;

        if (v != Vector2.zero)
        {
            float angle = Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            animator.SetBool("isWalk", true);
        }
        else
        {
            animator.SetBool("isWalk", false);
        }       
    }

    #region Триггеры
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (playerStats.IsMonster || !view.IsMine) //Монстры не могут подбирать предметы
            return;

        if (other.CompareTag("Item"))
        {
            EventsManager.THIS.btnInteract.SetActive(true);
            selectedItem = other.gameObject; //Теперь можно подобрать предмет
        }
        else if (other.CompareTag("GeneratorCheckbox") && !EventsManager.generator_activated)
        {
            EventsManager.THIS.btnInteract.SetActive(true);
            EventsManager.generator_checkbox_overlapped = true;
        }
        else if (other.CompareTag("LiftCheckbox"))
        {
            EventsManager.THIS.btnInteract.SetActive(true);
            EventsManager.lift_checkbox_overlapped = true;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (!view.IsMine)
            return;

        if (other.CompareTag("Item"))
        {
            EventsManager.THIS.btnInteract.SetActive(false);
            selectedItem = null; //А теперь нельзя, отошли уже от него
        }
        else if (other.CompareTag("GeneratorCheckbox"))
        {
            EventsManager.generator_checkbox_overlapped = false;
            EventsManager.THIS.btnInteract.SetActive(false);
        }
        else if (other.CompareTag("LiftCheckbox"))
        {
            EventsManager.THIS.btnInteract.SetActive(false);
            EventsManager.lift_checkbox_overlapped = false;
        }
    }
    #endregion

    #endregion


    #region Логика самой игры. Отличия логики игры для каждого класса

    /// <summary>
    /// Вызывается, когда начался матч, у каждого игрока. Игроку задаётся роль,
    /// команда, позиция на точке спавна, стартовый инвентарь, спрайт и т.п.
    /// </summary>
    /// <param name="role">Тип игрока</param>
    /// <param name="new_position">Точка спавна</param>
    internal void InitMatchStarted(PlayerStats.PlayerStatsType role, Vector3 new_position)
    {
        playerStats = PlayerStats.Stats[role];
        this.transform.position = new_position;

        Current_health = playerStats.max_health;

        if (view.IsMine)
            StartCoroutine(FadeOutText(10.0f));

        graphicsNikita.SetActive(false);
        graphicsMiner.SetActive(false);
        graphicsGuard.SetActive(false);
        graphicsEnginier.SetActive(false);

        graphicsGoo.SetActive(false);
        graphicsCherv.SetActive(false);
        graphicsMouse.SetActive(false);
        graphicsFrog.SetActive(false);

        switch (role)
        {
            case PlayerStats.PlayerStatsType.scientist:
                graphicsNikita.SetActive(true);
                animator = graphicsNikita.GetComponent<Animator>();
                InventoryToolType = ItemStats.ItemTypes.first_aid_kit;
                weapon.setWeaponOnPlayerInit(ItemStats.ItemTypes.reagents);

                break;
            case PlayerStats.PlayerStatsType.miner:
                graphicsMiner.SetActive(true);
                animator = graphicsMiner.GetComponent<Animator>();
                InventoryToolType = ItemStats.ItemTypes.dynamite;
                weapon.setWeaponOnPlayerInit(ItemStats.ItemTypes.pick);

                break;
            case PlayerStats.PlayerStatsType.guard:
                graphicsGuard.SetActive(true);
                animator = graphicsGuard.GetComponent<Animator>();
                InventoryToolType = ItemStats.ItemTypes.stimulant;
                weapon.setWeaponOnPlayerInit(ItemStats.ItemTypes.pistol);

                break;
            case PlayerStats.PlayerStatsType.enginier:
                graphicsEnginier.SetActive(true);
                animator = graphicsEnginier.GetComponent<Animator>();
                InventoryToolType = ItemStats.ItemTypes.dynamite;

                weapon.setWeaponOnPlayerInit(ItemStats.ItemTypes.knife);
                

                break;

            case PlayerStats.PlayerStatsType.black_goo:
                graphicsGoo.SetActive(true);
                InventoryToolType = ItemStats.ItemTypes.goo_imitator;
                weapon.setWeaponOnPlayerInit(ItemStats.ItemTypes.goo_absorber);

                updateForCurrentPlayerClass += gooImitating;
                SoundManager.PlaySound(gameObject, "Spray");

                break;
            case PlayerStats.PlayerStatsType.slither:
                graphicsCherv.SetActive(true);
                InventoryToolType = ItemStats.ItemTypes.walls_breaker;
                weapon.setWeaponOnPlayerInit(ItemStats.ItemTypes.bite);

                FindObjectOfType<SnakeHead>().InstantinateSegments();
                if (view.IsMine)
                {
                    textHealth.text = "∞";
                    EventsManager.THIS.btnUse.SetActive(false);
                }
                else
                {
                    rigidbody.simulated = false;
                }
                SoundManager.PlaySound(gameObject, "Boom");

                break;
            case PlayerStats.PlayerStatsType.megarat:
                graphicsMouse.SetActive(true);
                //InventoryToolType = ItemStats.ItemTypes.invisiblity_hat;
                if (view.IsMine)
                {
                    EventsManager.THIS.btnUse.SetActive(false);
                }
                animator = graphicsMouse.GetComponent<Animator>();
                weapon.setWeaponOnPlayerInit(ItemStats.ItemTypes.claws);
                SoundManager.PlaySound(gameObject, "MouseLoud");

                break;
            case PlayerStats.PlayerStatsType.hypnotoad:
                graphicsFrog.SetActive(true);
                animator = graphicsFrog.GetComponent<Animator>();
                InventoryToolType = ItemStats.ItemTypes.jump;
                weapon.setWeaponOnPlayerInit(ItemStats.ItemTypes.tongue);
                SoundManager.PlaySound(gameObject, "WOA");
                break;
            default:
                break;
        }
        InventoryTool = ItemStats.ItemsStats[inventoryToolType];
        InventoryToolCount = InventoryTool.start_ammo;
        if (role == PlayerStats.PlayerStatsType.enginier)
        {
            InventoryToolCount *= 10;
        }

        if (playerStats.IsMonster)
            updateForCurrentPlayerClass += regenerate_monsters;
    }

    //Вызывается в Update
    delegate void UpdateForCurrentPlayerClass();
    UpdateForCurrentPlayerClass updateForCurrentPlayerClass;

    //Вызовется у каждого монстра
    private void regenerate_monsters()
    {
        if (current_health < playerStats.max_health)
        {
            current_health += Time.deltaTime;
        }
        if (speedMultiplyer < 1)
        {
            speedMultiplyer += Time.deltaTime / 10;
        }
        else if (speedMultiplyer > 1)
        {
            speedMultiplyer -= Time.deltaTime * 10;
            if (speedMultiplyer < 1)
            {
                speedMultiplyer = 1;
            }
        }
    }

    //Для слизняка в режиме жыжи
    private void gooImitating()
    {
        if (!this.rigidbody.simulated)
        {
            Current_health -= Time.deltaTime * 55;
            this.transform.rotation = Quaternion.identity;
            var v = new Vector2(joystick.Horizontal, joystick.Vertical);
            transform.Translate(v * Time.deltaTime * 5);
        }
    }


    /// <summary>
    /// Использование игроком инструмента из инвентаря, вся логика обработки
    /// </summary>
    internal void UseInstrument()
    {
        Debug.Log($"used {inventoryToolType}");

        InventoryToolCount--;
        Current_health += InventoryTool.health_add_after_used;


        //ППЛГОНД
        switch (inventoryToolType)
        {
            case ItemStats.ItemTypes.mine:
            case ItemStats.ItemTypes.dynamite:

                if (view.IsMine)
                    PhotonNetwork.Instantiate("Bimb", transform.position, Quaternion.identity);

                break;
            case ItemStats.ItemTypes.armor:
                //Надеваем/снимаем бронник
                armorUsedFlag = !armorUsedFlag;
                if (armorUsedFlag)
                {
                    resistanceMultiplyer *= 0.3f;
                    speedMultiplyer /= 2f;
                    SoundManager.PlaySound(gameObject, "Spray");
                    if (view.IsMine)
                        textHealth.text = $"{current_health} (use item to take armor off)";
                }
                else
                {
                    resistanceMultiplyer /= 0.3f;
                    speedMultiplyer *= 2f;
                    SoundManager.PlaySound(gameObject, "Took");
                    if (view.IsMine)
                        textHealth.text = $"{current_health} (use item to take armor on)";
                }
                break;
            case ItemStats.ItemTypes.stimulant:
                speedMultiplyer *= 1.3f;
                resistanceMultiplyer *= 0.9f;
                SoundManager.PlaySound(gameObject, "PickUse");
                break;
            case ItemStats.ItemTypes.didgeridoo:
                foreach (var player in EventsManager.THIS.players)
                {
                    if (player.playerStats.IsMonster)
                        player.speedMultiplyer *= -1;
                    else
                    {
                        if (!view.IsMine)
                            player.Current_health += 4;
                    }
                }
                SoundManager.PlaySound(gameObject, "didgeridoo");
                break;
            case ItemStats.ItemTypes.invisiblity_hat:

                if (resistanceMultiplyer < 1000)
                {
                    resistanceMultiplyer += 9999; //Теперь игрока можно ваншотнуть
                    speedMultiplyer *= 0.5f; //А ещё он теперь черепаха
                    if (view.IsMine)
                        textHealth.text = "[You are invisible]";

                    graphics.SetActive(false);
                }
                else
                {
                    resistanceMultiplyer -= 9999; 
                    speedMultiplyer /= 0.5f;
                    if (view.IsMine)
                        textHealth.text = ((int)current_health).ToString();

                    graphics.SetActive(true);
                }
                SoundManager.PlaySound(gameObject, "GoOo");
                break;
            case ItemStats.ItemTypes.goo_imitator:

                this.rigidbody.simulated = !this.rigidbody.simulated;
                SoundManager.PlaySound(gameObject, "GoOo");

                break;
            case ItemStats.ItemTypes.walls_breaker:
                break;
            case ItemStats.ItemTypes.jump:

                if (/*view.IsMine &&*/ Time.time > timeJumpWait)
                {
                    animator.SetBool("isJump", true);
                    timeJumpWait = Time.time + 3;
                    speedMultiplyer = 15;
                    SoundManager.PlaySound(gameObject, "WOA");
                    StartCoroutine(stopJumping());
                }
                break;

            default:
                break;
        }
    }
    float timeJumpWait = 0;

    IEnumerator stopJumping()
    {
        yield return new WaitForSeconds(1.5f);
        animator.SetBool("isJump", false);
    }

    internal void Escape()
    {
        transform.position = new Vector3(999, 999, 999);
        escaped = true;
        if (view.IsMine)
        {
            textHealth.text = "Successfully escaped :)";

            EventsManager.spectatorMode = true;
            CameraFollow.target = EventsManager.THIS.generator.transform;
        }
    }

    #endregion


    #region вспомогательные функции

    //Анимация с текстом, где игроку пишет, кто он и что ему нужно делать
    IEnumerator FadeOutText(float fadeTime)
    {
        Text tbGuide = GameObject.Find("tbExplainWhatToDo").GetComponent<Text>();
        tbGuide.gameObject.transform.rotation = Quaternion.identity;
        tbGuide.text = $"You are {playerStats.rolename}\n{playerStats.guide}";

        Color originalColor = tbGuide.color;
        Color transparentColor = new Color(originalColor.r, originalColor.g, originalColor.b, 0);

        float elapsedTime = 0.0f;

        while (elapsedTime < fadeTime)
        {
            tbGuide.color = Color.Lerp(originalColor, transparentColor, elapsedTime / fadeTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        tbGuide.transform.position = new Vector3(999,999,999);
        tbGuide.color = transparentColor;
    }

    #endregion
}