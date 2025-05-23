using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

public class GameController : NetworkBehaviour
{
    private List<bool> playerReady = new List<bool>();
    public NetworkVariable<bool> gameStarted = new NetworkVariable<bool>(false);

    public void AddPlayer()
    {
        playerReady.Add(false);
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetPlayerReadyServerRpc(int playerIndex)
    {
        playerReady[playerIndex] = !playerReady[playerIndex];

        if (playerReady.All(x => x))
        {
            gameStarted.Value = true;
            // Load the next scene for all players
            NetworkManager.SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
        }
    }
}
public class PlayerController : NetworkBehaviour
{
    private GameController gameController;
    private int playerIndex;

    public override void OnNetworkSpawn()
    {
        if (IsLocalPlayer)
        {
            gameController = GameObject.Find("GameController").GetComponent<GameController>();
            playerIndex = gameController.NetworkManager.ConnectedClients.Count - 1;
            gameController.AddPlayer();
        }
    }

    void Update()
    {
        if (!IsLocalPlayer) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            gameController.SetPlayerReadyServerRpc(playerIndex);
        }
    }
}
