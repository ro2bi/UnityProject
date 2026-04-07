using UnityEngine;

public class TabletInteractAction : MonoBehaviour, IInteractAction
{
    [SerializeField] private TabletWorldSimple tabletWorld;

    public void Execute()
    {
        if (tabletWorld == null) return;

        if (tabletWorld.IsOpen) return;

        tabletWorld.UseTablet();
    }
}
