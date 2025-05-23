using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    public PuzzleSocketValidator[] socketValidators;

    [Header("Monitor 1")]
    public Renderer monitorRenderer1;
    public Material finalImageMaterial1;

    [Header("Monitor 2")]
    public Renderer monitorRenderer2;
    public Material finalImageMaterial2;

    public Material defaultMaterial;

    private bool imageShown = false;

    void Start()
    {
        if (monitorRenderer1 && defaultMaterial)
            monitorRenderer1.material = defaultMaterial;

        if (monitorRenderer2 && defaultMaterial)
            monitorRenderer2.material = defaultMaterial;
    }

    void Update()
    {
        if (!imageShown && AllSocketsCorrect())
        {
            ShowFinalImages();
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

    void ShowFinalImages()
    {
        if (monitorRenderer1 && finalImageMaterial1)
            monitorRenderer1.material = finalImageMaterial1;

        if (monitorRenderer2 && finalImageMaterial2)
            monitorRenderer2.material = finalImageMaterial2;

        Debug.Log("✅ All puzzle pieces correct. Both images displayed.");
    }
}
