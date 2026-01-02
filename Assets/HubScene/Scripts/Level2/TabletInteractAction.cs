using UnityEngine;

public class TabletInteractAction : MonoBehaviour, IInteractAction
{
    [SerializeField] private TabletWorldSimple tabletWorld;

    public void Execute()
    {
        // Якщо посилання не задане нічого не робимо
        if (tabletWorld == null) return;

        // Якщо планшет вже відкритий повторно не відкриваємо
        if (tabletWorld.IsOpen) return;

        tabletWorld.UseTablet();
    }
}
