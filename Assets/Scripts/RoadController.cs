using UnityEngine;

public class RoadController : MonoBehaviour
{
    public GameObject road;
    public GameObject wall;

    private void Start()
    {
        // На старті дорога вимкнена
        road.SetActive(false);

        // На старті двері вимкнені
        wall.SetActive(false);
    }

    public void ActivateRoad()
    {
        road.SetActive(true);
        wall.SetActive(false);
    }

    public void ReplaceRoadWithWall()
    {
        road.SetActive(false);
        wall.SetActive(true);
    }
}
