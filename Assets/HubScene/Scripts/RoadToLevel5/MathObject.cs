using UnityEngine;
using System.Collections.Generic;
using System;

public class MathObject : MonoBehaviour, IInteractable
{
    [Header("Награда")]
    public ItemData itemToSpawn; // Какой предмет появится после победы
    public GameObject worldItemPrefab; // Тот же префаб, что в InventorySystem

    public List<MathTimingLevel> objectLevels = new List<MathTimingLevel>();
    public void Interact()
    {
        
            MathTimingMinigame.Instance.StartMinigame(objectLevels, Finish);
        
    }

    private void Finish()
    {
        // 1. Определяем точку спавна (позиция этого объекта)
        Vector3 spawnPosition = transform.position + new Vector3(0, 0, 0);

        // 2. Создаем предмет
        if (itemToSpawn != null && worldItemPrefab != null)
        {
            // Создаем префаб WorldItem в точке spawnPosition
            GameObject dropped = Instantiate(worldItemPrefab, spawnPosition, Quaternion.identity);

            WorldItem wItem = dropped.GetComponent<WorldItem>();
            if (wItem != null)
            {
                // Настраиваем данные предмета (иконку, количество и т.д.)
                wItem.SetItem(itemToSpawn, 1);
                Debug.Log($"Нагорода {itemToSpawn.itemName} з'явилась у точці {spawnPosition}");
            }
        }

        // 3. Удаляем объект-загадку, так как он больше не нужен
        Destroy(gameObject);
    }
}