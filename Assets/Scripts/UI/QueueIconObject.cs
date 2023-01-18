using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QueueIconObject : MonoBehaviour
{
    public Unit unit;

    public Image icon;
    public TextMeshProUGUI speed;

    public void Setup(Unit unit)
    {
        this.unit = unit;
        unit.orderIcon = this;

        icon.sprite = unit.icon;
        speed.text = unit.statsManager.GetAttributeValue(AttributeType.Speed).ToString();

        if (unit.isEnemy)
            icon.GetComponent<RectTransform>().localScale = new Vector3(-1, 1, 1);
        else
            icon.GetComponent<RectTransform>().localScale = Vector3.one;
    }

    public void HandleTurn()
    {
        if (unit.hasTurn)
        {
            icon.color = new Color(1.0f, 1.0f, 1.0f);
        }
        else
        {
            icon.color = new Color(0.3f, 0.3f, 0.3f);
        }
    }
}
