using UnityEngine;

// Цей скрипт зберігає X та Y
// Він перевіряє умови трьох етапів головоломки
// Він рахує спроби у вигляді сердечок
// Він відкриває двері після проходження останнього етапу
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

    // Двері які відкриваються після проходження
    [SerializeField] private DoorPuzzle door;

    [Header("Спроби")]
    // Кількість етапів у головоломці
    // За умовою у тебе їх 3
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
    // 0 перший 1 другий 2 третій
    private int stage;

    // Межі значень змінних
    private const int minValue = 0;
    private const int maxValue = 10;

    // Чи потрібно показувати сердечка
    // Сердечка показуються тільки після першого відкриття планшета
    private bool heartsVisible;

    // Публічні властивості щоб інші скрипти могли читати стан
    public int X => x;
    public int Y => y;
    public int Lives => lives;
    public bool HeartsVisible => heartsVisible;

    // Чи завершена головоломка
    public bool IsCompleted => stage >= totalStages;

    private void Start()
    {
        // Початковий стан
        stage = 0;
        lives = maxLives;

        // На старті сердечка приховані
        heartsVisible = false;

        // Закриваємо двері на старті
        if (door != null)
            door.CloseDoor();

        // Скидаємо X і Y
        ResetValues();
    }

    public void EnableHearts()
    {
        // Викликається при першому відкритті планшета
        // Після цього сердечка можна показувати
        heartsVisible = true;
    }

    public void DisableHearts()
    {
        // Викликається після перемоги або поразки
        // Сердечка ховаються з екрана
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
        // Зберігаємо старе значення для порівняння
        int old = x;

        // Змінюємо X та обмежуємо межами
        x = Mathf.Clamp(x + value, minValue, maxValue);

        // Повертаємо true якщо значення змінилось
        return x != old;
    }

    public bool AddY(int value)
    {
        // Аналогічно працюємо з Y
        int old = y;
        y = Mathf.Clamp(y + value, minValue, maxValue);
        return y != old;
    }

    public string GetEquationText()
    {
        // Якщо вже пройдено всі етапи показуємо повідомлення
        if (IsCompleted)
            return "Готово";

        // Текст етапу 1
        if (stage == 0)
            return "x + y = 8\nx > y";

        // Текст етапу 2
        if (stage == 1)
            return "x - y = 2\nx кратен 2";

        // Текст етапу 3
        if (stage == 2)
            return "2x + 1 = 3y\nx парне, y непарне";

        // Запасний варіант
        return "Рівень пройдено!";
    }

    public CheckResult CheckAnswer()
    {
        // Якщо вже все пройдено то відповіді більше не потрібні
        if (IsCompleted)
            return CheckResult.Correct;

        // Перевіряємо умову поточного етапу
        bool correct = IsCorrectForStage();

        if (correct)
        {
            // Переходимо на наступний етап
            stage++;

            // Після успіху скидаємо X і Y
            ResetValues();

            // Якщо це був останній етап то відкриваємо двері
            if (IsCompleted && door != null)
                door.OpenDoor();

            return CheckResult.Correct;
        }

        // Якщо помилка зменшуємо спроби
        lives--;

        // Після помилки теж скидаємо X і Y
        ResetValues();

        // Якщо ще є спроби телепорту не буде
        if (lives > 0)
            return CheckResult.WrongStillAlive;

        // Якщо спроб немає то планшет зробить затримку 2 секунди
        // А потім викличе TeleportFailAndReset
        return CheckResult.WrongNoLives;
    }

    private bool IsCorrectForStage()
    {
        // Тут реальна логіка перевірки
        // Вона має збігатися з текстом у GetEquationText

        if (stage == 0)
        {
            // Етап 1
            return (x + y == 8) && (x > y);
        }

        if (stage == 1)
        {
            // Етап 2
            return (x - y == 2) && (x % 2 == 0);
        }

        if (stage == 2)
        {
            // Етап 3
            // Умови розбиті на частини для простого пояснення
            bool equationOk = (2 * x + 1 == 3 * y);
            bool xEven = (x % 2 == 0);
            bool yOdd = (y % 2 != 0);

            return equationOk && xEven && yOdd;
        }

        // Якщо етапи закінчились
        return true;
    }

    public void TeleportFailAndReset()
    {
        // Цей метод викликається тільки після затримки 2 секунди
        // Тут відбувається поразка і повний скидання

        // Телепорт у хаб
        if (player != null && hubReturnPoint != null)
            player.position = hubReturnPoint.position;

        // Скидаємо етапи щоб почати з початку
        stage = 0;

        // Відновлюємо спроби
        lives = maxLives;

        // Скидаємо значення
        ResetValues();

        // Ховаємо сердечка після поразки
        DisableHearts();

        // Закриваємо двері щоб знову треба було пройти головоломку
        if (door != null)
            door.CloseDoor();
    }

    public void WinFinalize()
    {
        // Цей метод викликається після затримки 2 секунди при перемозі
        // Тут ми ховаємо сердечка щоб вони не висіли на екрані
        DisableHearts();
    }
}
