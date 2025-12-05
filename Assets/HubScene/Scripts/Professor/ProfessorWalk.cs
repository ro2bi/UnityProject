using UnityEngine;

public class ProfessorWalker : MonoBehaviour
{
    Animator anim;

    [Header("Точки маршрута")]
    public Transform[] points;
    public float speed = 1f;

    [Header("Анимации")]
    public string disappearAnimation = "Disappear";

    private int currentPoint = 0;
    private bool isWaiting = true;
    private bool playerInside = false;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (points.Length == 0) return;

        // Обработка ожидания взаимодействия
        if (isWaiting)
        {
            anim.SetFloat("Speed", 0);

            // Ждём взаимодействия с игроком
            if (playerInside && Input.GetKeyDown(KeybindManager.GetKey(KeybindManager.INTERACT)))
            {
                isWaiting = false;
            }

            return; // Не двигаться пока не взаимодействуют
        }

        // Движение к целевой точке
        Transform target = points[currentPoint];
        Vector3 direction = (target.position - transform.position).normalized;

        transform.position += direction * speed * Time.deltaTime;
        anim.SetFloat("Speed", speed);

        // Поворот
        if (direction.x > 0.1f)
            transform.localScale = new Vector3(1, 1, 1);
        else if (direction.x < -0.1f)
            transform.localScale = new Vector3(-1, 1, 1);

        // Достижение точки
        if (Vector3.Distance(transform.position, target.position) < 0.1f)
        {
            // Последняя точка - исчезновение
            if (currentPoint == points.Length - 1)
            {
                anim.SetFloat("Speed", 0);
                anim.Play(disappearAnimation);
                enabled = false; // Отключить скрипт
                return;
            }

            // Обычная точка - ожидание взаимодействия
            isWaiting = true;
            currentPoint++;

            if (currentPoint >= points.Length)
                currentPoint = 0;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInside = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInside = false;
    }
}