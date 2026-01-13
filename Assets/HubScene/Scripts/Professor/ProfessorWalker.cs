using UnityEngine;
using System.Collections;
using TMPro;

public class ProfessorWalker : MonoBehaviour
{
    // ===================== DATA =====================
    [System.Serializable]
    public class TimedPhrase
    {
        [TextArea] public string text;
        public float duration = 2.5f;
    }

    [System.Serializable]
    public class Segment
    {
        [Header("Маршрут")]
        public Transform startPoint;
        public Transform endPoint;

        [Header("Рух")]
        public float speed = 1.5f; // 👈 ІНДИВІДУАЛЬНА ШВИДКІСТЬ
        public bool playWalkAnimation = true;

        [Header("Фрази під час руху")]
        public TimedPhrase[] walkPhrases;

        [Header("Підказки після зупинки")]
        [TextArea] public string[] hints;
    }


    [Header("Сегменти (0→1, 2→3, 4→5 …)")]
    public Segment[] segments;

    [Header("Автостарт")]
    public float autoStartDelay = 1f; // 👈 ЗАДЕРЖКА ПЕРЕД АВТОСТАРТОМ

    [Header("Анімації")]
    public string speedParam = "Speed";

    [Header("Анімації")]
    public Animator anim;
    public string disappearAnim = "Disappear";
    public string appearAnim = "Appear";
    public float disappearDuration = 0.7f;

