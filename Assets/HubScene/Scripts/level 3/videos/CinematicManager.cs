using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using TMPro;

public class CinematicManager : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public GameObject videoOverlay; // Тот самый черный фон для видео

    [Header("Win Panel UI")]
    public GameObject winScreen;
    public Text errorText;
    public GameObject quitButton;
    public EquationManager equationManager;

    public void PlayIntro(LevelData data)
    {
        // Если видео нет, просто ничего не делаем
        if (data == null || data.introVideo == null)
        {
            Debug.Log("Интро видео не назначено, пропускаем.");
            return;
        }
        PlayVideoOnly(data.introVideo);
    }

    private void PlayVideoOnly(VideoClip clip)
    {
        videoOverlay.SetActive(true);
        videoPlayer.clip = clip;
        videoPlayer.Play();
        // Когда видео закончится, выключить оверлей
        videoPlayer.loopPointReached += (vp) => {
            videoOverlay.SetActive(false);
        };
    }

    public void PlayOutcome(VideoClip clip, bool isWin, bool isLastLevel, int errorsCount = 0)
    {
        if (clip == null)
        {
            if (isWin) ShowWinScreen(errorsCount, isLastLevel);
            return;
        }

        videoOverlay.SetActive(true);
        videoPlayer.clip = clip;
        videoPlayer.Play();

        videoPlayer.loopPointReached += (vp) => {
            videoOverlay.SetActive(false);
            if (isWin) ShowWinScreen(errorsCount, isLastLevel);
        };
    }

    private void ShowWinScreen(int errors, bool isLastLevel)
    {
        if (winScreen != null)
        {
            winScreen.SetActive(true);

            // Выключаем кнопку QUIT, если это последний уровень
            if (quitButton != null)
            {
                quitButton.SetActive(!isLastLevel);
            }

            if (errorText != null)
                errorText.text = $"You've made {errors} mistakes";
        }
    }

    public void OnNextButtonClick()
    {
        winScreen.SetActive(false);
        equationManager.GoToNextLevel();
    }

    public void OnExitButtonClick()
    {
        winScreen.SetActive(false);
        equationManager.OpenWallAndFinish();
    }
}