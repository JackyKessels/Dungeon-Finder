using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public enum UnitType
{
    Humanoid,
    Beast,
    Undead,
    Demon,
    Elemental
}

public delegate void EffectEvent(Unit unit);

public delegate void AbilityEvent(Unit caster, Unit target, Active active);

public class Unit : MonoBehaviour
{
    [HideInInspector] public EffectManager effectManager;

    // General
    [HideInInspector] public new string name;
    [HideInInspector] public UnitType unitType;
    [HideInInspector] public Sprite icon;
    [HideInInspector] public SpriteRenderer unitRenderer;
    [HideInInspector] public Sprite sprite;
    [HideInInspector] public bool isEnemy;
    [HideInInspector] public Animator animator;

    // Queue
    [HideInInspector] public BattleState state;
    [HideInInspector] public int queueNumber;
    [HideInInspector] public int battleNumber;
    [HideInInspector] public QueueIconObject orderIcon;
    [HideInInspector] public bool hasTurn;

    // Stats
    public StatsManager statsManager;

    // Spellbook
    public Spellbook spellbook;

    // Floating Combat Text
    [HideInInspector] public FCTHandler fctHandler;

    [HideInInspector] public EffectEvent OnStartBattle;
    [HideInInspector] public EffectEvent OnRoundStart;

    [HideInInspector] public AbilityEvent OnAbilityCast;

    public virtual UnitObject GetUnitObject()
    {
        return null;
    }


    private void Awake()
    {
        effectManager = GetComponent<EffectManager>();
        unitRenderer = GetComponentInChildren<SpriteRenderer>();
        animator = GetComponent<Animator>();
        sprite = unitRenderer.sprite;
    }

    public void TriggerStartBattleEvent()
    {
        OnStartBattle?.Invoke(this);
    }

    public void TriggerRoundStartEvent()
    {
        OnRoundStart?.Invoke(this);
    }

    public void SetState(int i, bool isEnemy)
    {
        this.isEnemy = isEnemy;
        if (isEnemy)
        {       
            if (i == 0)
                state = BattleState.ENEMY_TURN_1;
            else if (i == 1)
                state = BattleState.ENEMY_TURN_2;
            else if (i == 2)
                state = BattleState.ENEMY_TURN_3;
        }
        else
        {
            if (i == 0)
                state = BattleState.MEMBER_TURN_1;
            else if (i == 1)
                state = BattleState.MEMBER_TURN_2;
            else if (i == 2)
                state = BattleState.MEMBER_TURN_3;
        }
    }

    public bool IsTargetEnemy(Unit target)
    {
        // Enemy + Enemy = false
        // Enemy + Friendly = true
        // Not enemy + Enemy = true
        // Not enemy + Friendly = false

        if (isEnemy) // Unit is Enemy
        {
            if (target.isEnemy)
                return false;
            else
                return true;
        }
        else // Unit is Hero
        {
            if (target.isEnemy)
                return true;
            else
                return false;
        }
    }

    public IEnumerator FadeInAndOut(bool fadeIn, bool darkness, float duration)
    {
        SpriteRenderer tempSpriteRenderer = gameObject.GetComponentInChildren<SpriteRenderer>();

        if (tempSpriteRenderer == null)
        {
            yield break;
        }

        Color noAlphaBlack = new Color(0f, 0f, 0f, 0f);
        Color noAlphaCurrent = new Color(1f, 1f, 1f, 0f);
        Color yesAlphaCurrent = new Color(1f, 1f, 1f, 1f);

        float counter = 0f;

        while (counter < duration)
        {
            counter += Time.deltaTime;

            if (fadeIn)
            {
                if (darkness)
                    tempSpriteRenderer.color = Color.Lerp(noAlphaBlack, yesAlphaCurrent, counter / duration);
                else
                    tempSpriteRenderer.color = Color.Lerp(noAlphaCurrent, yesAlphaCurrent, counter / duration);
            }
            else
            {
                if (darkness)
                    tempSpriteRenderer.color = Color.Lerp(yesAlphaCurrent, noAlphaBlack, counter / duration);
                else
                    tempSpriteRenderer.color = Color.Lerp(yesAlphaCurrent, noAlphaCurrent, counter / duration);
            }

            yield return null;
        }
    }

    public void PositionUnit(int i)
    {
        UnitPosition[] positions = isEnemy ? TeamManager.Instance.enemyPositions : TeamManager.Instance.heroPositions;

        transform.position = new Vector3(positions[i].transform.position.x,
                                         positions[i].transform.position.y + (sprite.bounds.size.y * unitRenderer.transform.localScale.y) / 2,
                                         positions[i].transform.position.z);
    }

    public void ReviveUnit(bool fullRestore)
    {       
        statsManager.currentHealth = fullRestore ? statsManager.GetAttributeValue((int)AttributeType.Health) : 1;
        statsManager.isDead = false;
        GetComponentInChildren<SpriteRenderer>().color = Color.white;
        GetComponent<CapsuleCollider2D>().enabled = true;
    }

    public void RestoreHealth()
    {
        statsManager.currentHealth = statsManager.GetAttributeValue((int)AttributeType.Health);
    }

    public IEnumerator MoveIntoBattle(GameObject unit, Vector3 end, float speed)
    {
        while (unit.transform.position != end)
        {
            unit.transform.position = Vector3.MoveTowards(unit.transform.position, end, speed * Time.deltaTime);
            yield return null;
        }
    }
}
