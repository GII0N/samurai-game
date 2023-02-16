using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    [Header("Camera")]
    public Transform camera;
    public bool lockCursor;

    [Range(0.1f, 10)] public float lookSensitivity; // int or float?

    private int maxUpRotation = 90; // public or private?, int or float?, pre defined to 90?
    private int maxDownRotation = 90; // public or private?, int or float?, pre defined to 90?

    private float xRotation = 0; // int or float?

    [Header("Movement")]
    public CharacterController controller;

    // Speed of forwards and backwards movement
    [Range(0.5f, 20)] public float walkSpeed; // int or float?

    // Speed of sideways (left and right) movement
    [Range(0.5f, 15)] public float strafeSpeed; // int or float?

    private KeyCode sprintKey;

    // How many times faster movement along the X and Z axes
    // is when sprinting
    [Range(1, 3)] public float sprintFactor; // int or float?

    [Range(0.5f, 10)] public float jumpHeight; // int or float?
    public int maxJumps;

    private Vector3 velocity = Vector3.zero;
    private int jumpsSinceLastLand = 0;

    void Start() {
        if(lockCursor) {
            Cursor.lockState = CursorLockMode.Locked;
        }
        sprintKey = KeyCode.LeftShift;
    }

    void Update() {
        transform.Rotate(0, Input.GetAxis("Mouse X") * lookSensitivity, 0);
        xRotation -= Input.GetAxis("Mouse Y") * lookSensitivity;
        xRotation = Mathf.Clamp(xRotation, -maxUpRotation, maxDownRotation);
        camera.localRotation = Quaternion.Euler(xRotation, 0, 0);

        velocity.x = Input.GetAxis("Horizontal") * strafeSpeed;
        velocity.z = Input.GetAxis("Vertical") * walkSpeed;
        velocity = transform.TransformDirection(velocity);

        if(Input.GetKey(sprintKey)) { Sprint(); }

        // Apply manual gravity
        velocity.y += Physics.gravity.y * Time.deltaTime;

        if(controller.isGrounded && velocity.y < 0) { Land(); }

        if(Input.GetButton("Jump")) {
            if(controller.isGrounded) {
                Jump();
            }
            else if(jumpsSinceLastLand < maxJumps) {
                Jump();
            }
        }
        controller.Move(velocity * Time.deltaTime);
    }

    private void Sprint() {
        velocity.x *= sprintFactor;
        velocity.z *= sprintFactor;
    }

    private void Jump() {
        velocity.y = Mathf.Sqrt(jumpHeight * -2 * Physics.gravity.y);
        jumpsSinceLastLand++;
    }

    private void Land() {
        velocity.y = 0;
        jumpsSinceLastLand = 0;
    }
}
