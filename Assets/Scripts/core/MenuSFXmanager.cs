using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MenuSFXManager : MonoBehaviour
{
    // ССЫЛКИ И ПАРАМЕТРЫ
    public AudioMixer masterMixer;
    public string exposedVolumeParameter = "SFXVolume";

    [Header("UI Display")]
    public Text volumeText; // Ссылка на компонент Text для отображения значения

    [Range(0f, 1f)]
    public float volume = 0.5f; // Текущая громкость (0.0 до 1.0)

    private const string VolumePrefKey = "SFXVolume";


    private void Start()
    {
        // 1. Загрузка сохраненного значения
        if (PlayerPrefs.HasKey(VolumePrefKey))
        {
            volume = PlayerPrefs.GetFloat(VolumePrefKey);
        }

        // 2. Применение громкости при старте и обновление UI
        ApplyVolume();
        UpdateVolumeUI();
    }

    // --- ФУНКЦИЯ ДЛЯ КНОПКИ ---

    public void ToggleOrIncreaseVolume() // Назовем ее так для ясности
    {
        // Увеличение на 20% (0.2f), с ограничением [0.0, 1.0]
        volume = Mathf.Clamp01(volume + 0.2f);

        // Если громкость достигла максимума (1.0), сбросим ее на 0.0, 
        // чтобы кнопка работала как "переключатель" (0 -> 0.2 -> ... -> 1.0 -> 0.0)
        // Если вам нужно, чтобы она просто останавливалась на 1.0, удалите следующий if.
        if (volume > 1.0f)
        {
            volume = 0.0f;
        }

        ApplyVolume();
        UpdateVolumeUI();
    }

    // --- ЛОГИКА ПРИМЕНЕНИЯ ---

    private void ApplyVolume()
    {
        // Преобразование громкости [0, 1] в децибелы [-80, 0]
        float dB;
        if (volume == 0)
        {
            dB = -80f;
        }
        else
        {
            dB = 20f * Mathf.Log10(volume);
        }

        masterMixer.SetFloat(exposedVolumeParameter, dB);

        // Сохранение
        PlayerPrefs.SetFloat(VolumePrefKey, volume);
        PlayerPrefs.Save();
    }

    // --- ОБНОВЛЕНИЕ ТЕКСТА ---

    private void UpdateVolumeUI()
    {
        if (volumeText != null)
        {
            // Преобразуем значение [0.0, 1.0] в целое число [0, 100]
            int volumePercent = Mathf.RoundToInt(volume * 100f);

            // Обновляем текст
            volumeText.text = "SFX VOLUME: " + volumePercent;
        }
    }
}