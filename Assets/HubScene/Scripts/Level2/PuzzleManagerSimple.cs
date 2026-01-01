using UnityEngine;

// Цей скрипт зберігає X та Y
// Перевіряє умови етапів
// Зберігає кількість спроб у вигляді сердечок
// При поразці телепортує гравця у хаб
// При успіху останнього етапу відкриває двері
public class PuzzleManagerSimple : MonoBehaviour
{
    // Результат перевірки відповіді
    // Потрібно щоб планшет знав коли закриватись
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

    [Header("Спроби")]
    // Максимальна кількість сердечок
    [SerializeField] private int maxLives = 3;

    // Поточна кількість сердечок
    private int lives;

    [Header("Значення")]
    private int x = 0;
    private int y = 0;

    // Номер поточного етапу
    private int stage = 0;

    // Межі значень
    private const int minValue = 0;
    private const int maxValue = 10;

    // Доступ до X Y та сердечок з інших скриптів
    public int X => x;
    public int Y => y;
    public int Lives => lives;

    private void Start()
    {
        // На старті встановлюємо кількість спроб
        lives = maxLives;

        // Закриваємо двері на старті
        if (door != null)
            door.CloseDoor();

        // Обнуляємо значення
        ResetValues();
    }

    public string GetEquationText()
    {
        // Текст умови для поточного етапу
        if (stage == 0)
            return "x + y = 8\nx > y";

        if (stage == 1)
            return "x - y = 2\nx кратен 2";

        if (stage == 2)
            return "x + y = 8\nx парне\ny непарне";

        // Якщо всі етапи пройдені
        return "Готово";
    }

    public void ResetValues()
    {
        // Скидаємо значення змінних
        x = 0;
        y = 0;
    }

    public bool AddX(int value)
    {
        // Зберігаємо старе значення
        int old = x;

        // Міняємо X і обмежуємо межами
        x += value;
        x = Mathf.Clamp(x, minValue, maxValue);

        // Повертаємо true якщо значення змінилось
        return x != old;
    }

    public bool AddY(int value)
    {
        // Аналогічно працюємо з Y
        int old = y;

        y += value;
        y = Mathf.Clamp(y, minValue, maxValue);

        return y != old;
    }

    public CheckResult CheckAnswer()
    {
        // Перевіряємо умову поточного етапу
        bool correct = false;

        if (stage == 0)
        {
            correct = (x + y == 8) && (x > y);
        }
        else if (stage == 1)
        {
            correct = (x - y == 2) && (x % 2 == 0);
        }
        else if (stage == 2)
        {
            correct = (x + y == 8) && (x % 2 == 0) && (y % 2 != 0);
        }

        if (correct)
        {
            // Якщо відповідь правильна переходимо на наступний етап
            stage++;

            // Після успіху обнуляємо X та Y
            ResetValues();

            // Якщо це був останній етап відкриваємо двері
            if (stage >= 3 && door != null)
                door.OpenDoor();

            return CheckResult.Correct;
        }

        // Якщо відповідь неправильна віднімаємо серце
        lives--;

        // Після помилки теж обнуляємо значення
        ResetValues();

        // Якщо ще є спроби не телепортуємо
        if (lives > 0)
            return CheckResult.WrongStillAlive;

        // Якщо спроб не залишилось телепортуємо
        Fail();

        // Відновлюємо життя щоб наступна спроба почалась знову з 3
        lives = maxLives;

        return CheckResult.WrongNoLives;
    }

    private void Fail()
    {
        // Телепорт у хаб
        if (player != null && hubReturnPoint != null)
            player.position = hubReturnPoint.position;
    }
}
