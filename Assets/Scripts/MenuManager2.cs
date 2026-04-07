using UnityEngine;
using TMPro;
using System;

public class MenuManager2 : MonoBehaviour
{
    [Header("��������� UI")]
    public GameObject congratsWindow;
    public TextMeshProUGUI timeText;

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
            timeText.text = string.Format("�� ������� ��� ��: {0:D2}:{1:D2}", t.Minutes, t.Seconds);

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