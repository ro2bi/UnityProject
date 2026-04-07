using UnityEngine;

public class ProfessorFinalTrigger : MonoBehaviour
{
    public ProfessorWalker professor;
    public bool oneTime = true;

    private bool triggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered && oneTime) return;

        if (other.CompareTag("Player"))
        {
            triggered = true;

            if (professor != null)
            {
                professor.StartCoroutine(professor.FinalDisappear());
            }
        }
    }
}