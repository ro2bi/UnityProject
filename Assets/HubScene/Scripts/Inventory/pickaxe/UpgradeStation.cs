using UnityEngine;

public class UpgradeStation : MonoBehaviour, IInteractable
{
    [Header("��������� ���������")]
    public string toolToUpgrade = "Stick";
    public ItemData resultItem;

    [Header("������")]
    public GameObject effect;
    public AudioClip upgradeSound;

    [SerializeField] private string interactionText = "Press E to Interact";
    private readonly string interactKeyName = KeybindManager.INTERACT;

    private bool playerInside = false;

    public void Interact()
    {
        ItemData currentTool = InventorySystem.Instance.GetEquippedTool();

        if (currentTool != null && currentTool.itemName == toolToUpgrade)
        {
            DoUpgrade();
        }
        else
        {
            Debug.Log("����� ������������ ���� ������, ����� ������� � ����� �����!");
        }
    }

    private void DoUpgrade()
    {
        InventorySystem.Instance.UpgradeEquippedTool(resultItem);

        if (effect) Instantiate(effect, transform.position, Quaternion.identity);
        if (upgradeSound && SoundManager.instance) SoundManager.instance.PlaySound(upgradeSound);

        Debug.Log("������� ������� �������!");

        Destroy(gameObject); 
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        ProfessorWalker professor = GetComponent<ProfessorWalker>();
        if (professor != null && professor.finalSegmentOnlyByTrigger)
            return;

        playerInside = true;
        KeyCode key = KeybindManager.GetKey(interactKeyName);
        string finalPrompt = interactionText.Replace("Press E", $"Press {key.ToString()}");

        UIManagerNew.ShowInteractionPrompt(finalPrompt);
    }
}