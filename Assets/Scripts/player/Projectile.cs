using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed;
    // speed - скорость снаряда, с которой он будет двигаться

    private float direction;
    // direction - направление движения снаряда, которое будет установлено при запуске
    private bool hit;
    // hit - переменная, которая будет использоваться для проверки, попал ли снаряд в цель
    private BoxCollider2D boxCollider;
    // boxCollider - компонент, который отвечает за столкновения снаряда
    private Animator anim;
    // anim - компонент, который отвечает за анимацию снаряда
    private float lifetime;
    // lifetime - время жизни снаряда, которое будет увеличиваться с каждым кадром
    // lifetime - переменная, которая будет использоваться для проверки, не истекло ли время жизни снаряда

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        // Получаем компонент BoxCollider2D, который отвечает за столкновения снаряда
        anim = GetComponent<Animator>();
        // Получаем компонент Animator, который отвечает за анимацию снаряда
    }

    private void Update()
    {
        if (hit)
        {
            return;
            // Если снаряд уже попал в цель, то ничего не делаем
        }
        float movementSpeed = speed * Time.deltaTime * direction;
        // Вычисляем скорость движения снаряда, умножая скорость на время между кадрами и направление
        transform.Translate(movementSpeed, 0, 0);
        // Двигаем снаряд в направлении его локальной оси X (вправо)

        lifetime += Time.deltaTime;
        // Увеличиваем время жизни снаряда на время, прошедшее с последнего кадра
        if (lifetime > 5)
        {
            gameObject.SetActive(false);
            // Если время жизни снаряда превышает 5 секунд, деактивируем его
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    // Метод, который вызывается при столкновении с другим объектом
    // collision - объект, с которым произошло столкновение
    {
        hit = true;
        // Устанавливаем флаг hit в true, чтобы не обрабатывать дальнейшие столкновения
        boxCollider.enabled = false;
        // Отключаем коллайдер снаряда, чтобы он больше не вызывал столкновения
        anim.SetTrigger("explode");
        // Запускаем анимацию взрыва снаряда

        if (collision.tag == "Enemy")
        {
            collision.GetComponent<Health>().TakeDamage(1);
        }
    }

    public void SetDirection(float _direction)
    // Метод, который устанавливает направление движения снаряда
    {
        lifetime = 0;
        // Сбрасываем время жизни снаряда, чтобы он мог двигаться заново
        direction = _direction;
        // Устанавливаем направление движения снаряда
        gameObject.SetActive(true);
        // Активируем снаряд, чтобы он мог двигаться
        hit = false;
        // Сбрасываем флаг hit, чтобы снаряд мог снова столкнуться с целью
        boxCollider.enabled = true;
        // Включаем коллайдер снаряда, чтобы он мог вызывать столкновения

        float localScaleX = transform.localScale.x;
        if (Mathf.Sign(localScaleX) != _direction)
        {
            // Проверяем, совпадает ли знак локального масштаба по оси X с направлением движения
            localScaleX = -localScaleX;
            // Если не совпадает, то инвертируем знак локального масштаба по оси X
        }

        transform.localScale = new Vector3(localScaleX, transform.localScale.y, transform.localScale.z);
        // Устанавливаем новый локальный масштаб снаряда, чтобы он смотрел в правильном направлении
    }

    private void Deactivate()
    {
        gameObject.SetActive(false);
        // Деактивируем снаряд, чтобы он больше не был видим и не вызывал столкновения
    }
}
