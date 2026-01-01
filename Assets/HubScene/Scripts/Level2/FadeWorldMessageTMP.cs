using System.Collections;
using TMPro;
using UnityEngine;

public class FadeWorldMessageTMP : MonoBehaviour
{
    // Скільки часу текст буде повністю видимим
    [SerializeField] private float visibleTime = 1f;

    // Скільки часу буде тривати плавне зникнення
    [SerializeField] private float fadeTime = 1f;

    // Посилання на компонент TextMeshPro
    private TMP_Text tmp;

    // Посилання на поточну корутину
    // Потрібно щоб зупиняти попереднє зникнення
    private Coroutine routine;

    private void Awake()
    {
        // Отримуємо компонент TextMeshPro з цього об’єкта
        tmp = GetComponent<TMP_Text>();

        // На старті гри текст не повинен бути видимим
        // Тому очищаємо текст і робимо його прозорим
        if (tmp != null)
        {
            tmp.text = "";
            SetAlpha(0f);
        }
    }

    public void Show(string message)
    {
        // Цей метод викликається коли потрібно показати повідомлення
        // Наприклад коли гравець намагається вийти за межі значень

        // Якщо компонент тексту не знайдений нічого не робимо
        if (tmp == null) return;

        // Якщо попередня анімація ще триває
        // Ми її зупиняємо щоб не було накладання
        if (routine != null)
            StopCoroutine(routine);

        // Встановлюємо новий текст повідомлення
        tmp.text = message;

        // Робимо текст повністю видимим
        SetAlpha(1f);

        // Запускаємо корутину плавного зникнення
        routine = StartCoroutine(FadeRoutine());
    }

    private IEnumerator FadeRoutine()
    {
        // Спочатку чекаємо певний час
        // В цей момент текст просто відображається
        yield return new WaitForSeconds(visibleTime);

        float t = 0f;

        // Поступово зменшуємо прозорість тексту
        // Поки не мине весь час зникнення
        while (t < fadeTime)
        {
            // Збільшуємо таймер
            t += Time.deltaTime;

            // Обчислюємо нову прозорість
            float alpha = Mathf.Lerp(1f, 0f, t / fadeTime);

            // Встановлюємо прозорість
            SetAlpha(alpha);

            // Чекаємо наступний кадр
            yield return null;
        }

        // Після завершення робимо текст повністю прозорим
        SetAlpha(0f);

        // Обнуляємо посилання на корутину
        routine = null;
    }

    private void SetAlpha(float value)
    {
        // Цей метод змінює прозорість кольору тексту

        // Беремо поточний колір
        Color c = tmp.color;

        // Міняємо тільки альфа канал
        c.a = value;

        // Повертаємо оновлений колір назад у текст
        tmp.color = c;
    }
}
