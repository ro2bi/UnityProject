using System.Collections;
using TMPro;
using UnityEngine;

public class TabletWorldSimple : MonoBehaviour
{
    [Header("Обєкти меню")]
    [SerializeField] private GameObject tabletScreen;

    [SerializeField] private GameObject grayOverlay;

    [Header("Тексти")]
    [SerializeField] private TMP_Text equationText;

    [SerializeField] private TMP_Text xValueText;

    [SerializeField] private TMP_Text yValueText;

    [SerializeField] private FadeWorldMessageTMP errorText;

    [Header("Логіка")]
    [SerializeField] private PuzzleManagerSimple puzzle;

    [Header("Гравець")]
    [SerializeField] private Rigidbody2D playerRb;

    [Header("Планшет у світі")]
    [SerializeField] private GameObject tabletWorldObject;

    [SerializeField] private SpriteRenderer tabletWorldSprite;

    [SerializeField] private Sprite solvedSprite;

    [SerializeField] private UsableObject usableObject;

    private bool isOpen;

    private bool waitingFail;

    private bool waitingWin;

    private RigidbodyConstraints2D savedConstraints;

    public bool IsOpen => isOpen;

    private void Start()
    {
        if (grayOverlay != null) grayOverlay.SetActive(false);
        if (tabletScreen != null) tabletScreen.SetActive(false);

        isOpen = false;
        waitingFail = false;
        waitingWin = false;

        if (usableObject == null)
            usableObject = GetComponent<UsableObject>();
    }

    public void UseTablet()
    {
        if (isOpen) return;

        if (puzzle == null)
        {
            Debug.LogError("Не підключено PuzzleManagerSimple у TabletWorldSimple");
            return;
        }

        if (playerRb == null)
        {
            Debug.LogError("Не підключено Rigidbody2D гравця у TabletWorldSimple");
            return;
        }

        isOpen = true;
        waitingFail = false;
        waitingWin = false;

        UIManagerNew.HideInteractionPrompt();

        puzzle.EnableHearts();

        if (grayOverlay != null) grayOverlay.SetActive(true);
        if (tabletScreen != null) tabletScreen.SetActive(true);

        savedConstraints = playerRb.constraints;

        playerRb.velocity = Vector2.zero;
        playerRb.angularVelocity = 0f;

        playerRb.constraints = RigidbodyConstraints2D.FreezeAll;

        RefreshTexts();
    }

    public void CloseTablet()
    {
        if (!isOpen) return;

        isOpen = false;
        waitingFail = false;
        waitingWin = false;

        if (grayOverlay != null) grayOverlay.SetActive(false);
        if (tabletScreen != null) tabletScreen.SetActive(false);

        if (playerRb != null)
            playerRb.constraints = savedConstraints;
    }

    public void PressXPlus()
    {
        if (!CanPressButtons()) return;

        bool changed = puzzle.AddX(1);

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
        if (!CanPressButtons()) return;

        var result = puzzle.CheckAnswer();

        if (result == PuzzleManagerSimple.CheckResult.WrongStillAlive)
        {
            if (errorText != null)
                errorText.Show("Неправильно. Мінус серце");

            RefreshTexts();
            return;
        }

        if (result == PuzzleManagerSimple.CheckResult.WrongNoLives)
        {
            waitingFail = true;

            if (errorText != null)
                errorText.Show("Завдання не пройдено!");

            RefreshTexts();

            StartCoroutine(FailRoutine());
            return;
        }

        RefreshTexts();

        if (puzzle.IsCompleted)
        {
            waitingWin = true;
            StartCoroutine(WinRoutine());
        }
    }

    private IEnumerator FailRoutine()
    {
        yield return new WaitForSeconds(2f);

        if (puzzle != null)
            puzzle.TeleportFailAndReset();

        CloseTablet();
    }

    private IEnumerator WinRoutine()
    {
        yield return new WaitForSeconds(2f);

        if (puzzle != null)
            puzzle.WinFinalize();

        CloseTablet();

        if (tabletWorldSprite != null && solvedSprite != null)
            tabletWorldSprite.sprite = solvedSprite;

        if (usableObject != null)
            usableObject.DisableInteraction();

        UIManagerNew.HideInteractionPrompt();
    }

    private bool CanPressButtons()
    {
        if (!isOpen) return false;

        if (waitingFail) return false;

        if (waitingWin) return false;

        if (puzzle == null) return false;

        return true;
    }

    private void RefreshTexts()
    {
        if (puzzle == null) return;

        if (equationText != null)
            equationText.text = puzzle.GetEquationText();

        if (xValueText != null)
            xValueText.text = $"{puzzle.X}";

        if (yValueText != null)
            yValueText.text = $"{puzzle.Y}";
    }
}