using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AbilityCast : MonoBehaviour
{
    public static AbilityCast CastAbility(FCTDataSprite fctsSprite)
    {
        float overheadOffset = (fctsSprite.unit.sprite.bounds.size.y * fctsSprite.unit.unitRenderer.transform.localScale.y) / 2;

        if (!fctsSprite.moving)
            overheadOffset = 2f;

        Vector3 newPos = new Vector3(fctsSprite.unit.transform.position.x, fctsSprite.unit.transform.position.y + overheadOffset, fctsSprite.unit.transform.position.z);
        Transform transform = Instantiate(GameAssets.i.abilityCast, newPos, Quaternion.identity);

        AbilityCast abilityCast = transform.GetComponent<AbilityCast>();
        abilityCast.Setup(fctsSprite.active, fctsSprite.moving);

        return abilityCast;
    }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private SpriteRenderer spriteRenderer;

    private static int sortingOrder;

    private float fade;
    private float fadeTimer;
    private float speed;
    private float sizeTarget;

    private Color spriteColor;

    public void Setup(Active active, bool moving)
    {
        spriteRenderer.sprite = active.activeAbility.icon;
        spriteColor = spriteRenderer.color;

        fade = 3f;
        fadeTimer = 0.5f;
        speed = moving ? 1f : 0;

        sortingOrder++;
        spriteRenderer.sortingOrder = sortingOrder;
    }

    private void Update()
    {
        transform.position += new Vector3(0f, speed) * Time.deltaTime;

        fadeTimer -= Time.deltaTime;
        if (fadeTimer < 0)
        {
            spriteColor.a -= fade * Time.deltaTime;
            spriteRenderer.color = spriteColor;

            if (spriteColor.a < 0)
                Destroy(gameObject);
        }
    }
}
