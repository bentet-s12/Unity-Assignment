using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    public bool isTargetPlate = false;
    public bool isActivated = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Cube"))
        {
            isActivated = true;
            Debug.Log("Cube placed on plate");
            PressurePlateManager.Instance.UpdateActivationCount();

        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Cube"))
        {
            isActivated = false;
            Debug.Log("Cube removed from plate");
            PressurePlateManager.Instance.UpdateActivationCount();

        }
    }
}
