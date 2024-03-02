using Assets.Scripts;
using UnityEngine;

/// <summary>
/// ������ �������� � �����, ������� ����� ��������� � ������������
/// </summary>
public class Item : MonoBehaviour
{
    public ItemStats itemStats;
    public ItemStats.ItemTypes itemType;

    public int itemID = -1; //MUST BE UNIQUE FOR EACH ITEM ON MAP
    public int count = 1;

    // Start is called before the first frame update
    void Start()
    {
        itemStats = ItemStats.ItemsStats[itemType];
        if (itemStats == null )
        {
            Debug.LogError("Item of null type!");
        }
        count = itemStats.start_ammo;

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

    /// <summary>
    /// ������ �������� ����� Start, ��������������� �������� ����� �����
    /// </summary>
    public void restart()
    {
        Start();
    }
}
