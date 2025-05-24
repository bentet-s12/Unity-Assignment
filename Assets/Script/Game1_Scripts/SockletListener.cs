using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class SocketListener : MonoBehaviour
{
    public SocketGame socketGameManager; // Reference to your game manager script
    public GameObject thisSocket;        // The specific socket this script is attached to

    private XRSocketInteractor socketInteractor;

    void Awake()
    {
        socketInteractor = GetComponent<XRSocketInteractor>();
        socketInteractor.selectEntered.AddListener(OnObjectPlaced);
    }

    void OnDestroy()
    {
        socketInteractor.selectEntered.RemoveListener(OnObjectPlaced);
    }

    private void OnObjectPlaced(SelectEnterEventArgs args)
    {
        GameObject placedObject = args.interactableObject.transform.gameObject;

        if (socketGameManager != null && thisSocket != null)
        {
            socketGameManager.CheckObjectPlacement(thisSocket, placedObject);
        }
    }
}
