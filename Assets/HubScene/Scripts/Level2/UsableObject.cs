using UnityEngine;

public class UsableObject : MonoBehaviour
{
    // Текст підказки який бачить гравець
    // Він показується коли гравець знаходиться поруч
    [SerializeField] private string interactionText = "Натисніть E щоб використати";

    // Клавіша взаємодії
    // Її можна змінити якщо потрібно
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    // Чи знаходиться гравець у зоні використання
    private bool playerInside = false;

    private void Update()
    {
        // Перевіряємо чи гравець поруч
        // І чи була натиснута клавіша взаємодії
        if (playerInside && Input.GetKeyDown(interactKey))
        {
            Use();
        }
    }

    private void Use()
    {
        // Цей метод викликається коли гравець використовує обєкт
        // Сам обєкт не знає що саме він робить
        // Він просто повідомляє що з ним взаємодіяли

        // Отримуємо всі компоненти які реалізують IInteractAction
        // Це дозволяє підключати різну логіку окремими скриптами
        IInteractAction[] actions = GetComponents<IInteractAction>();

        // Викликаємо дію у кожного знайденого компонента
        for (int i = 0; i < actions.Length; i++)
        {
            actions[i].Execute();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Перевіряємо що в тригер зайшов саме гравець
        if (!other.CompareTag("Player")) return;

        playerInside = true;

        // Показуємо підказку взаємодії
        UIManagerNew.ShowInteractionPrompt(interactionText);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Перевіряємо що гравець вийшов з тригера
        if (!other.CompareTag("Player")) return;

        playerInside = false;

        // Ховаємо підказку взаємодії
        UIManagerNew.HideInteractionPrompt();
    }
}
