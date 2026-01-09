using UnityEngine;
using System.Collections.Generic;
using System;

public class BreakableMathWall : MonoBehaviour, IInteractable
{
    [Header("Настройки инструмента")]
    public string requiredToolName = "IronPickaxe";

    [Header("Математические уровни стены")]
    public List<MathTimingLevel> wallLevels = new List<MathTimingLevel>();

    [Header("Что исчезнет после разрушения")]
    [Tooltip("Перетащи сюда все объекты, которые должны пропасть (стена, коллизия и т.д.)")]
    public List<GameObject> objectsToDestroy = new List<GameObject>();

    [Header("Эффекты")]
    public GameObject breakEffect;
    public AudioClip breakSound;

    public void Interact()
    {
        ItemData tool = InventorySystem.Instance.GetEquippedTool();

        if (tool != null && tool.itemName == requiredToolName)
        {
            MathTimingMinigame.Instance.StartMinigame(wallLevels, OnMinigameSuccess);
        }
        else
        {
            Debug.Log($"Нужна: {requiredToolName}");
        }
    }

    private void OnMinigameSuccess()
    {
        // Эффекты
        if (breakEffect) Instantiate(breakEffect, transform.position, Quaternion.identity);
        if (breakSound && SoundManager.instance) SoundManager.instance.PlaySound(breakSound);

        // УДАЛЯЕМ ВСЕ НАЗНАЧЕННЫЕ ОБЪЕКТЫ
        if (objectsToDestroy.Count > 0)
        {
            foreach (GameObject obj in objectsToDestroy)
            {
                if (obj != null) Destroy(obj);
            }
        }
        else
        {
            // Если список пуст, удаляем хотя бы сам объект со скриптом
            Destroy(gameObject);
        }

        Debug.Log("Стена и связанные объекты уничтожены!");
    }
}