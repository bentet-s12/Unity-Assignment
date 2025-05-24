using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class SocketGame : MonoBehaviour
{
    public GameObject[] indicatorblocks;
    public GameObject[] sockets;
    public GameObject indicatorCube;
    public GameObject objectToPlace;
    public XRInteractionManager interactionManager;
   

    private List<GameObject> correctSockets = new List<GameObject>();
    private List<GameObject> correctBlocks = new List<GameObject>();


    void Start()
    {
        Debug.Log("New Round Starting...");
        StartCoroutine(RoundDelay());
        SelectCorrectSockets();
    }

    void StartNewRound()
    {
        foreach (GameObject block in indicatorblocks)
        {
            block.GetComponent<Renderer>().material.color = Color.white;
        }

        Debug.Log("New Round Starting...");
        StartCoroutine(RoundDelay());

        SelectCorrectSockets();

        var interactable = objectToPlace.GetComponent<XRGrabInteractable>();
        if (interactable != null && interactable.isSelected)
        {
            var interactors = new List<IXRSelectInteractor>(interactable.interactorsSelecting);
            foreach (IXRSelectInteractor interactor in interactors)
            {
                interactionManager.SelectExit(interactor, interactable);
            }
        }

        objectToPlace.GetComponent<Collider>().enabled = false;
        interactable.enabled = false;


        indicatorCube.GetComponent<Renderer>().material.color = Color.white;
        StartCoroutine(ReenableAfterDelay(interactable, 0.5f));
    }

    void SelectCorrectSockets()
    {
        correctSockets.Clear();
        correctBlocks.Clear();

        List<int> availableIndices = new List<int>();
        for (int i = 0; i < sockets.Length; i++)
        {
            availableIndices.Add(i);
        }

        int count = Random.Range(2, 4);
        for (int i = 0; i < count; i++)
        {
            int randomIndex = Random.Range(0, availableIndices.Count);
            int selected = availableIndices[randomIndex];
            correctSockets.Add(sockets[selected]);
            correctBlocks.Add(indicatorblocks[selected]);
            availableIndices.RemoveAt(randomIndex);
        }

        for (int i = 0; i < count; i++)
        {

            correctBlocks[i].GetComponent<Renderer>().material.color = Color.green;

            Debug.Log($"Correct sockets this round: {correctSockets.Count}");
        }
        }

        IEnumerator RoundDelay()
    {
        yield return new WaitForSeconds(3);
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
            if (correctSockets.Contains(socketUsed))
            {
                indicatorCube.GetComponent<Renderer>().material.color = Color.green;
            }
            else
            {
                indicatorCube.GetComponent<Renderer>().material.color = Color.red;
            }

            Invoke(nameof(StartNewRound), 1f);
        }
    }
}
