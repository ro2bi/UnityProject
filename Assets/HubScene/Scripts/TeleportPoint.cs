using UnityEngine;

public class TeleportPoint : MonoBehaviour
{
    [Header("Куда телепортировать игрока")]
    public Transform teleportTarget;

    private bool playerInside = false;

    void Update()
    {
        // Проверка нажатия кнопки "Interact" через KeybindManager
        if (playerInside && Input.GetKeyDown(KeybindManager.GetKey(KeybindManager.INTERACT)))
        {
            TeleportPlayer();
        }
    }

    private void TeleportPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null && teleportTarget != null)
        {
            player.transform.position = teleportTarget.position;
            Debug.Log("Player teleported to: " + teleportTarget.position);
        }
        else
        {
            Debug.LogError("Teleport failed! Missing Player or TeleportTarget.");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
            Debug.Log("Press " + KeybindManager.GetKey(KeybindManager.INTERACT) + " to teleport.");
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
