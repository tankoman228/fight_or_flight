using Assets.Scripts;
using UnityEngine;

/// <summary>
/// —крипт предмета с карты, который можно подобрать и использовать
/// </summary>
public class Item : MonoBehaviour
{
    public ItemStats itemStats;
    public ItemStats.ItemTypes itemType;

    public int itemID = -1; //MUST BE UNIQUE FOR EACH ITEM ON MAP
    public int count = -1;

    // Start is called before the first frame update
    void Start()
    {
        itemStats = ItemStats.ItemsStats[itemType];

        if (count == -1)
            count = itemStats.start_ammo;

        if (itemStats == null || (!itemStats.isWeapon && count == 0))
        {
            Debug.LogError("Item of null type or null instruments!");
            Destroy(this.gameObject);
        }     

        if (itemID == -1)
        {
            /*
             ≈сли уникальный айдишник не задан из редактора, выбираю так, чтобы у каждого
            игрока айди был один, но выбран уникальным дл€ кажого псевдослучайным образом
            (шанс совпадени€ пипец мал, но не нулевой, потому в случае чего стоит задать
            айди из редактора, дл€ прода вообще лучше всего задать каждому). ≈сли вдруг айди
            окжетс€ одинаковый, будут ужасно непри€тные баги!
             */
            itemID = (int)(
                itemStats.damage * itemStats.regarge_seconds * itemStats.start_ammo + 
                transform.position.x * transform.position.y + transform.position.x + transform.position.y);
        }
    }

    /// <summary>
    /// «аново вызывает метод Start, реинициализаци€ согласно новым пол€м
    /// </summary>
    public void restart()
    {
        Start();
    }
}
