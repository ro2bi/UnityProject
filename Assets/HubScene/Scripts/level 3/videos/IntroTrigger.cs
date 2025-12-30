using UnityEngine;

public class IntroTrigger : MonoBehaviour
{
    // —юда в инспекторе перетащи LevelData первого уровн€
    public LevelData levelData;
    private bool hasPlayed = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // ѕровер€ем игрока и что видео еще не играло
        if (other.CompareTag("Player") && !hasPlayed)
        {
            if (levelData != null)
            {
                hasPlayed = true;

                // Ќаходим CinematicManager и передаем ему данные нашего уровн€
                CinematicManager cinem = FindObjectOfType<CinematicManager>();
                if (cinem != null)
                {
                    cinem.PlayIntro(levelData);
                }
            }
            else
            {
                Debug.LogWarning("IntroTrigger: Ќе назначен LevelData в инспекторе!");
            }
        }
    }
}