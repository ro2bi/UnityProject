using UnityEngine;
using System.Collections;
using TMPro;

public class ProfessorWalker : MonoBehaviour
{
    [Header("Рух")]
    public Transform[] points;
    public float speed = 1f;

    [Header("Анімації")]
    public Animator anim;
    public string walkHorizontalParam = "Speed";
    public string disappearAnimation = "Disappear";
    public string appearAnimation = "Appear";
    [Tooltip("Тривалість анімації зникнення")]
    public float disappearDuration = 0.7f;

    [System.Serializable]
    public class DialogueSet
    {
        [TextArea(2, 5)]
        public string[] phrases;
        public float[] displayTimes;
    }

    [System.Serializable]
    public class HintSet
    {
        [TextArea(2, 5)]
        public string[] hints;
    }

    [Header("Діалоги для кожної точки")]
    public DialogueSet[] dialoguesForPoints;

    [Header("Підказки для кожної точки")]
    public HintSet[] hintsForPoints;

    [Header("UI елементи")]
    public GameObject textDisplayObject; // табличка внизу екрану для діалогів під час руху
    public TextMeshProUGUI textMeshPro;
    public CanvasGroup canvasGroup;

    [Header("Табличка підказок біля професора")]
    public GameObject hintBubbleObject; // табличка що буде біля професора
    public TextMeshProUGUI hintBubbleText;
    public CanvasGroup hintBubbleCanvasGroup;

    [Header("Підказки на фінальній точці")]
    [TextArea(2, 5)] public string repeatHint1 = "Щоб пройти рівень, зроби перше завдання...";
    [TextArea(2, 5)] public string repeatHint2 = "Тепер зверни увагу на друге завдання...";
    public int hintLevel = 2; // рівень, на якому активні підказки

    [Header("Логіка рівнів")]
    public int currentLevel = 1;

    private int currentPoint = 0;
    private bool isWaiting = true;
    private bool reachedFinalPoint = false;
    private int hintStep = 0;
    private bool playerInside = false;
    private int currentHintIndex = 0;
    private bool isAtPoint = false;

    private Coroutine dialogueCoroutine;

    private void Start()
    {
        if (anim == null) anim = GetComponent<Animator>();
        if (canvasGroup == null && textDisplayObject != null)
            canvasGroup = textDisplayObject.GetComponent<CanvasGroup>();
        if (hintBubbleCanvasGroup == null && hintBubbleObject != null)
            hintBubbleCanvasGroup = hintBubbleObject.GetComponent<CanvasGroup>();

        if (textDisplayObject != null)
            textDisplayObject.SetActive(false);

        if (hintBubbleObject != null)
            hintBubbleObject.SetActive(false);
    }

    private void Update()
    {
        if (points.Length == 0) return;

        if (reachedFinalPoint)
        {
            WaitForInteraction();
            return;
        }

        // Якщо професор стоїть на точці і чекає взаємодії
        if (isAtPoint && playerInside && Input.GetKeyDown(KeybindManager.GetKey(KeybindManager.INTERACT)))
        {
            ShowNextHintAtPoint();
            return;
        }

        // Якщо професор чекає початку руху
        if (isWaiting && !isAtPoint && playerInside && Input.GetKeyDown(KeybindManager.GetKey(KeybindManager.INTERACT)))
        {
            isWaiting = false;
            StartDialogueForPoint(currentPoint);
        }

        if (!isWaiting && !isAtPoint)
        {
            MoveAlongPath();
        }
    }

    private void MoveAlongPath()
    {
        Transform target = points[currentPoint];
        Vector3 dir = (target.position - transform.position).normalized;
        transform.position += dir * speed * Time.deltaTime;

        anim.SetFloat(walkHorizontalParam, Mathf.Abs(dir.x));

        if (dir.x > 0.01f) transform.localScale = new Vector3(1, 1, 1);
        else if (dir.x < -0.01f) transform.localScale = new Vector3(-1, 1, 1);

        // Досягнення точки
        if (Vector3.Distance(transform.position, target.position) < 0.1f)
        {
            StopDialogue();
            anim.SetFloat(walkHorizontalParam, 0);

            // Перевіряємо чи є підказки для цієї точки
            if (hintsForPoints != null && currentPoint < hintsForPoints.Length &&
                hintsForPoints[currentPoint].hints.Length > 0)
            {
                isAtPoint = true;
                currentHintIndex = 0;
                // Не показуємо підказку автоматично, чекаємо натискання клавіші
            }
            else
            {
                // Якщо підказок немає, переходимо до наступної точки
                MoveToNextPoint();
            }
        }
    }

