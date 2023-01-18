using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Linq;

public enum BattleState
{
    START,
    MEMBER_TURN_1,
    MEMBER_TURN_2,
    MEMBER_TURN_3,
    ENEMY_TURN_1,
    ENEMY_TURN_2,
    ENEMY_TURN_3,
    WON,
    LOST,
    FLEE
}

public class BattleManager : MonoBehaviour, IUserInterface
{
    #region Singleton
    public static BattleManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.Log("Instance already exists.");
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    #endregion

    [Header("[ Manager Specific ]")]
    public Canvas userInterface;
    public Camera cameraObject;

    private BattleHUD battleHUD;
    private QueueManager queueManager;
    private GameManager gameManager;
    private TeamManager teamManager;
    private InventoryManager inventoryManager;
    private TooltipHandler tooltipHandler;

    private AudioSource audioSource;

    [Header("[ Teams ]")]
    public Transform positions;

    [Header("[ UI ]")]
    public GameObject victoryText;
    public GameObject defeatText;
    public ActionBar actionBar;
    public TextMeshProUGUI announcerText, roundText;
    public TextMeshProUGUI targetingText;
    public GameObject dungeonContainer;
    private TextMeshProUGUI dungeonName;
    private TextMeshProUGUI dungeonStage;

    [Header("[ Mechanics ]")]
    public Hero currentHero;
    public int round;

    private BattleState state;
    private Unit currentUnit;
    private Unit currentTarget;
    private Active currentAbility;

    public bool activeBattle = false;

    void Start()
    {
        battleHUD = BattleHUD.Instance;
        queueManager = QueueManager.Instance;
        gameManager = GameManager.Instance;
        teamManager = TeamManager.Instance;
        inventoryManager = InventoryManager.Instance;
        tooltipHandler = TooltipHandler.Instance;

        audioSource = gameManager.audioSource;
    }

    private void Update()
    {
        if (gameManager.gameState == GameState.BATTLE)
        {
            CheckHeroAbilityHotkeys();

            CheckItemHotkeys();

            CheckOtherHotkeys();
        }
    }

    private void CheckHeroAbilityHotkeys()
    {
        if (KeyboardHandler.CastAbility1())
        {
            OnAbilityButton(0, true);
        }
        if (KeyboardHandler.CastAbility2())
        {
            OnAbilityButton(1, true);
        }
        if (KeyboardHandler.CastAbility3())
        {
            OnAbilityButton(2, true);
        }
        if (KeyboardHandler.CastAbility4())
        {
            OnAbilityButton(3, true);
        }
    }

    private void CheckItemHotkeys()
    {
        if (KeyboardHandler.UseItem1() && actionBar.GetTotalItemAbilities() > 0)
        {
            OnAbilityButton(0, false);
        }
        if (KeyboardHandler.UseItem2() && actionBar.GetTotalItemAbilities() > 1)
        {
            OnAbilityButton(1, false);
        }
        if (KeyboardHandler.UseItem3() && actionBar.GetTotalItemAbilities() > 2)
        {
            OnAbilityButton(2, false);
        }
        if (KeyboardHandler.UseItem4() && actionBar.GetTotalItemAbilities() > 3)
        {
            OnAbilityButton(3, false);
        }
        if (KeyboardHandler.UseItem5() && actionBar.GetTotalItemAbilities() > 4)
        {
            OnAbilityButton(4, false);
        }
        if (KeyboardHandler.UseItem6() && actionBar.GetTotalItemAbilities() > 5)
        {
            OnAbilityButton(5, false);
        }
        if (KeyboardHandler.UseItem7() && actionBar.GetTotalItemAbilities() > 6)
        {
            OnAbilityButton(6, false);
        }
        if (KeyboardHandler.UseItem8() && actionBar.GetTotalItemAbilities() > 7)
        {
            OnAbilityButton(7, false);
        }
    }

