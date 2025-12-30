using UnityEngine;

public class ReplaceRoadTrigger : MonoBehaviour
{
    public RoadController controller;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        controller.ReplaceRoadWithWall();
        gameObject.SetActive(false);
    }
}