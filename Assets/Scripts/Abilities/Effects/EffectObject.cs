using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ProcType { Turn, Round }

[System.Serializable]
public abstract class EffectObject : ScriptableObject, IDescribable
{
    [Header("[ General ]")]
    public new string name = "New Effect";
    public Sprite icon;
    public bool hidden = false;
    [TextArea(10, 10)] public string description;

    [Header("[ Mechanics ]")]
    public bool isBuff;
    public ProcType procType = ProcType.Turn;
    public int duration = 1;
    public bool permanent = false;
    public bool stackable = false;
    public int maxStacks = 0;
    public bool refreshes = true;
    [Tooltip("True = Effect is removed when caster dies. | False = Effect persists when caster dies.")]
    public bool aura = false;
    public List<EffectObject> removeEffects;

    public string GetDescription(TooltipObject tooltipInfo)
    {
        return name + "\nEffect: " + ParseDescription(description, tooltipInfo);
    }

    protected abstract string ParseDescription(string s, TooltipObject tooltipInfo);

    public string DetermineColor(string temp, string check, AbilitySchool school)
    {
        if (temp.Contains(check))
        {
            temp = temp.Replace(check, "<color=" + ColorDatabase.SchoolColor(school) + ">{0}</color>");

            return string.Format(temp, school);
        }

        return temp;
    }

    public static string DurationText(EffectObject effectObject)
    {
        if (effectObject.permanent)
        {
            return "Full Fight";
        }
        else
        {
            if (effectObject.duration > 1)
            {
                return effectObject.duration + " Rounds";
            }
            else
            {
                return "1 Round";
            }        
        }
    }
}
