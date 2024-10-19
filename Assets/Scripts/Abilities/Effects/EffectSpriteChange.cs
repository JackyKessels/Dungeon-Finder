using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "New Sprite Change", menuName = "Unit/Effect Object/Sprite Change")]
public class EffectSpriteChange : EffectObject
{
    public Sprite newSprite;
    public Sprite newIcon;

    public void ChangeSprite(Unit target)
    {
        SpriteRenderer unitRenderer = target.GetComponentInChildren<SpriteRenderer>();
        unitRenderer.sprite = newSprite;
        target.sprite = newSprite;

        target.PositionUnit(target.battleNumber);
        ObjectUtilities.CreateSpecialEffects(specialEffects, target);

        target.icon = newIcon;

        BattleHUD.Instance.Refresh();
    }

    protected override string ParseDescription(string s, TooltipObject tooltipInfo)
    {
        return s;
    }
}
