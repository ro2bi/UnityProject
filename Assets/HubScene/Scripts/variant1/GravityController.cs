using UnityEngine;
using UnityEngine.UI;

public class GravityController : MonoBehaviour
{
    public static GravityController Instance;

    [Header("Gravity Settings")]
    public float minGravity = 1f;
    public float maxGravity = 20f;
    public float currentGravity = 10f;
    public float gravityStep = 0.2f; // Шаг изменения гравитации

    [Header("Slider Frames")]
    public GameObject[] sliderFrames; // Массив из 9 кадров слайдера
    private int currentFrameIndex = 0; // Текущий активный кадр

    [Header("Buttons")]
    public Button increaseButton; // Кнопка увеличения
    public Button decreaseButton; // Кнопка уменьшения

    [Header("References")]
    public Scale scale;
    public Text gravityText; // Опционально: текст для отображения

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        // Настройка кнопок
        if (increaseButton != null)
            increaseButton.onClick.AddListener(IncreaseGravity);

        if (decreaseButton != null)
            decreaseButton.onClick.AddListener(DecreaseGravity);

        // Устанавливаем начальный кадр
        SetSliderFrame(currentFrameIndex);
        UpdateGravity();
    }

    public void IncreaseGravity()
    {
        // Увеличиваем гравитацию
        currentGravity += gravityStep;
        currentGravity = Mathf.Min(currentGravity, maxGravity);

        // Переключаем кадр вверх
        currentFrameIndex = Mathf.Min(currentFrameIndex + 1, sliderFrames.Length - 1);

        SetSliderFrame(currentFrameIndex);
        UpdateGravity();
    }

    public void DecreaseGravity()
    {
        // Уменьшаем гравитацию
        currentGravity -= gravityStep;
        currentGravity = Mathf.Max(currentGravity, minGravity);

        // Переключаем кадр вниз
        currentFrameIndex = Mathf.Max(currentFrameIndex - 1, 0);

        SetSliderFrame(currentFrameIndex);
        UpdateGravity();
    }

    void SetSliderFrame(int frameIndex)
    {
        // Деактивируем все кадры
        for (int i = 0; i < sliderFrames.Length; i++)
        {
            if (sliderFrames[i] != null)
                sliderFrames[i].SetActive(false);
        }

        // Активируем нужный кадр
        if (frameIndex >= 0 && frameIndex < sliderFrames.Length && sliderFrames[frameIndex] != null)
        {
            sliderFrames[frameIndex].SetActive(true);
        }
    }

    void UpdateGravity()
    {
        // Применяем гравитацию ко всем Rigidbody
        Physics.gravity = new Vector3(0, -currentGravity, 0);

        // Обновляем текст (если есть)
        if (gravityText != null)
            gravityText.text = $"{currentGravity:F1}g";

        // Пересчитываем вес на весах
        if (scale != null)
            scale.CalculateWeight();

        Debug.Log($"Гравитация изменена: {currentGravity}g, Кадр: {currentFrameIndex}");
    }

    public float GetCurrentGravity()
    {
        return currentGravity;
    }
}