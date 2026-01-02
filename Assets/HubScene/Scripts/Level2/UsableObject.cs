using UnityEngine;

// Цей скрипт відповідає за базову взаємодію з обєктом
// Гравець підходить до тригера і бачить підказку
// При натисканні клавіші викликаються дії через IInteractAction
// Після проходження головоломки взаємодію можна вимкнути
public class UsableObject : MonoBehaviour
{
    // Текст підказки який бачить гравець
    [SerializeField] private string interactionText = "Натисніть E щоб використати";

    // Клавіша взаємодії
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    // Чи знаходиться гравець у зоні тригера
    private bool playerInside = false;

    // Чи дозволена взаємодія
    // Якщо false то підказка не показується і E не працює
    private bool interactionEnabled = true;

    private void Update()
    {
        // Якщо взаємодія вимкнена то виходимо
        if (!interactionEnabled) return;

        // Якщо гравець не поруч то виходимо
        if (!playerInside) return;

        // Якщо натиснули клавішу то викликаємо взаємодію
        if (Input.GetKeyDown(interactKey))
            Use();
    }

    private void Use()
    {
        // Додаткова перевірка на випадок якщо хтось викличе Use з іншого місця
        if (!interactionEnabled) return;

        // Обєкт сам не знає що він робить
        // Він лише викликає Execute у всіх компонентів IInteractAction
        IInteractAction[] actions = GetComponents<IInteractAction>();

        for (int i = 0; i < actions.Length; i++)
        {
            actions[i].Execute();
        }
    }

    public void DisableInteraction()
    {
        // Цей метод викликається після проходження головоломки
        // Він вимикає можливість взаємодії назавжди

        interactionEnabled = false;

        // Якщо гравець стоїть поруч то підказка могла бути видимою
        // Тому ми її ховаємо одразу
        UIManagerNew.HideInteractionPrompt();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Перевіряємо що зайшов саме гравець
        if (!other.CompareTag("Player")) return;

        playerInside = true;

        // Якщо взаємодія вимкнена то підказку не показуємо
        if (!interactionEnabled) return;

        // Показуємо текст підказки
        UIManagerNew.ShowInteractionPrompt(interactionText);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Перевіряємо що вийшов саме гравець
        if (!other.CompareTag("Player")) return;

        playerInside = false;

        // При виході ховаємо підказку завжди
        UIManagerNew.HideInteractionPrompt();
    }
}
