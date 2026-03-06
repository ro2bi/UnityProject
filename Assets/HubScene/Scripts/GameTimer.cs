using UnityEngine;

public class GameTimer : MonoBehaviour
{
    public static float timeSpent;
    private bool timerRunning = false;

    void Start()
    {
        // Обнуляем время при старте игры
        timeSpent = 0;
        timerRunning = true;
    }

    void Update()
    {
        if (timerRunning)
        {
            timeSpent += Time.deltaTime;
        }
    }

    public static void StopTimer()
    {
        // Метод для остановки (вызовем при входе в портал)
        PlayerPrefs.SetFloat("FinalTime", timeSpent);
        PlayerPrefs.Save();
    }
}