using UnityEngine;
using TMPro; // Используем для красивого текста
using System;

public class MenuManager2 : MonoBehaviour
{
    [Header("Настройки UI")]
    public GameObject congratsWindow; // Панель с поздравлением
    public TextMeshProUGUI timeText;   // Текст для отображения времени

    void Start()
    {
        if (PlayerPrefs.GetInt("GameFinished", 0) == 1)
        {
            ShowCongrats();
        }
        else
        {
            if (congratsWindow != null)
                congratsWindow.SetActive(false);
        }
    }

    void ShowCongrats()
    {
        if (congratsWindow != null)
        {
            congratsWindow.SetActive(true);

            float totalTime = PlayerPrefs.GetFloat("FinalTime", 0);

            TimeSpan t = TimeSpan.FromSeconds(totalTime);
            timeText.text = string.Format("Ви пройшли гру за: {0:D2}:{1:D2}", t.Minutes, t.Seconds);

            PlayerPrefs.SetInt("GameFinished", 0);
            PlayerPrefs.Save();
        }
    }

    public void CloseCongrats()
    {
        if (congratsWindow != null)
            congratsWindow.SetActive(false);
    }
}