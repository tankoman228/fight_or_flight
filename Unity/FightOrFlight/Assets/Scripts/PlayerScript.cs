using Assets.Scripts;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ������ ������ (������ ������), ��������� ������� ������
/// </summary>
public class PlayerScript : MonoBehaviour
{
    //�� getcomponent
    internal PhotonView view;
    FloatingJoystick joystick;
    Rigidbody2D rigidbody;
    Text textHealth;

    //�� ���������
    public GameObject graphicsMiner, graphicsGuard, graphicsEnginier, graphicsNikita, graphicsCherv, graphicsMouse, graphicsFrog, graphicsGoo;
    public GameObject graphics;

    #region ���� � ��������

    //�������� ������. ��������� ����� �������� ��� ���������
    public float Current_health { 
        
        get { return current_health; } 
        set {
            if (view.IsMine)
                textHealth.text = ((int)value).ToString();

            if (value <= playerStats.max_health)
                current_health = value;
            else
                current_health = playerStats.max_health;

            //���� ��������� ������ ������
            if (current_health <= 0 && view.IsMine)
            {
                EventsManager.THIS.SendPhotonEvent(EventsManager.EventCodes.PlayerDied, null);
                textHealth.text = "You are dead :(";
            }
        } 
    }
    private float current_health; //������� ��������

    internal PlayerStats playerStats = PlayerStats.Stats[PlayerStats.PlayerStatsType.basic];
    internal bool isAlive = true;
    internal static GameObject selectedItem = null; //�������, ���������� ���������, � ������� �������� ��-���
    internal static PlayerScript THIS; //������� �����
    internal bool escaped = false;

    //��������, ������������ � ���� ���� (� ���������� ������������� ���������)
    internal float speedMultiplyer = 1, resistanceMultiplyer = 1;
    internal bool armorUsedFlag = false;

    #endregion

    #region ���������

    public Weapon weapon; //������ ������, ������� ������ � ����� �����
    internal ItemStats InventoryTool { get; set; }  //������ � ������������
    public ItemStats.ItemTypes InventoryToolType { get { return inventoryToolType; } set
        {
            inventoryToolType = value;
            if (view.IsMine)
                TextureLoadingManager.loadSprite(value, EventsManager.THIS.imageInv1);
        }}
    private ItemStats.ItemTypes inventoryToolType;

    public int InventoryToolCount
    {
        get { return inventoryToolCount; }
        set { 
            inventoryToolCount = value; 
            EventsManager.THIS.textCountItem.text = value.ToString();
        }
    }
    private int inventoryToolCount = 0;
    #endregion

    #region ������ ����� Start() Update() OnTrigger()

