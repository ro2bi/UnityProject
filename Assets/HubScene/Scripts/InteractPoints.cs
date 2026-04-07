using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractPoints : MonoBehaviour
{
    public string interactionText = "Press E to interact";
    private bool playerInside = false;

    void Update()
    {
        KeyCode interactKey = KeybindManager.GetKey(KeybindManager.INTERACT);

        if (playerInside && Input.GetKeyDown(interactKey))
        {
            Interact();
        }
    }

    private void Interact()
    {
        Debug.Log("INTERACT with: " + gameObject.name);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;

            Debug.Log($"{interactionText} ({KeybindManager.GetKey(KeybindManager.INTERACT)})");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
        }
    }
}