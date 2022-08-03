using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAssets : MonoBehaviour
{
    private static GameAssets _i;

    public static GameAssets i
    {
        get
        {
            if (_i == null) _i = Instantiate(Resources.Load<GameAssets>("GameAssets"));
            return _i;
        }
    }

    [Header("Sounds")]
    public AudioClip click;
    public AudioClip coins;

    [Header("Start")]
    public GameObject heroPrefab;
    public GameObject enemyPrefab;
    public Sprite startBackground;
    public Sprite startBackgroundBlurred;

    [Header("General")]
    public Transform floatingCombatText;
    public Transform abilityCast;
    public NotificationObject notificationPrefab;
    public GameObject rewardPrefab;
    public Transform shortMessage;
    public GameObject blackTransition;

    public Sprite oneMember;
    public Sprite threeMembers;

    [Header("Abilities")]
    public Sprite cancelIcon;
    public Sprite noAbility;
    public Sprite genericAttackAnimation;
    public EffectObject chargeEffect;
    public EffectObject tauntImmune;

    [Header("Inventory")]
    public Sprite emptySlot;
    public Sprite restrictedSlot;
    public GameObject inventoryPrefab;

    [Header("Currency")]
    public Sprite goldIcon;
    public Sprite goldIconBorderless;
    public Sprite spiritOrbIcon;
    public Sprite spiritOrbIconBorderless;
    public Sprite experienceIcon;

    [Header("Shop")]
    public GameObject shopItemPrefab;

    [Header("General Attributes")]
    public Sprite health;
    public Sprite power;
    public Sprite wisdom;
    public Sprite armor;
    public Sprite resistance;
    public Sprite vitality;
    public Sprite speed;
    public Sprite accuracy;
    public Sprite crit;

    [Header("School Multipliers")]
    public Sprite healingMultiplier;
    public Sprite physicalMultiplier;
    public Sprite fireMultiplier;
    public Sprite iceMultiplier;
    public Sprite natureMultiplier;
    public Sprite arcaneMultiplier;
    public Sprite holyMultiplier;
    public Sprite shadowMultiplier;
    public Sprite critMultiplier;

    [Header("Events")]
    public Sprite fightEvent;
    public Sprite bossEvent;
    public Sprite mysteryEvent;
}