    private void CheckOtherHotkeys()
    {
        if (KeyboardHandler.UseFlask())
        {
            OnFlaskButton();
        }
        //if (Input.GetKeyDown(KeyCode.Z))
        //{
        //    OnFleeButton();
        //}
        if (KeyboardHandler.PassTurn())
        {
            OnPassButton();
        }
    }

    public void StartBattle(List<EnemyObject> unitObjects)
    {
        gameManager.gameState = GameState.BATTLE;
        activeBattle = true;

        HeroManager.Instance.heroInformationObject.SetActive(false);

        tooltipHandler.HideTooltip();

        // Initialize Damage Meter
        DamageMeterManager.Instance.ClearDamageMeters();

        // Setup both teams
        TeamManager.Instance.SetupBattle(unitObjects);

        // Go to correct camera and interface
        gameManager.cameraScript.GoToCamera(cameraObject, false);
        EnableUI(true);
        HeroManager.Instance.EnableUI(false);
        DungeonManager.Instance.EnableUI(false);

        actionBar.Initialize();

        // Setup the fight logistics
        StartCoroutine(SetupBattle());
    }

    IEnumerator SetupBattle()
    {
        state = BattleState.START;

        round = 1;

        teamManager.ApplyPreBattleEffects();
        teamManager.TriggerRoundStartPassives();

        queueManager.Setup();

        battleHUD.Refresh();

        // Set current actionbar to 
        currentHero = queueManager.GetFastestHero();
        actionBar.SetupActionBar(currentHero);
        actionBar.SetInteractable(false);

        //announcerText.SetText("Let the battle begin!");
        roundText.SetText("Round " + round);

        yield return new WaitForSeconds(1f);

        StartCoroutine(NextTurn());
    }

    void EndBattle()
    {
        //StopAllCoroutines();

        teamManager.heroes.ExpireEffects();

        StartCoroutine(ExitBattle());
    }

    IEnumerator ExitBattle()
    {
        if (state == BattleState.WON)
        {
            victoryText.SetActive(true);
        }
        else
        {
            defeatText.SetActive(true);
        }

        yield return new WaitForSeconds(2.0f);

        victoryText.SetActive(false);
        defeatText.SetActive(false);

        queueManager.RemoveQueue();

        if (state == BattleState.WON)
        {
            gameManager.BattleWon();
        }
        else if (state == BattleState.LOST)
        {
            gameManager.BattleLost();
        }
        else if (state == BattleState.FLEE)
        {
            gameManager.BattleFled();
        }

        teamManager.ResetEnemy();

        HeroManager.Instance.EnableUI(true);
        EnableUI(false);

        tooltipHandler.HideTooltip();

        activeBattle = false;
    }

    private bool CheckWinCondition()
    {
        if (teamManager.heroes.LivingMembers.Count == 0 || teamManager.enemies.LivingMembers.Count == 0)
        {
            state = teamManager.heroes.LivingMembers.Count == 0 ? BattleState.LOST : BattleState.WON;

            EndBattle();

            return false;
        }

        return true;
    }

    IEnumerator NextTurn()
    {
        // If the battle is over then stop running the rest of this code
        if (!CheckWinCondition())
            yield break;

        // Checks Speed after everyone has had a turn
        // Resorts the queue based on highest Speed for this round

        if (queueManager.queueTurn >= queueManager.queueSize)
        {
            //announcerText.SetText("Next round!");

            yield return new WaitForSeconds(0.5f);

            // NEW ROUND //

            round++;
            roundText.SetText("Round " + round);

            queueManager.Setup();

            // Handle ability cooldowns and effect durations for both teams
            foreach (Unit u in teamManager.heroes.LivingMembers)
            {
                u.spellbook.CooldownAbilities();
                u.effectManager.EffectDurationHandler(ProcType.Round);
            }

            foreach (Unit u in teamManager.enemies.LivingMembers)
            {
                u.spellbook.CooldownAbilities();
                u.effectManager.EffectDurationHandler(ProcType.Round);
            }

            // Trigger passives that apply buffs/debuffs at the start of the new round/
            teamManager.TriggerRoundStartPassives();

            battleHUD.Refresh();
        }

        // Foreach unit in battle, trigger active effects
        foreach (Unit u in teamManager.heroes.LivingMembers)
        {
            u.effectManager.EffectDurationHandler(ProcType.Turn);
        }

        foreach (Unit u in teamManager.enemies.LivingMembers)
        {
            u.effectManager.EffectDurationHandler(ProcType.Turn);
        }

        battleHUD.Refresh();

        currentUnit = queueManager.queueList[queueManager.queueTurn];
        tooltipHandler.HideTooltip();

        while (currentUnit.statsManager.isDead)
        {
            queueManager.queueTurn++;

            if (queueManager.queueTurn >= queueManager.queueSize)
            {
                StartCoroutine(NextTurn());
                yield break;
            }
                
            currentUnit = queueManager.queueList[queueManager.queueTurn];
        }

        state = currentUnit.state;

        if (currentUnit.isEnemy)
        {
            StartCoroutine(EnemyTurn());
        }
        else
        {
            currentHero = (Hero)currentUnit;

            PlayerTurn();
        }

        //-- Start of the turn --//

        StartTurnActions();
        //----------------------------//
    }

