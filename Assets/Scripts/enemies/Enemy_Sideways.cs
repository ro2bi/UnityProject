using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Sideways : MonoBehaviour
{
    [SerializeField] private float movementDistance;
    // Расстояние, на которое враг будет двигаться влево и вправо
    [SerializeField] private float speed;
    // Скорость движения врага
    [SerializeField] private float damage;
    // Урон, который наносит враг при столкновении с игроком
    private bool movingLeft;
    // Флаг, указывающий направление движения врага
    private float leftEdge;
    // Левая граница движения врага
    private float rightEdge;
    // Правая граница движения врага

    private void Awake()
    {
        leftEdge = transform.position.x - movementDistance;
        // Вычисляем левую границу движения врага благодаря вычитанию расстояния из позиции
        rightEdge = transform.position.x + movementDistance;
        // Вычисляем правую границу движения врага благодаря сумме позиции и расстояния
    }
    private void Update()
    {
        if (movingLeft)
        {
            if (transform.position.x > leftEdge)
            {
                transform.position = new Vector3(transform.position.x - speed * Time.deltaTime, transform.position.y, transform.position.z);
                // Двигаем врага влево, уменьшая его позицию по оси X с помощью вычитания скорости умноженной на время кадра из текущей позиции
            }
            else
            {
                movingLeft = false;
                // Если враг достиг левой границы, меняем направление движения на правое
            }
        }
        else
        {
            if (transform.position.x < rightEdge)
            {
                transform.position = new Vector3(transform.position.x + speed * Time.deltaTime, transform.position.y, transform.position.z);
                // Двигаем врага вправо, увеличивая его позицию по оси X с помощью прибавления скорости умноженной на время кадра к текущей позиции
            }
            else
            {
                movingLeft = true;
                // Если враг достиг правой границы, меняем направление движения на левое
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            collision.GetComponent<Health>().TakeDamage(damage);
            // Когда враг сталкивается с игроком, отнимаем здоровье на заданное значение damage
        }
    }
}
