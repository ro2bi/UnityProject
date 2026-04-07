using UnityEngine;

public class GameTimer : MonoBehaviour
{
    public static float timeSpent;
    private bool timerRunning = false;

    void Start()
    {
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
        PlayerPrefs.SetFloat("FinalTime", timeSpent);
        PlayerPrefs.Save();
    }
}