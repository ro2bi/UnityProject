using UnityEngine;
using System.Collections.Generic;
using System;

public class MathObject : MonoBehaviour, IInteractable
{
    [Header("�������")]
    public ItemData itemToSpawn;
    public GameObject worldItemPrefab;

    public List<MathTimingLevel> objectLevels = new List<MathTimingLevel>();
    public void Interact()
    {
        
            MathTimingMinigame.Instance.StartMinigame(objectLevels, Finish);
        
    }

    private void Finish()
    {
        Vector3 spawnPosition = transform.position + new Vector3(0, 0, 0);

        if (itemToSpawn != null && worldItemPrefab != null)
        {
            GameObject dropped = Instantiate(worldItemPrefab, spawnPosition, Quaternion.identity);

            WorldItem wItem = dropped.GetComponent<WorldItem>();
            if (wItem != null)
            {
                wItem.SetItem(itemToSpawn, 1);
                Debug.Log($"�������� {itemToSpawn.itemName} �'������� � ����� {spawnPosition}");
            }
        }

        Destroy(gameObject);
    }
}