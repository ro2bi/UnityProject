using UnityEngine;

public class ButtonController : MonoBehaviour
{
    [Header("Button visuals")]
    public GameObject normalButton;
    public GameObject pressedButton;
    public GameObject halfBrokenButton;

    [Header("Objects to DISABLE when pressed")]
    public GameObject[] disableOnPress;

    [Header("Objects to ENABLE when pressed")]
    public GameObject[] enableOnPress;

    [Header("Force thresholds")]
    public float minForceToPress = 20f;
    public float breakForce = 200f;

    [Header("Level")]
    public LevelManager levelManager;

    [Header("Professor teleport")]
    public Transform nextProfessorPosition;

    private bool isBroken = false;
    private bool isPressed = false;

    private void Start()
    {
        ShowNormal();
    }

    public void CheckWeight(float force)
    {
        if (isBroken)
        {
            if (force >= minForceToPress)
                ShowPressed();
            else
                ShowHalfBroken();
            return;
        }

        if (force <= 0)
        {
            isPressed = false;
            ShowNormal();
            return;
        }

        if (force < minForceToPress)
        {
            isPressed = false;
            ShowNormal();
        }
        else if (force < breakForce)
        {
            if (!isPressed)
            {
                isPressed = true;
                ApplyPressEffects();
            }
            ShowPressed();
        }
        else
        {
            isBroken = true;
            isPressed = false;
            ShowHalfBroken();
        }
    }

    private void ApplyPressEffects()
    {
        // ❌ деактивуємо обʼєкти (тут стіна ВИМИКАЄТЬСЯ)
        foreach (var obj in disableOnPress)
            if (obj != null) obj.SetActive(false);

        // ✅ активуємо обʼєкти (якщо щось інше потрібно)
        foreach (var obj in enableOnPress)
            if (obj != null) obj.SetActive(true);

        // ✅ ВИКЛИКАЄМО ТЕЛЕПОРТАЦІЮ ПРОФЕСОРА
        if (levelManager != null && nextProfessorPosition != null)
        {
            levelManager.OnLevelPassed(nextProfessorPosition.position);
        }
    }

    private void ShowNormal()
    {
        normalButton.SetActive(true);
        pressedButton.SetActive(false);
        halfBrokenButton.SetActive(false);
    }

    private void ShowPressed()
    {
        normalButton.SetActive(false);
        pressedButton.SetActive(true);
        halfBrokenButton.SetActive(false);
    }

    private void ShowHalfBroken()
    {
        normalButton.SetActive(false);
        pressedButton.SetActive(false);
        halfBrokenButton.SetActive(true);
    }
}