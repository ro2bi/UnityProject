using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;

// Цей скрипт керує відеоуроком та UI
// Він відкриває та закриває відео
// Він робить Play Pause
// Він робить перемотку через Slider
// Він робить зміну гучності через Slider
// Він запускає відео з першого разу через Prepare completed
// Він примусово налаштовує звук через AudioSource
// Важливо: на AudioSource НЕ повинно бути AudioClip у інспекторі, інакше звуку не буде
public class VideoLessonPlayer : MonoBehaviour
{
    [Header("Компоненти відео")]
    // Компонент який відтворює відео
    [SerializeField] private VideoPlayer videoPlayer;

    // Компонент який приймає звук від VideoPlayer
    // Через нього ми міняємо гучність
    [SerializeField] private AudioSource audioSource;

    [Header("Панель UI з відео")]
    // Головний обʼєкт інтерфейсу з відео
    [SerializeField] private GameObject videoUI;

    [Header("Елементи керування UI")]
    // Кнопка Play Pause
    [SerializeField] private Button playPauseButton;

    // Кнопка закриття
    [SerializeField] private Button closeButton;

    // Слайдер часу від 0 до 1
    [SerializeField] private Slider timeSlider;

    // Слайдер гучності від 0 до 1
    [SerializeField] private Slider volumeSlider;

    // Текст на кнопці Play Pause
    [SerializeField] private TMP_Text playPauseText;

    [Header("Кнопка відкриття відео у світі")]
    // Кнопка що зʼявляється коли гравець у тригері
    [SerializeField] private GameObject openVideoButtonObject;

    // Скрипт кнопки у світі
    // Через нього ми скидаємо спрайт після закриття відео
    [SerializeField] private VideoButtonHoverPress videoButtonHoverPress;


    // Чи гравець зараз у зоні тригера
    private bool playerInZone;

    // Чи відео зараз відкрите
    private bool videoOpen;

    // Чи користувач тягне слайдер часу руками
    private bool userDraggingTime;

    // Чи ми зараз чекаємо Prepare
    private bool waitingPrepare;

    private void Start()
    {
        // На старті UI відео має бути закритий
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

        // Підключаємо слайдери
        // Слайдер гучності завжди працює одразу
        if (volumeSlider != null)
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);

        // Слайдер часу викликається часто, тому перемотуємо тільки коли userDraggingTime true
        if (timeSlider != null)
            timeSlider.onValueChanged.AddListener(OnTimeSliderChanged);

        // Підписуємося на події VideoPlayer
        if (videoPlayer != null)
        {
            // Коли відео підготувалось, ми запускаємо Play
            videoPlayer.prepareCompleted += OnPrepared;

            // Коли відео закінчилось, закриваємо UI
            videoPlayer.loopPointReached += OnVideoFinished;
        }

        // Налаштовуємо звук так, щоб він точно зʼявився
        // Це робить привʼязку аудіо треку до audioSource
        SetupVideoAudio();

        // Встановлюємо стартову гучність з повзунка
        if (audioSource != null && volumeSlider != null)
            audioSource.volume = volumeSlider.value;

