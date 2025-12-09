using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : MonoBehaviour
{
    public GameObject normalButton;
    public GameObject pressedButton;
    public GameObject halfBrokenButton;
    public GameObject brokenButton;

    [Header("Force thresholds")]
    public float minForceToPress = 20f;   // меньше Ч не нажмЄтс€
    public float breakForce = 200f;       // больше Ч сломаетс€

    public void CheckWeight(float force)
    {
        if (force <= 0)
        {
            ShowNormal();
            return;
        }

        if (force < minForceToPress)
        {
            ShowNormal(); // слишком лЄгкий предмет Ч не нажмЄтс€
        }
        else if (force >= minForceToPress && force < breakForce)
        {
            ShowPressed(); // нормальное нажатие
        }
        else if (force >= breakForce)
        {
            ShowBroken(); // кнопка сломана
        }
    }

    void ShowNormal()
    {
        normalButton.SetActive(true);
        pressedButton.SetActive(false);
        halfBrokenButton.SetActive(false);
        brokenButton.SetActive(false);
    }

    void ShowPressed()
    {
        normalButton.SetActive(false);
        pressedButton.SetActive(true);
        halfBrokenButton.SetActive(false);
        brokenButton.SetActive(false);
    }

    void ShowHalfBroken()
    {
        normalButton.SetActive(false);
        pressedButton.SetActive(false);
        halfBrokenButton.SetActive(true);
        brokenButton.SetActive(false);
    }

    void ShowBroken()
    {
        normalButton.SetActive(false);
        pressedButton.SetActive(false);
        halfBrokenButton.SetActive(false);
        brokenButton.SetActive(true);
    }
}
