using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class WorldItem : MonoBehaviour
{
    public ItemData item;
    public int amount = 1;

    private bool canPickUp;

    private void Update()
    {
        if (canPickUp && Input.GetKeyDown(KeyCode.E))
        {
            TryPickUp();
        }
    }

    private void TryPickUp()
    {
        for (int i = 0; i < amount; i++)
        {
            if (!InventorySystem.Instance.AddItem(item))
                return;
        }

        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canPickUp = true;
            UIManagerNew.ShowInteractionPrompt($"Подобрать {item.itemName} [E]");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canPickUp = false;
            UIManagerNew.HideInteractionPrompt();
        }
    }
}
