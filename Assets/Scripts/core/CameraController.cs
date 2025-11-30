using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    //комнатный контроллер камеры
    [SerializeField] private float speed;
    // создаём публичную переменную для камеры
    private float currentPosX;
    // создаём приватную переменную для текущей позиции камеры по оси X
    private Vector3 velocity = Vector3.zero;
    // создаём приватную переменную для скорости движения камеры

    // следящая камера
    [SerializeField] private Transform player;
    // ссылка на игрока, за которым будет следить камера
    [SerializeField] private float aheadDistance;
    // расстояние, на которое камера будет следить за игроком вперёд
    [SerializeField] private float cameraSpeed;
    // скорость движения камеры
    private float lookAhead;
    // переменная для хранения расстояния, на которое камера будет следить за игроком

    private void Update()
    {
        //комнатный контроллер камеры
        //transform.position = Vector3.SmoothDamp(transform.position, new Vector3(currentPosX, transform.position.y, transform.position.z), ref velocity, speed);
        // обновляем позицию камеры, используя метод SmoothDamp для плавного движения

        // следящая камера
        transform.position = new Vector3(player.position.x + lookAhead, transform.position.y, transform.position.z);
        lookAhead = Mathf.Lerp(lookAhead, (aheadDistance * player.localScale.x), cameraSpeed * Time.deltaTime);
    }

    public void MoveToNewRoom(Transform _newroom)
    {
        currentPosX = _newroom.position.x;
        // устанавливаем текущую позицию камеры по оси X в позицию новой комнаты
    }
}
