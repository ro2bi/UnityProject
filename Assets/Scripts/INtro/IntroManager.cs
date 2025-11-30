using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class IntroManager : MonoBehaviour
{
    public GameObject[] frames;       // assign frames in Inspector
    public Button nextButton;         // assign the Next button
    public string menuSceneName = "Level1"; // after intro, load this scene

    private int currentIndex = 0;

    void Start()
    {
        ShowFrame(0);
        nextButton.onClick.AddListener(NextFrame);
    }

    void ShowFrame(int index)
    {
        for (int i = 0; i < frames.Length; i++)
            frames[i].SetActive(i == index);
    }

    void NextFrame()
    {
        Debug.Log("Button clicked!");

        currentIndex++;

        if (currentIndex < frames.Length)
        {
            ShowFrame(currentIndex);
        }
        else
        {
            SceneManager.LoadScene(menuSceneName);
        }
    }
}

