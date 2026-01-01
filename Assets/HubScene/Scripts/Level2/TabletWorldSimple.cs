using TMPro;
using UnityEngine;

// Цей скрипт керує меню планшета
// Він відкриває екран і блокує рух гравця
// Він обробляє натискання кнопок
// Він оновлює тексти та показує помилки
public class TabletWorldSimple : MonoBehaviour
{
    [Header("Обєкти меню")]
    [SerializeField] private GameObject tabletScreen;
    [SerializeField] private GameObject grayOverlay;

    [Header("Тексти")]
    [SerializeField] private TMP_Text equationText;

    // Окремий текст для X
    [SerializeField] private TMP_Text xValueText;

    // Окремий текст для Y
    [SerializeField] private TMP_Text yValueText;

    [SerializeField] private FadeWorldMessageTMP errorText;

    [Header("Логіка")]
    [SerializeField] private PuzzleManagerSimple puzzle;

    [Header("Гравець")]
    [SerializeField] private Rigidbody2D playerRb;

    // Чи відкрите меню
    private bool isOpen = false;

    // Збережені обмеження руху гравця
    private RigidbodyConstraints2D savedConstraints;

    public bool IsOpen => isOpen;

    public void UseTablet()
    {
        // Не відкриваємо меню повторно
        if (isOpen) return;
        isOpen = true;

        // Ховаємо підказку взаємодії
        UIManagerNew.HideInteractionPrompt();

        if (grayOverlay != null)
            grayOverlay.SetActive(true);

        if (tabletScreen != null)
            tabletScreen.SetActive(true);

        // Блокуємо гравця
        if (playerRb != null)
        {
            savedConstraints = playerRb.constraints;

            playerRb.velocity = Vector2.zero;
            playerRb.angularVelocity = 0f;
            playerRb.constraints = RigidbodyConstraints2D.FreezeAll;
        }

        RefreshTexts();
    }

    public void CloseTablet()
    {
        // Не закриваємо якщо вже закрито
        if (!isOpen) return;
        isOpen = false;

        if (grayOverlay != null)
            grayOverlay.SetActive(false);

        if (tabletScreen != null)
            tabletScreen.SetActive(false);

        // Повертаємо попередні обмеження
        if (playerRb != null)
            playerRb.constraints = savedConstraints;
    }

    public void PressXPlus()
    {
        bool changed = puzzle.AddX(1);

        if (!changed && errorText != null)
            errorText.Show("X вже максимум");

        RefreshTexts();
    }

    public void PressXMinus()
    {
        bool changed = puzzle.AddX(-1);

        if (!changed && errorText != null)
            errorText.Show("X вже мінімум");

        RefreshTexts();
    }

    public void PressYPlus()
    {
        bool changed = puzzle.AddY(1);

        if (!changed && errorText != null)
            errorText.Show("Y вже максимум");

        RefreshTexts();
    }

    public void PressYMinus()
    {
        bool changed = puzzle.AddY(-1);

        if (!changed && errorText != null)
            errorText.Show("Y вже мінімум");

        RefreshTexts();
    }

    public void PressCheck()
    {
        // Перевіряємо відповідь
        var result = puzzle.CheckAnswer();

        // Якщо ще є спроби просто показуємо повідомлення
        if (result == PuzzleManagerSimple.CheckResult.WrongStillAlive)
        {
            if (errorText != null)
                errorText.Show("Неправильно. Мінус серце");

            RefreshTexts();
            return;
        }

        // Якщо спроби закінчились закриваємо меню
        if (result == PuzzleManagerSimple.CheckResult.WrongNoLives)
        {
            if (errorText != null)
                errorText.Show("Спроби закінчились");

            RefreshTexts();
            CloseTablet();
            return;
        }

        // Якщо правильно просто оновлюємо тексти
        RefreshTexts();
    }

    private void RefreshTexts()
    {
        if (puzzle == null) return;

        // Текст умови
        if (equationText != null)
            equationText.text = puzzle.GetEquationText();

        // Окремі тексти для X та Y
        if (xValueText != null)
            xValueText.text = $"{puzzle.X}";

        if (yValueText != null)
            yValueText.text = $"{puzzle.Y}";
    }

    private void Start()
    {
        // На старті екран вимкнений
        if (grayOverlay != null)
            grayOverlay.SetActive(false);

        if (tabletScreen != null)
            tabletScreen.SetActive(false);

        isOpen = false;
    }
}
