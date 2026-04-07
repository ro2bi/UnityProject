using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuVolumeManager : MonoBehaviour
{
    [Range(0f, 1f)]
    public float volume = 0.5f;

    public int maxBricks = 10;
    public Transform bricksContainer;
    public GameObject brickPrefab;

    private List<GameObject> bricks = new List<GameObject>();

    private const string MUSIC_VOLUME_KEY = "musicVolume";

    private void Start()
    {
        for (int i = 0; i < maxBricks; i++)
        {
            GameObject brick = Instantiate(brickPrefab, bricksContainer);
            brick.SetActive(false);
            bricks.Add(brick);
        }

        volume = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, 1.0f);

        SetMusicVolume(volume);

        UpdateVolumeUI();
    }

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

    public void SetMusicVolume(float newVolume)
    {
        volume = newVolume;

        if (SoundManager.instance != null)
        {
            SoundManager.instance.SetMusicVolume(volume);
        }
    }

    private void UpdateVolumeUI()
    {
        int bricksToShow = Mathf.RoundToInt(volume * maxBricks);

        for (int i = 0; i < maxBricks; i++)
        {
            bricks[i].SetActive(i < bricksToShow);

            if (i < bricksToShow)
            {
                bricks[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, (i * 38f) - 174f);
            }
        }
    }
}