    [Header("UI діалогів")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;

    [Header("UI підказок")]
    public GameObject hintBubble;
    public TextMeshProUGUI hintText;
    [Header("Фінальний сегмент")]
    public bool finalSegmentOnlyByTrigger = false;
    [Header("Фінальная стіна")]
    public GameObject finalWall;
    [Header("Фінальний контролер дороги")]
    public RoadController roadController;
    [Header("Настройки платных подсказок")]
    public string hintItemName = "Книга Знаний"; // Название предмета из ItemData
    private bool isCurrentHintUnlocked = false; // Разблокирована ли подсказка на этой остановке

    // ===================== STATE =====================
    private int segmentIndex = 0;
    private bool waitingForInteract = true;
    private bool walking = false;
    private bool showingHints = false;
    private int hintIndex = 0;
    private bool playerInside = false;

    private Coroutine phraseRoutine;

    // ===================== START =====================
    private void Start()
    {
        // Вимикаємо панелі на старті
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        if (hintBubble != null)
            hintBubble.SetActive(false);

        // АВТОСТАРТ ЧЕРЕЗ 1 СЕКУНДУ
        StartCoroutine(AutoStart());
    }

    // ===================== АВТОСТАРТ =====================
    private IEnumerator AutoStart()
    {
        yield return new WaitForSeconds(autoStartDelay);
        StartSegment();
    }

    // ===================== UPDATE =====================
    private void Update()
    {
        if (!finalSegmentOnlyByTrigger &&
        waitingForInteract && playerInside &&
        Input.GetKeyDown(KeybindManager.GetKey(KeybindManager.INTERACT)))
        {
            StartSegment();
        }

        if (walking)
        {
            Move();
        }

        if (showingHints && playerInside &&
            Input.GetKeyDown(KeybindManager.GetKey(KeybindManager.INTERACT)))
        {
            ShowNextHint();
        }
    }

    // ===================== LOGIC =====================
    private void StartSegment()
    {
        waitingForInteract = false;
        walking = true;
        showingHints = false;
        isCurrentHintUnlocked = false; // Сбрасываем при начале движения
        hintIndex = 0;

        if (hintBubble != null)
            hintBubble.SetActive(false);

        Segment s = segments[segmentIndex];

        if (anim != null)
        {
            if (s.playWalkAnimation)
                anim.SetFloat(speedParam, 1f);
            else
                anim.SetFloat(speedParam, 0f);
        }

        if (phraseRoutine != null) StopCoroutine(phraseRoutine);
        phraseRoutine = StartCoroutine(ShowPhrases(s.walkPhrases));
    }

    private void Move()
    {
        Segment s = segments[segmentIndex];
        Vector3 dir = (s.endPoint.position - transform.position).normalized;

        // Движение
        transform.position += dir * s.speed * Time.deltaTime;

        if (anim != null && s.playWalkAnimation)
            anim.SetFloat(speedParam, Mathf.Abs(dir.x));

        // Проверка, дошли ли до конца сегмента
        if (Vector3.Distance(transform.position, s.endPoint.position) < 0.1f)
        {
            walking = false;

            if (anim != null)
                anim.SetFloat(speedParam, 0);

            if (dialoguePanel != null)
                dialoguePanel.SetActive(false);

            showingHints = true;

            // ===== ДОБАВЛЯЕМ =====
            if (segmentIndex == segments.Length - 1)
            {
                // Последний сегмент — открываем путь
                if (roadController != null)
                    roadController.ActivateRoad();

                // Если используется отдельная стена
                if (finalWall != null)
                    finalWall.SetActive(false);
            }
        }
    }

    private IEnumerator ShowPhrases(TimedPhrase[] phrases)
    {
        // Фрази показуються ЗАВЖДИ під час ходіння
        if (dialoguePanel != null)
            dialoguePanel.SetActive(true);

        foreach (var p in phrases)
        {
            if (dialogueText != null)
                dialogueText.text = p.text;
            yield return new WaitForSeconds(p.duration);
        }

        // Закриваємо панель після всіх фраз
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
    }

    private void ShowNextHint()
    {
        // 1. Проверяем, разблокирована ли подсказка
        if (!isCurrentHintUnlocked)
        {
            // Пытаемся "купить" доступ к подсказкам за предмет
            if (InventorySystem.Instance.HasItem(hintItemName))
            {
                InventorySystem.Instance.RemoveItemByName(hintItemName);
                isCurrentHintUnlocked = true;
                Debug.Log("Подсказка оплачена предметом.");
                // После оплаты сразу показываем первую подсказку (идем дальше по коду)
            }
            else
            {
                // Предмета нет - выводим предупреждение
                if (hintBubble != null) hintBubble.SetActive(true);
                if (hintText != null) hintText.text = $"Нужен предмет: {hintItemName}";
                return; // Выходим, не инкрементируя hintIndex
            }
        }

        // 2. Логика показа самих подсказок (если оплачено)
        string[] hints = segments[segmentIndex].hints;

        if (hintIndex < hints.Length)
        {
            if (hintBubble != null)
                hintBubble.SetActive(true);

            if (hintText != null)
                hintText.text = hints[hintIndex];

            hintIndex++;
        }
        else
        {
            // Все подсказки показаны
            if (hintBubble != null)
                hintBubble.SetActive(false);
        }
    }

    // ===================== LEVEL MANAGER CALLS =====================
    public IEnumerator DisappearTeleportAppear(Vector3 newPos)
    {
        // Ховаємо всі UI панелі
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
        if (hintBubble != null)
            hintBubble.SetActive(false);

        // Зупиняємо анімацію перед зникненням
        if (anim != null)
            anim.SetFloat(speedParam, 0);

        // Анімація зникнення
        if (anim != null)
            anim.Play(disappearAnim);
        yield return new WaitForSeconds(disappearDuration);

        // Телепорт
        transform.position = newPos;

        // Анімація появи
        if (anim != null)
            anim.Play(appearAnim);
        yield return new WaitForSeconds(0.5f);

        // Переходимо до наступного сегмента
        segmentIndex++;
        if (segmentIndex >= segments.Length)
            segmentIndex = 0;

        // ВАЖНО
        waitingForInteract = true;   // ждём игрока
        showingHints = false;
        walking = false;

        finalSegmentOnlyByTrigger = true;
    }

    // ===================== TRIGGERS =====================
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInside = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;

            // НЕ закриваємо UI коли гравець виходить
            // Фрази та підказки продовжують показуватися
        }
    }
    public void StartCurrentSegmentExternally()
    {
        if (waitingForInteract)
        {
            StartSegment();
        }
    }
    public IEnumerator FinalDisappear()
    {
        // Отключаем все UI панели
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
        if (hintBubble != null)
            hintBubble.SetActive(false);

        // Останавливаем анимацию движения
        if (anim != null)
            anim.SetFloat(speedParam, 0);

        // Анимация исчезновения
        if (anim != null)
            anim.Play(disappearAnim);

        yield return new WaitForSeconds(disappearDuration);

        // Отключаем объект полностью
        gameObject.SetActive(false);
    }
}