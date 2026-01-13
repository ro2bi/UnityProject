using UnityEngine;

// Тригер вибору шляху
// Працює тільки з конкретним гравцем
public class ChoiceTrigger : MonoBehaviour
{
    // Індекс етапу до якого належить тригер
    [SerializeField] private int stageIndex;

    // Чи є вибір правильним
    [SerializeField] private bool isCorrect;

    // Менеджер етапів
    [SerializeField] private LevelStagesManager manager;

    // Transform гравця
    [SerializeField] private Transform player;

    private void Awake()
    {
        if (manager == null || player == null)
        {
            Debug.LogError("ChoiceTrigger: відсутні посилання");
            enabled = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform != player)
            return;

        if (isCorrect)
        {
            manager.OnCorrectChoice(stageIndex);
        }
        else
        {
            manager.OnWrongChoice();
        }
    }
}
