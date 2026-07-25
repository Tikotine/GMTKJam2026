using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TextureMapper : MonoBehaviour
{
    Renderer cachedRenderer;
    public Texture targetTexture;
    private void Awake()
    {
        cachedRenderer = GetComponent<Renderer>();
    }
    public void SetTexture()
    {
        if (cachedRenderer == null)
            cachedRenderer = GetComponent<Renderer>();
        if (targetTexture != null)
            cachedRenderer.sharedMaterial.mainTexture = targetTexture;
    }
    private void Update()
    {
        SetTexture();
    }
    private void OnGUI()
    {
        SetTexture();
    }
}
