using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractPoints : MonoBehaviour
{
    public string interactionText = "Press E to interact";
    private bool playerInside = false;

    void Update()
    {
        // Получаем кнопку взаимодействия из KeybindManager
        KeyCode interactKey = KeybindManager.GetKey(KeybindManager.INTERACT);

        if (playerInside && Input.GetKeyDown(interactKey))
        {
            Interact();
        }
    }

    private void Interact()
    {
        Debug.Log("INTERACT with: " + gameObject.name);
        // Здесь ты можешь открывать диалоги, UI, квесты, инвентарь и т.д.
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;

            // Можешь заменить на UI, если нужно
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