using UnityEngine;
using System.Collections;

public class WeightObject : MonoBehaviour
{
    public enum ItemType { Number, Operator }
    public ItemType type;
    public int numericValue;
    public string operatorSymbol;

    [Header("Old Systems (Don't delete!)")]
    public float weight;
    public bool lockSortingOrder;

    [Header("Visibility Settings")]
    public bool revealOnlyInSlot = false;
    public GameObject visualContent;

    private bool hasBeenRevealed = false;

    [HideInInspector] public Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position;

        if (revealOnlyInSlot && visualContent != null)
            visualContent.SetActive(false);
        else if (visualContent != null)
            visualContent.SetActive(true);
    }

    public void SetVisibility(bool isVisible)
    {
        if (!revealOnlyInSlot) return;

        if (isVisible)
        {
            hasBeenRevealed = true;
            if (visualContent != null) visualContent.SetActive(true);
        }
        else
        {
            if (!hasBeenRevealed && visualContent != null)
            {
                visualContent.SetActive(false);
            }
        }
    }

    public void ReturnToStart()
    {
        StopAllCoroutines();
        StartCoroutine(MoveToPos(startPosition));
    }

    public IEnumerator MoveToPos(Vector3 target)
    {
        float t = 0;
        Vector3 start = transform.position;
        while (t < 1)
        {
            t += Time.deltaTime * 5;
            transform.position = Vector3.Lerp(start, target, t);
            yield return null;
        }
        transform.position = target;
    }
}