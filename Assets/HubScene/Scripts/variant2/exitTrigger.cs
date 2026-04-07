using UnityEngine;

public class LevelExitTrigger : MonoBehaviour
{
    [Header("���� ��� ����������")]
    public GameObject closingWall;

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (closingWall != null)
            {
                closingWall.SetActive(true);
            }
        }
    }
}