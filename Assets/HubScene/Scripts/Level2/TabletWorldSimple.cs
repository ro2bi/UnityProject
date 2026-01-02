using System.Collections;
using TMPro;
using UnityEngine;

// Цей скрипт керує меню планшета
// Він відкриває і закриває екран який прикріплений до камери
// Він блокує рух гравця поки меню відкрите
// Він обробляє натискання кнопок X і Y та кнопку CHECK
// Він робить затримку 2 секунди при поразці і при перемозі
// Після перемоги він міняє спрайт маленького планшета у світі
// Після перемоги він вимикає взаємодію з маленьким планшетом
public class TabletWorldSimple : MonoBehaviour
{
    [Header("Обєкти меню")]
    // Екран планшета який прикріплений до камери
    [SerializeField] private GameObject tabletScreen;

    // Сірий оверлей який затемнює світ
    [SerializeField] private GameObject grayOverlay;

    [Header("Тексти")]
    // Текст умови поточного етапу
    [SerializeField] private TMP_Text equationText;

    // Окремий текст для X
    [SerializeField] private TMP_Text xValueText;

    // Окремий текст для Y
    [SerializeField] private TMP_Text yValueText;

    // Повідомлення про помилки
    // Наприклад коли значення вже на максимумі
    [SerializeField] private FadeWorldMessageTMP errorText;

    [Header("Логіка")]
    // Менеджер головоломки
    [SerializeField] private PuzzleManagerSimple puzzle;

    [Header("Гравець")]
    // Rigidbody гравця щоб заморозити рух
    [SerializeField] private Rigidbody2D playerRb;

    [Header("Планшет у світі")]
    // Обєкт маленького планшета у кімнаті
    // Його можна залишити активним або вимкнути після перемоги
    [SerializeField] private GameObject tabletWorldObject;

    // SpriteRenderer маленького планшета у кімнаті
    // Саме тут ми міняємо текстуру після перемоги
    [SerializeField] private SpriteRenderer tabletWorldSprite;

    // Спрайт який буде після проходження
    [SerializeField] private Sprite solvedSprite;

    // Компонент взаємодії який треба вимкнути після перемоги
    [SerializeField] private UsableObject usableObject;

    // Чи відкрите меню зараз
    private bool isOpen;

    // Чи йде затримка перед поразкою
    private bool waitingFail;

    // Чи йде затримка перед перемогою
    private bool waitingWin;

    // Збережені constraints гравця
    // Потрібно щоб повернути їх після закриття меню
    private RigidbodyConstraints2D savedConstraints;

    public bool IsOpen => isOpen;

    private void Start()
    {
        // На старті меню вимкнене
        if (grayOverlay != null) grayOverlay.SetActive(false);
        if (tabletScreen != null) tabletScreen.SetActive(false);

        isOpen = false;
        waitingFail = false;
        waitingWin = false;

        // Якщо поле не заповнили то спробуємо взяти компонент з цього обєкта
        if (usableObject == null)
            usableObject = GetComponent<UsableObject>();
    }

    public void UseTablet()
    {
        // Не відкриваємо меню повторно
        if (isOpen) return;

        // Перевіряємо що puzzle підключений
        if (puzzle == null)
        {
            Debug.LogError("Не підключено PuzzleManagerSimple у TabletWorldSimple");
            return;
        }

        // Перевіряємо що Rigidbody гравця підключений
        if (playerRb == null)
        {
            Debug.LogError("Не підключено Rigidbody2D гравця у TabletWorldSimple");
            return;
        }

        isOpen = true;
        waitingFail = false;
        waitingWin = false;

        // Ховаємо підказку взаємодії
        UIManagerNew.HideInteractionPrompt();

        // Сердечка показуються тільки після першого відкриття планшета
        puzzle.EnableHearts();

        // Вмикаємо затемнення і екран
        if (grayOverlay != null) grayOverlay.SetActive(true);
        if (tabletScreen != null) tabletScreen.SetActive(true);

        // Запамятовуємо старі constraints
        savedConstraints = playerRb.constraints;

        // Зупиняємо рух гравця
        playerRb.velocity = Vector2.zero;
        playerRb.angularVelocity = 0f;

        // Заморожуємо все
        playerRb.constraints = RigidbodyConstraints2D.FreezeAll;

        // Оновлюємо текст умови та значення
        RefreshTexts();
    }

