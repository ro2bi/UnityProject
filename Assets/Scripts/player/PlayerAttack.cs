using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private float attackCooldown;
    // attackCooldown - время между атаками
    [SerializeField] private Transform firePoint;
    // firePoint - точка, из которой будет вылетать снаряд при атаке
    // firePoint - это Transform, который указывает на позицию и ориентацию точки, из которой будет вылетать снаряд
    [SerializeField] private GameObject[] fireballs;
    // fireballs - массив снарядов, которые будут использоваться при атаке
    private Animator anim;
    // anim - компонент Animator, который отвечает за анимацию игрока
    private PlayerMovement playerMovement;
    // playerMovement - ссылка на компонент PlayerMovement, который отвечает за движение игрока
    private float cooldownTimer = Mathf.Infinity;
    // cooldownTimer - таймер, который отсчитывает время между атаками, инициализируется большим значением, чтобы игрок мог сразу атаковать
    [SerializeField] private  AudioClip FireballSound;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
        // Получаем компоненты Animator и PlayerMovement из объекта игрока
    }

    private void Update()
    {
        if (Input.GetMouseButton(0) && cooldownTimer > attackCooldown && playerMovement.canAttack())
        {
            // Проверяем, нажата ли левая кнопка мыши и может ли игрок атаковать
            Attack();
        }
        cooldownTimer += Time.deltaTime;
        // Увеличиваем таймер кулдауна на время, прошедшее с последнего кадра
    }

    private void Attack()
    {
        SoundManager.instance.PlaySound(FireballSound);
        anim.SetTrigger("attack");
        cooldownTimer = 0;

        fireballs[FindFireball()].transform.position = firePoint.position;
        fireballs[FindFireball()].GetComponent<Projectile>().SetDirection(Mathf.Sign(transform.localScale.x));
        // Устанавливаем позицию первого снаряда в точку выстрела и задаем направление движения снаряда
    }

    private int FindFireball()
    {
        for (int i = 0; i < fireballs.Length; i++)
        {
            if (!fireballs[i].activeInHierarchy)
            {
                return i; // Возвращаем индекс первого неактивного снаряда
            }
        }
        return 0; // Здесь можно реализовать логику поиска свободного снаряда в массиве fireballs
    }
}
