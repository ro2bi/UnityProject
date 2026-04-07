using UnityEngine;

public class IntroTrigger : MonoBehaviour
{
    public LevelData levelData;
    private bool hasPlayed = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !hasPlayed)
        {
            if (levelData != null)
            {
                hasPlayed = true;

                CinematicManager cinem = FindObjectOfType<CinematicManager>();
                if (cinem != null)
                {
                    cinem.PlayIntro(levelData);
                }
            }
            else
            {
                Debug.LogWarning("IntroTrigger: �� �������� LevelData � ����������!");
            }
        }
    }
}