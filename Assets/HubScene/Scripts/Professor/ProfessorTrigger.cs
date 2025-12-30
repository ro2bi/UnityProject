using UnityEngine;

public class ProfessorTrigger : MonoBehaviour
{
    // Посилання на професора
    public ProfessorWalker professor;

    // Чи спрацьовує тригер тільки один раз
    public bool oneTime = true;

    private bool triggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Якщо вже спрацювало і потрібно тільки один раз
        if (triggered && oneTime) return;

        // Перевіряємо, що торкнувся гравець
        if (other.CompareTag("Player"))
        {
            triggered = true;

            // Запускаємо поточний сегмент професора
            professor.StartCurrentSegmentExternally();
        }
    }
}
