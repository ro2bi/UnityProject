using UnityEngine;

public class WorldItem : MonoBehaviour
{
    [Header("Данные предмета")]
    public ItemData item;
    public int amount = 1;

    [Header("Визуал (опционально)")]
    public SpriteRenderer spriteRenderer;

    private bool canPickUp;

    private void Start()
    {
        // Автоматически устанавливаем иконку предмета
        if (item != null && spriteRenderer != null)
        {
            spriteRenderer.sprite = item.icon;
        }
    }

    private void Update()
    {
        if (canPickUp && Input.GetKeyDown(KeybindManager.GetKey(KeybindManager.INTERACT)))
        {
            TryPickUp();
        }
    }

    private void TryPickUp()
    {
        if (item == null)
        {
            Debug.LogWarning("⚠️ WorldItem: Предмет не назначен!");
            return;
        }

        // Пытаемся добавить предметы в инвентарь
        int addedCount = 0;

        for (int i = 0; i < amount; i++)
        {
            if (InventorySystem.Instance.AddItem(item))
            {
                addedCount++;
            }
            else
            {
                Debug.Log($"⚠️ Инвентарь полон! Подобрано {addedCount} из {amount}");

                // Обновляем количество оставшихся предметов
                amount -= addedCount;
                return;
            }
        }

        Debug.Log($"✅ Подобрано: {item.itemName} x{addedCount}");

        // Уничтожаем объект после подбора
        Destroy(gameObject);
    }

    // ✅ Для 2D используем Collider2D
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canPickUp = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canPickUp = false;
        }
    }

    // Визуальная подсказка в редакторе
    private void OnDrawGizmos()
    {
        if (item != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, 0.5f);
        }
    }
}