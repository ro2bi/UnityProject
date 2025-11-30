using UnityEngine;
using UnityEngine.UI;

public class Vsync : MonoBehaviour
{

    public void SetVSync(bool isVSyncEnabled)
    {
        QualitySettings.vSyncCount = isVSyncEnabled ? 1 : 0;
        
        if (isVSyncEnabled)
        {
            Application.targetFrameRate = -1; // -1 означает, что FPS не ограничен движком
        }
    }
}