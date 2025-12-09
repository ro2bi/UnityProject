using UnityEngine;

public class PressureButton : MonoBehaviour
{
    [Header("Button Settings")]
    public float targetWeight = 750f; // Целевой вес
    public float tolerance = 50f; // Допуск
    public float breakWeight = 1000f; // Вес, при котором кнопка ломается

    [Header("References")]
    public Scale scale;
    public Material normalMaterial;
    public Material pressedMaterial;
    public Material brokenMaterial;

    [Header("Visual")]
    public Transform buttonVisual; // Визуальная часть кнопки
    public float pressDepth = 0.1f; // Глубина нажатия

    private enum ButtonState { Normal, Pressed, Broken }
    private ButtonState currentState = ButtonState.Normal;

    private Vector3 originalPosition;
    private Renderer buttonRenderer;

    void Start()
    {
        buttonRenderer = buttonVisual.GetComponent<Renderer>();
        originalPosition = buttonVisual.localPosition;
    }

    void Update()
    {
        CheckWeight();
    }

    void CheckWeight()
    {
        if (scale == null || currentState == ButtonState.Broken)
            return;

        float weight = scale.GetCurrentWeight();

        if (weight >= breakWeight)
        {
            // Кнопка ломается
            BreakButton();
        }
        else if (weight >= targetWeight - tolerance && weight <= targetWeight + tolerance)
        {
            // Кнопка нажата правильно
            PressButton();
        }
        else
        {
            // Кнопка в обычном состоянии
            ResetButton();
        }
    }

    void PressButton()
    {
        if (currentState != ButtonState.Pressed)
        {
            currentState = ButtonState.Pressed;

            // Визуальное нажатие
            buttonVisual.localPosition = originalPosition - new Vector3(0, pressDepth, 0);

            if (buttonRenderer != null && pressedMaterial != null)
                buttonRenderer.material = pressedMaterial;

            Debug.Log("Кнопка нажата правильно!");
            OnButtonPressed();
        }
    }

    void ResetButton()
    {
        if (currentState == ButtonState.Pressed)
        {
            currentState = ButtonState.Normal;

            // Возвращаем кнопку
            buttonVisual.localPosition = originalPosition;

            if (buttonRenderer != null && normalMaterial != null)
                buttonRenderer.material = normalMaterial;
        }
    }

    void BreakButton()
    {
        if (currentState != ButtonState.Broken)
        {
            currentState = ButtonState.Broken;

            if (buttonRenderer != null && brokenMaterial != null)
                buttonRenderer.material = brokenMaterial;

            Debug.Log("Кнопка сломалась!");
            OnButtonBroken();
        }
    }

    void OnButtonPressed()
    {
        // Здесь можно добавить логику при правильном нажатии
        // Например, открыть дверь, активировать что-то и т.д.
    }

    void OnButtonBroken()
    {
        // Здесь можно добавить эффекты поломки
    }

    // Для сброса кнопки (можно вызвать из другого скрипта)
    public void RepairButton()
    {
        currentState = ButtonState.Normal;
        buttonVisual.localPosition = originalPosition;

        if (buttonRenderer != null && normalMaterial != null)
            buttonRenderer.material = normalMaterial;
    }
}