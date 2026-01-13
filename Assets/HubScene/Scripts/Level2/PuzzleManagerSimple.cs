using UnityEngine;

// Цей скрипт зберігає X та Y
// Він перевіряє умови трьох етапів головоломки
// Він рахує спроби у вигляді сердечок
// Він відкриває двері після проходження останнього етапу
// Він також показує дорогу після проходження
// Він виконує повний скидання після поразки
public class PuzzleManagerSimple : MonoBehaviour
{
    // Результат перевірки відповіді
    // Планшет використовує це щоб вирішити що робити далі
    public enum CheckResult
    {
        Correct,
        WrongStillAlive,
        WrongNoLives
    }

    [Header("Посилання")]
    // Посилання на гравця для телепорту у хаб
    [SerializeField] private Transform player;

    // Точка куди повертається гравець у хабі
    [SerializeField] private Transform hubReturnPoint;

    // Основні двері які відкриваються після проходження
    [SerializeField] private DoorPuzzle door;

    // Обʼєкт дороги
    // Зʼявляється після повного проходження головоломки
    [SerializeField] private GameObject roadObject;

    // Обʼєкт тригера який має зʼявитися після перемоги
    // Його потрібно перетягнути в інспекторі
    // На старті він буде вимкнений щоб не було видно і щоб не працював колайдер
    [SerializeField] private GameObject triggerAfterWin;

    [Header("Спроби")]
    // Кількість етапів у головоломці
    private const int totalStages = 3;

    // Максимальна кількість спроб
    [SerializeField] private int maxLives = 3;

    // Поточна кількість спроб
    private int lives;

    [Header("Значення")]
    // Поточні значення змінних
    private int x;
    private int y;

    // Поточний етап
    private int stage;

    // Межі значень змінних
    private const int minValue = 0;
    private const int maxValue = 10;

    // Чи потрібно показувати сердечка
    private bool heartsVisible;

    public int X => x;
    public int Y => y;
    public int Lives => lives;
    public bool HeartsVisible => heartsVisible;

    // Чи завершена головоломка
    public bool IsCompleted => stage >= totalStages;

    private void Start()
    {
        // Початковий стан головоломки
        stage = 0;
        lives = maxLives;

        // На старті сердечка приховані
        heartsVisible = false;

        // Переконуємось що обʼєкт дверей активний
        // Це потрібно щоб CloseDoor міг відпрацювати
        if (door != null)
        {
            door.gameObject.SetActive(true);
            door.CloseDoor();
        }

        // Ховаємо дорогу на старті
        if (roadObject != null)
            roadObject.SetActive(false);

        // Ховаємо тригер на старті
        // Якщо його вимкнути як GameObject, він не буде видимий і не буде реагувати на дотики
        if (triggerAfterWin != null)
            triggerAfterWin.SetActive(false);

        // Скидаємо X та Y
        ResetValues();
    }

    public void EnableHearts()
    {
        // Викликається при першому відкритті планшета
        heartsVisible = true;
    }

    public void DisableHearts()
    {
        // Викликається після перемоги або поразки
        heartsVisible = false;
    }

    public void ResetValues()
    {
        // Скидання значень змінних
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
            // Переходимо на наступний етап
            stage++;

            // Скидаємо значення
            ResetValues();

            // Якщо це був останній етап
            if (IsCompleted)
            {
                // Переконуємось що двері активні
                // Це потрібно щоб OpenDoor міг відпрацювати
                if (door != null)
                {
                    door.gameObject.SetActive(true);
                    door.OpenDoor();
                }

                // Показуємо дорогу після перемоги
                if (roadObject != null)
                    roadObject.SetActive(true);

                // Показуємо тригер після перемоги
                // Після цього тригер стає видимим і починає працювати
                if (triggerAfterWin != null)
                    triggerAfterWin.SetActive(true);
            }

            return CheckResult.Correct;
        }

        // Якщо відповідь неправильна
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
        // Телепорт у хаб
        if (player != null && hubReturnPoint != null)
            player.position = hubReturnPoint.position;

        // Повний скидання головоломки
        stage = 0;
        lives = maxLives;
        ResetValues();
        DisableHearts();

        // Переконуємось що обʼєкт дверей активний
        // Це потрібно щоб CloseDoor міг відпрацювати
        if (door != null)
        {
            door.gameObject.SetActive(true);
            door.CloseDoor();
        }

        // Ховаємо дорогу після поразки
        if (roadObject != null)
            roadObject.SetActive(false);

        // Ховаємо тригер після поразки
        // Це важливо якщо гравець програв після того як вже колись переміг
        if (triggerAfterWin != null)
            triggerAfterWin.SetActive(false);
    }

    public void WinFinalize()
    {
        // Прибираємо сердечка після перемоги
        DisableHearts();
    }
}
