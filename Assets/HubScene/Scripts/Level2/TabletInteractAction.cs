using UnityEngine;

public class TabletInteractAction : MonoBehaviour, IInteractAction
{
    [SerializeField] private TabletWorldSimple tabletWorld;

    public void Execute()
    {
        // Відкриваємо меню планшета
        if (tabletWorld != null)
            tabletWorld.UseTablet();
    }
}
