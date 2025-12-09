//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class ProfessorTalk : MonoBehaviour
//{
//    [TextArea(2, 5)]
//    public string dialogText = "Привет, студент! Сегодня мы изучаем магию Unity.";

//    private bool playerInside = false;

//    private void Update()
//    {
//        if (playerInside && Input.GetKeyDown(KeybindManager.GetKey(KeybindManager.INTERACT)))
//        {
//            DialogUI.instance.ShowText(dialogText);
//        }

//        if (playerInside && Input.GetKeyDown(KeyCode.Escape))
//        {
//            DialogUI.instance.Hide();
//        }
//    }

//    private void OnTriggerEnter2D(Collider2D other)
//    {
//        if (other.CompareTag("Player"))
//            playerInside = true;
//    }

//    private void OnTriggerExit2D(Collider2D other)
//    {
//        if (other.CompareTag("Player"))
//        {
//            playerInside = false;
//            DialogUI.instance.Hide();
//        }
//    }
//}


