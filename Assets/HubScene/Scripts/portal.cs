using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    private bool canEnter = false;

    void Update()
    {
        // Если игрок рядом и нажал E (или другую клавишу)
        if (canEnter && Input.GetKeyDown(KeyCode.E))
        {
            EnterPortal();
        }
    }

    private void EnterPortal()
    {
        GameTimer.StopTimer(); // Сохраняем время
        PlayerPrefs.SetInt("GameFinished", 1); // Флаг, что игра пройдена
        SceneManager.LoadScene(1); // Переходим в Меню (сцена 1)
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canEnter = true;
            Debug.Log("Нажми E, чтобы войти в портал");
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