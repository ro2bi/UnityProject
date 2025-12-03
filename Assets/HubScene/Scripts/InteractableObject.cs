using UnityEngine;

// Этот скрипт можно вешать на двери, сундуки, кнопки и т.д.
public class InteractableObject : MonoBehaviour
{
    // Текст, который будет отображаться. 
    // Назначается в Инспекторе для каждого объекта.
    [Header("Текст подсказки")]
    [SerializeField] private string interactionText = "Press E to Interact";

    // Какой-то скрипт, который выполняет основное действие (открытие двери, и т.д.)
    //private IActionComponent action;

    // Имя действия для KeybindManager
    private readonly string interactKeyName = KeybindManager.INTERACT;

    private bool playerInside = false;

    // В идеале получать компонент действия здесь
    private void Awake()
    {
        // action = GetComponent<IActionComponent>(); 
    }

    private void Update()
    {
        if (playerInside && Input.GetKeyDown(KeybindManager.GetKey(interactKeyName)))
        {
            // Выполнить действие объекта
            ExecuteAction();
        }
    }

    private void ExecuteAction()
    {
        Debug.Log($"Объект {gameObject.name} активирован!");
        // action?.Execute();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;

            // Получаем клавишу из KeybindManager
            KeyCode key = KeybindManager.GetKey(interactKeyName);

            // Формируем финальный текст, подставляя клавишу
            string finalPrompt = interactionText.Replace("Press E", $"Press {key.ToString()}");

            // ПОКАЗАТЬ ОКНО
            UIManager.ShowInteractionPrompt(finalPrompt);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
            // СКРЫТЬ ОКНО
            UIManager.HideInteractionPrompt();
        }
    }
}
