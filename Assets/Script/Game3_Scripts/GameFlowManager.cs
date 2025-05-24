using UnityEngine;

public class GameFlowManager : MonoBehaviour
{
    [Header("Puzzle Symbol Validators")]
    public SymbolMatchValidator player1Validator;
    public SymbolMatchValidator player2Validator;

    [Header("Scene Objects")]
    public GameObject midWall;             // The wall separating the rooms
    public BackDoorTrigger backDoorTrigger; // Reference to back door trigger script

    private bool gameComplete = false;

    void Update()
    {
        if (gameComplete) return;

        if (player1Validator.isMatched && player2Validator.isMatched)
        {
            gameComplete = true;

            // 💥 Destroy the mid-wall
            if (midWall != null)
                Destroy(midWall);

            // 🔓 Unlock the back door system
            if (backDoorTrigger != null)
                backDoorTrigger.Unlock();

            Debug.Log("✅ Puzzle complete! Mid-wall destroyed. Back door enabled.");
        }
    }
}
