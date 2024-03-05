using Assets.Scripts;
using Photon.Pun;
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

    #region Поля и свйоства

    //Здоровье игрока. Обновляет шкалу здоровья при изменении
    public float Current_health { 
        
        get { return current_health; } 
        set {
            if (view.IsMine)
                textHealth.text = ((int)value).ToString();  
            
            current_health = value;

            if (current_health < 0)
            {
                transform.position = Vector3.zero;
            }
        } 
    }
    private float current_health; //Текущее здоровье

    internal PlayerStats playerStats = PlayerStats.Stats[PlayerStats.PlayerStatsType.basic];
    internal static GameObject selectedItem = null; //Предмет, являющийся триггером, с которым возможно вз-вие
    internal static PlayerScript THIS; //Текущий игрок
    #endregion

    #region Инвентарь

    public Weapon weapon; //Скрипт оружия, которое держит в руках игрок
    internal ItemStats InventoryTool { get; set; }  //Ячейка с инструментом
    internal ItemStats.ItemTypes InventoryToolType { get; set; }

    internal int inventoryToolCount = 0; 
    #endregion

    #region Методы Юнити Start() Update() OnTrigger()

    //Инициализация полей
    void Start()
    {
        view = GetComponent<PhotonView>();
        joystick = FindFirstObjectByType<FloatingJoystick>();
        rigidbody = GetComponent<Rigidbody2D>();
        textHealth = GameObject.Find("tHealth").GetComponent<Text>();

        updateForCurrentPlayerClass = new UpdateForCurrentPlayerClass(() => { });

        if (view.IsMine)
        {
            THIS = this; //продубрировал для удобства
            EventsManager.currentPlayer = this;
            CameraFollow.target = this.transform;

            GameObject.Find("tHealth").GetComponent<Text>().text = PhotonNetwork.CurrentRoom.Name;
        }
    }

    //Управление, вращение спрайтом
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

        updateForCurrentPlayerClass.Invoke(); //Поведение и логика конкретного игрока
    }

    #region Триггеры
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (view.IsMine && other.CompareTag("Item"))
        {
            EventsManager.THIS.btnInteract.SetActive(true);
            selectedItem = other.gameObject; //Теперь можно подобрать предмет
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (view.IsMine && other.CompareTag("Item"))
        {
            EventsManager.THIS.btnInteract.SetActive(false);
            selectedItem = null; //А теперь нельзя, отошли уже от него
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


    /// <summary>
    /// Использование игроком инструмента из инвентаря, вся логика обработки
    /// </summary>
    internal void UseInstrument()
    {
        Debug.Log($"used {InventoryToolType}");

        inventoryToolCount--;
        Current_health += InventoryTool.health_add_after_used;

        //ППЛГОНД

        switch (InventoryToolType)
        {
            default:
                break;
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

        tbGuide.color = transparentColor;
    }


    #endregion
}