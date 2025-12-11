using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravitySliderFrames : MonoBehaviour
{
    public GameObject[] frames; // 9 кадров (g=2..18)
    public ScaleController scale;

    int currentIndex = 4; // например g=10 по умолчанию

    public float[] gValues = { 2, 4, 6, 8, 10, 12, 14, 16, 18 };

    void ShowFrame()
    {
        for (int i = 0; i < frames.Length; i++)
            frames[i].SetActive(i == currentIndex);

        scale.SetGravity(gValues[currentIndex]); 
        Debug.Log($"g = {gValues[currentIndex]}");
    }

    public void IncreaseG()
    {
        if (currentIndex < frames.Length - 1)
        {
            currentIndex++;
            ShowFrame();
        }
    }

    public void DecreaseG()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            ShowFrame();
        }
    }
}
