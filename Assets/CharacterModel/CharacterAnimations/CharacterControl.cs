using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterControl : MonoBehaviour
{
    [Header("Components")]
    public CharacterController characterController;
    public Animator animator;
    public Transform xrRigCamera;

    [Header("XR Input Actions")]
    public InputActionProperty moveInput;       // Vector2: primary2DAxis
    public InputActionProperty axisClickInput;  // Button: primary2DAxisClick

    [Header("Movement Settings")]
    public float walkSpeed = 1.5f;
    public float sprintSpeed = 3f;

    [Header("Input Smoothing")]
    public float smoothTime = 0.1f;

    private float smoothedSpeed = 0f;
    private float speedVelocity = 0f;

    void Start()
    {
        moveInput.action.Enable();
        axisClickInput.action.Enable();
    }

    void Update()
    {
        Vector2 input = moveInput.action.ReadValue<Vector2>();
        bool isSprinting = axisClickInput.action.IsPressed();

        // Get movement direction relative to headset
        Vector3 forward = new Vector3(xrRigCamera.forward.x, 0, xrRigCamera.forward.z).normalized;
        Vector3 right = new Vector3(xrRigCamera.right.x, 0, xrRigCamera.right.z).normalized;
        Vector3 move = (forward * input.y + right * input.x).normalized;

        float currentSpeed = isSprinting ? sprintSpeed : walkSpeed;
        characterController.Move(move * currentSpeed * Time.deltaTime);

        // Smooth input magnitude to prevent flickering isWalking toggle
        float targetSpeed = input.magnitude;
        smoothedSpeed = Mathf.SmoothDamp(smoothedSpeed, targetSpeed, ref speedVelocity, smoothTime);

        float deadzone = 0.15f;
        animator.SetBool("isWalking", smoothedSpeed > deadzone);

        // Optional: uncomment if you want to add sprint bool to animator
        // animator.SetBool("isSprinting", isSprinting);
    }
}