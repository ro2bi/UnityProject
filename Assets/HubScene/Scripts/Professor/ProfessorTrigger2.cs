using UnityEngine;

public class ProfessorFinalTrigger : MonoBehaviour
{
    public ProfessorWalker professor; // Ссылка на профессора
    public bool oneTime = true;       // Чтобы сработало один раз

    private bool triggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered && oneTime) return;

        if (other.CompareTag("Player"))
        {
            triggered = true;

            // Запускаем плавное исчезновение профессора
            if (professor != null)
            {
                professor.StartCoroutine(professor.FinalDisappear());
            }
        }
    }
}