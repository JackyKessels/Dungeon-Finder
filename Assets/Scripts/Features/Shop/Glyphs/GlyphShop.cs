using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlyphShop : Shop
{
    [Header("[ Glyph Shop Specific ]")]
    [SerializeField] GameObject _glyphPrefab;

    public override void SetupShop()
    {
        ObjectUtilities.ClearContainer(shopContainer);

        List<GlyphObject> glyphObjects = GlyphManager.Instance.GetGlyphObjects();

        if (glyphObjects.Count <= 0)
            return;

        foreach (GlyphObject glyphObject in glyphObjects)
        {
            CreateGlyph(glyphObject);
        }
    }

    private void CreateGlyph(GlyphObject glyphObject)
    {
        GameObject obj = ObjectUtilities.CreateSimplePrefab(_glyphPrefab, shopContainer);

        GlyphPrefab glyphPrefab = obj.GetComponent<GlyphPrefab>();    
        if (glyphPrefab != null)
        {
            glyphPrefab.Setup(glyphObject);
        }
    }
}
