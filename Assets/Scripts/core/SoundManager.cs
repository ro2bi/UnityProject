using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance { get; private set; }
    private AudioSource soundSource;
    private AudioSource musicSource;

    private void Awake()
    {
        soundSource = GetComponent<AudioSource>();
        musicSource = transform.GetChild(0).GetComponent<AudioSource>();

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != null && instance != this)
            Destroy(gameObject);

        ChangeMusicVolume(0.4f);
        ChangeSoundVolume(0.2f);
    }
    public void PlaySound(AudioClip _sound)
    {
        soundSource.PlayOneShot(_sound);
    }

    public void ChangeSoundVolume(float _change)
    {
        ChangeSourceVolume(0.1f, "soundVolume", _change, soundSource);
    }
    /*public void ChangeMusicVolume(float _change)
    {
        ChangeSourceVolume(0.2f, "musicVolume", _change, musicSource);
    }*/

    private void ChangeSourceVolume(float baseVolume, string volumeName, float change, AudioSource source)
    {
        float currentVolume = PlayerPrefs.GetFloat(volumeName, 1);
        currentVolume += change;

        if (currentVolume > 1)
            currentVolume = 0;
        else if (currentVolume < 0)
            currentVolume = 1;

        float finalVolume = currentVolume * baseVolume;
        source.volume = finalVolume;

        PlayerPrefs.SetFloat(volumeName, currentVolume);
    }

    public void SetMusicVolume(float newVolume)
    {
        float finalVolume = Mathf.Clamp01(newVolume);

        musicSource.volume = finalVolume;

        PlayerPrefs.SetFloat("musicVolume", finalVolume);
        PlayerPrefs.Save();

        Debug.Log("��������� ������ ����������� ��: " + finalVolume);
    }

    public void IncreaseMusicVolume(float step = 0.05f)
    {
        float current = PlayerPrefs.GetFloat("musicVolume", 1f);
        current += step;

        if (current > 1f) current = 1f;

        SetMusicVolume(current);
    }

    public void DecreaseMusicVolume(float step = 0.05f)
    {
        float current = PlayerPrefs.GetFloat("musicVolume", 1f);
        current -= step;

        if (current < 0f) current = 0f;

        SetMusicVolume(current);
    }

    public void ChangeMusicVolume(float _change)
    {
        
        float currentVolume = PlayerPrefs.GetFloat("musicVolume", 1);
        currentVolume += _change;
        if (currentVolume > 1) currentVolume = 0;
        else if (currentVolume < 0) currentVolume = 1;
        SetMusicVolume(currentVolume);
        
    }
}