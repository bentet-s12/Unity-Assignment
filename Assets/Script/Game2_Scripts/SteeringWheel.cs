using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class SteeringWheelVR : MonoBehaviour
{
    public Transform wheelVisual;         // rotating wheel model
    public Transform platformToRotate;    // the platform to rotate
    public float maxRotation = 180f;
    public float platformSpeed = 45f;

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
            float deltaY = delta.eulerAngles.y;
            if (deltaY > 180f) deltaY -= 360f;

            wheelAngle = Mathf.Clamp(wheelAngle + deltaY, -maxRotation, maxRotation);
            initialGrabRotation = current;

            // Rotate the root around Y axis
            transform.localRotation = Quaternion.Euler(0f, wheelAngle, 0f);

            // Rotate platform
            if (platformToRotate)
            {
                platformToRotate.Rotate(Vector3.up, (wheelAngle / maxRotation) * platformSpeed * Time.deltaTime);
            }

            // **DO NOT** reset wheelVisual.localRotation here — keep it slanted!
            // Remove or comment out this line:
            // wheelVisual.localRotation = Quaternion.identity;
        }
    }

}
