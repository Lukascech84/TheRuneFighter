using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 5.0f;
    public float gravity = -9.81f;
    public float jumpSpeed = 2.0f;
    public bool isPc;
    public CharacterController controller;

    [Header("Rotation Settings")]
    [SerializeField] private float rotationSmoothTime = 0.15f;

    private Vector2 move, mouseLook, joystickLook;
    private Vector3 rotationTarget, playerVelocity;
    private bool isJumping;
    private Camera mainCamera;
    private float rotationVelocity;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    // Input callbacks
    public void OnMove(InputAction.CallbackContext context) => move = context.ReadValue<Vector2>().normalized;

    public void OnMouseLook(InputAction.CallbackContext context)
        => mouseLook = Vector2.ClampMagnitude(context.ReadValue<Vector2>(), 1f);

    public void OnJoystickLook(InputAction.CallbackContext context)
        => joystickLook = Vector2.ClampMagnitude(context.ReadValue<Vector2>(), 1f);

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && controller.isGrounded)
        {
            isJumping = true;
        }
    }

    private void Update()
    {
        ApplyGravity();
        if (isPc) HandleMouseLook();
        else HandleJoystickLook();
    }

    private void ApplyGravity()
    {
        if (controller.isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = -2f;
        }
        else
        {
            playerVelocity.y += gravity * Time.deltaTime;
        }
        controller.Move(playerVelocity * Time.deltaTime);
    }

    private void HandleMouseLook()
    {
        if (mainCamera == null) return;

        // Ray from the camera towards the mouse position
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        // Check if the ray hits a surface in the game world
        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            // Get the point where the ray hit the ground
            Vector3 targetPosition = hitInfo.point;
            targetPosition.y = transform.position.y; // Keep the target at the player's level

            // Calculate the direction to look at
            Vector3 directionToLook = targetPosition - transform.position;

            if (directionToLook.sqrMagnitude > 0.01f)
            {
                // Smoothly rotate the player to look at the target direction
                Quaternion targetRotation = Quaternion.LookRotation(directionToLook);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSmoothTime);
            }
        }

        MoveWithDirection();
    }

    private void HandleJoystickLook()
    {
        if (joystickLook.sqrMagnitude < 0.1f)
        {
            MoveWithoutAim();
        }
        else
        {
            Vector3 aimDirection = new Vector3(joystickLook.x, 0, joystickLook.y);
            Quaternion targetRotation = Quaternion.LookRotation(aimDirection, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSmoothTime);
            MoveWithDirection();
        }
    }

    private void MoveWithoutAim()
    {
        Vector3 movement = new Vector3(move.x, 0f, move.y);
        if (movement.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movement);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSmoothTime);
        }
        ApplyMovement(movement);
    }

    private void MoveWithDirection()
    {
        Vector3 forward = mainCamera.transform.forward;
        forward.y = 0;
        forward.Normalize();

        Vector3 right = mainCamera.transform.right;
        right.y = 0;
        right.Normalize();

        Vector3 movement = forward * move.y + right * move.x;
        ApplyMovement(movement);
    }

    private void ApplyMovement(Vector3 movement)
    {
        if (isJumping && controller.isGrounded)
        {
            playerVelocity.y = jumpSpeed;
            isJumping = false;
        }

        movement *= speed;
        movement.y = playerVelocity.y;  // Maintain vertical velocity from gravity or jump
        controller.Move(movement * Time.deltaTime);
    }
}
