using System.Collections;
using TMPro;
using UnityEngine;

public class FadeWorldMessageTMP : MonoBehaviour
{
    [SerializeField] private float visibleTime = 1f;

    [SerializeField] private float fadeTime = 1f;

    private TMP_Text tmp;

    private Coroutine routine;

    private void Awake()
    {
        tmp = GetComponent<TMP_Text>();

        if (tmp != null)
        {
            tmp.text = "";
            SetAlpha(0f);
        }
    }

    public void Show(string message)
    {
        if (tmp == null) return;

        if (routine != null)
            StopCoroutine(routine);

        tmp.text = message;

        SetAlpha(1f);

        routine = StartCoroutine(FadeRoutine());
    }

    private IEnumerator FadeRoutine()
    {
        yield return new WaitForSeconds(visibleTime);

        float t = 0f;

        while (t < fadeTime)
        {
            t += Time.deltaTime;

            float alpha = Mathf.Lerp(1f, 0f, t / fadeTime);

            SetAlpha(alpha);

            yield return null;
        }

        SetAlpha(0f);

        routine = null;
    }

    private void SetAlpha(float value)
    {
        Color c = tmp.color;

        c.a = value;

        tmp.color = c;
    }
}