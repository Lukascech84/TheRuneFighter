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

    private void Awake()
    {
        // Cache main camera for performance
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
            playerVelocity.y = -2f; // Small downward force for consistent grounded detection
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

        Ray ray = mainCamera.ScreenPointToRay(mouseLook);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            rotationTarget = hit.point;
        }
        MoveWithAim(rotationTarget);
    }

    private void HandleJoystickLook()
    {
        if (joystickLook.sqrMagnitude < 0.1f)
        {
            MoveWithoutAim();
        }
        else
        {
            Vector3 aimDirection = transform.position + new Vector3(joystickLook.x, 0, joystickLook.y);
            MoveWithAim(aimDirection);
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

    private void MoveWithAim(Vector3 aimPoint)
    {
        Vector3 lookDirection = aimPoint - transform.position;
        lookDirection.y = 0;

        if (lookDirection.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSmoothTime);
        }

        Vector3 movement = new Vector3(move.x, 0, move.y);
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
