using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class SteeringWheelVR : MonoBehaviour
{
    public Transform wheelVisual;         // rotating wheel model
    public Transform platformToRotate;    // the platform to rotate
    public float platformSpeed = 0.05f;
    public float rotationSensitivity = 0.1f; // for smoother turning

    private XRBaseInteractor interactor;
    private Quaternion initialGrabRotation;
    private float wheelAngle = 0f;

    private void OnEnable()
    {
        var grab = GetComponent<XRGrabInteractable>();
        grab.selectEntered.AddListener(OnGrab);
        grab.selectExited.AddListener(OnRelease);
    }

    private void OnDisable()
    {
        var grab = GetComponent<XRGrabInteractable>();
        grab.selectEntered.RemoveListener(OnGrab);
        grab.selectExited.RemoveListener(OnRelease);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        interactor = args.interactorObject as XRBaseInteractor;
        initialGrabRotation = interactor.transform.rotation;
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        interactor = null;
    }

    private void Update()
    {
        if (interactor != null)
        {
            Quaternion current = interactor.transform.rotation;
            Quaternion delta = current * Quaternion.Inverse(initialGrabRotation);

            // Calculate smooth delta Y angle
            float deltaY = Mathf.DeltaAngle(0f, delta.eulerAngles.y);

            wheelAngle += deltaY * rotationSensitivity;
            initialGrabRotation = current;

            // Rotate the wheel itself
            transform.localRotation = Quaternion.Euler(0f, wheelAngle, 0f);

            // Rotate the platform gradually based on current wheel angle
            if (platformToRotate)
            {
                float platformTargetSpeed = (wheelAngle / 360f) * platformSpeed;
                platformToRotate.Rotate(Vector3.up, platformTargetSpeed * Time.deltaTime);
            }
        }
    }               
}
