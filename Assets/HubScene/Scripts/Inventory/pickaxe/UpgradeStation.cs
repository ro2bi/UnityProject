using UnityEngine;

public class UpgradeStation : MonoBehaviour, IInteractable
{
    [Header("Настройки улучшения")]
    public string toolToUpgrade = "Stick";     // Что ищем в руках
    public ItemData resultItem;                // На что меняем (перетащи сюда SharpStick)

    [Header("Визуал")]
    public GameObject effect;                  // Эффект искр или пыли
    public AudioClip upgradeSound;             // Звук заточки

    [SerializeField] private string interactionText = "Press E to Interact";
    private readonly string interactKeyName = KeybindManager.INTERACT;

    private bool playerInside = false;

    public void Interact()
    {
        // 1. Проверяем, что у игрока в руках
        ItemData currentTool = InventorySystem.Instance.GetEquippedTool();

        if (currentTool != null && currentTool.itemName == toolToUpgrade)
        {
            DoUpgrade();
        }
        else
        {
            Debug.Log("Чтобы использовать этот камень, нужно держать в руках палку!");
            // Можно вывести подсказку на экран
        }
    }

    private void DoUpgrade()
    {
        // 2. Вызываем метод улучшения
        InventorySystem.Instance.UpgradeEquippedTool(resultItem);

        // 3. Эффекты
        if (effect) Instantiate(effect, transform.position, Quaternion.identity);
        if (upgradeSound && SoundManager.instance) SoundManager.instance.PlaySound(upgradeSound);

        Debug.Log("Предмет успешно улучшен!");

        Destroy(gameObject); 
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // Если объект — профессор и финальный сегмент, не показываем подсказку
        ProfessorWalker professor = GetComponent<ProfessorWalker>();
        if (professor != null && professor.finalSegmentOnlyByTrigger)
            return;

        playerInside = true;
        KeyCode key = KeybindManager.GetKey(interactKeyName);
        string finalPrompt = interactionText.Replace("Press E", $"Press {key.ToString()}");

        UIManagerNew.ShowInteractionPrompt(finalPrompt);
    }
}