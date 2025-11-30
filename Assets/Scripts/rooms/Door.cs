using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private Transform previousRoom;
    // комната, из которой дверь ведёт
    [SerializeField] private Transform nextRoom;
    // комната, в которую дверь ведёт
    [SerializeField] private CameraController cam;
    // ссылка на контроллер камеры  

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // проверяем, что объект, вошедший в триггер, является игроком
        if (collision.tag == "Player")
        {
            if (collision.transform.position.x < transform.position.x)
            {
                cam.MoveToNewRoom(nextRoom);
                // если игрок находится слева от двери, перемещаем его в следующую комнату
                if (cam != null && nextRoom != null)
                    cam.MoveToNewRoom(nextRoom);
            }
            else
            {
                cam.MoveToNewRoom(previousRoom);
                // если игрок находится справа от двери, перемещаем его в предыдущую комнату
                if (cam != null && previousRoom != null)
                    cam.MoveToNewRoom(previousRoom);
            }
        }
    }
}
