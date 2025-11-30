using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthCollectible : MonoBehaviour
{
    [SerializeField] private float healthValue;
    [SerializeField] private AudioClip pickUpHeartSound;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            SoundManager.instance.PlaySound(pickUpHeartSound);
            collision.GetComponent<Health>().AddHealth(healthValue);
            // Вызываем метод AddHealth у компонента Health игрока, чтобы увеличить его здоровье на healthValue
            // collision.GetComponent<Health>() - получает компонент Health у объекта, с которым произошло столк
            gameObject.SetActive(false);
            // Деактивируем объект после сбора
        }
    }
}
