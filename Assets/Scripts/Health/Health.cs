using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("Health")]
    // Health - класс, отвечающий за управление здоровьем игрока
    [SerializeField] private float startingHealth;
    // startingHealth - начальное здоровье игрока, устанавливается в инспекторе Unity
    public float currentHealth { get; private set; }
    // currentHealth - текущее здоровье игрока, доступно только для чтения извне класса Health;
    // { get; private set; } - позволяет другим классам только читать текущее здоровье, но не изменять его напрямую

    private Animator anim;
    // anim - ссылка на компонент Animator, который управляет анимациями игрока

    private bool dead;
    // dead - флаг, указывающий, мертв ли игрок; используется для предотвращения повторного вызова анимации смерти

    [Header("iFrames")]
    [SerializeField] private float iFramesDuration;
    // iFramesDuration - продолжительность неуязвимости после получения урона, устанавливается в инспекторе Unity
    [SerializeField] private int numberOfFlashes;
    // numberOfFlashes - количество миганий игрока при получении урона, устанавливается в инспекторе Unity

    private SpriteRenderer spriteRend;
    // spriteRend - ссылка на компонент SpriteRenderer, который управляет отображением спрайта игрока

    [Header("Components")]
    [SerializeField] private Behaviour[] components;

    [Header("Death sound")]
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private AudioClip hurtSound;

    private void Awake()
    {
        currentHealth = startingHealth;
        // Инициализируем текущее здоровье игрока значением начального здоровья
        anim = GetComponent<Animator>();
        // Получаем компонент Animator, который прикреплен к тому же объекту, что и этот скрипт
        spriteRend = GetComponent<SpriteRenderer>();
        // Получаем компонент SpriteRenderer, который прикреплен к тому же объекту, что и этот скрипт
    }

    public void TakeDamage(float _damage)
    {
        currentHealth = Mathf.Clamp(currentHealth - _damage, 0, startingHealth);
        // Уменьшаем текущее здоровье игрока на значение урона, устанавливаем его в диапазоне от 0 до начального здоровья
        // Mathf.Clamp() - ограничивает значение в заданном диапазоне

        if (currentHealth > 0)
        {
            anim.SetTrigger("hurt");
            // Если текущее здоровье больше 0, вызываем анимацию "Hurt" (получение урона)
            StartCoroutine(Invunerability());
            // Запускаем корутин Invunerability(), который реализует неуязвимость
            SoundManager.instance.PlaySound(hurtSound);
        }

        else
        {
            if (!dead)
            {
                anim.SetTrigger("die");
                // Если текущее здоровье стало 0 или меньше, вызываем анимацию "Die" (смерть)
                GetComponent<PlayerMovement>().enabled = false;
                // Отключаем компонент PlayerMovement, чтобы игрок не мог двигаться после смерти

                //ENEMY
                if (GetComponentInParent<EnemyPatrol>() != null)
                {
                    GetComponentInParent<EnemyPatrol>().enabled = false;
                }

                if (GetComponent<bot_enemy>() != null)
                {
                    GetComponent<bot_enemy>().enabled = false;
                }

                foreach (Behaviour component in components)
                    component.enabled = false;

                dead = true;
                // Устанавливаем флаг dead в true, чтобы предотвратить повторный вызов анимации смерти
                SoundManager.instance.PlaySound(deathSound);
            }
        }
    }

    public void AddHealth(float _value)
    {
        currentHealth = Mathf.Clamp(currentHealth + _value, 0, startingHealth);
        // Увеличиваем текущее здоровье игрока на значение _value, устанавливаем его в диапазоне от 0 до начального здоровья
        // Используем Mathf.Clamp() для ограничения значения в заданном диапазоне
    }

    private IEnumerator Invunerability()
    {
        Physics2D.IgnoreLayerCollision(8, 9, true);
        // Игнорируем столкновения между слоями 10 и 11 (например, игрок и враги) для реализации неуязвимости
        for (int i = 0; i < numberOfFlashes; i++)
        {
            spriteRend.color = new Color(1, 0, 0, 0.5f);
            // Меняем цвет спрайта на красный с прозрачностью 0.5 для мигания
            yield return new WaitForSeconds(iFramesDuration / (numberOfFlashes * 2));
            // Ждем половину времени неуязвимости, деленное на количество миганий
            spriteRend.color = Color.white;
            // Возвращаем цвет спрайта к белому
            yield return new WaitForSeconds(iFramesDuration / (numberOfFlashes * 2));
            // Ждем половину времени неуязвимости, деленное на количество миганий
        }
        Physics2D.IgnoreLayerCollision(8, 9, false);
        // Восстанавливаем столкновения после завершения неуязвимости
    }

    public bool IsAlive()
    {
        return !dead;
    }

//Respawn
    public void Respawn()
    {
        AddHealth(startingHealth);
        anim.ResetTrigger("die");
        anim.Play("Idle");
        StartCoroutine(Invunerability());

        //Activate all attached component classes
        foreach (Behaviour component in components)
            component.enabled = true;
    }
}