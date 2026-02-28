using UnityEngine;
using UnityEngine.UI;

public class SliderController : MonoBehaviour
{
    // --- ПЕРЕТАЩИ ЭТО В ИНСПЕКТОРЕ ---
    // Ссылки должны быть одинаковыми на обоих триггерах!
    public Slider targetSlider;      // Сам слайдер
    public GameObject sliderParent;  // Объект с канвасом, который прячем/показываем

    // --- НАСТРОЙКИ ---
    public enum TriggerType { Start, Stop }
    public TriggerType type; // Выбери "Start" для первого триггера, "Stop" для второго

    public float duration = 10f; // За сколько секунд заполнится слайдер
    public string playerTag = "Player";

    // --- СИСТЕМНЫЕ ПЕРЕМЕННЫЕ ---
    private static bool isTimerRunning = false; // Статус таймера (один на всю игру)

    void Start()
    {
        // При запуске игры прячем слайдер
        if (type == TriggerType.Start && sliderParent != null)
        {
            sliderParent.SetActive(false);
        }
    }

    void Update()
    {
        // Если таймер запущен, то заполняем слайдер
        if (isTimerRunning && targetSlider != null)
        {
            // Прибавляем к значению слайдера кусочек времени
            targetSlider.value += Time.deltaTime / duration;

            // Если заполнился до конца, останавливаем
            if (targetSlider.value >= 1f)
            {
                isTimerRunning = false;
                Debug.Log("Время вышло!");
                // Здесь можно добавить логику проигрыша, если нужно
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other) // Для 3D: OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            if (type == TriggerType.Start)
            {
                // ЗАПУСКАЕМ ТАЙМЕР
                Debug.Log("Старт таймера!");
                if (sliderParent != null) sliderParent.SetActive(true);
                if (targetSlider != null) targetSlider.value = 0; // Сбрасываем значение
                isTimerRunning = true;
            }
            else if (type == TriggerType.Stop)
            {
                // ОСТАНАВЛИВАЕМ ТАЙМЕР
                Debug.Log("Стоп таймера!");
                if (sliderParent != null) sliderParent.SetActive(false);
                isTimerRunning = false;
            }
        }
    }
}