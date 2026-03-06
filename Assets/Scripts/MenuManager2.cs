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
        // Проверяем через PlayerPrefs, пришел ли игрок из портала
        // 1 - значит игра пройдена, 0 - обычный вход в меню
        if (PlayerPrefs.GetInt("GameFinished", 0) == 1)
        {
            ShowCongrats();
        }
        else
        {
            // На всякий случай скрываем окно, если это просто запуск игры
            if (congratsWindow != null)
                congratsWindow.SetActive(false);
        }
    }

    void ShowCongrats()
    {
        if (congratsWindow != null)
        {
            congratsWindow.SetActive(true);

            // Получаем сохраненное время из PlayerPrefs
            float totalTime = PlayerPrefs.GetFloat("FinalTime", 0);

            // Превращаем секунды в красивый формат (Минуты:Секунды)
            TimeSpan t = TimeSpan.FromSeconds(totalTime);
            timeText.text = string.Format("Ви пройшли гру за: {0:D2}:{1:D2}", t.Minutes, t.Seconds);

            // Сбрасываем флаг прохождения, чтобы при следующем обычном 
            // переходе в меню окно не вылетало снова
            PlayerPrefs.SetInt("GameFinished", 0);
            PlayerPrefs.Save();
        }
    }

    // Метод для кнопки "Закрыть" на окне поздравления
    public void CloseCongrats()
    {
        if (congratsWindow != null)
            congratsWindow.SetActive(false);
    }
}