using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleController : MonoBehaviour
{
    public TMPro.TextMeshPro weightText;
    public ButtonController button;

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
        weightText.text = $"Bрур: {effectiveWeight} H";

        button.CheckWeight(effectiveWeight);
    }

    public void SetGravity(float newG)
    {
        gravityValue = newG;
        if (currentObj != null)
            UpdateScale();
    }
}
