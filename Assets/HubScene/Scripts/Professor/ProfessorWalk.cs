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

        [Header("Фрази під час руху")]
        public TimedPhrase[] walkPhrases;

        [Header("Підказки після зупинки")]
        [TextArea] public string[] hints;
    }

    [Header("Сегменти (0→1, 2→3, 4→5 …)")]
    public Segment[] segments;

    [Header("Рух")]
    public float speed = 1.5f;
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
        // ✅ Вимикаємо панелі на старті
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        if (hintBubble != null)
            hintBubble.SetActive(false);
    }

    // ===================== UPDATE =====================
    private void Update()
    {
        if (waitingForInteract && playerInside &&
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
        hintIndex = 0;

        // ✅ ХОВАЄМО підказки при старті ходіння
        if (hintBubble != null)
            hintBubble.SetActive(false);

        Segment s = segments[segmentIndex];
        transform.position = s.startPoint.position;

        if (phraseRoutine != null) StopCoroutine(phraseRoutine);
        phraseRoutine = StartCoroutine(ShowPhrases(s.walkPhrases));
    }

    private void Move()
    {
        Segment s = segments[segmentIndex];
        Vector3 dir = (s.endPoint.position - transform.position).normalized;
        transform.position += dir * speed * Time.deltaTime;

        anim.SetFloat(speedParam, Mathf.Abs(dir.x));

        if (Vector3.Distance(transform.position, s.endPoint.position) < 0.1f)
        {
            walking = false;
            anim.SetFloat(speedParam, 0);
            StopDialogue();

            showingHints = true;
        }
    }

    private IEnumerator ShowPhrases(TimedPhrase[] phrases)
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(true);

        foreach (var p in phrases)
        {
            if (dialogueText != null)
                dialogueText.text = p.text;
            yield return new WaitForSeconds(p.duration);
        }

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
    }

    private void StopDialogue()
    {
        if (phraseRoutine != null)
            StopCoroutine(phraseRoutine);

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
    }

    private void ShowNextHint()
    {
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
            // Всі підказки показані - ховаємо панель
            if (hintBubble != null)
                hintBubble.SetActive(false);
        }
    }

    // ===================== LEVEL MANAGER CALLS =====================
    public IEnumerator DisappearTeleportAppear(Vector3 newPos)
    {
        // ✅ Ховаємо всі UI панелі
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
        if (hintBubble != null)
            hintBubble.SetActive(false);

        anim.Play(disappearAnim);
        yield return new WaitForSeconds(disappearDuration);

        transform.position = newPos;

        anim.Play(appearAnim);
        yield return new WaitForSeconds(0.5f);

        segmentIndex++;
        if (segmentIndex >= segments.Length)
            segmentIndex = 0;

        waitingForInteract = true;
        showingHints = false;
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

            // ✅ Ховаємо UI коли гравець виходить
            if (hintBubble != null)
                hintBubble.SetActive(false);
            if (dialoguePanel != null)
                dialoguePanel.SetActive(false);
        }
    }
}