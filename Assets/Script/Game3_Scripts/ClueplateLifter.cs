using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class CluePlateLifter : MonoBehaviour
{
    public GameObject correctObject;
    public Transform puzzleBox;
    public float liftHeight = 0.3f;
    public float liftSpeed = 1f;

    private XRSocketInteractor socket;
    private Vector3 loweredPos;
    private Vector3 liftedPos;
    private bool shouldLift = false;

    void Start()
    {
        socket = GetComponent<XRSocketInteractor>();

        if (puzzleBox != null)
        {
            loweredPos = puzzleBox.position;
            liftedPos = loweredPos + new Vector3(0, liftHeight, 0);
        }
    }

    void Update()
    {
        if (socket == null || puzzleBox == null) return;

        var selected = socket.GetOldestInteractableSelected();
        shouldLift = (selected != null && selected.transform.gameObject == correctObject);

        Vector3 targetPos = shouldLift ? liftedPos : loweredPos;
        puzzleBox.position = Vector3.MoveTowards(puzzleBox.position, targetPos, liftSpeed * Time.deltaTime);
    }
}
