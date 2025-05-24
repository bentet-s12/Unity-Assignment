using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    public GameObject door = null;      // door
    public float openedHeight = 2.0f;   // opened door height
    public float speed = 1.0f;          // door opening speed

    Vector3 openedPos = Vector3.zero;   // position of the opened door
    Vector3 closedPos = Vector3.zero;   // position of the closed door
    bool isActivated = false;           // whether the door is activated
    int direction = -1;                 // opening or closing
    float interpolate = 0.0f;           // interpolation amount

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        closedPos = door.transform.position;    // set closed position to current position

        openedPos = door.transform.position;    // set opened position based on height variable
        openedPos.y += openedHeight;
    }

    // Update is called once per frame
    void Update()
    {
        if(isActivated)
        {
            // calculate the interpolation amount
            interpolate += direction * speed * Time.deltaTime;

            // if door completely opened
            if (interpolate > 1.0f)
            {
                interpolate = 1.0f;
                isActivated = false;
            }
            // if door completely closed
            else if (interpolate < 0.0f)
            {
                interpolate = 0.0f;
                isActivated = false;
            }

            // interpolate between open and closed positions
            door.transform.position = Vector3.Lerp(closedPos, openedPos, interpolate);
        }
    }

    // called when a GameObject collides with the collider
    void OnTriggerEnter(Collider other)
    {
        // activate the door and set direction to open
        isActivated = true;
        direction = 1;
    }

    // called when a GameObject exits the collider
    void OnTriggerExit(Collider other)
    {
        // activate the door and set direction to close
        isActivated = true;
        direction = -1;
    }
}
