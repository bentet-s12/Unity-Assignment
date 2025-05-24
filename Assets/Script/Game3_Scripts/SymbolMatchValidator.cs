using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class SymbolMatchValidator : MonoBehaviour
{
    public XRSocketInteractor socket;
    public Renderer[] referenceMonitors;

    public bool isMatched = false;
    public GameObject matchedObject { get; private set; }

    void Start()
    {
        if (socket == null)
            socket = GetComponent<XRSocketInteractor>();
    }

    void Update()
    {
        if (isMatched || socket == null || referenceMonitors == null)
            return;

        var selectedInteractable = socket.GetOldestInteractableSelected();

        if (selectedInteractable is XRBaseInteractable interactable)
        {
            Renderer symbolRenderer = interactable.GetComponent<Renderer>();
            if (symbolRenderer == null) return;

            Texture selectedTexture = symbolRenderer.material.mainTexture;

            foreach (var monitor in referenceMonitors)
            {
                if (monitor != null && monitor.material.mainTexture == selectedTexture)
                {
                    isMatched = true;
                    matchedObject = interactable.gameObject;
                    Debug.Log($"{name}: ✅ Correct symbol matched with monitor.");
                    break;
                }
            }
        }
    }
}
