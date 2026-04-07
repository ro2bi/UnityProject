using UnityEngine;

public class HeartsViewSimple : MonoBehaviour
{
    [SerializeField] private PuzzleManagerSimple puzzle;

    [SerializeField] private GameObject[] hearts;

    private void Start()
    {
        SetAllHearts(false);
    }

    private void Update()
    {
        if (puzzle == null) return;
        if (hearts == null) return;

        if (!puzzle.HeartsVisible)
        {
            SetAllHearts(false);
            return;
        }

        int lives = puzzle.Lives;

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
        if (hearts == null) return;

        for (int i = 0; i < hearts.Length; i++)
        {
            if (hearts[i] == null) continue;
            if (hearts[i].activeSelf != active)
                hearts[i].SetActive(active);
        }
    }
}