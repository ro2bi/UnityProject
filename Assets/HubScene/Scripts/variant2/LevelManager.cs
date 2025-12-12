using UnityEngine;
using System.Collections;

public class LevelManager : MonoBehaviour
{
    [Header("Player & Boundaries")]
    public Transform player;
    public Collider2D roomBoundary;
    public GameObject closingWall;

    [Header("Professor")]
    public ProfessorWalker professor;
    public Transform[] levelPositions; // позиції професора для кожного рівня

    [Header("Button (тільки для рівня 1)")]
    public ButtonController button;

    private int currentLevel = 0;
    private bool levelRunning = false;

    private void Start()
    {
        if (closingWall != null)
            closingWall.SetActive(true);

        StartCoroutine(StartLevelRoutine());
    }

    private void Update()
    {
        if (levelRunning)
        {
            // Гравець вийшов за межі кімнати → рівень закінчується
            if (!roomBoundary.bounds.Contains(player.position))
            {
                StartCoroutine(EndLevelRoutine());
            }
        }
    }

    // ───────────────────────────────────────────────
    // СТАРТ РІВНЯ
    // ───────────────────────────────────────────────
    private IEnumerator StartLevelRoutine()
    {
        levelRunning = false;

        // Активуємо стіну
        if (closingWall != null)
            closingWall.SetActive(true);

        // Телепортація професора
        professor.TeleportInstant(levelPositions[currentLevel].position);

        // Програти анімацію появи
        yield return professor.PlayAppearRoutine();

        // Підготувати професора до цього рівня
        professor.PrepareForLevel(currentLevel);

        levelRunning = true;
        Debug.Log("▶ Старт рівня " + (currentLevel + 1));
    }

    // ───────────────────────────────────────────────
    // КІНЕЦЬ РІВНЯ
    // ───────────────────────────────────────────────
    private IEnumerator EndLevelRoutine()
    {
        levelRunning = false;

        if (closingWall != null)
            closingWall.SetActive(false);

        Debug.Log("■ Кінець рівня " + (currentLevel + 1));

        // Професор зникає
        yield return professor.PlayDisappearRoutine();

        // Наступний рівень
        currentLevel++;
        if (currentLevel >= levelPositions.Length)
            currentLevel = 0; // цикл

        StartCoroutine(StartLevelRoutine());
    }

    // Для кнопки (якщо треба вручну завершити рівень)
    public void ForceLevelEnd()
    {
        StartCoroutine(EndLevelRoutine());
    }
}