        UpdatePlayPauseText();
    }

    private void Update()
    {
        // Оновлюємо слайдер часу щоб він показував прогрес відео
        // Не оновлюємо коли відео закрите
        // Не оновлюємо поки відео готується
        // Не оновлюємо коли користувач тягне слайдер

        if (!videoOpen) return;
        if (waitingPrepare) return;
        if (videoPlayer == null) return;
        if (timeSlider == null) return;
        if (userDraggingTime) return;

        // frameCount має бути доступний щоб рахувати прогрес
        if (videoPlayer.frameCount <= 0) return;

        // Прогрес рахуємо через кадри
        float progress = (float)videoPlayer.frame / (float)videoPlayer.frameCount;

        // Ставимо значення без виклику події
        timeSlider.SetValueWithoutNotify(progress);
    }

    public void SetPlayerInZone(bool value)
    {
        // Цей метод викликає ShowVideoButtonInTrigger
        // Ми запамʼятовуємо чи гравець у зоні
        playerInZone = value;

        // Якщо відео закрите, керуємо кнопкою відкриття
        // У зоні показуємо, поза зоною ховаємо
        if (!videoOpen && openVideoButtonObject != null)
            openVideoButtonObject.SetActive(playerInZone);
    }

    public bool IsVideoOpen()
    {
        // Цей метод викликає ShowVideoButtonInTrigger
        // Він потрібен щоб не показувати кнопку якщо відео вже відкрите
        return videoOpen;
    }

    public void OpenVideo()
    {
        // Відкриваємо UI і запускаємо Prepare

        if (videoUI != null)
            videoUI.SetActive(true);

        // Ховаємо кнопку відкриття, поки відео відкрите
        if (openVideoButtonObject != null)
            openVideoButtonObject.SetActive(false);

        videoOpen = true;

        // Скидаємо слайдер на початок
        if (timeSlider != null)
            timeSlider.SetValueWithoutNotify(0f);

        // Перед запуском ще раз налаштовуємо звук
        // Це корисно якщо щось змінилося у сцені
        SetupVideoAudio();

        // Готуємо відео
        // Реальний запуск відбудеться у OnPrepared
        if (videoPlayer != null)
        {
            waitingPrepare = true;

            // Зупиняємо і ставимо на початок
            videoPlayer.Stop();
            videoPlayer.frame = 0;

            // Підготовка
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
            
        // Коли відео закрилось
        // Ми повертаємо кнопку у початковий вигляд
        if (videoButtonHoverPress != null)
            videoButtonHoverPress.ResetToNormal();

        UpdatePlayPauseText();
    }

    private void OnPrepared(VideoPlayer vp)
    {
        // Відео готове до відтворення
        // Тепер запускаємо Play
        // Це виправляє проблему коли з першого разу була тільки картинка

        waitingPrepare = false;

        // Якщо відео вже закрили, нічого не запускаємо
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
        // Оновлюємо текст на кнопці
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
        // Змінюємо гучність
        // Гучність змінюємо саме у AudioSource

        if (audioSource == null) return;
        audioSource.volume = value;
    }

    private void OnTimeSliderChanged(float value)
    {
        // Перемотку робимо тільки коли користувач тягне слайдер руками
        // Це потрібно щоб Update не заважав користувачу

        if (!userDraggingTime) return;

        SeekToSliderValue(value);
    }

    public void BeginDragTimeSlider()
    {
        // Викликається скриптом TimeSliderDragEvents
        // Тут ми кажемо що користувач почав тягнути
        userDraggingTime = true;
    }

    public void EndDragTimeSlider()
    {
        // Викликається скриптом TimeSliderDragEvents
        // Тут робимо фінальну перемотку і вимикаємо прапорець

        if (timeSlider != null)
            SeekToSliderValue(timeSlider.value);

        userDraggingTime = false;
    }

    private void SeekToSliderValue(float value)
    {
        // Реальна перемотка
        // Ми перемотуємо по кадрах, бо так працює стабільніше ніж через time

        if (waitingPrepare) return;
        if (videoPlayer == null) return;

        // Якщо frameCount не відомий, перемотувати рано
        if (videoPlayer.frameCount <= 0) return;

        // Обчислюємо потрібний кадр
        // value від 0 до 1
        long targetFrame = (long)(value * (float)videoPlayer.frameCount);

        // Обмежуємо щоб не вийти за межі
        if (targetFrame < 0) targetFrame = 0;
        if (targetFrame >= (long)videoPlayer.frameCount) targetFrame = (long)videoPlayer.frameCount - 1;

        // Ставимо кадр
        videoPlayer.frame = targetFrame;
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        // Коли відео закінчилось, закриваємо UI
        CloseVideo();
    }

    private void SetupVideoAudio()
    {
        // Цей метод примусово налаштовує звук відео
        // Він потрібен бо іноді у Unity звук не зʼявляється через налаштування треку

        if (videoPlayer == null) return;
        if (audioSource == null) return;

        // Дуже важливо
        // На цьому AudioSource не має бути AudioClip у інспекторі
        // Інакше AudioSource буде зайнятий іншим звуком і відео не буде чути

        // Кажемо VideoPlayer, що звук має йти через AudioSource
        videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;

        // Вмикаємо аудіо трек 0
        videoPlayer.EnableAudioTrack(0, true);

        // Прибираємо mute з треку
        videoPlayer.SetDirectAudioMute(0, false);

        // Привʼязуємо трек до нашого AudioSource
        videoPlayer.SetTargetAudioSource(0, audioSource);
    }
}
