using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonTrigger : MonoBehaviour
{
    public ButtonController button;
    public float gravityValue = 0;
    private WeightObject currentObj;

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.TryGetComponent(out WeightObject obj))
        {
            currentObj = obj;
            float force = obj.weight * gravityValue;
            button.CheckWeight(force);
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.TryGetComponent(out WeightObject obj) && obj == currentObj)
        {
            currentObj = null;
            button.CheckWeight(0);
        }
    }

    public void SetGravity(float g)
    {
        gravityValue = g;
        if (currentObj != null)
        {
            float force = currentObj.weight * gravityValue;
            button.CheckWeight(force);
        }
        else
        {
            button.CheckWeight(0);
        }
    }
}