using UnityEngine;
using System.Collections; // Нужно для корутин

[RequireComponent(typeof(Rigidbody2D))]
public class WeightObject : MonoBehaviour
{

    public bool lockSortingOrder = false;

    public enum ItemType { Number, Operator }
    [Header("Type Settings")]
    public ItemType type = ItemType.Number;
    public int numericValue;
    public string operatorSymbol;

    [Header("Physics")]
    public float weight = 1f;

    [HideInInspector] public Vector3 startPosition;

    private void Start()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.mass = weight;
        startPosition = transform.position;
    }

    public void ReturnToStart()
    {
        transform.SetParent(null);
        // Вместо мгновенного перемещения используем плавное, если хотим
        StopAllCoroutines();
        StartCoroutine(MoveToPos(startPosition));

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.isKinematic = false;
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = true;
    }

    // Плавное перемещение к новой точке
    public IEnumerator MoveToPos(Vector3 targetPos)
    {
        float elapsed = 0;
        float duration = 0.5f; // Полдюжины секунд на переезд
        Vector3 startPos = transform.position;

        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPos;
        startPosition = targetPos; // Обновляем "базу"
    }
}