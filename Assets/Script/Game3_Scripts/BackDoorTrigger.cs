using UnityEngine;

public class BackDoorTrigger : MonoBehaviour
{
    public GameObject door;           // Assign the visible door object
    public float openedHeight = 3.0f; // How high the door should lift
    public float speed = 2.0f;        // Speed of door movement

    private Vector3 closedPos;
    private Vector3 openedPos;
    private float interpolate = 0f;
    private int direction = -1;       // -1 = close, 1 = open
    private bool isActive = false;
    private bool isUnlocked = false;

    void Start()
    {
        if (door == null)
        {
            Debug.LogError("❌ BackDoorTrigger: Door not assigned!");
            return;
        }

        closedPos = door.transform.position;
        openedPos = closedPos + new Vector3(0, openedHeight, 0);
    }

    void Update()
    {
        if (!isUnlocked || !isActive) return;

        interpolate += direction * speed * Time.deltaTime;
        interpolate = Mathf.Clamp01(interpolate);
        door.transform.position = Vector3.Lerp(closedPos, openedPos, interpolate);

        if (interpolate == 0f || interpolate == 1f)
            isActive = false;
    }

    // 🚪 Call this from GameFlowManager when puzzle + image match is complete
    public void Unlock()
    {
        isUnlocked = true;
        Debug.Log("🔓 Back door unlocked!");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isUnlocked) return;
        direction = 1;
        isActive = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!isUnlocked) return;
        direction = -1;
        isActive = true;
    }
}
