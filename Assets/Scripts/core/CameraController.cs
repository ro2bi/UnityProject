using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float speed;

    [SerializeField] private float verticalOffset;
    [SerializeField] private float verticalSpeed;

    private float currentPosX;
    private float currentPosY;

    private Vector3 velocity = Vector3.zero;

    [SerializeField] private Transform player;

    [SerializeField] private float aheadDistance;

    [SerializeField] private float cameraSpeed;

    private float lookAhead;

    private void Update()
    {
        Vector3 targetPosition = new Vector3(
            player.position.x + lookAhead,
            player.position.y + verticalOffset,
            transform.position.z
        );

        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref velocity,
            verticalSpeed
        );

        lookAhead = Mathf.Lerp(lookAhead, (aheadDistance * player.localScale.x), cameraSpeed * Time.deltaTime);
    }

    public void MoveToNewRoom(Transform _newroom)
    {
        currentPosX = _newroom.position.x;
    }
}