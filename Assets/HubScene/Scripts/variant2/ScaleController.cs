using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleController : MonoBehaviour
{
    public TMPro.TextMeshPro weightText;
    public ButtonController button;
    public ButtonTrigger buttonTrigger; // ← ДОБАВЬ ЭТО

    private WeightObject currentObj;
    public float gravityValue = 10f;

    public void SetObject(WeightObject obj)
    {
        currentObj = obj;
        UpdateScale();
    }

    public void RemoveObject()
    {
        currentObj = null;
        weightText.text = "";
    }

    void UpdateScale()
    {
        float effectiveWeight = currentObj.weight * gravityValue;
        weightText.text = $"Bага: {effectiveWeight} H";
    }

    public void SetGravity(float newG)
    {
        gravityValue = newG;

        // Устанавливаем g для кнопки тоже
        if (buttonTrigger != null)
            buttonTrigger.SetGravity(newG);

        if (currentObj != null)
            UpdateScale();
    }
}