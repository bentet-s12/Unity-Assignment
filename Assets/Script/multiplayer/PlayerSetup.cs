using Unity.Netcode;
using UnityEngine;

public class PlayerSetup : NetworkBehaviour
{
    public GameObject xrRig; // XR Origin that holds camera + controllers

    void Start()
    {
        if (!IsOwner && xrRig != null)
        {
            xrRig.SetActive(false);
        }
    }
}
