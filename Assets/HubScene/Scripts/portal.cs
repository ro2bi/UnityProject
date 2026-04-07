using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    private bool canEnter = false;

    void Update()
    {
        if (canEnter && Input.GetKeyDown(KeyCode.E))
        {
            EnterPortal();
        }
    }

    private void EnterPortal()
    {
        GameTimer.StopTimer();
        PlayerPrefs.SetInt("GameFinished", 1);
        SceneManager.LoadScene(1);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canEnter = true;
            Debug.Log("����� E, ����� ����� � ������");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canEnter = false;
        }
    }
}