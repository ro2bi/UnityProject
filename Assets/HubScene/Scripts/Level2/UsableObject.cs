using UnityEngine;

public class UsableObject : MonoBehaviour
{
    [SerializeField] private string interactionText = "Натисніть E щоб використати";

    [SerializeField] private KeyCode interactKey = KeyCode.E;

    private bool playerInside = false;

    private bool interactionEnabled = true;

    private void Update()
    {
        if (!interactionEnabled) return;

        if (!playerInside) return;

        if (Input.GetKeyDown(interactKey))
            Use();
    }

    private void Use()
    {
        if (!interactionEnabled) return;

        IInteractAction[] actions = GetComponents<IInteractAction>();

        for (int i = 0; i < actions.Length; i++)
        {
            actions[i].Execute();
        }
    }

    public void DisableInteraction()
    {
        interactionEnabled = false;

        UIManagerNew.HideInteractionPrompt();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerInside = true;

        if (!interactionEnabled) return;

        UIManagerNew.ShowInteractionPrompt(interactionText);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerInside = false;

        UIManagerNew.HideInteractionPrompt();
    }
}