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
    public int count = -1;

    private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        itemStats = ItemStats.ItemsStats[itemType];

        if (count == -1)
            count = itemStats.start_ammo;
        if (count == 0)
            Destroy(this.gameObject);

        if (itemStats == null || (!itemStats.isWeapon && count == 0))
        {
            Debug.LogError("Item of null type or null instruments!");
            Destroy(this.gameObject);
        }     

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

        switch(itemType)
        {
            case ItemStats.ItemTypes.pick:
                //spriteRenderer.sprite = ...;
                break;
            default:
                break;
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
