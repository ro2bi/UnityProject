using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScalePlatform : MonoBehaviour
{
    public ScaleController scale;

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.TryGetComponent(out WeightObject obj))
            scale.SetObject(obj);
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.TryGetComponent(out WeightObject obj))
            scale.RemoveObject();
    }
}
