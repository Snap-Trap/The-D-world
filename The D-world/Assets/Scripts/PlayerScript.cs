using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerScript : MonoBehaviour
{
    public PlayerCameraScript playerCamera;

    public InputAction forward, backward, left, right, jump;

    public float maxRayDistance = 1.2f;

    public float speed, jumpForce, playerRotation;

    private Rigidbody rb;

    public LayerMask whatIsGround;

    [SerializeField] private bool isGrounded;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {

        // Makes it so the other script that controls the camera gives the rotation and you can use it here so forward goes forward based on--
        // what the player's forward is instead of the scene's forward
        // Touch this and I'll break your kneecaps
        playerRotation = playerCamera.turn.x;

        Quaternion playerRotationQuaternion = Quaternion.Euler(0, playerRotation, 0);

        Vector3 forwardMovement = playerRotationQuaternion * Vector3.forward;
        Vector3 rightMovement = playerRotationQuaternion * Vector3.right;

        Vector3 movement = Vector3.zero;


        // basic movement shit

        if (forward.ReadValue<float>() == 1)
        {
            movement += forwardMovement * speed;
        }
        if (backward.ReadValue<float>() == 1)
        {
            movement += -forwardMovement * speed;
        }
        if (left.ReadValue<float>() == 1)
        {
            movement += -rightMovement * speed;
        }
        if (right.ReadValue<float>() == 1)
        {
            movement += rightMovement * speed;
        }

        rb.velocity = new Vector3(movement.x, rb.velocity.y, movement.z);



        // Checks for jump button and if ground is touched
        if (isGrounded && jump.triggered)
        {
            rb.velocity += new Vector3(0, jumpForce, 0);
        }

        // Raycast you bloodgunging watermelon
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out RaycastHit groundHit, maxRayDistance, whatIsGround))
        {
            Debug.Log("Floor hit");
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
            Debug.Log("Did not hit the floor");
        }


        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down), Color.red, maxRayDistance);
    }


    // Input System stuff
    private void OnEnable()
    {
        forward.Enable();
        backward.Enable();
        left.Enable();
        right.Enable();
        jump.Enable();
    }

    private void OnDisable()
    {
        forward.Disable();
        backward.Disable();
        left.Disable();
        right.Disable();
        jump.Disable();
    }
}
