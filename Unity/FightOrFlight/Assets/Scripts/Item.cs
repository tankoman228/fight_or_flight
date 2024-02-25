using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
             ���� ���������� �������� �� ����� �� ���������, ������� ���, ����� � �������
            ������ ���� ��� ����, �� ������ ���������� ��� ������ ��������������� �������
            (���� ���������� ����� ���, �� �� �������, ������ � ������ ���� ����� ������
            ���� �� ���������, ��� ����� ������ ����� ����� ������ �������). ���� ����� ����
            ������� ����������, ����� ������ ���������� ����!
             */
            itemID = (int)(
                itemStats.damage * itemStats.regarge_seconds * itemStats.start_ammo + 
                transform.position.x * transform.position.y + transform.position.x + transform.position.y);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
