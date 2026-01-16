using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;

// Цей скрипт керує відеоуроком та UI елементами
// Він відкриває та закриває відео
// Він ховає кнопку відкриття коли відео відкрите
// Він повертає кнопку відкриття коли відео закрите і гравець у зоні
// Він робить Play Pause
// Він робить перемотку через Slider
// Він робить зміну гучності через Slider
// Він виправляє перший запуск через Prepare completed
// Він робить перемотку через кадри frame щоб працювало стабільно
public class VideoLessonPlayer : MonoBehaviour
{
    [Header("Компоненти відео")]
    [SerializeField] private VideoPlayer videoPlayer;

    // Цей AudioSource використовується як вихід звуку від VideoPlayer
    // Через нього ми керуємо гучністю
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

    // Чи гравець у зоні тригера
    private bool playerInZone;

    // Чи відео зараз відкрите
    private bool videoOpen;

    // Чи користувач тягне timeSlider
    private bool userDraggingTime;

    // Чи ми чекаємо доки відео підготується
    private bool waitingPrepare;

    private void Start()
    {
        // На старті ховаємо UI відео
        if (videoUI != null)
            videoUI.SetActive(false);

        videoOpen = false;
        userDraggingTime = false;
        waitingPrepare = false;

        // Підключаємо кнопки
        if (playPauseButton != null)
            playPauseButton.onClick.AddListener(TogglePlayPause);

        if (closeButton != null)
            closeButton.onClick.AddListener(CloseVideo);

        // Підключаємо слайдер гучності
        if (volumeSlider != null)
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);

        // Підключаємо слайдер часу
        // Тут перемотка спрацьовує тільки якщо userDraggingTime true
        if (timeSlider != null)
            timeSlider.onValueChanged.AddListener(OnTimeSliderChanged);

        // Підписуємося на Prepare completed
        // Це потрібно щоб відео точно запускалось з першого разу
        if (videoPlayer != null)
            videoPlayer.prepareCompleted += OnPrepared;

        // Підписуємося на кінець відео
        if (videoPlayer != null)
            videoPlayer.loopPointReached += OnVideoFinished;

        // Налаштовуємо звук
        // Це примусово привʼязує звук відео до нашого AudioSource
        // І тоді слайдер гучності реально впливає
        if (videoPlayer != null && audioSource != null)
        {
            videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
            videoPlayer.SetTargetAudioSource(0, audioSource);
        }

        // Ставимо стартову гучність з повзунка
        if (audioSource != null && volumeSlider != null)
            audioSource.volume = volumeSlider.value;

