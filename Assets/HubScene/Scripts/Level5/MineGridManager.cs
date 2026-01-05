using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class MineGridManager2D : MonoBehaviour
{
    [Header("Настройки поля")]
    public int width = 10;
    public int height = 10;
    public int mineCount = 15;
    public float spacing = 1.1f; // Расстояние между центрами клеток
    public int safeRows = 2;

    [Header("Ссылки на префабы")]
    public GameObject cellPrefab;

    [Header("Ссылки на Игрока")]
    public Transform playerTransform;
    public Transform startPoint; // Точка телепортации при смерти

    [Header("UI элементы")]
    public GameObject deathScreenUI;
    public GameObject mathUIPanel;
    public TMP_Text mathEquationText;
    public TMP_InputField answerInputField;
    public Button submitButton;

    private MineCell2D[,] grid;
    private MineCell2D currentSolvingCell; // Клетка, которую мы сейчас "решаем"

    // Статическая переменная, чтобы другие скрипты (например, движение игрока)
    // могли знать, заблокировано ли управление.
    public static bool IsUIOpen = false;

    void Start()
    {
        // Скрываем окна при старте
        if (mathUIPanel != null) mathUIPanel.SetActive(false);
        if (deathScreenUI != null) deathScreenUI.SetActive(false);

        // Настраиваем кнопку подтверждения
        if (submitButton != null)
            submitButton.onClick.AddListener(CheckAnswer);

        GenerateGrid();
    }

    void Update()
    {
        // Если окно открыто, проверяем нажатие Enter для подтверждения ответа
        if (IsUIOpen && Input.GetKeyDown(KeyCode.Return))
        {
            CheckAnswer();
        }
    }

    void GenerateGrid()
    {
        grid = new MineCell2D[width, height];

        // 1. Создание клеток
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 pos = new Vector3(x * spacing, y * spacing, 0) + transform.position;
                GameObject obj = Instantiate(cellPrefab, pos, Quaternion.identity, transform);

                MineCell2D cell = obj.GetComponent<MineCell2D>();
                cell.Setup(x, y, this);
                grid[x, y] = cell;
            }
        }

        // 2. Расстановка мин (С ЗАПРЕТОМ В ВЕРХНИХ РЯДАХ)
        int placedMines = 0;
        while (placedMines < mineCount)
        {
            int rx = Random.Range(0, width);
            // Мины могут спавниться только ниже "безопасной зоны"
            // (height - safeRows) ограничивает спавн по вертикали
            int ry = Random.Range(0, height - safeRows);

            if (!grid[rx, ry].isMine)
            {
                grid[rx, ry].isMine = true;
                placedMines++;
            }
        }

        // 3. Подсчет соседей
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (!grid[x, y].isMine)
                {
                    grid[x, y].neighboringMines = CountMinesAround(x, y);
                }
            }
        }

        // 4. ПРИНУДИТЕЛЬНОЕ ОТКРЫТИЕ БЕЗОПАСНОЙ ЗОНЫ
        // Проходим по верхним рядам и открываем их
        for (int x = 0; x < width; x++)
        {
            for (int y = height - safeRows; y < height; y++)
            {
                grid[x, y].Reveal(true);
            }
        }
    }

    int CountMinesAround(int x, int y)
    {
        int count = 0;
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                int nx = x + i;
                int ny = y + j;

                if (nx >= 0 && nx < width && ny >= 0 && ny < height)
                {
                    if (grid[nx, ny].isMine) count++;
                }
            }
        }
        return count;
    }

    // Вызывается из MineCell2D, когда игрок наступает на клетку с цифрой
    public void OpenMathPuzzle(MineCell2D cell)
    {
        currentSolvingCell = cell;
        IsUIOpen = true;

        mathUIPanel.SetActive(true);

        // Генерируем текст примера (берется из статического класса MathEquationGenerator)
        string equation = MathEquationGenerator.GetEquation(cell.neighboringMines);
        mathEquationText.text = "РЕШИТЕ ПРИМЕР, ЧТОБЫ ПРОЙТИ: \n" + equation;

        answerInputField.text = "";
        answerInputField.ActivateInputField(); // Сразу ставим курсор в поле ввода
    }

    // Проверка ответа из InputField
    public void CheckAnswer()
    {
        if (currentSolvingCell == null) return;

        int playerAnswer;
        bool isNumeric = int.TryParse(answerInputField.text, out playerAnswer);

        if (isNumeric && playerAnswer == currentSolvingCell.neighboringMines)
        {
            // Правильно!
            IsUIOpen = false;
            mathUIPanel.SetActive(false);
            currentSolvingCell.Reveal(true); // Открываем клетку
        }
        else
        {
            // Неправильно - можно добавить тряску окна или звук ошибки
            Debug.Log("Неверный ответ!");
            answerInputField.text = "";
            answerInputField.ActivateInputField();
        }
    }

    // Рекурсивное открытие пустых соседей (как в сапере)
    public void RevealEmptyNeighbors(int x, int y)
    {
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                int nx = x + i;
                int ny = y + j;

                if (nx >= 0 && nx < width && ny >= 0 && ny < height)
                {
                    MineCell2D neighbor = grid[nx, ny];
                    if (!neighbor.isRevealed && !neighbor.isMine)
                    {
                        neighbor.Reveal(true);
                    }
                }
            }
        }
    }

    public void TriggerGameOver()
    {
        Debug.Log("БАБАХ!");
        deathScreenUI.SetActive(true);

        // Телепортируем игрока на начало
        if (playerTransform != null && startPoint != null)
        {
            playerTransform.position = startPoint.position;
        }
    }

    public void CheckWin()
    {
        // Здесь можно добавить проверку: открыты ли все безопасные клетки
    }
}