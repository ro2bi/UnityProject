using UnityEngine;

public class ChoiceTrigger : MonoBehaviour
{
    [SerializeField] private int stageIndex;
    [SerializeField] private bool isCorrect;
    [SerializeField] private LevelStagesManager manager;
    [SerializeField] private Transform player;

    // Метод для динамической смены правильности ответа
    public void SetCorrect(bool value)
    {
        isCorrect = value;
    }

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
            manager.OnCorrectChoice(stageIndex);
        else
            manager.OnWrongChoice();
    }
}