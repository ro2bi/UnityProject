using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MenuSFXManager : MonoBehaviour
{
    public AudioMixer masterMixer;
    public string exposedVolumeParameter = "SFXVolume";

    [Header("UI Display")]
    public Text volumeText;

    [Range(0f, 1f)]
    public float volume = 0.5f;

    private const string VolumePrefKey = "SFXVolume";


    private void Start()
    {
        if (PlayerPrefs.HasKey(VolumePrefKey))
        {
            volume = PlayerPrefs.GetFloat(VolumePrefKey);
        }

        ApplyVolume();
        UpdateVolumeUI();
    }


    public void ToggleOrIncreaseVolume()
    {
        volume = Mathf.Clamp01(volume + 0.2f);

        if (volume > 1.0f)
        {
            volume = 0.0f;
        }

        ApplyVolume();
        UpdateVolumeUI();
    }


    private void ApplyVolume()
    {
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

        PlayerPrefs.SetFloat(VolumePrefKey, volume);
        PlayerPrefs.Save();
    }

    private void UpdateVolumeUI()
    {
        if (volumeText != null)
        {
            int volumePercent = Mathf.RoundToInt(volume * 100f);

            volumeText.text = "SFX VOLUME: " + volumePercent;
        }
    }
}