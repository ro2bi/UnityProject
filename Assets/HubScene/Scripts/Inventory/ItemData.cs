using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    [Header("Основная информация")]
    public string itemName = "Новый предмет";
    public Sprite icon;
    [TextArea(3, 5)]
    public string description = "Описание предмета";

    [Header("Экономика")]
    public int buyPrice = 10;
    public int sellPrice = 5;

    [Header("Тип предмета")]
    public ItemType itemType;

    [Header("Для одежды")]
    public EquipmentSlot equipmentSlots;
    public Sprite headSprite;
    public Sprite bodySprite;
    public Sprite legsSprite;

    [Header("Для еды (опционально)")]
    public int healthRestore = 0;
    public int energyRestore = 0;

    [Header("Эффект при использовании (опционально)")]
    public ItemUseEffect useEffect = ItemUseEffect.None;
    // для прыжка
    public float jumpHeightBonus;
    public float jumpDurationBonus;
    public float buffDuration = 0f;

    // для скорости
    public float speedBonus;

    // общее время эффекта
    public float effectDuration = 3f;

    [Header("Use settings")]
    public bool isUsable = false;

    [Header("Визуал в мире")]
    public float worldScale = 1.0f;
}

public enum ItemType
{
    Clothing,
    Potion,
    Food  
}

[System.Flags]
public enum EquipmentSlot
{
    None = 0,
    Head = 1,
    Body = 2,
    Legs = 4,
    BodyAndLegs = Body | Legs  // Комбинезон
}

public enum ItemUseEffect
{
    None,
    JumpBoost,
    SpeedBoost
}