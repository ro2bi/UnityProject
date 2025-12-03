using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    //комнатный контроллер камеры
    [SerializeField] private float speed;

    [SerializeField] private float verticalOffset; // Смещение камеры по Y относительно игрока
    [SerializeField] private float verticalSpeed;  // Скорость сглаживания по Y
    // создаём публичную переменную для камеры
    private float currentPosX;
    private float currentPosY;
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
        // Следящая камера

        // Вычисляем целевую позицию (target position) с учетом горизонтального опережения и вертикального смещения
        Vector3 targetPosition = new Vector3(
            player.position.x + lookAhead,
            player.position.y + verticalOffset, // Добавляем позицию игрока по Y + смещение
            transform.position.z
        );

        // Используем SmoothDamp для плавного движения к целевой позиции
        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref velocity, // Используем общую переменную velocity
            verticalSpeed // Используем скорость сглаживания по Y
        );

        // Обновление опережения (Look Ahead) по оси X
        lookAhead = Mathf.Lerp(lookAhead, (aheadDistance * player.localScale.x), cameraSpeed * Time.deltaTime);
    }

    public void MoveToNewRoom(Transform _newroom)
    {
        currentPosX = _newroom.position.x;
        // устанавливаем текущую позицию камеры по оси X в позицию новой комнаты
    }
}
