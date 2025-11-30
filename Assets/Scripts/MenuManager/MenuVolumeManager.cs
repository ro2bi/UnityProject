using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Оставляем, если используете UI компоненты (но не обязательно)

public class MenuVolumeManager : MonoBehaviour
{
    // 1. Убираем ссылки на AudioMixer, так как используем Singleton SoundManager.
    // public AudioMixer masterMixer; 
    // public string exposedVolumeParameter = "MusicVolume"; 

    // Переменная 'volume' теперь будет отражать громкость, установленную в PlayerPrefs
    [Range(0f, 1f)]
    public float volume = 0.5f;

    public int maxBricks = 10;
    public Transform bricksContainer;
    public GameObject brickPrefab;

    private List<GameObject> bricks = new List<GameObject>();

    // Ключ для PlayerPrefs должен соответствовать тому, который использует SoundManager для музыки
    private const string MUSIC_VOLUME_KEY = "musicVolume";

    private void Start()
    {
        // 1. Создание UI "кирпичиков"
        for (int i = 0; i < maxBricks; i++)
        {
            GameObject brick = Instantiate(brickPrefab, bricksContainer);
            brick.SetActive(false);
            bricks.Add(brick);
        }

        // 2. Загружаем сохраненную громкость из PlayerPrefs
        // Загружаем то, что SoundManager сохранил. 1.0 - значение по умолчанию.
        volume = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, 1.0f);

        // 3. Убеждаемся, что SoundManager знает об этой громкости (если он еще не сделал это сам)
        // SetMusicVolume устанавливает громкость напрямую и вызывает ApplyVolume
        SetMusicVolume(volume);

        // 4. Обновляем UI
        UpdateVolumeUI();
    }

    // Изменяем логику: теперь мы не просто увеличиваем/уменьшаем, а вызываем SetMusicVolume
    public void IncreaseVolume()
    {
        float newVolume = Mathf.Clamp01(volume + 0.1f);
        SetMusicVolume(newVolume);
        UpdateVolumeUI();
    }

    public void DecreaseVolume()
    {
        float newVolume = Mathf.Clamp01(volume - 0.1f);
        SetMusicVolume(newVolume);
        UpdateVolumeUI();
    }

    // НОВЫЙ МЕТОД: Устанавливает громкость через Singleton SoundManager
    public void SetMusicVolume(float newVolume)
    {
        volume = newVolume; // Обновляем локальную переменную

        if (SoundManager.instance != null)
        {
            // !!! ВАЖНО: Мы будем использовать SetMusicVolume для установки ТОЧНОГО значения
            SoundManager.instance.SetMusicVolume(volume);
        }
    }


    // 5. Убираем ApplyVolume, так как вся логика переместилась в SoundManager.SetMusicVolume
    // private void ApplyVolume() { /* ... */ }

    // ... (Оставляем UpdateVolumeUI без изменений)
    private void UpdateVolumeUI()
    {
        int bricksToShow = Mathf.RoundToInt(volume * maxBricks);

        for (int i = 0; i < maxBricks; i++)
        {
            bricks[i].SetActive(i < bricksToShow);

            if (i < bricksToShow)
            {
                // UI код
                bricks[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, (i * 38f) - 174f);
            }
        }
    }
}