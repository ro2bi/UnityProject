using UnityEngine;
using System.Collections;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    public ProfessorWalker professor;
    public GameObject closingWall;

    private bool levelPassed = false;

    private void Awake()
    {
        Instance = this;
    }

    public void OnLevelPassed(Vector3 nextProfessorPos)
    {
        if (levelPassed) return;
        levelPassed = true;

        closingWall.SetActive(false);

        StartCoroutine(LevelEndRoutine(nextProfessorPos));
    }

    private IEnumerator LevelEndRoutine(Vector3 pos)
    {
        yield return professor.DisappearTeleportAppear(pos);

        levelPassed = false;
    }
}