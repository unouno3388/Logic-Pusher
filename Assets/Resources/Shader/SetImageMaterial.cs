// 2026/1/5 AI-Tag
// This was created with the help of Assistant, a Unity Artificial Intelligence product.

using UnityEngine;
using UnityEngine.UI;

public class SetImageMaterial : MonoBehaviour
{
    public Material customMaterial; // Assign your custom material in the Inspector

    void Start()
    {
        Image image = GetComponent<Image>();
        if (image != null && customMaterial != null)
        {
            image.material = customMaterial;
        }
        else
        {
            Debug.LogWarning("Image component or custom material is missing!");
        }
    }
}