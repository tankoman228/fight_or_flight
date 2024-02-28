using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// —крипт предмета с карты, который можно подобрать и использовать
/// </summary>
public class Item : MonoBehaviour
{
    public ItemStats itemStats;
    public ItemStats.ItemTypes itemType;

    public int itemID = -1; //MUST BE UNIQUE FOR EACH ITEM ON MAP
    
    // Start is called before the first frame update
    void Start()
    {
        itemStats = ItemStats.ItemsStats[itemType];

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
}