    void StartTurnActions()
    {
        CheckWinCondition();

        queueManager.queueTurn++;
    }

    void EndTurn()
    {
        currentUnit.hasTurn = false;
        queueManager.SetIcon(currentUnit);
        StartCoroutine(NextTurn());
    }

    IEnumerator EnemyTurn()
    {
        Enemy currentEnemy = currentUnit as Enemy;

        // "Thinking"
        yield return new WaitForSeconds(1f);

        // Checks if the unit is stunned, yes = pass, no = try ability cast
        if (!currentUnit.effectManager.IsCrowdControlled(CrowdControlType.Stun) && !currentUnit.effectManager.IsCrowdControlled(CrowdControlType.Incapacitate))
        {
            // Handle swift abilities
            Active swiftAbility = currentEnemy.GetOffCooldownSwiftAbility();

            if (swiftAbility != null)
            {
                yield return EnemyCastAbility(swiftAbility, true);

                CheckWinCondition();

                yield return new WaitForSeconds(0.5f);
            }

            // Checks if the unit has any abilities that are not on cooldown
            Active currentAbility = currentEnemy.ChooseValidAbility();

            // Pass turn if there is no valid ability or if there are no targets
            if (currentAbility.activeAbility != null && teamManager.heroes.LivingMembers.Count() != 0)
            {
                // If the chosen ability is a charged ability and nothing is being charged then charge this Turn
                if (currentEnemy.IsCharging(currentAbility))
                {
                    currentEnemy.ChargeTarget(currentAbility);

                    Color passColor = new Color(1f, 0, 0);

                    FCTData fctData = new FCTData(false, currentUnit, "Charging", passColor);
                    currentUnit.fctHandler.AddToFCTQueue(fctData);
                }
                // Otherwise just cast it
                else
                {
                    yield return EnemyCastAbility(currentAbility, false);
                }
            }
            else
            {
                Color passColor = new Color(1f, 0.5f, 0.5f);

                FCTData fctData = new FCTData(false, currentUnit, "Pass", passColor);
                currentUnit.fctHandler.AddToFCTQueue(fctData);
            }
        }
        else
        {
            Color passColor = new Color(1f, 0.5f, 0.5f);

            FCTData fctData = new FCTData(false, currentUnit, "Stunned", passColor);
            currentUnit.fctHandler.AddToFCTQueue(fctData);
        }

        // Wait to show damage taken
        yield return new WaitForSeconds(1.5f);

        EndTurn();
    }

    IEnumerator EnemyCastAbility(Active active, bool swift)
    {
        HandleCooldown(active);

        Enemy castingEnemy = (currentUnit as Enemy);

        CastAbility(active);

        yield return new WaitForSeconds(active.activeAbility.castTime);

        CastAnimation(active);

        if (active.activeAbility is TargetAbility t)
        {
            int x = castingEnemy.CheckTarget(t);

            Unit target = teamManager.heroes.LivingMembers[x];

            active.Trigger(currentUnit, target);
        }
        else if (active.activeAbility is InstantAbility i)
        {
            active.Trigger(currentUnit, null);   
        }

        if (!swift)
        {
            castingEnemy.ResetChargedAbility(castingEnemy);
        }

        // Update HUD
        battleHUD.Refresh();
    }


