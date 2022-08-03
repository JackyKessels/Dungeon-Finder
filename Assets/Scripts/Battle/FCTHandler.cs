using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FCTHandler : MonoBehaviour
{
    private Queue<FCTData> queueFCT = new Queue<FCTData>();
    private Queue<FCTDataSprite> queueFCTSprite = new Queue<FCTDataSprite>();

    private readonly float cooldownFCT = 0.25f;
    private readonly float cooldownSprite = 0.5f;

    private bool active = false;

    private void Update()
    {
        if (active)
        {
            StartCoroutine(EmptyFCTQueue());
            active = false;
        }
    }

    IEnumerator EmptyFCTQueue()
    {
        while (queueFCT.Count > 0)
        {
            CreateFCT();
            yield return new WaitForSeconds(cooldownFCT);
        }

        while (queueFCTSprite.Count > 0)
        {
            CreateFCTSprite();
            yield return new WaitForSeconds(cooldownSprite);
        }
    }

    private void CreateFCT()
    {
        FCTData first = queueFCT.Dequeue();

        FloatingCombatText.SendText(first);
    }

    private void CreateFCTSprite()
    {
        FCTDataSprite first = queueFCTSprite.Dequeue();

        AbilityCast.CastAbility(first);
    }

    public void AddToFCTQueue(FCTData fctData)
    {
        queueFCT.Enqueue(fctData);

        active = true;
    }

    public void AddToFCTQueue(FCTDataSprite fctSprite)
    {
        queueFCTSprite.Enqueue(fctSprite);

        active = true;
    }
}

[System.Serializable]
public class FCTData
{
    public bool combatValue = false;

    public Unit unit;
    public string text;
    public Color color;

    // Combat specific
    public bool isGlancing = false;
    public bool isCriticalHit = false;
    public Color criticalColor;

    public FCTData(bool _combatValue, Unit _unit, string _text, bool _isGlancing, bool _isCriticalHit, Color _baseColor, Color _criticalColor)
    {
        combatValue = _combatValue;

        unit = _unit;
        text = _text;
        color = _baseColor;
        isGlancing = _isGlancing;
        isCriticalHit = _isCriticalHit;
        criticalColor = _criticalColor;
    }

    public FCTData(bool _combatValue, Unit _unit, string _text, Color _color)
    {
        combatValue = _combatValue;

        unit = _unit;
        text = _text;
        color = _color;
    }
}

public class FCTDataSprite
{
    public Unit unit;
    public Active active;
    public bool moving;

    public FCTDataSprite(Unit _unit, Active _active, bool _moving)
    {
        unit = _unit;
        active = _active;
        moving = _moving;  
    }
}
