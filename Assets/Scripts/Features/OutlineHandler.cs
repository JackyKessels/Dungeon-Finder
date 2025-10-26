using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineHandler : MonoBehaviour
{
    private SpriteRenderer _renderer;

    public Material defaultMaterial;
    public Material outlineMaterial;

    [ColorUsage(true, true)]
    public Color outlineColor = Color.blue;

    void Start()
    {
        _renderer = transform.GetComponentInChildren<SpriteRenderer>();
    }

    void OnMouseEnter()
    {
        _renderer.material = outlineMaterial;
        _renderer.material.SetFloat("_OutlineThickness", 0.005f);
        _renderer.material.SetColor("_OutlineColor", outlineColor);
    }

    void OnMouseExit()
    {
        _renderer.material = defaultMaterial;
    }
}