    private void PlayerTurn()
    {
        actionBar.SetupActionBar(currentHero);
        actionBar.UpdateCooldowns(currentHero);

        if (currentUnit.effectManager.IsCrowdControlled(CrowdControlType.Stun) || currentUnit.effectManager.IsCrowdControlled(CrowdControlType.Incapacitate))
        {
            StartCoroutine(CrowdControlled());
        }
    }

    public void OnAbilityButton(int i, bool isHeroAbility)
    {
        if (isHeroAbility)
        {
            ActionBarButton button = actionBar.abilities[i].GetComponent<ActionBarButton>();

            if (button.interactable)
            {
                StartCoroutine(Ability(i, isHeroAbility));
                button.active = false;
            }
        }
        else
        {
            ActionBarButton button = actionBar.itemBar.transform.GetChild(i).GetComponent<ActionBarButton>();

            if (button.interactable)
            {
                StartCoroutine(Ability(i, isHeroAbility));
                button.active = false;
            }
        }
    }

    public void OnFleeButton()
    {
        ConfirmationBoxHandler.Instance.SetupConfirmationBox("Flee from battle?", new List<ConfirmationButton>()
                                                                                { new ConfirmationButton("Yes", ConfirmFlee),
                                                                                  new ConfirmationButton("No", null) });
    }

    public void OnFlaskButton()
    {
        ActionBarButton button = actionBar.flask.GetComponent<ActionBarButton>();

        if (button.interactable)
        {
            if (currentUnit.spellbook.HasFlask())
            {
                StartCoroutine(DrinkFlask());
            }
            else
            {
                Debug.Log("No flask equipped.");
            }

            button.active = false;
        }
    }

    public void OnPassButton()
    {
        ActionBarButton button = actionBar.pass.GetComponent<ActionBarButton>();

        if (button.interactable)
        {
            StartCoroutine(PassTurn());
            button.active = false;
        }
    }

    IEnumerator CrowdControlled()
    {
        actionBar.SetInteractable(false);

        Color passColor = new Color(1f, 0.5f, 0.5f);

        FCTData fctData = new FCTData(false, currentUnit, "Stunned", passColor);
        currentUnit.fctHandler.AddToFCTQueue(fctData);

        yield return new WaitForSeconds(1.5f);

        EndTurn();
    }

    IEnumerator PassTurn()
    {
        actionBar.SetInteractable(false);

        Color passColor = new Color(1f, 0.5f, 0.5f);

        FCTData fctData = new FCTData(false, currentUnit, "Pass", passColor);
        currentUnit.fctHandler.AddToFCTQueue(fctData);

        yield return new WaitForSeconds(0.75f);

        EndTurn();
    }

    private void ConfirmFlee()
    {
        Debug.Log("RUN AWAY LITTLE GIRL, RUN AWAY");

        Color fleeColor = new Color(1f, 1f, 1f);

        foreach (Unit u in teamManager.heroes.LivingMembers)
        {
            FCTData fctData = new FCTData(false, u, "Flee!", fleeColor);
            u.fctHandler.AddToFCTQueue(fctData);
        }

        state = BattleState.FLEE;
        EndBattle();
    }

    public IEnumerator DrinkFlask()
    {
        actionBar.SetInteractable(false);

        currentAbility = currentUnit.spellbook.flaskAbility;

        CastAbility(currentAbility);

        yield return new WaitForSeconds(currentAbility.activeAbility.castTime);

        CastAnimation(currentAbility);

        currentAbility.Trigger(currentUnit, currentTarget);

        currentAbility.PutOnCooldown();

        battleHUD.Refresh();

        // Wait to show damage taken
        yield return new WaitForSeconds(1.2f);

        //-- Turn ends --//  
        // Only end turn if the ability expends the turn.
        if (currentAbility.activeAbility.endTurn)
        {
            EndTurn();
        }
        else
        {
            actionBar.UpdateCooldowns(currentHero, true);
        }
    }

