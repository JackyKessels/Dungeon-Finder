using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Glyph Database", menuName = "Systems/Glyph Database Object")]
public class GlyphDatabaseObject : ScriptableObject
{
    public List<GlyphObject> glyphObjects;

    [ContextMenu("Update IDs")]
    public void UpdateId()
    {
        List<GlyphObject> noDupes = new List<GlyphObject>(new HashSet<GlyphObject>(glyphObjects));
        noDupes.RemoveAll(item => item == null);
        glyphObjects.Clear();
        glyphObjects.AddRange(noDupes);

        for (int i = 0; i < glyphObjects.Count; i++)
        {
            if (glyphObjects[i].id != i)
                glyphObjects[i].id = i;
        }
    }
}