        UpdatePlayPauseText();
    }

    private void Update()
    {
        // Оновлюємо timeSlider щоб показувати прогрес відео
        // Не оновлюємо якщо відео закрите
        // Не оновлюємо поки відео готується
        // Не оновлюємо коли користувач тягне слайдер руками

        if (!videoOpen) return;
        if (waitingPrepare) return;
        if (videoPlayer == null) return;
        if (timeSlider == null) return;
        if (userDraggingTime) return;

        // frameCount має бути > 0, інакше прогрес рахувати не можна
        if (videoPlayer.frameCount <= 0) return;

        // Прогрес рахуємо по кадрах
        float progress = 0f;
        if (videoPlayer.frameCount > 0)
            progress = (float)videoPlayer.frame / (float)videoPlayer.frameCount;

        timeSlider.SetValueWithoutNotify(progress);
    }

    public void SetPlayerInZone(bool value)
    {
        // Цей метод викликає ShowVideoButtonInTrigger
        playerInZone = value;

        // Якщо відео зараз закрите, то кнопка відкриття може показуватися
        // Якщо гравець не у зоні, кнопку ховаємо
        if (!videoOpen && openVideoButtonObject != null)
            openVideoButtonObject.SetActive(playerInZone);
    }

    public bool IsVideoOpen()
    {
        // Цей метод викликає ShowVideoButtonInTrigger
        return videoOpen;
    }

    public void OpenVideo()
    {
        // Відкриваємо UI і запускаємо підготовку відео

        if (videoUI != null)
            videoUI.SetActive(true);

        // Ховаємо кнопку відкриття
        if (openVideoButtonObject != null)
            openVideoButtonObject.SetActive(false);

        videoOpen = true;

        // Скидаємо слайдер на початок
        if (timeSlider != null)
            timeSlider.SetValueWithoutNotify(0f);

        // Готуємо відео
        // Реальний запуск відбудеться у OnPrepared
        if (videoPlayer != null)
        {
            waitingPrepare = true;

            // Зупиняємо і ставимо на початок
            videoPlayer.Stop();
            videoPlayer.frame = 0;

            // Запускаємо підготовку
            videoPlayer.Prepare();
        }

        UpdatePlayPauseText();
    }

    public void CloseVideo()
    {
        // Закриваємо відео та UI

        if (videoPlayer != null)
            videoPlayer.Stop();

        waitingPrepare = false;
        userDraggingTime = false;

        if (videoUI != null)
            videoUI.SetActive(false);

        videoOpen = false;

        // Показуємо кнопку відкриття тільки якщо гравець у зоні
        if (openVideoButtonObject != null)
            openVideoButtonObject.SetActive(playerInZone);

        UpdatePlayPauseText();
    }

    private void OnPrepared(VideoPlayer vp)
    {
        // Відео підготовилось
        // Тепер можна запускати відтворення
        // Це робить перший запуск стабільним

        waitingPrepare = false;

        // Якщо раптом UI вже закрили, не запускаємо
        if (!videoOpen) return;

        vp.Play();
        UpdatePlayPauseText();
    }

    private void TogglePlayPause()
    {
        // Перемикаємо Play Pause

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
        // Оновлюємо напис на кнопці
        // Loading коли відео ще готується

        if (playPauseText == null) return;

        if (waitingPrepare)
        {
            playPauseText.text = "Loading";
            return;
        }

        if (videoPlayer != null && videoPlayer.isPlaying)
            playPauseText.text = "Pause";
        else
            playPauseText.text = "Play";
    }

    private void OnVolumeChanged(float value)
    {
        // Змінюємо гучність через AudioSource

        if (audioSource == null) return;
        audioSource.volume = value;
    }

    private void OnTimeSliderChanged(float value)
    {
        // Цей метод викликається коли змінюється timeSlider
        // Перемотку робимо тільки коли користувач тягне руками
        // Це потрібно щоб Update не заважав користувачу

        if (!userDraggingTime) return;

        SeekToSliderValue(value);
    }

    public void BeginDragTimeSlider()
    {
        // Викликається коли користувач почав тягнути timeSlider
        // Ми дозволяємо перемотку
        userDraggingTime = true;
    }

    public void EndDragTimeSlider()
    {
        // Викликається коли користувач відпустив timeSlider
        // Тут робимо фінальну перемотку, щоб точно стати у потрібну точку

        if (timeSlider != null)
            SeekToSliderValue(timeSlider.value);

        userDraggingTime = false;
    }

    private void SeekToSliderValue(float value)
    {
        // Реальна перемотка
        // Ми робимо перемотку по кадрах frame, це працює стабільно
        // value у слайдера від 0 до 1

        if (waitingPrepare) return;
        if (videoPlayer == null) return;

        // frameCount має бути відомий
        if (videoPlayer.frameCount <= 0) return;

        // Обчислюємо потрібний кадр
        long targetFrame = (long)(value * (float)videoPlayer.frameCount);

        // Обмежуємо кадр щоб не вийти за межі
        if (targetFrame < 0) targetFrame = 0;
        if (targetFrame >= (long)videoPlayer.frameCount) targetFrame = (long)videoPlayer.frameCount - 1;

        // Ставимо потрібний кадр
        videoPlayer.frame = targetFrame;
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        // Коли відео закінчилось, закриваємо UI
        CloseVideo();
    }
}