    public void CloseTablet()
    {
        // Не закриваємо якщо вже закрито
        if (!isOpen) return;

        isOpen = false;
        waitingFail = false;
        waitingWin = false;

        // Вимикаємо екран
        if (grayOverlay != null) grayOverlay.SetActive(false);
        if (tabletScreen != null) tabletScreen.SetActive(false);

        // Повертаємо рух гравцю
        if (playerRb != null)
            playerRb.constraints = savedConstraints;
    }

    public void PressXPlus()
    {
        // Кнопки не працюють під час затримок
        if (!CanPressButtons()) return;

        // Пробуємо збільшити X
        bool changed = puzzle.AddX(1);

        // Якщо не змінилось то X вже 10
        if (!changed && errorText != null)
            errorText.Show("X вже максимум");

        RefreshTexts();
    }

    public void PressXMinus()
    {
        if (!CanPressButtons()) return;

        bool changed = puzzle.AddX(-1);

        if (!changed && errorText != null)
            errorText.Show("X вже мінімум");

        RefreshTexts();
    }

    public void PressYPlus()
    {
        if (!CanPressButtons()) return;

        bool changed = puzzle.AddY(1);

        if (!changed && errorText != null)
            errorText.Show("Y вже максимум");

        RefreshTexts();
    }

    public void PressYMinus()
    {
        if (!CanPressButtons()) return;

        bool changed = puzzle.AddY(-1);

        if (!changed && errorText != null)
            errorText.Show("Y вже мінімум");

        RefreshTexts();
    }

    public void PressCheck()
    {
        // Під час затримок не дозволяємо натискати CHECK
        if (!CanPressButtons()) return;

        // Перевіряємо відповідь
        var result = puzzle.CheckAnswer();

        // Помилка але спроби ще є
        if (result == PuzzleManagerSimple.CheckResult.WrongStillAlive)
        {
            if (errorText != null)
                errorText.Show("Неправильно. Мінус серце");

            RefreshTexts();
            return;
        }

        // Поразка бо спроб більше немає
        if (result == PuzzleManagerSimple.CheckResult.WrongNoLives)
        {
            waitingFail = true;

            // Показуємо фінальний напис
            if (errorText != null)
                errorText.Show("Завдання не пройдено!");

            // Оновлюємо тексти щоб гравець бачив стан
            RefreshTexts();

            // Чекаємо 2 секунди і потім телепорт
            StartCoroutine(FailRoutine());
            return;
        }

        // Правильна відповідь
        // Показуємо новий етап або Готово
        RefreshTexts();

        // Якщо головоломка вже завершена
        // Робимо паузу щоб гравець це побачив
        if (puzzle.IsCompleted)
        {
            waitingWin = true;
            StartCoroutine(WinRoutine());
        }
    }

    private IEnumerator FailRoutine()
    {
        // Чекаємо 2 секунди перед телепортом
        yield return new WaitForSeconds(2f);

        // Робимо поразку і повний скидання
        if (puzzle != null)
            puzzle.TeleportFailAndReset();

        // Закриваємо меню і повертаємо рух
        CloseTablet();
    }

    private IEnumerator WinRoutine()
    {
        // Чекаємо 2 секунди щоб гравець побачив перемогу
        yield return new WaitForSeconds(2f);

        // Ховаємо сердечка після перемоги
        if (puzzle != null)
            puzzle.WinFinalize();

        // Закриваємо меню і повертаємо рух
        CloseTablet();

        // Міняємо спрайт маленького планшета у світі
        if (tabletWorldSprite != null && solvedSprite != null)
            tabletWorldSprite.sprite = solvedSprite;

        // Вимикаємо взаємодію з маленьким планшетом
        // Це прибирає натискання E і підказку
        if (usableObject != null)
            usableObject.DisableInteraction();

        // На всякий випадок ховаємо підказку ще раз
        UIManagerNew.HideInteractionPrompt();
    }

    private bool CanPressButtons()
    {
        // Кнопки працюють тільки якщо меню відкрите
        if (!isOpen) return false;

        // Під час паузи перед поразкою кнопки не працюють
        if (waitingFail) return false;

        // Під час паузи перед перемогою кнопки не працюють
        if (waitingWin) return false;

        // Без логіки кнопки не працюють
        if (puzzle == null) return false;

        return true;
    }

    private void RefreshTexts()
    {
        // Якщо логіки немає то нічого не робимо
        if (puzzle == null) return;

        // Оновлюємо текст умови
        if (equationText != null)
            equationText.text = puzzle.GetEquationText();

        // Оновлюємо X
        if (xValueText != null)
            xValueText.text = $"{puzzle.X}";

        // Оновлюємо Y
        if (yValueText != null)
            yValueText.text = $"{puzzle.Y}";
    }
}
