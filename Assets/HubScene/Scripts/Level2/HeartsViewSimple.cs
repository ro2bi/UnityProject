using UnityEngine;

// Цей скрипт показує сердечка у лівому верхньому кутку
// Сердечка це звичайні GameObject зі SpriteRenderer
// Кількість активних сердечок залежить від puzzle.Lives
public class HeartsViewSimple : MonoBehaviour
{
    [SerializeField] private PuzzleManagerSimple puzzle;

    // Масив сердечок у правильному порядку
    // Спочатку перше серце потім друге потім третє
    [SerializeField] private GameObject[] hearts;

    private void Update()
    {
        if (puzzle == null) return;
        if (hearts == null) return;

        int lives = puzzle.Lives;

        // Вмикаємо тільки ті сердечка які відповідають кількості спроб
        for (int i = 0; i < hearts.Length; i++)
        {
            if (hearts[i] == null) continue;

            bool shouldBeActive = i < lives;
            if (hearts[i].activeSelf != shouldBeActive)
                hearts[i].SetActive(shouldBeActive);
        }
    }
}
