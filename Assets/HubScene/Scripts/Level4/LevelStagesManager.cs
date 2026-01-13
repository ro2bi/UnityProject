using UnityEngine;

// Менеджер керує стінами етапів та телепортами
// Обʼєкти етапів не вимикаються
// При правильному виборі етап закривається і відкривається наступний
// При помилці всі етапи скидаються і гравець телепортується у точку програшу
public class LevelStagesManager : MonoBehaviour
{
    [System.Serializable]
    public class StageData
    {
        // Індекс етапу починаючи з 0
        public int stageIndex;

        // Стіни які закривають вхід у етап
        public GameObject enterWalls;

        // Стіни які зʼявляються позаду після проходження етапу
        public GameObject closeBehindWalls;

        // Точка телепорта після проходження цього етапу
        public Transform teleportAfterStage;
    }

    [Header("Player")]
    // Transform гравця
    [SerializeField] private Transform player;

    // Rigidbody2D гравця
    [SerializeField] private Rigidbody2D playerRb;

    [Header("Lose")]
    // Точка телепорта у випадку програшу
    // Підключається вручну в інспекторі
    [SerializeField] private Transform loseTeleportPoint;

    [Header("Stages")]
    // Дані всіх етапів
    [SerializeField] private StageData[] stages;

    private Vector3 startPosition;
    private int currentStage;

    private void Awake()
    {
        if (player == null || playerRb == null || loseTeleportPoint == null || stages == null || stages.Length == 0)
        {
            Debug.LogError("LevelStagesManager: відсутні обовʼязкові посилання");
            enabled = false;
            return;
        }
    }

    private void Start()
    {
        // Запамʼятовуємо стартову позицію гравця
        startPosition = player.position;

        // Ініціалізуємо рівень
        ResetLevel();
    }

    // Викликається при правильному виборі
    public void OnCorrectChoice(int stageIndex)
    {
        if (stageIndex != currentStage)
            return;

        // Закриваємо шлях назад
        SetCloseBehindWalls(stageIndex, true);

        // Телепорт після проходження етапу
        StageData stage = GetStage(stageIndex);
        if (stage != null && stage.teleportAfterStage != null)
        {
            Teleport(stage.teleportAfterStage.position);
        }

        // Переходимо до наступного етапу
        currentStage++;

        if (currentStage >= stages.Length)
            return;

        // Відкриваємо наступний етап
        SetEnterWalls(currentStage, false);
    }

    // Викликається при неправильному виборі
    public void OnWrongChoice()
    {
        // Телепорт у точку програшу
        Teleport(loseTeleportPoint.position);

        // Скидання стану рівня
        ResetLevel();

        // Повернення на старт
        Teleport(startPosition);
    }

    // Початковий стан рівня
    private void ResetLevel()
    {
        currentStage = 0;

        for (int i = 0; i < stages.Length; i++)
        {
            SetEnterWalls(i, true);
            SetCloseBehindWalls(i, false);
        }

        // Перший етап відкритий
        SetEnterWalls(0, false);
    }

    private void SetEnterWalls(int stageIndex, bool value)
    {
        StageData stage = GetStage(stageIndex);
        if (stage != null && stage.enterWalls != null)
        {
            stage.enterWalls.SetActive(value);
        }
    }

    private void SetCloseBehindWalls(int stageIndex, bool value)
    {
        StageData stage = GetStage(stageIndex);
        if (stage != null && stage.closeBehindWalls != null)
        {
            stage.closeBehindWalls.SetActive(value);
        }
    }

    private StageData GetStage(int stageIndex)
    {
        for (int i = 0; i < stages.Length; i++)
        {
            if (stages[i].stageIndex == stageIndex)
                return stages[i];
        }
        return null;
    }

    // Телепорт з обнуленням швидкості
    private void Teleport(Vector3 position)
    {
        playerRb.velocity = Vector2.zero;
        playerRb.angularVelocity = 0f;
        player.position = position;
    }
}
