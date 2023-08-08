using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlyphManager : MonoBehaviour
{
    #region Singleton
    public static GlyphManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.Log("Instance already exists.");
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    #endregion

    private TeamManager teamManager;

    [SerializeField] GlyphDatabaseObject glyphDatabaseObject;

    public List<int> UnlockedGlyphs { get; private set; } = new List<int>();

    // Keep list of currently active glyphs,
    // lock those so you can't keep buying the same.

    // Also glyph effects need to be applied every battle

    private void Start()
    {
        teamManager = TeamManager.Instance;
    }

    public void UnlockStarterGlyphs()
    {
        UnlockedGlyphs.Add(0);
        UnlockedGlyphs.Add(1);
    }

    public List<GlyphObject> GetGlyphObjects()
    {
        List<GlyphObject> glyphObjects = new List<GlyphObject>();

        foreach (int glyphObjectId in UnlockedGlyphs)
        {
            glyphObjects.Add(glyphDatabaseObject.glyphObjects[glyphObjectId]);
        }

        return glyphObjects;
    }

    public void ApplyGlyph(GlyphObject glyphObject)
    {
        if (glyphObject.heroEffects.Count > 0)
        {
            foreach (EffectObject effectObject in glyphObject.heroEffects)
            {
                foreach (Unit unit in teamManager.heroes.LivingMembers)
                {
                    unit.effectManager.PreparePreBattleEffect(effectObject);
                }
            }
        }

        // APPLY ENEMY EFFECTS TO ENEMIES
        // PREBATTLE DOESNT WORK BECAUSE ENEMIES DONT EXIST AT THIS POINT
    }

    public List<int> SaveGlyphs()
    {
        return UnlockedGlyphs;
    }

    public void LoadGlyphs(List<int> glyphs)
    {
        UnlockedGlyphs = glyphs;
    }
}
