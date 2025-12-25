using UnityEngine;

// ���� ������ ����� ������ �� �����, �������, ������ � �.�.
public class InteractableObject : MonoBehaviour
{
    // �����, ������� ����� ������������. 
    // ����������� � ���������� ��� ������� �������.
    [Header("����� ���������")]
    [SerializeField] private string interactionText = "Press E to Interact";

    // �����-�� ������, ������� ��������� �������� �������� (�������� �����, � �.�.)
    //private IActionComponent action;

    // ��� �������� ��� KeybindManager
    private readonly string interactKeyName = KeybindManager.INTERACT;

    private bool playerInside = false;

    // � ������ �������� ��������� �������� �����
    private void Awake()
    {
        // action = GetComponent<IActionComponent>(); 
    }

    private void Update()
    {
        // Если объект — профессор и финальный сегмент, блокируем интеракцию
        ProfessorWalker professor = GetComponent<ProfessorWalker>();
        if (professor != null && professor.finalSegmentOnlyByTrigger)
            return;

        if (playerInside && Input.GetKeyDown(KeybindManager.GetKey(interactKeyName)))
        {
            ExecuteAction();
        }
    }

    private void ExecuteAction()
    {
        Debug.Log($"������ {gameObject.name} �����������!");
        // action?.Execute();
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

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        ProfessorWalker professor = GetComponent<ProfessorWalker>();
        if (professor != null && professor.finalSegmentOnlyByTrigger)
            return;

        playerInside = false;
        UIManagerNew.HideInteractionPrompt();
    }
}
