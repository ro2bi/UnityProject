using UnityEngine;

public class LevelStartTrigger1 : MonoBehaviour
{
    public LevelStagesManager manager;
    public Transform player;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform == player)
        {
            manager.StartLevelTimer();
            gameObject.SetActive(false);
        }
    }
}