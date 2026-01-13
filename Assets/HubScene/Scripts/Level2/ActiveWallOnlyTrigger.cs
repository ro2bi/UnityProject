using UnityEngine;

// Цей скрипт висить на 2D тригері
// При дотику гравця він активує двері або стіну, які складаються з кількох частин під одним батьківським обʼєктом
// Також він робить тригер одноразовим, щоб він не спрацьовував повторно
public class ActivateWallOnlyTrigger : MonoBehaviour
{
    // Перетягни сюди БАТЬКІВСЬКИЙ обʼєкт дверей або стіни з Hierarchy
    // Саме цей обʼєкт має містити всередині дві або більше частин
    [SerializeField] private GameObject wallParent;

    // Якщо це ввімкнено, то після активації буде вимкнено SpriteRenderer тригера
    // Це потрібно якщо ти хочеш щоб тригер зник візуально, але обʼєкт залишився в сцені
    [SerializeField] private bool hideTriggerSprite = true;

    // Якщо це ввімкнено, то після активації буде вимкнено Collider2D тригера
    // Це робить тригер одноразовим і прибирає повторні спрацювання
    [SerializeField] private bool disableTriggerCollider = true;

    private Collider2D cachedTriggerCollider;
    private SpriteRenderer cachedTriggerSprite;

    private void Awake()
    {
        // Зберігаємо посилання на компоненти тригера, щоб не шукати їх кожен раз при торканні
        cachedTriggerCollider = GetComponent<Collider2D>();
        cachedTriggerSprite = GetComponent<SpriteRenderer>();

        // Додаткова перевірка, щоб одразу бачити помилку якщо на обʼєкті немає Collider2D
        // Тригер у 2D працює тільки якщо є Collider2D і в нього увімкнено Is Trigger
        if (cachedTriggerCollider == null)
        {
            Debug.LogWarning("На обʼєкті тригера немає Collider2D, OnTriggerEnter2D не буде викликатись", this);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Перевіряємо, що зайшов саме гравець по тегу
        // Якщо тег не заданий або інший, код не спрацює
        if (!other.CompareTag("Player"))
            return;

        // Якщо не підключили батьківський обʼєкт дверей або стіни, то активувати нічого
        if (wallParent == null)
        {
            Debug.LogWarning("Не підключено wallParent у інспекторі, немає що активувати", this);
            MakeTriggerOneTime();
            return;
        }

        // Активуємо батьківський обʼєкт, щоб усі частини стали активними разом
        wallParent.SetActive(true);

        // Іноді частини дверей залишаються невидимими або без колайдерів, якщо на дітях вимкнені компоненти
        // Тому ми примусово вмикаємо Renderer і Collider2D на всіх дочірніх обʼєктах
        EnableChildrenRenderersAndColliders(wallParent);

        // Робимо тригер одноразовим після успішної активації
        MakeTriggerOneTime();
    }

    private void EnableChildrenRenderersAndColliders(GameObject parent)
    {
        // Вмикаємо всі Renderer у дітей, включно з неактивними на момент пошуку
        var renderers = parent.GetComponentsInChildren<Renderer>(true);
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] != null)
                renderers[i].enabled = true;
        }

        // Вмикаємо всі Collider2D у дітей, включно з неактивними на момент пошуку
        // Це важливо для дверей або стін, які повинні блокувати гравця
        var colliders2D = parent.GetComponentsInChildren<Collider2D>(true);
        for (int i = 0; i < colliders2D.Length; i++)
        {
            if (colliders2D[i] != null)
                colliders2D[i].enabled = true;
        }
    }

    private void MakeTriggerOneTime()
    {
        // Вимикаємо колайдер тригера, щоб OnTriggerEnter2D більше не викликався
        // Це безпечніше ніж одразу вимикати весь gameObject, особливо якщо є інші скрипти або анімації
        if (disableTriggerCollider && cachedTriggerCollider != null)
        {
            cachedTriggerCollider.enabled = false;
        }

        // Ховаємо спрайт тригера, якщо потрібно прибрати його з екрану
        if (hideTriggerSprite && cachedTriggerSprite != null)
        {
            cachedTriggerSprite.enabled = false;
        }
    }
}
