using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class randomizesocket : MonoBehaviour
{
   // public ScoreManagerC scorescript;
   // public socketchecker socketscript;

    public List<XRSocketInteractor> allSockets = new List<XRSocketInteractor>();
    public List<GameObject> indicatorBlocks = new List<GameObject>();
    public GameObject indicatorCube;

    public AudioSource correctA = null;
    public AudioSource wrongA = null;

    public GameObject replacementPrefabL;
    public GameObject replacementPrefabW;
    public GameObject cube;
    public GameObject key;

    public float socketCooldown = 1f;
    public float autoResetTime = 3f;

    private List<GameObject> spawned = new List<GameObject>();
    private List<XRSocketInteractor> correctSockets = new List<XRSocketInteractor>();
    private List<GameObject> activeIndicatorBlocks = new List<GameObject>();

    // Tracks which correct sockets currently have objects
    private HashSet<XRSocketInteractor> filledCorrectSockets = new HashSet<XRSocketInteractor>();

    private bool canTriggerWin = true;

    void Start()
    {
       // scorescript = FindAnyObjectByType<ScoreManagerC>();
        // Register removal event listeners
        foreach (var socket in allSockets)
        {
            socket.selectExited.AddListener(args =>
            {
                OnObjectRemoved(socket);
            });
        }
    }

    public void startgame()
    {
        ClearSocket();
        spawncube();
        //scorescript.resetscore();
        GetCorrectSockets();
        ResetIndicatorCube();

        // At start, all correct sockets are filled
        filledCorrectSockets = new HashSet<XRSocketInteractor>(correctSockets);

        canTriggerWin = true;
        StartCoroutine(AutoResetTimer());
    }

    public void WinningSocket()
    {
        if (!canTriggerWin) return;

        canTriggerWin = false;
        XRSocketInteractor winnerSocket = correctSockets[Random.Range(0, correctSockets.Count)];
        StartCoroutine(socketfiller(winnerSocket));
    }

    private void GetCorrectSockets()
    {
        correctSockets.Clear();
        activeIndicatorBlocks.Clear();

        List<int> availableIndices = new List<int>();
        for (int i = 0; i < allSockets.Count; i++)
            availableIndices.Add(i);

        int count = Random.Range(2, 4); // Randomly select 2 or 3 sockets

        for (int i = 0; i < count; i++)
        {
            int randomIndex = Random.Range(0, availableIndices.Count);
            int selectedIndex = availableIndices[randomIndex];
            availableIndices.RemoveAt(randomIndex);

            correctSockets.Add(allSockets[selectedIndex]);

            GameObject indicator = indicatorBlocks[selectedIndex];
            if (indicator.TryGetComponent<Renderer>(out Renderer rend))
            {
                rend.material.color = Color.green;
            }
            activeIndicatorBlocks.Add(indicator);
        }

        Debug.Log($"Correct sockets this round: {correctSockets.Count}");
    }

    private void ClearSocket()
    {
        GameObject[] foundObjects = GameObject.FindGameObjectsWithTag("socketgame");
        foreach (GameObject obj in foundObjects)
        {
            Destroy(obj);
        }
        spawned.Clear();

        foreach (GameObject indicator in activeIndicatorBlocks)
        {
            if (indicator.TryGetComponent<Renderer>(out Renderer rend))
            {
                rend.material.color = Color.white;
            }
        }

        ResetIndicatorCube();
        activeIndicatorBlocks.Clear();
        correctSockets.Clear();
        filledCorrectSockets.Clear();
    }

    private void spawncube()
    {
        for (int i = 0; i < allSockets.Count; i++)
        {
            SpawnCubeForSocket(allSockets[i]);
        }
    }

    private void SpawnCubeForSocket(XRSocketInteractor socket)
    {
        Vector3 spawnPos = socket.attachTransform.position + new Vector3(0, 0.5f, 0);
        GameObject newCube = Instantiate(cube, spawnPos, Quaternion.identity);
        newCube.tag = "socketgame";
    }
    void SpawnKeyAt(Vector3 position)
    {
        Instantiate(key, position, Quaternion.identity);
    }
    // Called when an object is removed from a socket
    public void OnObjectRemoved(XRSocketInteractor socket)
    {
        if (!canTriggerWin)
            return;

        if (correctSockets.Contains(socket))
        {
            // Mark this correct socket as now empty
            filledCorrectSockets.Remove(socket);

            Debug.Log($"Removed object from correct socket. Remaining: {filledCorrectSockets.Count}");

            if (filledCorrectSockets.Count == 0)
            {
                // All correct sockets emptied => success
                Debug.Log("All correct sockets cleared! You win!");
                SetIndicatorColor(Color.green);
                correctA.Play();
                //scorescript.scorecounter();

                canTriggerWin = false;
                SpawnKeyAt(socket.attachTransform.position);
            }
        }
        else
        {
            // Removed from wrong socket => failure
            Debug.Log("Removed object from wrong socket! You lose!");
            SetIndicatorColor(Color.red);
            wrongA.Play();
            ResetGame();
            // You can decide to reset immediately or allow retry:
            canTriggerWin = true;
        }
    }

  
    IEnumerator socketfiller(XRSocketInteractor winnerSocket)
    {
        XRSocketInteractor pickupSocket = itemcheck();

        if (pickupSocket == null)
        {
            Debug.LogWarning("No empty socket found.");
            yield break;
        }

        GameObject result = null;

        if (correctSockets.Contains(pickupSocket))
        {
            filledCorrectSockets.Remove(pickupSocket);

            Transform winnerpoint = pickupSocket.attachTransform;
            result = Instantiate(replacementPrefabW, winnerpoint);
            //scorescript.scorecounter();
            SetIndicatorColor(Color.green);
            correctA.Play();

            if (filledCorrectSockets.Count == 0)
            {
                yield return new WaitForSeconds(autoResetTime);
                ResetGame();
                yield break;
            }
        }
        else
        {
            result = Instantiate(replacementPrefabL, pickupSocket.transform);
            SetIndicatorColor(Color.red);
            wrongA.Play();
        }

        result.transform.localPosition = Vector3.zero;
        result.tag = "socketgame";

        yield return new WaitForSeconds(0.5f);
        canTriggerWin = true;
    }

    private void ResetGame()
    {
        ClearSocket();
        spawncube();
        GetCorrectSockets();
        ResetIndicatorCube();
        canTriggerWin = true;

        filledCorrectSockets = new HashSet<XRSocketInteractor>(correctSockets);
        StartCoroutine(AutoResetTimer());
    }

    private void SetIndicatorColor(Color color)
    {
        if (indicatorCube != null && indicatorCube.TryGetComponent<Renderer>(out Renderer rend))
        {
            rend.material.color = color;
        }
    }

    private void ResetIndicatorCube()
    {
        SetIndicatorColor(Color.white);
    }

    public XRSocketInteractor itemcheck()
    {
        foreach (XRSocketInteractor socket in allSockets)
        {
            if (!socket.hasSelection)
            {
                return socket;
            }
        }
        return null;
    }
    private IEnumerator AutoResetTimer()
    {
        yield return new WaitForSeconds(autoResetTime);
        ResetGame();
    }
}
