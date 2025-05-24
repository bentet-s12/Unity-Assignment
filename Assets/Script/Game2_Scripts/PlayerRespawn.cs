using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    public Transform respawnPoint1;
    public Transform respawnPoint2;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("FireZone1"))
        {
            Respawn1();
        }

        if (other.CompareTag("FireZone2"))
        {
            Respawn2();
        }

    }

    void Respawn1()
    {
        if (respawnPoint1 != null)
        {
            // Reset position
            transform.position = respawnPoint1.position;

            // Optional: reset velocity if using Rigidbody
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }
        else
        {
            Debug.LogWarning("No respawn point assigned!");
        }
    }

    void Respawn2()
    {
        if (respawnPoint2 != null)
        {
            // Reset position
            transform.position = respawnPoint2.position;

            // Optional: reset velocity if using Rigidbody
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }
        else
        {
            Debug.LogWarning("No respawn point assigned!");
        }
    }
}
