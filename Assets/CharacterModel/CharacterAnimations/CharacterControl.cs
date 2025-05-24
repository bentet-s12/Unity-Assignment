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
    [SerializeField] private Transform headset; // Main Camera under XR Origin

    [Header("Smoothing")]
    public float smoothTime = 0.1f;

    [Header("Offset")]
    public Vector3 positionOffset = new Vector3(0f, -1.5f, -0.5f); // 1.5m below and 0.5m behind headset

    private float smoothedSpeed = 0f;
    private float speedVelocity = 0f;
    private Vector3 lastHeadPosition;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            var remoteCam = GetComponentInChildren<Camera>();
            if (remoteCam != null)
                remoteCam.enabled = false;
            return;
        }

        moveInput.action.Enable();

        if (headset == null)
        {
            Camera cam = GetComponentInChildren<Camera>();
            if (cam != null)
            {
                headset = cam.transform;
                Debug.Log("Headset auto-assigned from child camera.");
            }
            else
            {
                Debug.LogWarning("No headset assigned or found in children!");
            }
        }

        if (headset != null)
            lastHeadPosition = headset.position;

        _controller = GetComponent<CharacterController>();
        _camera = GetComponentInChildren<Camera>();
        _animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (!IsOwner || headset == null)
            return;

        Vector2 input = moveInput.action.ReadValue<Vector2>();
        float joystickMagnitude = input.magnitude;

        Vector3 currentHeadPosition = headset.position;
        Vector3 headDelta = currentHeadPosition - lastHeadPosition;
        headDelta.y = 0f;

        float headMovementMagnitude = headDelta.magnitude / Time.deltaTime;
        lastHeadPosition = currentHeadPosition;

        float totalMovement = Mathf.Max(joystickMagnitude, headMovementMagnitude);

        smoothedSpeed = (totalMovement < 0.05f)
            ? 0f
            : Mathf.SmoothDamp(smoothedSpeed, totalMovement, ref speedVelocity, smoothTime);

        animator.SetBool("isWalking", smoothedSpeed > deadzone);
        animator.SetBool("isWalkingBackward", input.y < -deadzone && smoothedSpeed > deadzone);
        animator.SetFloat("walkSpeed", smoothedSpeed);
    }

    void LateUpdate()
    {
        if (!IsOwner || headset == null)
            return;

        Vector3 offsetWorld = headset.forward * positionOffset.z
                            + headset.right * positionOffset.x
                            + Vector3.up * positionOffset.y;

        Vector3 targetPosition = headset.position + offsetWorld;
        transform.position = new Vector3(targetPosition.x, transform.position.y, targetPosition.z);

        Vector3 lookDir = headset.forward;
        lookDir.y = 0f;
        if (lookDir.sqrMagnitude > 0.01f)
        {
            transform.forward = lookDir.normalized;
        }
    }
}
