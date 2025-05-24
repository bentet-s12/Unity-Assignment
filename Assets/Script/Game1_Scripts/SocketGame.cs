using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using System.Collections.Generic;

public class SocketGame : MonoBehaviour
{
    // Sockets and Cube
    public GameObject[] sockets; // Array of the three sockets
    public GameObject indicatorCube; // Cube that will show red/green
    public GameObject objectToPlace; // Object that the player will place
    private GameObject correctSocket; // Randomly selected socket

    public Transform restingPlace;

    // Timing control
    public XRInteractionManager interactionManager;


    void Start()
    {
        Debug.Log("New Round Starting...");
        // Start a new round
        StartCoroutine(RoundDelay());
        int randomIndex = Random.Range(0, sockets.Length);
        correctSocket = sockets[randomIndex];
    }

    void StartNewRound()
    {
        Debug.Log("New Round Starting...");

        StartCoroutine(RoundDelay());

        // Randomly select a new correct socket
        int randomIndex = Random.Range(0, sockets.Length);
        correctSocket = sockets[randomIndex];

        var interactable = objectToPlace.GetComponent<XRGrabInteractable>();
        if (interactable != null && interactable.isSelected)
        {
            var interactors = new List<IXRSelectInteractor>(interactable.interactorsSelecting);
            foreach (IXRSelectInteractor interactor in interactors)
            {
                interactionManager.SelectExit(interactor, interactable);
            }
        }

        // Temporarily disable interactable and collider to prevent re-snapping
        objectToPlace.GetComponent<Collider>().enabled = false;
        interactable.enabled = false;

        // Move object to starting position
        if (restingPlace != null)
        {
            transform.SetParent(null);
            transform.position = restingPlace.position;
        }
        else
        {
            Debug.LogWarning("Resting place not assigned!");
        }

        Debug.Log("Resetting to: " + restingPlace.position);
        // Reset weapon to resting place
        objectToPlace.transform.SetPositionAndRotation(restingPlace.position, restingPlace.rotation);

        // Reset indicator color
        indicatorCube.GetComponent<Renderer>().material.color = Color.white;

        // Re-enable after short delay
        StartCoroutine(ReenableAfterDelay(interactable, 0.5f));
    }


    IEnumerator RoundDelay()
    {
        yield return new WaitForSeconds(1);
    }

    IEnumerator ReenableAfterDelay(XRGrabInteractable interactable, float delay)
    {
        yield return new WaitForSeconds(delay);
        objectToPlace.GetComponent<Collider>().enabled = true;
        interactable.enabled = true;
    }


    public void CheckObjectPlacement(GameObject socketUsed, GameObject placedObject)
    {
        if (placedObject == objectToPlace)
        {
            if (socketUsed == correctSocket)
            {
                indicatorCube.GetComponent<Renderer>().material.color = Color.green;
                Invoke(nameof(StartNewRound), 1f);
            }
            else
            {
                indicatorCube.GetComponent<Renderer>().material.color = Color.red;
                Invoke(nameof(StartNewRound), 1f);
            }
        }
    }
}