using System.Collections;
using System.Collections.Generic;
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
}

public enum ItemType
{
    Clothing,  // Одежда
    Food       // Еда
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
