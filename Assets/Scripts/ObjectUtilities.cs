using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public static class ObjectUtilities
{
    public static void ClearContainer(GameObject container)
    {
        foreach (Transform child in container.transform)
        {
            Object.Destroy(child.gameObject);
        }

        container.transform.DetachChildren();
    }

    public static GameObject CreateSimplePrefab(GameObject prefab, GameObject parent)
    {
        GameObject obj = null;

        if (prefab)
        {
            obj = Object.Instantiate(prefab, parent.transform.position, Quaternion.identity);
            obj.transform.SetParent(parent.transform);
            obj.transform.localScale = Vector3.one;
        }

        return obj;
    }

    public static TooltipIcon CreateTooltipIcon(GameObject parent)
    {
        var obj = CreateSimplePrefab(GameAssets.i.tooltipIconPrefab, parent);

        TooltipIcon tooltip = obj.GetComponent<TooltipIcon>();

        return tooltip;
    }

    public static void AddEvent(GameObject obj, EventTriggerType type, UnityAction<BaseEventData> action)
    {
        EventTrigger trigger = obj.GetComponent<EventTrigger>();
        var eventTrigger = new EventTrigger.Entry();
        eventTrigger.eventID = type;
        eventTrigger.callback.AddListener(action);
        trigger.triggers.Add(eventTrigger);
    }

    public static void CreateSpecialEffects(List<ParticleSystem> specialEffects, Unit target, bool checkIsDead = false)
    {
        if (checkIsDead && target.statsManager.isDead)
        {
            return;
        }

        if (specialEffects.Count > 0)
        {
            SpriteRenderer sprite = target.GetComponentInChildren<SpriteRenderer>();

            foreach (ParticleSystem specialEffect in specialEffects)
            {
                ParticleSystem particle = Object.Instantiate(specialEffect, target.transform.position, Quaternion.identity);
                particle.transform.SetParent(sprite.transform);
                particle.transform.localScale = Vector3.one;
            }
        }
    }

    public static void LaunchSpecialEffects(List<ParticleSystem> specialEffects, Unit caster, Unit target, float travelTime, bool trailing)
    {
        if (specialEffects.Count > 0)
        {
            SpriteRenderer sprite = target.GetComponentInChildren<SpriteRenderer>();

            foreach (ParticleSystem specialEffect in specialEffects)
            {
                ParticleSystem particle = Object.Instantiate(specialEffect, caster.transform.position, Quaternion.identity);
                particle.transform.SetParent(caster.transform);
                particle.transform.localScale = Vector3.one;
                Missile missile = particle.gameObject.AddComponent<Missile>();
                missile.Setup(caster, target, travelTime, trailing);
            }
        }
    }

    public static void BlackTransition(bool fadeIn)
    {
        GameObject genericHandler = GameObject.Find("UI Generic Handler");

        GameObject obj = CreateSimplePrefab(GameAssets.i.blackTransition, genericHandler);

        BlackTransition blackTransition = obj.GetComponent<BlackTransition>();

        blackTransition.fadeIn = fadeIn;

        if (fadeIn)
        {
            blackTransition.canvasGroup.alpha = 1;

        }
        else
        {
            blackTransition.canvasGroup.alpha = 0;
        }

    }

    public static List<GameObject> GetChildObjects(GameObject obj)
    {
        List<GameObject> childObjects = new List<GameObject>();

        foreach (Transform child in obj.transform)
        {
            childObjects.Add(child.gameObject);
        }

        return childObjects;
    }
}
