using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class PuzzleSocketValidator : MonoBehaviour
{
    public GameObject correctTile;     // The specific tile that matches this socket
    public bool IsCorrect { get; private set; }

    private XRSocketInteractor socket;

    void Start()
    {
        socket = GetComponent<XRSocketInteractor>();
    }

    void Update()
    {
        var selected = socket.GetOldestInteractableSelected();
        IsCorrect = (selected != null && selected.transform.gameObject == correctTile);
    }
}
