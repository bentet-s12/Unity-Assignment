using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class PressurePlateManager : MonoBehaviour
{
    public static PressurePlateManager Instance;

    public List<PressurePlate> allPlates;
    public TextMeshPro activationText;
    public GameObject door;

    private List<PressurePlate> targetPlates = new List<PressurePlate>();

    void Awake() => Instance = this;

    void Start()
    {
        // Randomly pick 3 unique plates
        while (targetPlates.Count < 3)
        {
            var candidate = allPlates[Random.Range(0, allPlates.Count)];
            if (!targetPlates.Contains(candidate))
            {
                candidate.isTargetPlate = true;
                targetPlates.Add(candidate);
            }
        }

        UpdateActivationCount();
    }

    public void UpdateActivationCount()
    {
        int activeCount = 0;
        foreach (var plate in targetPlates)
        {
            if (plate.isActivated)
                activeCount++;
        }

        activationText.text = $"Plates Activated: {activeCount} / 3";

        if (activeCount == 3)
        {
            OpenDoor();
        }
        else
        {
            CloseDoor();
        }
    }

    void OpenDoor()
    {
        // Move door or play animation
        door.SetActive(false); // or trigger door animation here
    }
    void CloseDoor()
    {
        if (door != null)
        {
            door.SetActive(true);
        }
    }
}
