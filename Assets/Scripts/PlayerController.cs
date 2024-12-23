using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

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
    [SerializeField] private LayerMask layersToInclude; // Pøidáno pole pro definici vrstev, které chcete zahrnout do raycastu.

    private Vector2 move;
    private Vector2 mouseLook;
    private Vector2 joystickLook;
    private Vector3 rotationTarget, playerVelocity;
    private bool isJumping;
    private bool isDashing = false;
    private float dashCooldownTimer = 0f;
    private Vector3 dashDirection;
    private Camera mainCamera;
    private PlayerAttributeManager PlayerAtm;
    private Animator animator;
    private float velocityZ = 0f;
    private float velocityX = 0f;
    private float dashDistance;
    private float dashCooldown;
    private float dashDuration;

    private void Awake()
    {
        mainCamera = Camera.main;
        PlayerAtm = gameObject.GetComponent<PlayerAttributeManager>();
        animator = gameObject.GetComponent<Animator>();
        dashDistance = PlayerAtm.dashDistance;
        dashCooldown = PlayerAtm.dashCooldown;
        dashDuration = PlayerAtm.dashDuration;
}

    // Input callbacks
    public void OnMove(InputAction.CallbackContext context)
        => move = context.ReadValue<Vector2>().normalized;

    public void OnMouseLook(InputAction.CallbackContext context)
        => mouseLook = Vector2.ClampMagnitude(context.ReadValue<Vector2>(), 1f);

    public void OnJoystickLook(InputAction.CallbackContext context)
        => joystickLook = Vector2.ClampMagnitude(context.ReadValue<Vector2>(), 1f);

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && controller.isGrounded)
        {
            isJumping = true;
            animator.SetTrigger("jump");
        }
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.performed && dashCooldownTimer <= 0f && !isDashing)
        {
            PerformDash();
        }
    }

    private void Update()
    {
        if (Time.timeScale == 0) return;

        if (isDashing)
            return; // Skip regular movement updates during dash

        if(dashCooldownTimer >= 0) dashCooldownTimer -= Time.deltaTime;
        PlayerAtm.dashCurrentCoolDown = dashCooldownTimer;

        ApplyGravity();
        if (isPc) HandleMouseLook();
        else HandleJoystickLook();


        UpdateBlendTreeValues();

        // Nastavení hodnot v Animatoru
        animator.SetFloat("VelocityZ", Mathf.Lerp(-1.5f, 1.5f, Mathf.InverseLerp(-speed, speed, velocityZ)));
        animator.SetFloat("VelocityX", Mathf.Lerp(-1.5f, 1.5f, Mathf.InverseLerp(-speed, speed, velocityX)));
    }

    private void PerformDash()
    {
        // Smìr dashu podle pohybu z klávesnice
        Vector3 movementDirection = new Vector3(move.x, 0, move.y);

        if (movementDirection.sqrMagnitude > 0.01f)
        {
            dashDirection = movementDirection.normalized;
        }
        else
        {
            // Pokud není pohyb, dash je ve smìru pohledu postavy
            dashDirection = transform.forward;
        }

        StartCoroutine(DashCoroutine());
    }

    private IEnumerator DashCoroutine()
    {
        isDashing = true;
        dashCooldownTimer = dashCooldown;

        float dashElapsedTime = 0f;

        while (dashElapsedTime < dashDuration)
        {
            Vector3 dashVelocity = dashDirection * (dashDistance / dashDuration);
            controller.Move(dashVelocity * Time.deltaTime);
            dashElapsedTime += Time.deltaTime;;
            yield return null;
        }

        isDashing = false;
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

        if (!isDashing) // Avoid gravity influence during dash
        {
            controller.Move(playerVelocity * Time.deltaTime);
        }
    }

    private void HandleMouseLook()
    {
        if (mainCamera == null) return;

        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        // Použití layersToInclude namísto Physics.DefaultRaycastLayers
        if (Physics.Raycast(ray, out RaycastHit hitInfo, 100f, layersToInclude, QueryTriggerInteraction.Ignore))
        {
            Vector3 targetPosition = hitInfo.point;
            targetPosition.y = transform.position.y;
            Vector3 directionToLook = targetPosition - transform.position;

            if (directionToLook.sqrMagnitude > 0.01f)
            {
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


        // Nastavujeme rychlost pohybu
        Vector3 movementWithoutY = new Vector3(movement.x, 0, movement.z);
        playerVelocity.z = movementWithoutY.z * speed;
        playerVelocity.x = movementWithoutY.x * speed;

        movement *= speed;
        movement.y = playerVelocity.y;
        PlayerCombatRanged rangeScript = gameObject.GetComponent<PlayerCombatRanged>();
        //rangeScript.spawnOffset.y += movement.y;
        controller.Move(movement * Time.deltaTime);
    }

    private void UpdateBlendTreeValues()
    {
        // Pozice kurzoru ve svìtových souøadnicích
        Vector3 mouseWorldPosition = GetMouseWorldPosition();

        // Smìr od postavy ke kurzoru (v rovinì XZ)
        Vector3 directionToCursor = (mouseWorldPosition - transform.position);
        directionToCursor.y = 0; // Zajistíme, že smìr je v rovinì XZ
        directionToCursor.Normalize();

        // Rychlost postavy v rovinì XZ
        Vector3 playerVelocityFlat = new Vector3(playerVelocity.x, 0, playerVelocity.z);

        // Projekce rychlosti na smìr ke kurzoru (velocityZ)
        velocityZ = Vector3.Dot(playerVelocityFlat, directionToCursor);

        // Kolmý smìr ke kurzoru (otoèení o 90° kolem osy Y)
        Vector3 perpendicularDirection = Vector3.Cross(Vector3.up, directionToCursor);

        // Projekce rychlosti na kolmý smìr (velocityX)
        velocityX = Vector3.Dot(playerVelocityFlat, perpendicularDirection);
    }

    // Získání pozice kurzoru ve svìtových souøadnicích
    private Vector3 GetMouseWorldPosition()
    {
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, layersToInclude, QueryTriggerInteraction.Ignore))
        {
            return hitInfo.point;
        }
        return transform.position; // Pokud raycast nezasáhne, vrátí pozici postavy
    }

}

