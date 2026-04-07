using UnityEngine;
using System.Collections;
using TMPro;

public class ProfessorWalker : MonoBehaviour
{
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
        public float speed = 1.5f;
        public bool playWalkAnimation = true;

        [Header("Фрази під час руху")]
        public TimedPhrase[] walkPhrases;

        [Header("Підказки після зупинки")]
        [TextArea] public string[] hints;
    }

    [Header("Сегменти (0→1, 2→3, 4→5 …)")]
    public Segment[] segments;

    [Header("Автостарт")]
    public float autoStartDelay = 1f;

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
    public string hintItemName = "Книга Знаний";
    private bool isCurrentHintUnlocked = false;

    private int segmentIndex = 0;
    private bool waitingForInteract = true;
    private bool walking = false;
    private bool showingHints = false;
    private int hintIndex = 0;
    private bool playerInside = false;

    private Coroutine phraseRoutine;

    private void Start()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        if (hintBubble != null)
            hintBubble.SetActive(false);

        StartCoroutine(AutoStart());
    }

    private IEnumerator AutoStart()
    {
        yield return new WaitForSeconds(autoStartDelay);
        StartSegment();
    }

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

    private void StartSegment()
    {
        waitingForInteract = false;
        walking = true;
        showingHints = false;
        isCurrentHintUnlocked = false;
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

        transform.position += dir * s.speed * Time.deltaTime;

        if (anim != null && s.playWalkAnimation)
            anim.SetFloat(speedParam, Mathf.Abs(dir.x));

        if (Vector3.Distance(transform.position, s.endPoint.position) < 0.1f)
        {
            walking = false;

            if (anim != null)
                anim.SetFloat(speedParam, 0);

            if (dialoguePanel != null)
                dialoguePanel.SetActive(false);

            showingHints = true;

            if (segmentIndex == segments.Length - 1)
            {
                if (roadController != null)
                    roadController.ActivateRoad();

                if (finalWall != null)
                    finalWall.SetActive(false);
            }
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

    private void ShowNextHint()
    {
        if (!isCurrentHintUnlocked)
        {
            if (InventorySystem.Instance.HasItem(hintItemName))
            {
                InventorySystem.Instance.RemoveItemByName(hintItemName);
                isCurrentHintUnlocked = true;
                Debug.Log("Подсказка оплачена предметом.");
            }
            else
            {
                if (hintBubble != null) hintBubble.SetActive(true);
                if (hintText != null) hintText.text = $"Нужен предмет: {hintItemName}";
                return;
            }
        }

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
            if (hintBubble != null)
                hintBubble.SetActive(false);
        }
    }

    public IEnumerator DisappearTeleportAppear(Vector3 newPos)
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
        if (hintBubble != null)
            hintBubble.SetActive(false);

        if (anim != null)
            anim.SetFloat(speedParam, 0);

        if (anim != null)
            anim.Play(disappearAnim);
        yield return new WaitForSeconds(disappearDuration);

        transform.position = newPos;

        if (anim != null)
            anim.Play(appearAnim);
        yield return new WaitForSeconds(0.5f);

        segmentIndex++;
        if (segmentIndex >= segments.Length)
            segmentIndex = 0;

        waitingForInteract = true;
        showingHints = false;
        walking = false;

        finalSegmentOnlyByTrigger = true;
    }

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
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
        if (hintBubble != null)
            hintBubble.SetActive(false);

        if (anim != null)
            anim.SetFloat(speedParam, 0);

        if (anim != null)
            anim.Play(disappearAnim);

        yield return new WaitForSeconds(disappearDuration);

        gameObject.SetActive(false);
    }
}