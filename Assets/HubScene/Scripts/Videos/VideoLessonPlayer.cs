using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;

public class VideoLessonPlayer : MonoBehaviour
{
    [Header("Компоненти відео")]
    [SerializeField] private VideoPlayer videoPlayer;

    [SerializeField] private AudioSource audioSource;

    [Header("Панель UI з відео")]
    [SerializeField] private GameObject videoUI;

    [Header("Елементи керування UI")]
    [SerializeField] private Button playPauseButton;

    [SerializeField] private Button closeButton;

    [SerializeField] private Slider timeSlider;

    [SerializeField] private Slider volumeSlider;

    [SerializeField] private TMP_Text playPauseText;

    [Header("Кнопка відкриття відео у світі")]
    [SerializeField] private GameObject openVideoButtonObject;

    [SerializeField] private VideoButtonHoverPress videoButtonHoverPress;


    private bool playerInZone;

    private bool videoOpen;

    private bool userDraggingTime;

    private bool waitingPrepare;

    private void Start()
    {
        if (videoUI != null)
            videoUI.SetActive(false);

        videoOpen = false;
        userDraggingTime = false;
        waitingPrepare = false;

        if (playPauseButton != null)
            playPauseButton.onClick.AddListener(TogglePlayPause);

        if (closeButton != null)
            closeButton.onClick.AddListener(CloseVideo);

        if (volumeSlider != null)
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);

        if (timeSlider != null)
            timeSlider.onValueChanged.AddListener(OnTimeSliderChanged);

        if (videoPlayer != null)
        {
            videoPlayer.prepareCompleted += OnPrepared;

            videoPlayer.loopPointReached += OnVideoFinished;
        }

        SetupVideoAudio();

        if (audioSource != null && volumeSlider != null)
            audioSource.volume = volumeSlider.value;

        UpdatePlayPauseText();
    }

    private void Update()
    {
        if (!videoOpen) return;
        if (waitingPrepare) return;
        if (videoPlayer == null) return;
        if (timeSlider == null) return;
        if (userDraggingTime) return;

        if (videoPlayer.frameCount <= 0) return;

        float progress = (float)videoPlayer.frame / (float)videoPlayer.frameCount;

        timeSlider.SetValueWithoutNotify(progress);
    }

    public void SetPlayerInZone(bool value)
    {
        playerInZone = value;

        if (!videoOpen && openVideoButtonObject != null)
            openVideoButtonObject.SetActive(playerInZone);
    }

    public bool IsVideoOpen()
    {
        return videoOpen;
    }

    public void OpenVideo()
    {
        if (videoUI != null)
            videoUI.SetActive(true);

        if (openVideoButtonObject != null)
            openVideoButtonObject.SetActive(false);

        videoOpen = true;

        if (timeSlider != null)
            timeSlider.SetValueWithoutNotify(0f);

        SetupVideoAudio();

        if (videoPlayer != null)
        {
            waitingPrepare = true;

            videoPlayer.Stop();
            videoPlayer.frame = 0;

            videoPlayer.Prepare();
        }

        UpdatePlayPauseText();
    }

    public void CloseVideo()
    {
        if (videoPlayer != null)
            videoPlayer.Stop();

        waitingPrepare = false;
        userDraggingTime = false;

        if (videoUI != null)
            videoUI.SetActive(false);

        videoOpen = false;

        if (openVideoButtonObject != null)
            openVideoButtonObject.SetActive(playerInZone);
            
        if (videoButtonHoverPress != null)
            videoButtonHoverPress.ResetToNormal();

        UpdatePlayPauseText();
    }

    private void OnPrepared(VideoPlayer vp)
    {
        waitingPrepare = false;

        if (!videoOpen) return;

        vp.Play();
        UpdatePlayPauseText();
    }

    private void TogglePlayPause()
    {
        if (videoPlayer == null) return;
        if (waitingPrepare) return;

        if (videoPlayer.isPlaying)
            videoPlayer.Pause();
        else
            videoPlayer.Play();

        UpdatePlayPauseText();
    }

    private void UpdatePlayPauseText()
    {
        if (playPauseText == null) return;

        if (waitingPrepare)
        {
            playPauseText.text = "Загрузка";
            return;
        }

        if (videoPlayer != null && videoPlayer.isPlaying)
            playPauseText.text = "Пауза";
        else
            playPauseText.text = "Продовжити";
    }

    private void OnVolumeChanged(float value)
    {
        if (audioSource == null) return;
        audioSource.volume = value;
    }

    private void OnTimeSliderChanged(float value)
    {
        if (!userDraggingTime) return;

        SeekToSliderValue(value);
    }

    public void BeginDragTimeSlider()
    {
        userDraggingTime = true;
    }

    public void EndDragTimeSlider()
    {
        if (timeSlider != null)
            SeekToSliderValue(timeSlider.value);

        userDraggingTime = false;
    }

    private void SeekToSliderValue(float value)
    {
        if (waitingPrepare) return;
        if (videoPlayer == null) return;

        if (videoPlayer.frameCount <= 0) return;

        long targetFrame = (long)(value * (float)videoPlayer.frameCount);

        if (targetFrame < 0) targetFrame = 0;
        if (targetFrame >= (long)videoPlayer.frameCount) targetFrame = (long)videoPlayer.frameCount - 1;

        videoPlayer.frame = targetFrame;
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        CloseVideo();
    }

    private void SetupVideoAudio()
    {
        if (videoPlayer == null) return;
        if (audioSource == null) return;

        videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;

        videoPlayer.EnableAudioTrack(0, true);

        videoPlayer.SetDirectAudioMute(0, false);

        videoPlayer.SetTargetAudioSource(0, audioSource);
    }
}