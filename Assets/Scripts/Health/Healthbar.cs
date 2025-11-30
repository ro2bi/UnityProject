using System.Collections;
using System.Collections.Generic;
/*using Microsoft.Unity.VisualStudio.Editor; - убрал. Оно не нужно для работы кода — этот неймспейс используется только внутри редактора Visual Studio, 
чтобы интеграция с Unity работала (например, для автоматического открытия скриптов).
В игровом коде это бесполезно — он не содержит ничего, связанного с UI, логикой, MonoBehaviour и т.д.
Иногда он может вызывать конфликты или лишние зависимости.*/
using UnityEngine;
using UnityEngine.UI; // - добавил, чтобы использовать UI элементы, такие как Image

public class Healthbar : MonoBehaviour
{
    [SerializeField] private Health playerhealth;
    // playerhealth - ссылка на компонент Health, который отвечает за здоровье игрока
    [SerializeField] private Image totalhealthBar;
    // totalhealthBar - ссылка на UI элемент Image, который отображает полное здоровье игрока
    [SerializeField] private Image currenthealthBar;
    // currenthealthBar - ссылка на UI элемент Image, который отображает текущее здоровье игрока

    private void Start()
    {
        totalhealthBar.fillAmount = playerhealth.currentHealth / 10;
        // Инициализируем заполнение полной полоски здоровья в зависимости от начального здоровья игрока
    }

    private void Update()
    {
        currenthealthBar.fillAmount = playerhealth.currentHealth / 10;
        // Обновляем заполнение текущей полоски здоровья в зависимости от текущего здоровья игрок
    }
}