    private void ShowNextHintAtPoint()
    {
        if (!isAtPoint) return;

        HintSet hintSet = hintsForPoints[currentPoint];

        if (currentHintIndex < hintSet.hints.Length)
        {
            // Показуємо наступну підказку
            ShowHintBubble(hintSet.hints[currentHintIndex]);
            currentHintIndex++;
        }
        else
        {
            // Всі підказки показані, переходимо далі
            HideHintBubble();
            isAtPoint = false;
            MoveToNextPoint();
        }
    }

    private void MoveToNextPoint()
    {
        currentPoint++;
        if (currentPoint >= points.Length)
        {
            reachedFinalPoint = true;
            isWaiting = true;
        }
        else
        {
            isWaiting = true;
        }
    }

    private void ShowHintBubble(string hint)
    {
        if (hintBubbleObject == null || hintBubbleText == null) return;

        hintBubbleObject.SetActive(true);
        hintBubbleText.text = hint;

        if (hintBubbleCanvasGroup != null)
            hintBubbleCanvasGroup.alpha = 1f;
    }

    private void HideHintBubble()
    {
        if (hintBubbleObject == null) return;

        if (hintBubbleCanvasGroup != null)
            hintBubbleCanvasGroup.alpha = 0f;

        hintBubbleObject.SetActive(false);
    }

    private void StartDialogueForPoint(int pointIndex)
    {
        if (dialogueCoroutine != null) StopCoroutine(dialogueCoroutine);

        if (dialoguesForPoints != null && pointIndex < dialoguesForPoints.Length)
        {
            DialogueSet set = dialoguesForPoints[pointIndex];
            if (set != null && set.phrases.Length > 0)
                dialogueCoroutine = StartCoroutine(ShowPhrasesSequentially(set));
        }
    }

    private IEnumerator ShowPhrasesSequentially(DialogueSet set)
    {
        if (textDisplayObject != null) textDisplayObject.SetActive(true);
        for (int i = 0; i < set.phrases.Length; i++)
        {
            if (textMeshPro != null) textMeshPro.text = set.phrases[i];
            float waitTime = (set.displayTimes != null && i < set.displayTimes.Length) ? set.displayTimes[i] : 3f;
            yield return new WaitForSeconds(waitTime);
        }
        if (textDisplayObject != null) textDisplayObject.SetActive(false);
    }

    private void StopDialogue()
    {
        if (dialogueCoroutine != null)
        {
            StopCoroutine(dialogueCoroutine);
            dialogueCoroutine = null;
        }
        if (textDisplayObject != null) textDisplayObject.SetActive(false);
    }

    private void WaitForInteraction()
    {
        anim.SetFloat(walkHorizontalParam, 0);
        if (playerInside && currentLevel == hintLevel && Input.GetKeyDown(KeybindManager.GetKey(KeybindManager.INTERACT)))
        {
            ShowNextHint();
        }
    }

    private void ShowNextHint()
    {
        if (hintBubbleText == null || hintBubbleObject == null) return;
        hintBubbleObject.SetActive(true);
        if (hintStep == 0)
        {
            hintBubbleText.text = repeatHint1;
            hintStep = 1;
        }
        else if (hintStep == 1)
        {
            hintBubbleText.text = repeatHint2;
            hintStep = 2;
        }
        if (hintBubbleCanvasGroup != null) hintBubbleCanvasGroup.alpha = 1f;
    }

    public void HideHint()
    {
        HideHintBubble();
    }

    public void TeleportProfessor(Vector3 newPosition)
    {
        StartCoroutine(TeleportRoutine(newPosition));
    }

    private IEnumerator TeleportRoutine(Vector3 newPos)
    {
        PlayDisappearAnimation();
        yield return new WaitForSeconds(disappearDuration);

        transform.position = newPos;

        if (!string.IsNullOrEmpty(appearAnimation))
        {
            anim.Play(appearAnimation);
            yield return new WaitForSeconds(0.7f);
        }

        reachedFinalPoint = true;
        isWaiting = true;
        hintStep = 0;
        HideHint();
    }

    public void PlayDisappearAnimation()
    {
        if (!string.IsNullOrEmpty(disappearAnimation) && anim != null)
            anim.Play(disappearAnimation);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) playerInside = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) playerInside = false;
    }

    public void ResumeWalking()
    {
        if (reachedFinalPoint)
        {
            reachedFinalPoint = false;
            isWaiting = false;
            StartDialogueForPoint(currentPoint);
        }
    }
}