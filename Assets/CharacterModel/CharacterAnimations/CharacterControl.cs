using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterControl : NetworkBehaviour
{
    private CharacterController _controller;
    private Camera _camera;
    private Animator _animator;

    [Header("XR Input Actions")]
    public InputActionProperty moveInput;

    [Header("Animation")]
    public Animator animator;
    public float deadzone = 0.15f;

    [Header("Tracking References")]
    public Transform headset; // Main Camera under XR Origin

    [Header("Smoothing")]
    public float smoothTime = 0.1f;

    [Header("Offset")]
    public Vector3 positionOffset = new Vector3(0f, -1.5f, -0.5f); // e.g., 1.5m below and 0.5m behind headset

    private float smoothedSpeed = 0f;
    private float speedVelocity = 0f;
    private Vector3 lastHeadPosition;

    public override void OnNetworkSpawn()
    {
        moveInput.action.Enable();
        lastHeadPosition = headset.position;

        _controller = GetComponent<CharacterController>();
        _camera = GetComponentInChildren<Camera>();
        _animator = GetComponentInChildren<Animator>();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (!IsOwner)
        {
            _camera.enabled = false;
        }
    }
    void Start()
    {
        
    }

    void Update()
    {
        if (!IsOwner)
        {
            return;
            
        }
        // 1. Joystick input
        Vector2 input = moveInput.action.ReadValue<Vector2>();
        float joystickMagnitude = input.magnitude;

        // 2. Headset world movement (XZ only)
        Vector3 currentHeadPosition = headset.position;
        Vector3 headDelta = currentHeadPosition - lastHeadPosition;
        headDelta.y = 0f;

        float headMovementMagnitude = headDelta.magnitude / Time.deltaTime;
        lastHeadPosition = currentHeadPosition;

        // 3. Combine movement input
        float totalMovement = Mathf.Max(joystickMagnitude, headMovementMagnitude);

        // Smooth total movement
        if (totalMovement < 0.05f)
        {
            smoothedSpeed = 0f;
            speedVelocity = 0f;
        }
        else
        {
            smoothedSpeed = Mathf.SmoothDamp(smoothedSpeed, totalMovement, ref speedVelocity, smoothTime);
        }

        // Trigger animation
        animator.SetBool("isWalking", smoothedSpeed > deadzone);
        animator.SetBool("isWalkingBackward", input.y < -deadzone && smoothedSpeed > deadzone);
        animator.SetFloat("walkSpeed", smoothedSpeed);
    }

    void LateUpdate()
    {
        // Apply position offset relative to headset forward
        Vector3 offsetWorld = headset.forward * positionOffset.z
                            + headset.right * positionOffset.x
                            + Vector3.up * positionOffset.y;

        Vector3 targetPosition = headset.position + offsetWorld;
        transform.position = new Vector3(targetPosition.x, transform.position.y, targetPosition.z);

        // Face same direction as headset
        Vector3 lookDir = headset.forward;
        lookDir.y = 0f;
        if (lookDir.sqrMagnitude > 0.01f)
        {
            transform.forward = lookDir.normalized;
        }
    }
}