    public void RemoveUnitFromBattle(Unit u)
    {
        // Set invisible
        StartCoroutine(u.FadeInAndOut(false, true, 1f));

        u.hasTurn = false;

        // Set dead
        teamManager.RemoveMember(u);

        //battleHUD.Refresh();
        //battleHUD.RemoveHUD(u);
        queueManager.RemoveFromSpeedList(u);
    }

    IEnumerator Ability(int spellNumber, bool isHeroAbility)
    {
        // Return if not a valid cast
        if (isHeroAbility)
        {
            if (currentUnit.spellbook.activeSpellbook[spellNumber].activeAbility == null)
                yield break;
        }
        else
        {
            if (currentUnit.spellbook.itemAbilities[spellNumber].activeAbility == null)
                yield break;
        }

        bool succesfulCast = false;

        actionBar.SetInteractable(false);

        Image icon;
        Sprite tempSprite;

        // Get the correct actionbar ability and save the icon
        if (isHeroAbility)
        {
            currentAbility = currentUnit.spellbook.activeSpellbook[spellNumber];

            icon = actionBar.abilities[spellNumber].GetComponent<Image>();
            tempSprite = icon.sprite;
        }
        else
        {
            currentAbility = currentUnit.spellbook.itemAbilities[spellNumber];

            icon = actionBar.itemBar.transform.GetChild(spellNumber).GetComponent<Image>();
            tempSprite = icon.sprite;
        }

        //-- Do Spell things --//
        if (currentAbility.activeAbility is TargetAbility t)
        {
            // Hero is taunted
            if (currentUnit.effectManager.TauntedBy() != null && t.targetsEnemies)
            {
                int targetNumber = currentUnit.effectManager.TauntedBy().battleNumber;
                currentTarget = teamManager.enemies.GetUnit(targetNumber);
            }
            // Start targeting mode
            else
            {
                currentTarget = null;

                icon.sprite = GameAssets.i.cancelIcon;

                //-- Do Player actions --//
                targetingText.gameObject.SetActive(true);
                targetingText.text = "Choose a target";

                yield return StartCoroutine(WaitForTarget(t, spellNumber, isHeroAbility));
            }

            // Result of WaitForTarget
            if (currentTarget != null)
            {
                targetingText.gameObject.SetActive(false);
                icon.sprite = tempSprite;

                succesfulCast = true;

                CastAbility(currentAbility);

                yield return new WaitForSeconds(t.castTime);

                CastAnimation(currentAbility);

                currentAbility.Trigger(currentUnit, currentTarget);

                battleHUD.Refresh();
                //-----------------------//
            }
            else
            {
                succesfulCast = false;
            }
        }
        else if (currentAbility.activeAbility is InstantAbility i)
        {
            succesfulCast = true;

            CastAbility(currentAbility);

            yield return new WaitForSeconds(i.castTime);

            CastAnimation(currentAbility);

            currentAbility.Trigger(currentUnit, currentTarget);

            battleHUD.Refresh();
        }
        else
        {
            yield return new WaitForSeconds(1f);
        }

        if (succesfulCast)
        {
            if (isHeroAbility)
                actionBar.abilities[spellNumber].GetComponent<ActionBarButton>().active = true;
            else
                actionBar.itemBar.transform.GetChild(spellNumber).GetComponent<ActionBarButton>().active = true;

            HandleCooldown(currentAbility);

            // Wait to show damage taken
            yield return new WaitForSeconds(1.5f);

            //-- Turn ends --//  
            // Only end turn if the ability expends the turn.
            if (currentAbility.activeAbility.endTurn)
            {
                EndTurn();
            }
            else
            {
                actionBar.UpdateCooldowns(currentHero, true);

                CheckWinCondition();
            }
        }
        else
        {
            // Cancel Cast //

            Debug.Log("CANCEL CAST");

            targetingText.gameObject.SetActive(false);

            actionBar.UpdateCooldowns(currentHero);

            icon.sprite = tempSprite;
        }

    }

