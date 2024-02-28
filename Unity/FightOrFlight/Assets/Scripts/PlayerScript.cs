using Assets.Scripts;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Скрипт игрока (именно логика)
/// </summary>
public class PlayerScript : MonoBehaviour
{
    //Из getcomponent
    internal PhotonView view;
    FloatingJoystick joystick;
    Rigidbody2D rigidbody;

    //Поля самого игрока
    public float Current_health { 
        
        get { return current_health; } 
        set {
            if (view.IsMine)
                GameObject.Find("tHealth").GetComponent<Text>().text = ((int)value).ToString();  
            
            current_health = value;
        } 
    }
    private float current_health;

    internal PlayerStats playerStats = new PlayerStats { };
    internal static GameObject selectedItem = null;
    internal static PlayerScript THIS;

    # region Инвентарь
    internal ItemStats InventoryTool { get; set; }  
    internal ItemStats InventoryWeapon {
        get { return inventoryWeapon; }
        set
        {
            //Обработка для изменения спрайта оружия у игрока
            //...

            Debug.Log($"player {view.Owner.ActorNumber} has picked {InventoryTool.damage}");
            inventoryWeapon = value;
        }
    }
    private ItemStats inventoryWeapon;
    internal int inventoryToolCount = 0;
    internal int inventoryWeaponCount = 0;
    #endregion

    #region Методы Юнити Start() Update() OnTrigger()

    void Start()
    {
        view = GetComponent<PhotonView>();
        joystick = FindFirstObjectByType<FloatingJoystick>();
        rigidbody = GetComponent<Rigidbody2D>();

        updateForCurrentPlayerClass = new UpdateForCurrentPlayerClass(empty_void);

        if (view.IsMine)
        {
            THIS = this; //продубрировал для удобства
            EventsManager.currentPlayer = this;
            CameraFollow.target = this.transform;

            GameObject.Find("tHealth").GetComponent<Text>().text = PhotonNetwork.CurrentRoom.Name;
        }
    }

    void Update()
    {
        if (!view.IsMine)
            return;

        var v = new Vector2(joystick.Horizontal, joystick.Vertical);
        rigidbody.velocity = v * playerStats.speed;

        if (v != Vector2.zero)
        {
            float angle = Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        updateForCurrentPlayerClass.Invoke();
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (view.IsMine && other.CompareTag("Item"))
        {
            EventsManager.THIS.btnInteract.SetActive(true);
            selectedItem = other.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (view.IsMine && other.CompareTag("Item"))
        {
            EventsManager.THIS.btnInteract.SetActive(false);
            selectedItem = null;
        }
    }

    #endregion


    #region Логика самой игры. Отличия логики игры для каждого класса

    /// <summary>
    /// Вызывается когда начался матч для каждого игрока. Игроку задаётся роль,
    /// команда, позиция на точке спавна, стартовый инвентарь.
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

        switch (role)
        {
            case PlayerStats.PlayerStatsType.scientist:
                break;
            case PlayerStats.PlayerStatsType.miner:
                break;
            case PlayerStats.PlayerStatsType.guard:
                break;
            case PlayerStats.PlayerStatsType.enginier:
                break;

            case PlayerStats.PlayerStatsType.black_goo:
                break;
            case PlayerStats.PlayerStatsType.slither:
                break;
            case PlayerStats.PlayerStatsType.megarat:
                break;
            case PlayerStats.PlayerStatsType.hypnotoad:
                break;
            default:
                break;
        }
    }

    //Вызывается в Update
    delegate void UpdateForCurrentPlayerClass();
    UpdateForCurrentPlayerClass updateForCurrentPlayerClass;


    void empty_void() {}


    #endregion


    #region вспомогательные функции

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