    //������������� �����
    void Start()
    {
        view = GetComponent<PhotonView>();
        joystick = FindFirstObjectByType<FloatingJoystick>();
        rigidbody = GetComponent<Rigidbody2D>();
        textHealth = GameObject.Find("tHealth").GetComponent<Text>();

        //������ �������
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
            THIS = this; //������������� ��� ��������
            EventsManager.currentPlayer = this;
            CameraFollow.target = this.transform;

            GameObject.Find("tHealth").GetComponent<Text>().text = PhotonNetwork.CurrentRoom.Name;
        }
    }

    //����������, �������� ��������
    void Update()
    {
        if (!view.IsMine)
            return;

        var v = new Vector2(joystick.Horizontal, joystick.Vertical);
        rigidbody.velocity = v * playerStats.speed * speedMultiplyer;

        if (v != Vector2.zero)
        {
            float angle = Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        updateForCurrentPlayerClass.Invoke(); //��������� � ������ ����������� ������
    }

    #region ��������
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (playerStats.IsMonster || !view.IsMine) //������� �� ����� ��������� ��������
            return;

        if (other.CompareTag("Item"))
        {
            EventsManager.THIS.btnInteract.SetActive(true);
            selectedItem = other.gameObject; //������ ����� ��������� �������
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
            selectedItem = null; //� ������ ������, ������ ��� �� ����
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


    #region ������ ����� ����. ������� ������ ���� ��� ������� ������

    /// <summary>
    /// ����������, ����� ������� ����, � ������� ������. ������ ������� ����,
    /// �������, ������� �� ����� ������, ��������� ���������, ������ � �.�.
    /// </summary>
    /// <param name="role">��� ������</param>
    /// <param name="new_position">����� ������</param>
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
                InventoryToolType = ItemStats.ItemTypes.first_aid_kit;
                weapon.setWeaponOnPlayerInit(ItemStats.ItemTypes.reagents);

                break;
            case PlayerStats.PlayerStatsType.miner:
                graphicsMiner.SetActive(true);
                InventoryToolType = ItemStats.ItemTypes.dynamite;
                weapon.setWeaponOnPlayerInit(ItemStats.ItemTypes.pick);

                break;
            case PlayerStats.PlayerStatsType.guard:
                graphicsGuard.SetActive(true);
                InventoryToolType = ItemStats.ItemTypes.stimulant;
                weapon.setWeaponOnPlayerInit(ItemStats.ItemTypes.pistol);

                break;
            case PlayerStats.PlayerStatsType.enginier:
                graphicsEnginier.SetActive(true);
                InventoryToolType = ItemStats.ItemTypes.knife;
                weapon.setWeaponOnPlayerInit(ItemStats.ItemTypes.mine);

                break;

            case PlayerStats.PlayerStatsType.black_goo:
                graphicsGoo.SetActive(true);
                InventoryToolType = ItemStats.ItemTypes.goo_imitator;
                weapon.setWeaponOnPlayerInit(ItemStats.ItemTypes.goo_absorber);

                break;
            case PlayerStats.PlayerStatsType.slither:
                graphicsCherv.SetActive(true);
                InventoryToolType = ItemStats.ItemTypes.walls_breaker;
                weapon.setWeaponOnPlayerInit(ItemStats.ItemTypes.bite);

                break;
            case PlayerStats.PlayerStatsType.megarat:
                graphicsMouse.SetActive(true);
                InventoryToolType = ItemStats.ItemTypes.jump;
                weapon.setWeaponOnPlayerInit(ItemStats.ItemTypes.claws);

                break;
            case PlayerStats.PlayerStatsType.hypnotoad:
                graphicsFrog.SetActive(true);
                InventoryToolType = ItemStats.ItemTypes.jump;
                weapon.setWeaponOnPlayerInit(ItemStats.ItemTypes.tongue);
                break;
            default:
                break;
        }
        InventoryTool = ItemStats.ItemsStats[inventoryToolType];
        InventoryToolCount = InventoryTool.start_ammo;

        if (playerStats.IsMonster)
            updateForCurrentPlayerClass += regenerate_monsters;
    }

    //���������� � Update
    delegate void UpdateForCurrentPlayerClass();
    UpdateForCurrentPlayerClass updateForCurrentPlayerClass;

    //��������� � ������� �������
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
    }


    /// <summary>
    /// ������������� ������� ����������� �� ���������, ��� ������ ���������
    /// </summary>
    internal void UseInstrument()
    {
        Debug.Log($"used {inventoryToolType}");

        InventoryToolCount--;
        Current_health += InventoryTool.health_add_after_used;

        //�������

        switch (inventoryToolType)
        {
            case ItemStats.ItemTypes.mine:
                break;
            case ItemStats.ItemTypes.armor:
                //��������/������� �������
                armorUsedFlag = !armorUsedFlag;
                if (armorUsedFlag)
                {
                    resistanceMultiplyer *= 0.5f;
                    speedMultiplyer /= 1.2f;
                }
                else
                {
                    resistanceMultiplyer /= 0.5f;
                    speedMultiplyer *= 1.2f;
                }
                break;
            case ItemStats.ItemTypes.stimulant:
                speedMultiplyer *= 1.3f;
                resistanceMultiplyer *= 0.9f;
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
                break;
            case ItemStats.ItemTypes.invisiblity_hat:

                if (resistanceMultiplyer < 1000)
                {
                    resistanceMultiplyer += 9999; //������ ������ ����� ����������
                    speedMultiplyer *= 0.05f; //� ��� �� ������ ��������
                    textHealth.text = "[You are invisible]";

                    graphics.SetActive(false);
                }
                else
                {
                    resistanceMultiplyer -= 9999; 
                    speedMultiplyer /= 0.05f;
                    textHealth.text = ((int)current_health).ToString();

                    graphics.SetActive(true);
                }
                break;
            case ItemStats.ItemTypes.goo_imitator:
                break;
            case ItemStats.ItemTypes.walls_breaker:
                break;
            case ItemStats.ItemTypes.jump:

                if (view.IsMine)
                    rigidbody.velocity *= 50;
                break;

            default:
                break;
        }
    }

    internal void Escape()
    {
        transform.position = new Vector3(999, 999, 999);
        escaped = true;
        if (view.IsMine)
            textHealth.text = "Successfully escaped :)";
    }

    #endregion


    #region ��������������� �������

    //�������� � �������, ��� ������ �����, ��� �� � ��� ��� ����� ������
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

        tbGuide.color = transparentColor;
    }

    #endregion
}