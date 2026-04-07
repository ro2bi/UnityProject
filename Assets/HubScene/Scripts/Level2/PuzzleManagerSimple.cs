using UnityEngine;

public class PuzzleManagerSimple : MonoBehaviour
{
    public enum CheckResult
    {
        Correct,
        WrongStillAlive,
        WrongNoLives
    }

    [Header("Посилання")]
    [SerializeField] private Transform player;

    [SerializeField] private Transform hubReturnPoint;

    [SerializeField] private DoorPuzzle door;

    [SerializeField] private GameObject roadObject;

    [SerializeField] private GameObject triggerAfterWin;

    [Header("Спроби")]
    private const int totalStages = 3;

    [SerializeField] private int maxLives = 3;

    private int lives;

    [Header("Значення")]
    private int x;
    private int y;

    private int stage;

    private const int minValue = 0;
    private const int maxValue = 10;

    private bool heartsVisible;

    public int X => x;
    public int Y => y;
    public int Lives => lives;
    public bool HeartsVisible => heartsVisible;

    public bool IsCompleted => stage >= totalStages;

    private void Start()
    {
        stage = 0;
        lives = maxLives;

        heartsVisible = false;

        if (door != null)
        {
            door.gameObject.SetActive(true);
            door.CloseDoor();
        }

        if (roadObject != null)
            roadObject.SetActive(false);

        if (triggerAfterWin != null)
            triggerAfterWin.SetActive(false);

        ResetValues();
    }

    public void EnableHearts()
    {
        heartsVisible = true;
    }

    public void DisableHearts()
    {
        heartsVisible = false;
    }

    public void ResetValues()
    {
        x = 0;
        y = 0;
    }

    public bool AddX(int value)
    {
        int old = x;
        x = Mathf.Clamp(x + value, minValue, maxValue);
        return x != old;
    }

    public bool AddY(int value)
    {
        int old = y;
        y = Mathf.Clamp(y + value, minValue, maxValue);
        return y != old;
    }

    public string GetEquationText()
    {
        if (IsCompleted)
            return "Готово";

        if (stage == 0)
            return "x + y = 8\nx > y";

        if (stage == 1)
            return "x - y = 2\nx кратен 2";

        if (stage == 2)
            return "2x + 1 = 3y\nx парне, y непарне";

        return "Рівень пройдено";
    }

    public CheckResult CheckAnswer()
    {
        if (IsCompleted)
            return CheckResult.Correct;

        bool correct = IsCorrectForStage();

        if (correct)
        {
            stage++;

            ResetValues();

            if (IsCompleted)
            {
                if (door != null)
                {
                    door.gameObject.SetActive(true);
                    door.OpenDoor();
                }

                if (roadObject != null)
                    roadObject.SetActive(true);

                if (triggerAfterWin != null)
                    triggerAfterWin.SetActive(true);
            }

            return CheckResult.Correct;
        }

        lives--;
        ResetValues();

        if (lives > 0)
            return CheckResult.WrongStillAlive;

        return CheckResult.WrongNoLives;
    }

    private bool IsCorrectForStage()
    {
        if (stage == 0)
            return (x + y == 8) && (x > y);

        if (stage == 1)
            return (x - y == 2) && (x % 2 == 0);

        if (stage == 2)
            return (2 * x + 1 == 3 * y)
                && (x % 2 == 0)
                && (y % 2 != 0);

        return true;
    }

    public void TeleportFailAndReset()
    {
        if (player != null && hubReturnPoint != null)
            player.position = hubReturnPoint.position;

        stage = 0;
        lives = maxLives;
        ResetValues();
        DisableHearts();

        if (door != null)
        {
            door.gameObject.SetActive(true);
            door.CloseDoor();
        }

        if (roadObject != null)
            roadObject.SetActive(false);

        if (triggerAfterWin != null)
            triggerAfterWin.SetActive(false);
    }

    public void WinFinalize()
    {
        DisableHearts();
    }
}