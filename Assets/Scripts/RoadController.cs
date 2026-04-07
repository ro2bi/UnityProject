using UnityEngine;

public class RoadController : MonoBehaviour
{
    public GameObject road;
    public GameObject wall;

    private void Start()
    {
        road.SetActive(false);

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
