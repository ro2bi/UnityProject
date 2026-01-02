using UnityEngine;

// Цей скрипт показує сердечка у лівому верхньому кутку
// Сердечка це звичайні GameObject зі SpriteRenderer
// Сердечка зявляються тільки після першого використання планшета
public class HeartsViewSimple : MonoBehaviour
{
    [SerializeField] private PuzzleManagerSimple puzzle;

    // Масив сердечок у правильному порядку
    // Спочатку перше серце потім друге потім третє
    [SerializeField] private GameObject[] hearts;

    private void Start()
    {
        // На старті приховуємо всі сердечка
        // Вони зявляться тільки після відкриття планшета
        SetAllHearts(false);
    }

    private void Update()
    {
        if (puzzle == null) return;
        if (hearts == null) return;

        // Якщо ще не було відкриття планшета то сердечка приховані
        if (!puzzle.HeartsVisible)
        {
            SetAllHearts(false);
            return;
        }

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

    private void SetAllHearts(bool active)
    {
        // Вмикаємо або вимикаємо всі сердечка одразу
        if (hearts == null) return;

        for (int i = 0; i < hearts.Length; i++)
        {
            if (hearts[i] == null) continue;
            if (hearts[i].activeSelf != active)
                hearts[i].SetActive(active);
        }
    }
}
