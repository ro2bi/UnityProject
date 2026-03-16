using UnityEngine;

public class Zonetriggerumova : MonoBehaviour
{
    public GameObject button;

    private void Start()
    {
        button.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            button.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            button.SetActive(false);
        }
    }
}