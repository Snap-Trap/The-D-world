using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraScript : MonoBehaviour
{
    public Vector2 turn;

    public float pitchMin = -30f;
    public float pitchMax = 30f;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        turn.x += Input.GetAxis("Mouse X");
        turn.y += Input.GetAxis("Mouse Y");

        turn.y = Mathf.Clamp(turn.y, pitchMin, pitchMax);

        transform.localRotation = Quaternion.Euler(-turn.y, turn.x, 0);
    }
}
