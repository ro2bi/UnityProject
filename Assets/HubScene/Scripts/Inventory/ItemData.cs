using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    [Header("�������� ����������")]
    public string itemName = "����� �������";
    public Sprite icon;
    [TextArea(3, 5)]
    public string description = "�������� ��������";

    [Header("���������")]
    public int buyPrice = 10;
    public int sellPrice = 5;

    [Header("��� ��������")]
    public ItemType itemType;

    [Header("��� ������")]
    public EquipmentSlot equipmentSlots;
    public Sprite headSprite;
    public Sprite bodySprite;
    public Sprite legsSprite;

    [Header("��� ��� (�����������)")]
    public int healthRestore = 0;
    public int energyRestore = 0;

    [Header("������ ��� ������������� (�����������)")]
    public ItemUseEffect useEffect = ItemUseEffect.None;
    public float jumpHeightBonus;
    public float jumpDurationBonus;
    public float buffDuration = 0f;

    public float speedBonus;

    public float effectDuration = 3f;

    [Header("Use settings")]
    public bool isUsable = false;

    [Header("������ � ����")]
    public float worldScale = 1.0f;
    public Sprite equipSprite;
}


public enum ItemType
{
    Clothing,
    Potion,
    Food,
    Tool
}

[System.Flags]
public enum EquipmentSlot
{
    None = 0,
    Head = 1,
    Body = 2,
    Legs = 4,
    BodyAndLegs = Body | Legs
}

public enum ItemUseEffect
{
    None,
    JumpBoost,
    SpeedBoost
}