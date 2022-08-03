using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ContextMenuElementObject : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] TextMeshProUGUI health;

    public void Setup(string text, string health)
    {
        this.text.text = text;

        if (health != "")
        {
            this.health.text = health;
        }
    }

    public void ColorText(string color)
    {
        text.color = GeneralUtilities.ConvertString2Color(color);
    }

    public void SetSmallCaps()
    {
        text.text = string.Format("<smallcaps>{0}</smallcaps>", text.text);
    }
}
