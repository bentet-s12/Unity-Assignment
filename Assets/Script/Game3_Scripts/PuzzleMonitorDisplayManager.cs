using UnityEngine;

public class PuzzleMonitorDisplayManager : MonoBehaviour
{
    public PuzzleSocketValidator[] socketValidators;

    [Header("Monitor")]
    public Renderer monitorRenderer;          // One monitor only
    public Material finalImageMaterial;       // The image to show when puzzle is done
    public Material defaultMaterial;          // Optional: blank/default material

    private bool imageShown = false;

    void Start()
    {
        if (monitorRenderer && defaultMaterial)
            monitorRenderer.material = defaultMaterial;
    }

    void Update()
    {
        if (imageShown) return;

        if (AllSocketsCorrect())
        {
            ShowFinalImage();
            imageShown = true;
        }
    }

    bool AllSocketsCorrect()
    {
        foreach (var validator in socketValidators)
        {
            if (!validator.IsCorrect)
                return false;
        }
        return true;
    }

    void ShowFinalImage()
    {
        if (monitorRenderer && finalImageMaterial)
            monitorRenderer.material = finalImageMaterial;

        Debug.Log($"{name}: ✅ Puzzle complete — monitor now shows final image.");
    }
}
