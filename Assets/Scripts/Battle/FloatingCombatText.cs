using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class FloatingCombatText : MonoBehaviour
{
    public static FloatingCombatText SendText(FCTData fctData)
    {
        float overheadOffset;

        if (fctData.combatValue)
            overheadOffset = 0;
        else
            overheadOffset = (fctData.unit.sprite.bounds.size.y * fctData.unit.unitRenderer.transform.localScale.y) / 2;

        Vector3 newPos = new Vector3(fctData.unit.transform.position.x, fctData.unit.transform.position.y + overheadOffset, fctData.unit.transform.position.z);
        Transform FCTTransform = Instantiate(GameAssets.i.floatingCombatText, newPos, Quaternion.identity);

        FloatingCombatText FCT = FCTTransform.GetComponent<FloatingCombatText>();
        FCT.Setup(fctData);

        return FCT;
    }

    private static int sortingOrder;

    private TextMeshPro textMesh;
    private float fade;
    private float fadeTimer;
    private float speed;
    private float randomDirectionVector;
    private float sizeTarget;
    private Vector3 moveDirection;

    private Color textColor;

    private void Awake()
    {
        textMesh = transform.GetComponent<TextMeshPro>();
    }

    public void Setup(FCTData fctData)
    {
        textMesh.SetText(fctData.text);

        float baseFontSize;
        if (fctData.combatValue)
        {
            if (fctData.isGlancing)
                baseFontSize = 6f;
            else if (!fctData.isCriticalHit)
                baseFontSize = 9f;
            else
                baseFontSize = 12f;
        }
        else
            baseFontSize = 6f;

        var initialSize = fctData.combatValue 
            ? (fctData.isCriticalHit ? 2.2f : 2.0f) 
            : 1f;
        textMesh.fontSize = baseFontSize * initialSize;
        sizeTarget = baseFontSize;

        textColor = fctData.isCriticalHit ? fctData.criticalColor : fctData.color;
        textMesh.color = textColor;

        fade = fctData.combatValue ? 2f : 2f;
        fadeTimer = fctData.combatValue ? 0.35f : 0.5f;
        speed = fctData.combatValue ? 2.5f : 1f;

        sortingOrder++;
        textMesh.sortingOrder = sortingOrder;

        if (fctData.combatValue)
        {
            Vector3 center = Vector3.zero;
            float maxAngle = 30f;
            float angle;

            if (fctData.unit.transform.position.x < center.x)
            {
                angle = Random.Range(0f, -maxAngle);
            }
            else
            {
                angle = Random.Range(0f, maxAngle);
            }

            float rad = angle * Mathf.Deg2Rad;
            float speed = 2.5f;
            moveDirection = new Vector3(Mathf.Sin(rad), Mathf.Cos(rad), 0) * speed;
        }
        else
        {
            moveDirection = Vector3.up * 1f;
        }
    }

    private void Update()
    {
        transform.position += moveDirection * Time.deltaTime;

        float scaleSpeed = 6f;
        textMesh.fontSize = Mathf.Lerp(textMesh.fontSize, sizeTarget, Time.deltaTime * scaleSpeed);

        fadeTimer -= Time.deltaTime;
        if (fadeTimer < 0)
        {
            textColor.a -= fade * Time.deltaTime;
            textMesh.color = textColor;

            if (textColor.a <= 0f)
            {
                Destroy(gameObject);
            }
        }
    }
}
