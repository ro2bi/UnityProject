using UnityEngine;

public class WorldItem : MonoBehaviour
{
    [Header("Данные предмета")]
    public ItemData item;
    public int amount = 1;

    [Header("Сохранение")]
    [Tooltip("Уникальный ID для предметов на сцене. Для выброшенных предметов оставьте пустым.")]
    public string uniqueID;

    [Header("Визуал (опционально)")]
    public SpriteRenderer spriteRenderer;

    private bool canPickUp;

    private void Start()
    {
        // Проверка на дубликаты
        if (!string.IsNullOrEmpty(uniqueID) && InventorySystem.Instance.WasItemPickedUp(uniqueID))
        {
            Destroy(gameObject);
            return;
        }

        // ПРИМЕНЯЕМ РАЗМЕР ИЗ ITEMDATA
        ApplyScale();

        if (item != null && spriteRenderer != null)
        {
            spriteRenderer.sprite = item.icon;
        }
    }

    public void SetItem(ItemData newItem, int newAmount = 1)
    {
        item = newItem;
        amount = newAmount;
        uniqueID = "";

        ApplyScale(); // ПРИМЕНЯЕМ РАЗМЕР ПРИ ВЫБРОСЕ

        if (spriteRenderer != null && item != null)
            spriteRenderer.sprite = item.icon;
    }

    private void ApplyScale()
    {
        if (item != null)
        {
            // Устанавливаем масштаб объекта по всем осям
            transform.localScale = new Vector3(item.worldScale, item.worldScale, 1f);
        }
    }

    private void Update()
    {
        // Проверка нажатия кнопки взаимодействия
        if (canPickUp && Input.GetKeyDown(KeybindManager.GetKey(KeybindManager.INTERACT)))
        {
            TryPickUp();
        }
    }

    private void TryPickUp()
    {
        if (item == null)
        {
            Debug.LogWarning("⚠WorldItem: Предмет не назначен!");
            return;
        }

        int addedCount = 0;

        for (int i = 0; i < amount; i++)
        {
            if (InventorySystem.Instance.AddItem(item))
            {
                addedCount++;
            }
            else
            {
                Debug.Log($"Инвентарь полон! Подобрано {addedCount} из {amount}");
                amount -= addedCount;
                return;
            }
        }

        // БАГ 1: Если предмет на сцене имел уникальный ID, помечаем его как "подобранный"
        if (!string.IsNullOrEmpty(uniqueID))
        {
            InventorySystem.Instance.RegisterPickedUpItem(uniqueID);
        }

        Debug.Log($"Подобрано: {item.itemName} x{addedCount}");
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) canPickUp = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) canPickUp = false;
    }

    private void OnDrawGizmos()
    {
        if (item != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, 0.5f);
        }
    }
}