    private void CastAbility(Active a)
    {
        FCTDataSprite fctSprite = new FCTDataSprite(currentUnit, a, true);
        currentUnit.fctHandler.AddToFCTQueue(fctSprite);
    }

    private void CastAnimation(Active a)
    {
        if (a.activeAbility.soundEffect != null)
            audioSource.PlayOneShot(a.activeAbility.soundEffect);

        if (a.activeAbility.animationType == AnimationType.Attack)
        {
            if (currentUnit.isEnemy)
            {
                currentUnit.animator.Play("Base Layer.Enemy_Attack", 0);
            }
            else
            {
                currentUnit.animator.Play("Base Layer.Hero_Attack", 0);
            }
        }
    }

    IEnumerator WaitForTarget(TargetAbility t, int index, bool isHeroAbility)
    {
        ActionBarButton button;

        if (isHeroAbility)
            button = actionBar.abilities[index].GetComponent<ActionBarButton>();
        else
            button = actionBar.GetItemAbilityButtons()[index].GetComponent<ActionBarButton>();

        while (true)
        {
            if (Input.GetMouseButtonDown(0))
            {
                // GAME RAYCAST
                RaycastHit2D hit = Physics2D.Raycast(cameraObject.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

                if (hit.collider != null)
                {
                    if (hit.collider.gameObject.tag == "Unit")
                    {
                        if (hit.collider.gameObject.GetComponent<Unit>().isEnemy && t.targetsEnemies)
                        {
                            currentTarget = hit.collider.GetComponent<Unit>();
                            actionBar.SetInteractable(false);
                            yield break;
                        }
                        else if (!hit.collider.gameObject.GetComponent<Unit>().isEnemy && t.targetsAllies)
                        {
                            currentTarget = hit.collider.GetComponent<Unit>();
                            actionBar.SetInteractable(false);
                            yield break;
                        }
                        else
                        {
                            Debug.Log("This is not a valid target");
                        }
                    }
                    else
                    {
                        Debug.Log("This is not a valid target");
                    }
                }

                // UI RAYCAST

                PointerEventData pointer = new PointerEventData(EventSystem.current);
                pointer.position = Input.mousePosition;
                List<RaycastResult> hitObjects = new List<RaycastResult>();

                EventSystem.current.RaycastAll(pointer, hitObjects);
 
                // Cancel targeting
                foreach (RaycastResult result in hitObjects)
                {
                    if (isHeroAbility)
                    {
                        if (result.gameObject == actionBar.abilities[index] && !button.active)
                        {
                            currentTarget = null;
                            button.active = true;
                            yield break;
                        }
                    }
                    else
                    {
                        if (result.gameObject == actionBar.itemBar.transform.GetChild(index).gameObject && !button.active)
                        {
                            currentTarget = null;
                            button.active = true;
                            yield break;
                        }
                    }
                }
            }

            if ((KeyboardHandler.Escape() || GeneralUtilities.GetMappedAbilityKey(index, isHeroAbility)) && !button.active)
            {
                currentTarget = null;
                button.active = true;
                yield break;
            }

            yield return null;
        }
    }

    private void HandleCooldown(Active active)
    {
        //if (active.cooldown > 0)
        //{
            if (active.activeAbility.resetChance > 0 && active.activeAbility.SuccessfulReset())
            {
                Color color = GeneralUtilities.ConvertString2Color("#9DD8FF");

                FCTData fctData = new FCTData(false, currentUnit, "Reset!", color);
                currentUnit.fctHandler.AddToFCTQueue(fctData);
                //FloatingCombatText.SendText(fctData);
            }
            else
            {
                active.currentCooldown = active.cooldown + 1;
            }
        //}
    }

    public void EnableUI(bool show)
    {
        userInterface.gameObject.SetActive(show);
    }
}
