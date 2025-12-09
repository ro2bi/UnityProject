using UnityEngine;
using System.Collections;
using TMPro;

public class ProfessorWalker : MonoBehaviour
{
    Animator anim;

    [Header("Точки маршруту")]
    public Transform[] points;
    public float speed = 1f;

    [Header("Анімації")]
    public string disappearAnimation = "Disappear";
    public string walkHorizontalParam = "Speed";

    [Header("Очікування перед зникненням")]
    public float waitBeforeDisappear = 1.5f;

    [Header("Діалоги для кожної точки")]
    [Tooltip("Кожен елемент - це масив фраз для однієї точки")]
    public DialogueSet[] dialoguesForPoints;

    [Header("Посилання на TextDisplay")]
    public GameObject textDisplayObject;
    public TextMeshProUGUI textMeshPro; // TMP компонент для тексту
    public CanvasGroup canvasGroup; // Для fade ефекту

    [Header("Налаштування fade")]
    public float fadeInDuration = 0.5f;
    public float fadeOutDuration = 0.3f;

    private int currentPoint = 0;
    private bool isWaiting = true;
    private bool playerInside = false;
    private bool isDisappearing = false;
    private bool isShowingDialogue = false;

    private Coroutine dialogueCoroutine;

    [System.Serializable]
    public class DialogueSet
    {
        [TextArea(2, 5)]
        public string[] phrases;
        [Tooltip("Час показу для кожної фрази (в секундах)")]
        public float[] displayTimes;
    }

    private void Start()
    {
        anim = GetComponent<Animator>();

        // Якщо CanvasGroup не призначений, спробуємо знайти або створити
        if (canvasGroup == null && textDisplayObject != null)
        {
            canvasGroup = textDisplayObject.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = textDisplayObject.AddComponent<CanvasGroup>();
            }
        }

        // Ховаємо панельку на старті
        if (textDisplayObject != null)
        {
            textDisplayObject.SetActive(false);
        }
    }

    void Update()
    {
        if (points.Length == 0) return;
        if (isDisappearing) return;

        // Очікування взаємодії
        if (isWaiting)
        {
            anim.SetFloat(walkHorizontalParam, 0);
            if (playerInside && Input.GetKeyDown(KeybindManager.GetKey(KeybindManager.INTERACT)))
            {
                isWaiting = false;
                // Запускаємо послідовний показ фраз
                StartDialogueSequence();
            }
            return;
        }

        Transform target = points[currentPoint];
        Vector3 direction = (target.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;

        anim.SetFloat(walkHorizontalParam, Mathf.Abs(direction.x) > 0.01f ? speed : 0);

        if (direction.x > 0.1f)
            transform.localScale = new Vector3(1, 1, 1);
        else if (direction.x < -0.1f)
            transform.localScale = new Vector3(-1, 1, 1);

        // Досягнення точки
        if (Vector3.Distance(transform.position, target.position) < 0.1f)
        {
            // Зупиняємо діалог
            if (dialogueCoroutine != null)
            {
                StopCoroutine(dialogueCoroutine);
                dialogueCoroutine = null;
            }

            HideDialogue();

            // Остання точка → чекаємо → зникаємо
            if (currentPoint == points.Length - 1)
            {
                StartCoroutine(DisappearAfterDelay());
                return;
            }

            isWaiting = true;
            currentPoint++;
            if (currentPoint >= points.Length)
                currentPoint = 0;
        }
    }

    private void StartDialogueSequence()
    {
        if (textDisplayObject == null || dialoguesForPoints == null) return;
        if (currentPoint >= dialoguesForPoints.Length) return;

        DialogueSet currentSet = dialoguesForPoints[currentPoint];
        if (currentSet == null || currentSet.phrases == null || currentSet.phrases.Length == 0) return;

        dialogueCoroutine = StartCoroutine(ShowPhrasesSequentially(currentSet.phrases));
    }

    private IEnumerator ShowPhrasesSequentially(string[] phrases)
    {
        isShowingDialogue = true;

        // Якщо TMP не призначений, спробуємо знайти
        if (textMeshPro == null && textDisplayObject != null)
        {
            textMeshPro = textDisplayObject.GetComponentInChildren<TextMeshProUGUI>();
        }

        if (textMeshPro == null)
        {
            Debug.LogError("TextMeshProUGUI не знайдено! Перетягни TMP компонент в поле textMeshPro");
            yield break;
        }

        DialogueSet currentSet = dialoguesForPoints[currentPoint];

        for (int i = 0; i < phrases.Length; i++)
        {
            string phrase = phrases[i];
            if (string.IsNullOrEmpty(phrase)) continue;

            // Fade out (якщо не перша фраза)
            if (i > 0 && canvasGroup != null)
            {
                yield return StartCoroutine(FadeOut());
            }

            // ВАЖЛИВО: Змінюємо текст ПОКИ панелька прозора
            textMeshPro.text = phrase;

            // Активуємо панельку (якщо перша фраза)
            if (i == 0 && textDisplayObject != null)
            {
                textDisplayObject.SetActive(true);
            }

            // Fade in
            if (canvasGroup != null)
            {
                yield return StartCoroutine(FadeIn());
            }

            Debug.Log($"Показую фразу: {phrase}");

            // Отримуємо час для поточної фрази
            float waitTime = 3f; // час за замовчуванням
            if (currentSet.displayTimes != null && i < currentSet.displayTimes.Length)
            {
                waitTime = currentSet.displayTimes[i];
            }

            // Чекаємо поки фраза відображається
            yield return new WaitForSeconds(waitTime);
        }

        // Fade out після останньої фрази
        if (canvasGroup != null)
        {
            yield return StartCoroutine(FadeOut());
        }

        // Деактивуємо панельку
        HideDialogue();
    }

    private IEnumerator FadeIn()
    {
        float elapsed = 0f;
        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeInDuration);
            yield return null;
        }
        canvasGroup.alpha = 1f;
    }

    private IEnumerator FadeOut()
    {
        float elapsed = 0f;
        float startAlpha = canvasGroup.alpha;
        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, elapsed / fadeOutDuration);
            yield return null;
        }
        canvasGroup.alpha = 0f;
    }

    private void HideDialogue()
    {
        if (textDisplayObject != null)
        {
            textDisplayObject.SetActive(false);
        }
        if (textMeshPro != null)
        {
            textMeshPro.text = "";
        }
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
        }
        isShowingDialogue = false;
    }

    private IEnumerator DisappearAfterDelay()
    {
        isDisappearing = true;
        anim.SetFloat(walkHorizontalParam, 0);

        // Зупиняємо діалог
        if (dialogueCoroutine != null)
        {
            StopCoroutine(dialogueCoroutine);
            dialogueCoroutine = null;
        }

        HideDialogue();

        yield return new WaitForSeconds(waitBeforeDisappear);
        anim.Play(disappearAnimation);

        enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInside = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInside = false;
    }
}