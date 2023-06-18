using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Glyph", menuName = "Glyph")]
public class GlyphObject : ScriptableObject
{
    [Header("[ General Information ]")]
    public int id = 0;
    public new string name = "New Glyph";
    public Sprite icon = null;
    public int cost = 0;
    public CurrencyType currency = CurrencyType.Gold;

    [Header("[ Effects ]")]
    public List<EffectObject> heroEffects;
    public List<EffectObject> enemyEffects;
}
