using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class MineGridManager2D : MonoBehaviour
{
    [Header("��������� ����")]
    public int width = 10;
    public int height = 10;
    public int mineCount = 15;
    public float spacing = 1.1f;
    public int safeRows = 2;

    [Header("������ �� �������")]
    public GameObject cellPrefab;

    [Header("������ �� ������")]
    public Transform playerTransform;
    public Transform startPoint;

    [Header("UI ��������")]
    public GameObject deathScreenUI;
    public GameObject mathUIPanel;
    public TMP_Text mathEquationText;
    public TMP_InputField answerInputField;
    public Button submitButton;

    private MineCell2D[,] grid;
    private MineCell2D currentSolvingCell;

    public static bool IsUIOpen = false;

    void Start()
    {
        if (mathUIPanel != null) mathUIPanel.SetActive(false);
        if (deathScreenUI != null) deathScreenUI.SetActive(false);

        if (submitButton != null)
            submitButton.onClick.AddListener(CheckAnswer);

        GenerateGrid();
    }

    void Update()
    {
        if (IsUIOpen && Input.GetKeyDown(KeyCode.Return))
        {
            CheckAnswer();
        }
    }

    void GenerateGrid()
    {
        grid = new MineCell2D[width, height];

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

        int placedMines = 0;
        while (placedMines < mineCount)
        {
            int rx = Random.Range(0, width);
            int ry = Random.Range(0, height - safeRows);

            if (!grid[rx, ry].isMine)
            {
                grid[rx, ry].isMine = true;
                placedMines++;
            }
        }

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

    public void OpenMathPuzzle(MineCell2D cell)
    {
        currentSolvingCell = cell;
        IsUIOpen = true;

        mathUIPanel.SetActive(true);

        string equation = MathEquationGenerator.GetEquation(cell.neighboringMines);
        mathEquationText.text = "������ ������, ����� ������: \n" + equation;

        answerInputField.text = "";
        answerInputField.ActivateInputField();
    }

    public void CheckAnswer()
    {
        if (currentSolvingCell == null) return;

        int playerAnswer;
        bool isNumeric = int.TryParse(answerInputField.text, out playerAnswer);

        if (isNumeric && playerAnswer == currentSolvingCell.neighboringMines)
        {
            IsUIOpen = false;
            mathUIPanel.SetActive(false);
            currentSolvingCell.Reveal(true);
        }
        else
        {
            Debug.Log("�������� �����!");
            answerInputField.text = "";
            answerInputField.ActivateInputField();
        }
    }

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
        Debug.Log("�����!");
        deathScreenUI.SetActive(true);

        if (playerTransform != null && startPoint != null)
        {
            playerTransform.position = startPoint.position;
        }
    }

    public void CheckWin()
    {
    }
}