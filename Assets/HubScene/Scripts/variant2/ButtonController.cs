using UnityEngine;
using System.Collections;

public class ButtonController : MonoBehaviour
{
    public GameObject normalButton;
    public GameObject pressedButton;
    public GameObject halfBrokenButton;

    [Header("Objects to disable when pressed")]
    public GameObject[] disableOnPress;

    [Header("Force thresholds")]
    public float minForceToPress = 20f;
    public float breakForce = 200f;

    [Header("Ссылка на профессора")]
    public ProfessorWalker professor;

    [Header("Нова позиція професора після 2-го рівня")]
    public Transform professorNewPosition;

    [Header("Гравець")]
    public Transform player;
    public Collider2D roomBoundary; // Триггер кімнати

    [Header("Логіка рівнів")]
    public int currentLevel = 1; // Початковий рівень
    private bool level2Started = false;

    private bool isBroken = false;
    private bool isPressed = false;

    private void Start()
    {
        ShowNormal();
    }

    private void Update()
    {
        if (isPressed && !level2Started)
        {
            // Перевіряємо чи гравець вийшов за межі кімнати
            if (!roomBoundary.bounds.Contains(player.position))
            {
                StartCoroutine(StartLevel2Routine());
            }
        }
    }

    public void CheckWeight(float force)
    {
        Debug.Log("Force = " + force);

        if (isBroken)
        {
            if (force >= minForceToPress)
            {
                if (!isPressed)
                {
                    isPressed = true;
                    DisableObjects();
                }
                ShowPressed();
            }
            else
            {
                isPressed = false;
                ShowHalfBroken();
            }
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
        else if (force >= minForceToPress && force < breakForce)
        {
            if (!isPressed)
            {
                isPressed = true;
                DisableObjects();
            }
            ShowPressed();
        }
        else if (force >= breakForce)
        {
            isBroken = true;
            isPressed = false;
            ShowHalfBroken();
        }
    }

    void DisableObjects()
    {
        foreach (var obj in disableOnPress)
        {
            if (obj != null)
            {
                obj.SetActive(false);
            }
        }
    }

    void ShowNormal()
    {
        normalButton.SetActive(true);
        pressedButton.SetActive(false);
        halfBrokenButton.SetActive(false);
    }

    void ShowPressed()
    {
        normalButton.SetActive(false);
        pressedButton.SetActive(true);
        halfBrokenButton.SetActive(false);
    }

    void ShowHalfBroken()
    {
        normalButton.SetActive(false);
        pressedButton.SetActive(false);
        halfBrokenButton.SetActive(true);
    }

    private IEnumerator StartLevel2Routine()
    {
        level2Started = true;
        currentLevel = 2;
        Debug.Log("Розпочато 2-й рівень!");

        if (professor != null && professorNewPosition != null)
        {
            // 1) Викликаємо анімацію зникнення
            professor.PlayDisappearAnimation();

            // 2) Чекаємо поки анімація закінчиться
            yield return new WaitForSeconds(professor.disappearDuration);

            // 3) Телепортуємо професора
            professor.TeleportProfessor(professorNewPosition.position);
        }
    }
}
