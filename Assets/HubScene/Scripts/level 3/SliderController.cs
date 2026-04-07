using UnityEngine;
using UnityEngine.UI;

public class SliderController : MonoBehaviour
{
    public Slider targetSlider;
    public GameObject sliderParent;

    public enum TriggerType { Start, Stop }
    public TriggerType type;

    public float duration = 10f;
    public string playerTag = "Player";

    private static bool isTimerRunning = false;

    void Start()
    {
        if (type == TriggerType.Start && sliderParent != null)
        {
            sliderParent.SetActive(false);
        }
    }

    void Update()
    {
        if (isTimerRunning && targetSlider != null)
        {
            targetSlider.value += Time.deltaTime / duration;

            if (targetSlider.value >= 1f)
            {
                isTimerRunning = false;
                Debug.Log("����� �����!");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            if (type == TriggerType.Start)
            {
                Debug.Log("����� �������!");
                if (sliderParent != null) sliderParent.SetActive(true);
                if (targetSlider != null) targetSlider.value = 0;
                isTimerRunning = true;
            }
            else if (type == TriggerType.Stop)
            {
                Debug.Log("���� �������!");
                if (sliderParent != null) sliderParent.SetActive(false);
                isTimerRunning = false;
            }
        }
    }
}