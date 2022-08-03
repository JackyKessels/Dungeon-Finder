using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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

    private Color textColor;

    private void Awake()
    {
        textMesh = transform.GetComponent<TextMeshPro>();
    }

    public void Setup(FCTData fctData)
    {
        textMesh.SetText(fctData.text);
        
        if (fctData.combatValue)
        {
            if (fctData.isGlancing)
            {
                // Glancing Hit
                textMesh.fontSize = 6f;
                sizeTarget = 1.5f;
                textColor = fctData.color;
            }
            else if (!fctData.isCriticalHit)
            {
                // Normal Hit
                textMesh.fontSize = 9f;
                sizeTarget = 2.25f;
                textColor = fctData.color;
            }
            else
            {
                // Critical Hit
                textMesh.fontSize = 12f;
                sizeTarget = 3f;
                textColor = fctData.criticalColor;
            }

            fade = 2f;
            fadeTimer = 0.35f;
            speed = 2.5f;
        }
        else
        {
            textMesh.fontSize = 6;
            sizeTarget = textMesh.fontSize;
            textColor = fctData.color;

            fade = 2f;
            fadeTimer = 0.5f;
            speed = 1f;
        }

        textMesh.color = textColor;

        sortingOrder++;
        textMesh.sortingOrder = sortingOrder;
    }

    private void Update()
    {
        transform.position += new Vector3(randomDirectionVector, speed) * Time.deltaTime;

        fadeTimer -= Time.deltaTime;
        if (fadeTimer < 0)
        {
            textColor.a -= fade * Time.deltaTime;
            textMesh.color = textColor;

            float sizeDif = textMesh.fontSize - sizeTarget;
            textMesh.fontSize -= sizeDif * Time.deltaTime;

            if (textColor.a < 0)
                Destroy(gameObject);
        }
    }
}
