using UnityEngine;

public class EquationManager : MonoBehaviour
{
    public EquationSlot[] allSlots; // Перетащи сюда все слоты уровня в инспекторе

    public void CheckFullEquation()
    {
        bool allDone = true;

        foreach (var slot in allSlots)
        {
            if (!slot.isCorrect)
            {
                allDone = false;
                break;
            }
        }

        if (allDone)
        {
            Debug.Log("УРАВНЕНИЕ РЕШЕНО! Армия побеждает!");
            // Здесь вызывай код победы, анимацию армии или переход на след. уровень
        }
    }
}