using System.Collections.Generic;
using UnityEngine;

public class Merchant : MonoBehaviour, IInteractable
{
    [Header("Товары этого торговца")]
    public List<ItemData> shopItems = new List<ItemData>();

    [Header("Имя торговца (опционально)")]
    public string merchantName = "Торговец";

    public void Interact()
    {
        // Когда игрок нажимает INTERACT рядом с нами
        if (UIManagerShop.Instance != null)
        {
            // Открываем магазин и передаем именно наш список товаров
            UIManagerShop.Instance.OpenMerchantShop(shopItems);
        }
    }
}