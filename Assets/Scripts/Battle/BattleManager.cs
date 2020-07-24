using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

/// <summary>
/// Attached to BattleMenuOptions (the container to UI objects) in the Battle scene.
/// This script is the main driver of the battle, and communicates with all components
/// through direct reference or through triggering or responding to events.
/// All scene-level variables are stored here.
/// Anything that needs to persist outside the scene is passed to BattleLoader.
/// </summary>
public class BattleManager : MonoBehaviour
{
    #region Fields
    // Debug-only: for setting in the inspector which battle to test
    [SerializeField]
    EnemyName defaultBattle = EnemyName.none;
    // Debug-only: toggles whether or not to set up a "buff" party of stock heroes
    [SerializeField]
    bool buffParty = false;

    // For displaying gold earnings
    [SerializeField]
    Text rewardText = null;
    [SerializeField]
    GameObject bankTarget = null;
    
    // Gold reward in the "bank", and reward on its way to the bank
    int battleGoldReward = 0;
    int goldToAdd = 0;

    // Hero stat displays, and the hero game objects
    [SerializeField]
    GameObject[] heroStatusBar = new GameObject[4];

    GameObject[] heroObj = new GameObject[4];
    BattleHero[] heroes;
    int activeHero = 0; // partyID of active hero

    // For debugging and cheating, applies to damage
    int cheatDamageMultiplier = 1; 

    // Main database of current enemies. Made of BattleEnemys
    EnemyParty enemyParty;

    // Time between enemy attacks
    EventTimer enemyAttackTimer;
    const float EnemyAttackTimerDuration = 0.1f;

    // Template of the enemy display name label in the bottom right
    [SerializeField]
    Text enemyChoiceText = null;

    // For creating the requisite enemychoice texts
    float enemyChoiceHeight;
    Vector2 enemyChoicePosition;
    Vector3 enemyChoiceLocalScale;
    List<Text> enemyChoiceTexts = new List<Text>();

    // Used to disable UI options in HeroStartTurn during the final enemy death
    bool finalEnemyDeath = false;
    bool finalHeroDeath = false;

    // List of all used battleID numbers (now unused)
    //List<int> usedIDs = new List<int>();

    // Submenu prefabs for creation later
    [SerializeField]
    GameObject magicSubMenuObj = null, blitzSubMenuObj = null, toolsSubMenuObj = null;

    // For autocrossbow
    int boltsInFlight = 0;

    // Center of hero or enemy location (For later whole-party effects, unused)
    Vector2 heroPartyPosition = new Vector2(-4.75f, 1.75f);
    Vector2 enemyPartyPosition = new Vector2(4.75f, 1.75f);

    // The battleID of the active heros target
    int choice = 0;

    // for UI enable/disable during player turns
    UIEnabler uiEnabler;

    // Events invoked by BattleManager
    Battle_DisplayDamageEvent displayDamageEvent = new Battle_DisplayDamageEvent();
    Battle_EnemyDeathBeginEvent enemyDeathBeginEvent = new Battle_EnemyDeathBeginEvent();
    Battle_RefreshHPMPSlidersEvent refreshHPMPSliders = new Battle_RefreshHPMPSlidersEvent();
    Battle_RefreshMPCostsEvent refreshMPCosts = new Battle_RefreshMPCostsEvent();

    // Spell effects prefab storage
    GameObject prefabFireball;
    GameObject prefabPoisonball;

    // Color references
    Color poisonColor = new Color(0.32f, 0.63f, 0.24f);

    // True until battle is fully initialized
    bool battleInitializing = true;

    #endregion Fields


    #region Awake_Start_Update_Methods
    /// <summary>
    /// Called upon creation. Initialization is only run if the battle scene is being run directly
    /// within the editor, otherwise it has already been run and this call does nothing.
    /// </summary>
    private void Awake()
    {
        Initializer.Run();
    }

    // Start is called before the first frame update.
    // This is an IEnumerator instead of a conventional method to use Coroutine features,
    // so that the method finishes only after all other components have finished loading.
    IEnumerator Start()
    {
        // BattleManager listens for and invokes several events.
        
        // invoked by TurnInvoker (Inv), and begins the enemyfight timer
        // passes null as damage when from inv
        EventManager.AddListener_Battle_TurnOver(TurnOver); 
        
        EventManager.AddListener_Battle_ChoiceSelection(SetChoice); // UI targeting of a target (highlighting)
        EventManager.AddListener_Battle_FightChoice(FightChoice); // When a target is confirmed

        EventManager.AddListener_Battle_SubMenuSelection(SubMenuChoice); // UI choice within a submenu


        // invoked by a propel object when a fight collision occurs, cues results of action (ex fight)
        EventManager.AddListener_Battle_PropelHitTarget(PropelTargetHit);
        

        EventManager.AddInvoker_Battle_DisplayDamage(this); // triggers the floating damage numbers
        EventManager.AddInvoker_Battle_RefreshHPMPSliders(this); // updates sliders 
        EventManager.AddInvoker_Battle_RefreshMPCosts(this); // updates MP cost display in Magic submenu

        EventManager.AddListener_Battle_ApplyDamage(ApplyDamage); // changes numeric value of HP/MP

        EventManager.AddInvoker_Battle_EnemyDeathBegin(this); // announces the start of enemy death sequence
        EventManager.AddListener_Battle_EnemyDeathEnd(EnemyDeath); // listens for the end of the enemy death sequence

        EventManager.AddListener_Battle_AddCoins(AddCoinsToPot); // listens for the arrival of a coin to the "bank"

        // Create a buffed party for debugging
        if (buffParty)
        {
            BattleLoader.NewBuffParty();
        }
    
        // Reference to hero array in BattleLoader
        heroes = BattleLoader.Party.Hero;

        // Define positions for hero gameObjects, based on number of heroes
        // The numbers below indicate the difference between default position and hero position
        Vector2[] positions;
        switch (heroes.Length)
        {
            case 1:
                positions = new Vector2[] { Vector2.zero };
                break;
            case 2:
                positions = new Vector2[] { new Vector2(0.75f, 0.625f), new Vector2(0f, -0.625f) };
                break;
            case 3:
                positions = new Vector2[] { new Vector2(0.75f, 1.25f), Vector2.zero, new Vector2(-0.75f, -1.25f) };
                break;
            case 4:
                positions = new Vector2[] { new Vector2(1.5f, 1.875f), new Vector2(0.75f, 0.625f),
                new Vector2(0f, -0.625f), new Vector2(-0.75f,-1.875f) };
                break;
            default:
                positions = new Vector2[] { Vector2.zero };
                break;
        }

        // Set up heroes
        for (int i = 0; i < heroes.Length; i++)
        {
            // instantiate heroObjs
            if (heroes[i] != null)
            {
                // Load the correct hero prefab, according to HeroType (Ex: EdgarHero)
                string path = @"BattlePrefabs\Hero\" + heroes[i].HeroType + "Hero";
                heroObj[i] = GameObject.Instantiate(Resources.Load<GameObject>(path));

                // Place heroObjs
                Vector2 position = heroObj[i].transform.position;
                position += positions[i];
                heroObj[i].transform.position = position;
                heroObj[i].name = "Hero " + i;

                // Push gameObject info into BattleHero object
                // Note: i is the partyID, not battleID
                // battleID = -partyID - 1. Ergo, the first hero(0) is -1 in battle.
                heroes[i].LoadObjs(i, heroObj[i]);

                // Store references to sliders and damage displays, and refresh them
                HeroDamageDisplay display = heroObj[i].GetComponent<HeroDamageDisplay>();
                display.SetHeroStatusBar(heroStatusBar[i]);
                display.SetDamageDisplay(heroes[i].BattleID, heroes[i].HP, heroes[i].HPMax,
                    heroes[i].MP, heroes[i].MPMax);
                display.SetNameAndHPMP(heroes[i].FullName, heroes[i].HP, heroes[i].MP);
            }
        }

        // Set up enemy party
        if (BattleLoader.BattleParty == EnemyName.none) // Debug-only
        {
            enemyParty = new EnemyParty(defaultBattle); // Default party is 1 wererat
        }
        else
        {
            // Construct enemyParty based on the EnemyName that triggered the battle
            // This creates gameObjects, BattleEnemy objects, and connects all.
            enemyParty = new EnemyParty(BattleLoader.BattleParty);
        }
        
        // Setup attack timer
        enemyAttackTimer = gameObject.AddComponent<EventTimer>();
        enemyAttackTimer.Duration = EnemyAttackTimerDuration;
        enemyAttackTimer.AddListener_Finished(ChooseEnemyAttack);

        // Create and label the bottom right enemy name selections
        CreateChoices();
        NumberChoices();

        // Set up UIEnabler (turns UI on and off)
        uiEnabler = gameObject.AddComponent<UIEnabler>();
        uiEnabler.RegisterBattleManager(this);

        // Load spell prefabs
        prefabFireball = Resources.Load<GameObject>(@"BattlePrefabs\Fireball");
        prefabPoisonball = Resources.Load<GameObject>(@"BattlePrefabs\Poisonball");

        // Wait until all other objects finish loading
        yield return new WaitForEndOfFrame();

        // ----------
        // Continuing at the end of frame, start the first turn of the battle
        TurnCounter.GenerateFirstTurns(BattleLoader.Party, enemyParty);

        // Skip the first turn, since it's random anyway. This queues the next action.
        TurnOver(null, TurnCounter.CurrentID);

        yield return null; // End of Couroutine
    }

    // Update is called once per frame
    void Update()
    {
        // If Esc, bring up Pause menu
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            MenuManager.GoToMenu(MenuName.Pause);
        }

        // Gold is added from the enemy to the bank on each frame
        if (goldToAdd > 0)
        {
            int amountToAdd = Mathf.CeilToInt((float)goldToAdd / 10f);
            battleGoldReward += amountToAdd;
            goldToAdd -= amountToAdd;
            rewardText.text = battleGoldReward.ToString();
            BattleLoader.BattleEarnings = battleGoldReward;
        }
    }
    #endregion Awake_Start_Update_Methods

    // Invoked when a coin reaches the "bank"
    void AddCoinsToPot(int value)
    {
        goldToAdd += value;
    }

    #region User_Input_Methods
    // Invoked when user clicks a "choice" object (Ex: enemy, enemy display name)
    public void SetChoice(int choice)
    {
        this.choice = choice;
    }

    // DefendText.OnClick()
    public void Click_DefendOption()
    {
        AudioManager.Chirp();
        heroes[activeHero].BattleMode = BattleMode.Defend;

        uiEnabler.EnableUI(false);
        TurnOver(null, TurnCounter.CurrentID);
    }

    // RunicText.OnClick()
    public void Click_RunicOption()
    {
        AudioManager.Chirp();
        AudioManager.PlaySound(AudioClipName.Runic);
        heroes[activeHero].BattleMode = BattleMode.Runic;

        uiEnabler.EnableUI(false);
        TurnOver(null, TurnCounter.CurrentID);
    }

    // StealText.OnClick()
    public void Click_StealOption()
    {
        AudioManager.Chirp();
        heroes[activeHero].BattleMode = BattleMode.Steal;
        heroes[activeHero].Sprites.UpdateSprite(Stance.Defend);
        PropelHero();

        uiEnabler.EnableUI(false);
        //TurnOver(null, TurnCounter.CurrentID);
    }

    // BlitzText.OnClick()
    public void Click_BlitzOption()
    {
        AudioManager.Chirp();
        // instantiate blitz menu
        // Sabin can Kick (does 0.5 physical damage to two enemies)
        // Sabin can AuraBolt (does 2.0 magic damage to one target) (unused)
        // Sabin can FireDance (does 1.0 magic damage to all enemies) (unused)
        blitzSubMenuObj.SetActive(true);
    }

    // ToolsText.OnClick()
    public void Click_ToolsOption()
    {
        AudioManager.Chirp();
        // instantiate blitz menu
        // Edgar can AutoCrossbow (does 0.3 phys damage to four random enemies)
        // Edger can Drill (does 1.2 physical damage to one enemy, no reduce)
        // Edgar can Chainsaw (unused)
        toolsSubMenuObj.SetActive(true);
    }

    // MagicText.OnClick()
    public void Click_MagicOption()
    {
        AudioManager.Chirp();
        // update selection and costs of spells, and whether they can be clicked.
        // based on HeroType
        refreshMPCosts.Invoke(heroes[activeHero].MP);

        // display magic submenu
        magicSubMenuObj.GetComponent<MagicSubMenu>().Initialize(heroes[activeHero].HeroType);
        magicSubMenuObj.SetActive(true);
        
    }
    
    public void Click_FleeOption ()
    {
        AudioManager.PlaySound(AudioClipName.RunAway); 
        MenuManager.GoToMenu(MenuName.BattleToTown);
    }

    public void Click_ItemOption()
    {
        AudioManager.Chirp(); 
        heroes[activeHero].BattleMode = BattleMode.Item;
        GameObject.Instantiate(Resources.Load<GameObject>(@"MenuPrefabs\prefabBattleInventory"));
    }
    #endregion User_Input_Methods

    #region Enemy_Actions
    // occurs at the end of an enemyAttackTimer
    void ChooseEnemyAttack()
    {
        // Store reference to the current enemy
        BattleEnemy enemy = enemyParty.Enemies[TurnCounter.CurrentID];

        // Only proceed if enemy HP > 0
        if (!enemy.IsDead)
        {
            // Create a list of potential, alive, heroes as victims
            List<BattleHero> victims = new List<BattleHero>();
            foreach (BattleHero potentialVictim in heroes)
            {
                if (!potentialVictim.IsDead)
                {
                    victims.Add(potentialVictim);
                }
            }

            // Don't bother attacking if all heros are dead
            if (victims.Count <= 0) { return; }

            // Pick a random valid hero as victim
            victims = Randomize<BattleHero>.List(victims);
            BattleHero victim = victims[0];

            // Check for poisoner attack, 25% chance
            List<EnemyName> poisonerList = new List<EnemyName>() { EnemyName.Wererat,
                EnemyName.Grease_Monkey,EnemyName.Cirpius, EnemyName.Poplium,EnemyName.Ninja,
                EnemyName.Apokryphos, EnemyName.Brainpan, EnemyName.Misfit, EnemyName.Atma_Weapon };
            
            if (poisonerList.Contains(enemy.Name) && (Random.value < 0.25f) // 25% chance
                && enemy.MP >= BattleAbilityData.Data[BattleMode.Magic_Poison].MP)
            {
                // If Runic is active, trigger Runic animation
                victim = TestForRunicOnCast(victim, AudioClipName.UseTome);

                // Create poison gameObject and send it at the victim
                PropelObject enemyPropel = enemy.GameObject.GetComponentInChildren<PropelObject>();
                GameObject poisonball = Instantiate(prefabPoisonball);
                poisonball.transform.position = enemy.GameObject.transform.position;
                poisonball.GetComponent<PropelProjectile>().PropelShoot(
                    enemy.BattleID, poisonball.transform.position, enemyPropel,
                    heroObj[victim.PartyID], victim.BattleID, BattleMode.Magic_Poison);

                // damage and turnover called upon impact
                return;
            }

            // Check for fireballer attack, 15% chance
            List<EnemyName> fireballerList = new List<EnemyName>() { EnemyName.Magitek_Armor,
            EnemyName.Tunnel_Armor, EnemyName.Bomb,EnemyName.Spit_Fire, EnemyName.Chupon,
                EnemyName.Ghost, EnemyName.Apokryphos, EnemyName.Misfit, EnemyName.Dragon,
                EnemyName.Atma_Weapon};

                        
            if (fireballerList.Contains(enemy.Name) && (Random.value < 0.20f) // 20% chance
                && enemy.MP >= BattleAbilityData.Data[BattleMode.Magic_Fireball].MP)
            {
                // If Runic is active, trigger Runic animation
                victim = TestForRunicOnCast(victim, AudioClipName.UseTome);

                // Create fireball and send it at the victim
                PropelObject enemyPropel = enemy.GameObject.GetComponentInChildren<PropelObject>();
                GameObject fireball = Instantiate(prefabFireball);
                fireball.transform.position = enemy.GameObject.transform.position;
                fireball.GetComponent<PropelProjectile>().PropelShoot(
                    enemy.BattleID, fireball.transform.position, enemyPropel,
                    heroObj[victim.PartyID], victim.BattleID, BattleMode.Magic_Fireball);

                // damage and turnover called upon impact
                return;
            }

            // if neither poisonball or fireball, physical attack by launching the enemy itself
            PropelEnemy(TurnCounter.CurrentID, victims[0]);
        }
    }

    // Returns the PartyID of a hero currently in BattleMode.Runic, null if none
    // Also handles audio of an enemy spell casting, and triggers Runic animation
    BattleHero TestForRunicOnCast(BattleHero defaultHero, AudioClipName defaultCastAudio)
    {
        int? runicID = null;
        foreach (BattleHero hero in heroes)
        {
            if (hero.BattleMode == BattleMode.Runic) 
            {
                runicID = hero.PartyID;
                hero.Sprites.UpdateSprite(Stance.RunicBlade);
                AudioManager.PlaySound(AudioClipName.Runic);
            }
        }

        // Play normal spellcasting audio if not Runic
        if (runicID == null) 
        { 
            AudioManager.PlaySound(defaultCastAudio);
            return defaultHero;
        }

        return heroes[(int)runicID];
    }

    // Calls Propel, necessary due to distinction between partyID and battleID
    void PropelEnemy(int enemyNum, BattleHero victim)
    {
        BattleEnemy enemy = enemyParty.Enemies[enemyNum];

        enemy.GameObject.GetComponentInChildren<PropelObject>().Propel(enemyNum, enemy.Position,
            heroObj[victim.PartyID], victim.BattleID, BattleMode.Fight);

    }
    #endregion

    #region Enemy_Targeting
    /// <summary>
    /// Creates the enemy display names in the bottom right, which can be
    /// used for targetting in lieu of clicking the enemy sprites. Either is a "choice."
    /// </summary>
    void CreateChoices()
    {
        // Store references to choice text template 
        enemyChoiceHeight = enemyChoiceText.rectTransform.rect.height;
        enemyChoicePosition = enemyChoiceText.transform.localPosition;
        enemyChoiceLocalScale = enemyChoiceText.transform.localScale;

        // Add template as first choice text
        enemyChoiceTexts.Add(enemyChoiceText);
        
        // Create additional choice texts if there's > 1 enemy
        for (int i = 1; i < enemyParty.Enemies.Count; i++)
        {
            GameObject newChoice = GameObject.Instantiate(enemyChoiceText.gameObject);
            newChoice.transform.SetParent(this.transform);
            newChoice.transform.localScale = enemyChoiceLocalScale;

            enemyChoiceTexts.Add(newChoice.GetComponent<Text>());
        }

    }

    /// <summary>
    /// Assigns new battleIDs to all valid enemies and choice texts.
    /// Sets the text of the choice texts, based on the current enemies.
    /// Called at the start of battle and everytime an enemy completes its death sequence.
    /// </summary>
    void NumberChoices()
    {
        // Only run if > 0 enemies
        for (int i = 0; i < enemyParty.Enemies.Count; i++)
        {
            // Reposition choice texts
            enemyChoiceTexts[i].text = enemyParty.Enemies[i].FullName;
            enemyChoiceTexts[i].GetComponent<EnemyChoice>().ChoiceNumber = i;
            enemyChoiceTexts[i].transform.localPosition = enemyChoicePosition + new Vector2(0, -enemyChoiceHeight * i / 2);
            // clear any selection
            enemyChoiceTexts[i].GetComponent<EnemyChoice>().MakeSelection(-1);
            enemyChoiceTexts[i].enabled = !enemyParty.Enemies[i].IsDead;
            
            // Set a new battleID in all enemy objects and gameObjects
            GameObject enemy = enemyParty.Enemies[i].GameObject;
            enemy.GetComponentInChildren<EnemyChoice>().ChoiceNumber = i;
            enemy.GetComponentInChildren<EnemyChoice>().MakeSelection(-1);
            enemy.GetComponentInChildren<DamageDisplay>(true).SetID(i);
            enemy.GetComponentInChildren<EnemyDeath>(true).SetID(i);
            enemy.GetComponentInChildren<PropelObject>(true).SetID(i);

            // if enemy is currently dying, prevent it from being clicked.
            if (enemyParty.Enemies[i].IsDead)
            {
                enemyChoiceTexts[i].enabled = false;
                enemy.GetComponentInChildren<PolygonCollider2D>(true).enabled = false;
            }
        }

        // Select the last non-dead enemy
        choice = 0;
        for (int i = enemyParty.Enemies.Count-1; i >= 0; i--)
        {
            if (!enemyParty.Enemies[i].IsDead) { choice = enemyParty.Enemies[i].BattleID; }
        }
    }
    #endregion Enemy_Targeting

    #region Hero_Abilities
    /// <summary>
    /// Places hero gameObject in its original position. Used after giving the hero a command,
    /// and at the end of a "propel".
    /// </summary>
    public void ReturnHeroToPosition()
    {
        heroObj[activeHero].transform.position = heroes[activeHero].HomePosition;
    }

    // Invoked when a selction is made on a Fight choice (enemy sprite or name is clicked)
    void FightChoice()
    {
        // Disable all UI elements while action is carried out
        uiEnabler.EnableUI(false);
        
        // Sets the action being executed (fight)
        heroes[activeHero].BattleMode = BattleMode.Fight;

        // Sets the correct sprite (during propel)
        heroes[activeHero].Sprites.UpdateSprite(Stance.Jump, null, 1); // use jump[1], no timer
        
        // Launches the hero at the enemy
        PropelHero();
    }

    /// <summary>
    /// Sets up the Propel method with the correct battleIDs
    /// </summary>
    void PropelHero()
    {
        // only respond to FightChoice invocations if it is the hero's turn
        if (heroes[activeHero].IsHeroTurn)
        {
            BattleEnemy enemy = enemyParty.Enemies[choice];
            if (!enemy.IsDead)
            {
                heroObj[activeHero].GetComponent<PropelObject>().Propel(
                    heroes[activeHero].BattleID, heroes[activeHero].HomePosition,
                    enemy.GameObject, choice, heroes[activeHero].BattleMode);
            }
        }
    }

    // Invoked when a SubMenu option is selected (Magic, Tools, Blitz)
    // Calls TurnOver if no object is propelled in the action
    void SubMenuChoice (BattleMode mode)
    {
        // Disable ui while action is carried out
        uiEnabler.EnableUI(false); 
        
        // store references
        BattleAbility ability = BattleAbilityData.Data[mode];
        BattleHero hero = heroes[activeHero];

        // choose action to carry out based on SubMenu option (BattleMode)
        switch (mode)
        {
            case BattleMode.Magic_Cure:
                // cure caster
                AudioManager.PlaySound(AudioClipName.Cure);
                Damage damage = BattleMath.Damage(hero.BStats, null, ability);
                displayDamageEvent.Invoke(hero.BattleID, damage); 
                if (ability.MP != null)
                {
                    hero.TakeMPDamage((int)ability.MP); 
                    refreshHPMPSliders.Invoke(hero.BattleID, 0, (int)ability.MP);
                }
                hero.Sprites.UpdateSprite(Stance.Cast, 0.2f, 0); 
                TurnOver(null, TurnCounter.CurrentID); 
                break;

            case BattleMode.Magic_Heal:
                // heal the whole party
                AudioManager.PlaySound(AudioClipName.Cure);
                foreach (BattleHero healedHero in heroes)
                {
                    damage = BattleMath.Damage(healedHero.BStats, null, ability); 
                    displayDamageEvent.Invoke(healedHero.BattleID, damage); 
                }
                                
                if (ability.MP != null) 
                {
                    hero.TakeMPDamage((int)ability.MP);
                    refreshHPMPSliders.Invoke(hero.BattleID, 0, (int)ability.MP);
                }
                hero.Sprites.UpdateSprite(Stance.Cast, 0.2f, 0); 
                TurnOver(null, TurnCounter.CurrentID); 
                break;

            case BattleMode.Magic_Fireball:
                // Create and launch fireball
                AudioManager.PlaySound(AudioClipName.Boom);
                {
                    PropelObject heroPropel = heroObj[activeHero].GetComponent<PropelObject>();
                    GameObject fireball = Instantiate(prefabFireball);
                    fireball.transform.position = heroObj[activeHero].transform.position;
                    fireball.GetComponent<PropelProjectile>().PropelShoot(
                        hero.BattleID, fireball.transform.position, heroPropel,
                        enemyParty.Enemies[choice].GameObject, choice, BattleMode.Magic_Fireball);
                    hero.Sprites.UpdateSprite(Stance.Cast, 0.2f, 0);
                }
                // the rest handled upon impact in PropelObjectHit
                break;

            case BattleMode.Magic_Poison:
                // Create and launch poisonball
                AudioManager.PlaySound(AudioClipName.UseTome);
                {
                    PropelObject heroPropel = heroObj[activeHero].GetComponent<PropelObject>();
                    GameObject poisonball = Instantiate(prefabPoisonball);
                    poisonball.transform.position = heroObj[activeHero].transform.position;
                    poisonball.GetComponent<PropelProjectile>().PropelShoot(
                        hero.BattleID, poisonball.transform.position, heroPropel,
                        enemyParty.Enemies[choice].GameObject, choice, BattleMode.Magic_Poison);
                    hero.Sprites.UpdateSprite(Stance.Cast, 0.2f, 0);
                }
                // the rest handled upon impact in PropelObjectHit
                break;

            case BattleMode.Blitz_Kick:
                // Sabin damages an initial victim and one random other victim
                hero.BattleMode = BattleMode.Blitz_Kick;
                
                // set up kick sprite
                hero.Sprites.UpdateSprite(Stance.Kick);
                PropelHero();

                // results and TurnOver handled upon impact 
                break;

            case BattleMode.Tools_AutoCrossbow:
                // Edgar launches 4 crossbow bolts at random enemies
                hero.BattleMode = BattleMode.Tools_AutoCrossbow;
                hero.Sprites.UpdateSprite(Stance.Defend);

                // Gets reference to Tool component on EdgarHero gameObject
                Tool tool = heroObj[activeHero].GetComponentInChildren<Tool>(true);
                
                // Create a stack of targets and list of potential victims
                Stack<GameObject> targetStack = new Stack<GameObject>();
                List<BattleEnemy> victims = new List<BattleEnemy>();

                // Add alive enemies to list of potential victims
                foreach (BattleEnemy enemy in enemyParty.Enemies)
                {
                    if (!enemy.IsDead)
                    {
                        victims.Add(enemy);
                    }
                }

                // Assemble 4 random victims from among the alive enemies
                for (int i = 0; i < 4; i++)
                {
                    int rand = Random.Range(0, victims.Count);
                    targetStack.Push(victims[rand].GameObject);
                }

                // Initiate timed sequence of launching bolts
                boltsInFlight = 4; // tracks the number of targets to hit.
                tool.Aim(hero.BattleID, targetStack);
                tool.UseTool(BattleMode.Tools_AutoCrossbow);

                // results and TurnOver handled by Tool and hit

                break;

            case BattleMode.Tools_Drill:
                // Edgar drills into an enemy, bypassing armor
                hero.BattleMode = BattleMode.Tools_Drill;
                hero.Sprites.UpdateSprite(Stance.Defend); // defend sprite has drill sprite over it
                
                // Launch Edgar with the drill
                heroObj[activeHero].GetComponentInChildren<Tool>(true).UseTool(BattleMode.Tools_Drill);

                // UseTool sets the drill sprite

                PropelHero();
                break;

            default:
                TurnOver(null, TurnCounter.CurrentID);
                break;
        }

    }


    /// <summary>
    /// Occurs when a propelled target (hero, enemy, ball) hits its destination. Triggers a TurnOver.
    /// Note: TurnOver occurs HERE upon impact, not when the target has returned home.
    /// Used for both enemies and heroes.
    /// </summary>
    /// <param name="attacker">battleID of attacker (or caster)</param>
    /// <param name="target">battleID of victim</param>
    /// <param name="mode"></param>
    void PropelTargetHit(int attacker, int target, BattleMode mode)
    {
        BattleAbility ability = null;
        
        // Load information about ability being used. (ex: Fireball != isPhysical)
        // ability is passed to BattleMath.Damage() for calculation of hits and for how much
        if (mode != BattleMode.none &&
            mode != BattleMode.Fight &&
            mode != BattleMode.Defend &&
            mode != BattleMode.Runic &&
            mode != BattleMode.Item)
        {
            ability = BattleAbilityData.Data[mode];
        }

        // Execute result of the action. Some actions have their own method (Ex: FightDamage)
        switch (mode)
        {
            case BattleMode.Fight:
                // A propelled enemy or hero has hit its fight target
                FightDamage(attacker, target);
                break;

            case BattleMode.Magic_Fireball:
                // A fireball has hit its target
                // Hero is attacker
                if (attacker < 0)
                {
                    foreach (BattleEnemy enemy in enemyParty.Enemies)
                    {
                        Damage damage = BattleMath.Damage(heroes[activeHero].BStats, enemy.Stats,
                            ability);
                        damage.Modify(cheatDamageMultiplier);
                        displayDamageEvent.Invoke(enemy.BattleID, damage);
                    }
                    TestForMPDamage(heroes[activeHero].BattleID, ability.MP);
                    TurnOver(null, TurnCounter.CurrentID);
                }
                // Enemy is attacker
                else
                {
                    BattleEnemy enemy = enemyParty.Enemies[attacker];

                    if (!TestForRunicOnImpact(ability.MP))
                    {
                        foreach (BattleHero hero in BattleLoader.Party.Hero)
                        {
                            Damage damage = BattleMath.Damage(hero.BStats,
                                enemy.Stats, ability);
                            displayDamageEvent.Invoke(hero.BattleID, damage);
                        }
                    }

                    TestForMPDamage(enemy.BattleID, ability.MP);
                    TurnOver(null, TurnCounter.CurrentID);
                }
                break;

            case BattleMode.Magic_Poison:
                // A poisonball has hit its target
                // Hero is attacker
                if (attacker < 0)
                {
                    BattleEnemy enemy = enemyParty.Enemies[target];
                    BattleStats stats = new BattleStats();
                    stats.Import(heroes[activeHero].BStats.Export());

                    // Store stats of Poisoner for calculating damage later
                    enemy.PoisonerBStats = stats;
                    enemy.PoisonCounter = 4;

                    // Apply green tint to victim
                    enemy.GameObject.GetComponentInChildren<SpriteRenderer>().color = poisonColor;

                    // Damage occurs on Start Turn methods

                    TestForMPDamage(heroes[activeHero].BattleID, ability.MP);
                    TurnOver(null, TurnCounter.CurrentID);
                }
                else
                // Enemy is attacker
                {
                    BattleEnemy enemy = enemyParty.Enemies[attacker];

                    if (!TestForRunicOnImpact(ability.MP))
                    {
                        BattleStats stats = new BattleStats();
                        stats.Import(enemy.Stats.Export());
                        BattleHero hero = heroes[BattleMath.ConvertHeroID(target)];

                        // Store stats of Poisoner for calculating damage later
                        hero.PoisonerBStats = stats; 
                        hero.PoisonCounter = 4;

                        // Apply green tint to victim
                        heroObj[BattleMath.ConvertHeroID(target)].GetComponent<SpriteRenderer>().color = poisonColor;

                        // Damage occurs on Start Turn methods
                    }

                    TestForMPDamage(enemy.BattleID, ability.MP);
                    TurnOver(null, TurnCounter.CurrentID);
                }
                break;

            case BattleMode.Blitz_Kick:
                // Sabin hits his primary target with a kick
                BattleEnemy kickEnemy = enemyParty.Enemies[target];

                Damage kickDamage = BattleMath.Damage(heroes[activeHero].BStats, kickEnemy.Stats,
                            ability);
                kickDamage.Modify(cheatDamageMultiplier);
                if (kickDamage.Amount == 0)
                {
                    AudioManager.PlaySound(AudioClipName.Miss);
                }
                else
                {
                    AudioManager.PlaySound(AudioClipName.Kick);
                }
                displayDamageEvent.Invoke(kickEnemy.BattleID, kickDamage);


                // choose secondary kick victim from the living
                if (enemyParty.Enemies.Count > 1)
                {
                    List<BattleEnemy> living = new List<BattleEnemy>();
                    foreach (BattleEnemy enemy in enemyParty.Enemies)
                    {
                        if (!enemy.IsDead && enemy != kickEnemy)
                        {
                            living.Add(enemy);
                        }
                    }

                    if (living.Count > 0)
                    {
                        int rand = Random.Range(0, living.Count);

                        Damage secondaryDamage = BattleMath.Damage(heroes[activeHero].BStats,
                            living[rand].Stats, ability);
                        secondaryDamage.Modify(cheatDamageMultiplier);
                        displayDamageEvent.Invoke(living[rand].BattleID, secondaryDamage);
                    }
                }

                TestForMPDamage(heroes[activeHero].BattleID, ability.MP);
                heroes[activeHero].Sprites.UpdateSprite();
                TurnOver(null, TurnCounter.CurrentID);
                break;

            case BattleMode.Tools_AutoCrossbow:
                // A bolt has hit its target
                // Bolts are launched on a timer, turn ends when the last bolt hits
                {
                    BattleEnemy enemy = enemyParty.Enemies[target];
                    BattleHero hero = heroes[activeHero];
                    
                    // Each bolt does damage
                    if (enemy != null && !enemy.IsDead)
                    {
                        Damage damage = BattleMath.Damage(hero.BStats,
                                    enemy.Stats, ability);
                        damage.Modify(cheatDamageMultiplier);
                        displayDamageEvent.Invoke(enemy.BattleID, damage);
                    }

                    // If the last bolt has hit, TurnOver
                    boltsInFlight--; // track the number of bolts that have hit, not in the quiver

                    Tool tool = heroObj[activeHero].GetComponentInChildren<Tool>(true);
                    if (boltsInFlight <= 0)
                    {
                        tool.gameObject.SetActive(false);

                        hero.BattleMode = BattleMode.none;
                        hero.Sprites.UpdateSprite();

                        TurnOver(null, TurnCounter.CurrentID);
                    }
                }
                break;

            case BattleMode.Tools_Drill:
                // Edgar hits his target with the drill
                // curly braces tell the following code that hero, enemy, damage are local to case
                {
                    BattleEnemy enemy = enemyParty.Enemies[target]; 
                    BattleHero hero = heroes[activeHero]; 
                    Damage damage = BattleMath.Damage(hero.BStats,
                                enemy.Stats, ability);
                    damage.Modify(cheatDamageMultiplier);
                    displayDamageEvent.Invoke(enemy.BattleID, damage);
                    
                    heroObj[activeHero].GetComponentInChildren<Tool>(true).gameObject.SetActive(false);
                    
                    hero.BattleMode = BattleMode.none;
                    hero.Sprites.UpdateSprite();

                }

                TurnOver(null, TurnCounter.CurrentID);
                break;

            case BattleMode.Steal:
                // Locke hits his target and attempts to steal
                // curly braces tell the following code that hero, enemy, damage are local to case
                {
                    AudioManager.PlaySound(AudioClipName.ThiefSteal);
                    BattleEnemy enemy = enemyParty.Enemies[target];
                    BattleHero hero = heroes[activeHero];

                    // Each enemy starts with 3 StealsRemaining
                    if (enemy.StealsRemaining > 0)
                    {
                        // 10 - 25% of min reward
                        float rand = Random.Range(0.1f, 0.25f);
                        int goldStolen = Mathf.CeilToInt(enemy.Stats.MinReward * rand);

                        // Send coins flying
                        rewardText.gameObject.SetActive(true);
                        CoinSpawner spawner = gameObject.AddComponent<CoinSpawner>();
                        spawner.StartSpawn(enemy.GameObject.transform.localPosition,
                            bankTarget.transform.localPosition, goldStolen, false);

                        enemy.StealsRemaining--;
                    }
                    else
                    {
                        // No StealsRemaining, so do nothing
                        // 0 damage is interpreted as a Miss. Passing this will display "Miss".
                        Damage damage = new Damage();
                        damage.Add(0);
                        displayDamageEvent.Invoke(enemy.BattleID, damage);
                    }

                    hero.BattleMode = BattleMode.none;
                }
                
                TurnOver(null, TurnCounter.CurrentID);
                break;
        }
    }

    /// <summary>
    /// Called when a poisonball or fireball hits a hero. Returns true if one of the characters
    /// has Runic active, and has that character absorb MP. Returns false otherwise, so the spell
    /// can act normally.
    /// </summary>
    /// <param name="mp">MP used to cast the spell, as defined in its ability data</param>
    /// <returns></returns>
    bool TestForRunicOnImpact (int? mp)
    {
        // Scan each hero for Runic
        foreach (BattleHero hero in heroes)
        {
            if (hero.BattleMode == BattleMode.Runic)
            {
                // show Runic animation
                hero.Sprites.UpdateSprite(Stance.RunicBlade, 0.3f, 0);
                hero.BattleMode = BattleMode.none;
                //absorb MP
                if (mp != null)
                {
                    Damage mpAbsorb = new Damage();
                    mpAbsorb.Add(-mp, false, false, true);
                    displayDamageEvent.Invoke(hero.BattleID, mpAbsorb);
                }
                return true;
            }
        }
        return false;
    }


    /// <summary>
    /// Deducts MP if the ability used required MP, and updates the sliders
    /// </summary>
    /// <param name="battleID">potential caster</param>
    /// <param name="mp">mp of ability according to its data</param>
    void TestForMPDamage (int battleID, int? mp)
    {
        if (mp != null)
        {
            if (battleID < 0)
            {
                heroes[activeHero].TakeMPDamage((int)mp);
            }
            else
            {
                enemyParty.Enemies[battleID].TakeMPDamage((int)mp);
            }
            refreshHPMPSliders.Invoke(battleID, 0, (int)mp);
        }
    }
    #endregion Hero_Abilities

    #region Damage_and_Death
    /// <summary>
    /// Called from BattleManager.PropelTargetHit(). Handles both heros and enemies.
    /// Generates physical Damage, modifies it. Invokes damageDisplay, and a TurnOver.
    /// </summary>
    /// <param name="attacker">battleID of attacker</param>
    /// <param name="target">battleID of target</param>
    void FightDamage(int attacker, int target)
    {
        // hero vs enemy target
        if (target >= 0)
        {
            BattleEnemy enemy = enemyParty.Enemies[target];
            int heroID = BattleMath.ConvertHeroID(attacker);

            // calculate damage
            Damage damage = BattleMath.FightDamage(heroes[heroID].BStats, enemy.Stats);

            if (damage.Amount == 0)
            {
                AudioManager.PlaySound(AudioClipName.Miss);
            }
            else
            {
                AudioManager.PlaySound(AudioClipName.Fight);
            }

            // modify damage (heroes only)
            damage.Modify(cheatDamageMultiplier);

            // display damage
            displayDamageEvent.Invoke(target, damage);

            // set hero sprite to idle
            heroes[heroID].Sprites.UpdateSprite(Stance.Idle);

            // displayDamage invokes ApplyDamage
            TurnOver(null, TurnCounter.CurrentID);
        }
        // enemy vs hero target
        else
        {
            BattleEnemy enemyAttacker = enemyParty.Enemies[attacker];
            int heroID = BattleMath.ConvertHeroID(target);

            // calculate damage
            Damage damage = BattleMath.FightDamage(enemyAttacker.Stats, heroes[heroID].BStats);

            // modify damage if hero is defending
            if (heroes[heroID].BattleMode == BattleMode.Defend)
            {
                damage.Modify(0.5f);
                AudioManager.PlaySound(AudioClipName.Defend);
            }
            else
            {
                if (damage.Amount == 0)
                {
                    AudioManager.PlaySound(AudioClipName.Miss);
                }
                else
                {
                    AudioManager.PlaySound(AudioClipName.Fight);
                }
            }

            // display damage
            displayDamageEvent.Invoke(target, damage);

            // show reaction for 0.1 sec
            heroes[heroID].Sprites.UpdateSprite(Stance.Struck, 0.2f);

            TurnOver(null, TurnCounter.CurrentID);
        }
    }

    /// <summary>
    /// Invoked by the DamageDisplay object ONLY. DamageDisplay converts each hit into int?s,
    /// and passes it here. 
    /// HP and MP are deducted here through BattleEnemy.TakeDamage() and BattleHero.TakeDamage(). 
    /// Whether or not a death has occured is calculated here, triggering enemyDeathBeginEvent.
    /// If all BattleHeros are dead, triggers a GameOver(Lose).
    /// </summary>
    /// <param name="battleID">battleID of hero or enemy</param>
    /// <param name="damage">hp damage as int?</param>
    /// <param name="mpDamage">mp damage as int?</param>
    void ApplyDamage(int battleID, int? damage, int? mpDamage)
    {
        // Enemy takes damage
        if (battleID >= 0)
        {
            BattleEnemy enemy = enemyParty.Enemies[battleID];
            
            // Apply damage to enemy. Will toggle IsDead if <= 0.
            if (damage != null) { enemy.TakeDamage((int)damage); }
            if (mpDamage != null) { enemy.TakeMPDamage((int)mpDamage); }

            // If enemy is dead, but there's more enemies out there
            if (enemy.IsDead && !finalEnemyDeath)
            {
                // Generate rewards and start them flying to the "bank"
                rewardText.gameObject.SetActive(true);
                CoinSpawner spawner = gameObject.AddComponent<CoinSpawner>();
                spawner.StartSpawn(enemy.GameObject.transform.localPosition,
                    bankTarget.transform.localPosition, enemy.Reward, enemy.IsBoss);

                // Begin red death fade out
                enemyDeathBeginEvent.Invoke(battleID, enemyParty.Enemies[battleID].IsBoss);

                // Check to see if this is the last enemy to begin its death.
                // if finalEnemyDeath = true, at the end of enemyDeathTimer, a GameOverWin
                // event is triggered.
                // finalEnemyDeath is tested in HeroStartTurn as well, to disable UI.

                finalEnemyDeath = true;
                foreach (BattleEnemy member in enemyParty.Enemies)
                {
                    if (!member.IsDead) { finalEnemyDeath = false; break; }
                }
                
                if (enemy.IsBoss) 
                { 
                    AudioManager.PlaySound(AudioClipName.BossDeath);
                }

                // remove this target from options
                NumberChoices();
            }
            
        }
        // Hero takes damage
        else
        {
            int id = BattleMath.ConvertHeroID(battleID); // convert battleID to partyID
            
            // Apply HP/MP damage
            if (damage != null) { heroes[id].TakeDamage((int)damage); }
            if (mpDamage != null) { heroes[id].TakeMPDamage((int)mpDamage); }

            // Check for hero death
            if (heroes[id].IsDead)
            {
                heroes[id].Sprites.UpdateSprite(Stance.Dead);
                // recalculate BattleIDs so that this hero is no longer targeted
                TurnCounter.RefreshIDs();
            }
            
            // Test for GameOver.Death
            bool gameOver = true;
            foreach (BattleHero hero in heroes)
            {
                if (!hero.IsDead)
                {
                    gameOver = false;
                    break;
                }
            }

            if (gameOver)
            {
                finalHeroDeath = true;
                AudioManager.EndGame();
                enemyAttackTimer.Stop();

                // All heroes are dead. Display the gameover(Death) menu prefab.
                BattleLoader.GameOver = GameOver.Death;
                MenuManager.GoToMenu(MenuName.GameOver);
            }

        }
    }

    /// <summary>
    /// Invoked at the end of the enemyDeathTimer, when an enemy is fading out.
    /// Until this method is invoked, an enemy IsDead, is not targetable, but still exists.
    /// This method will remove the enemy from the scene, and recalculate IDs.
    /// It will also trigger the end of the battle if finalEnemyDeath, or the completion
    /// of the dungeon if IsBoss.
    /// </summary>
    /// <param name="enemy"></param>
    void EnemyDeath(int enemy)
    {
        // Destroy the correct enemy gameObject
        Destroy(enemyParty.Enemies[enemy].GameObject);
        
        // Remove enemy from enemyParty
        enemyParty.Enemies.RemoveAt(enemy);

        // Destroy the last choice text (they are relabeled later)
        Destroy(enemyChoiceTexts[enemyChoiceTexts.Count - 1].gameObject);
        enemyChoiceTexts.RemoveAt(enemyChoiceTexts.Count - 1);

        // Reassign battleIDs amongst the remaining enemies
        foreach (BattleEnemy enemyRecount in enemyParty.Enemies)
        {
            enemyRecount.ResetID(enemyParty.Enemies.IndexOf(enemyRecount));
        }

        // Reassign BattleIDs throughout scene
        TurnCounter.RefreshIDs();

        // Renumber and relabel the choices and enemies
        NumberChoices();

        // Only trigger end if the last enemy has been removed
        if (enemyParty.Enemies.Count < 1) 
        {
            // Return to dungeon if it was an ordinary battle
            // Return to town if it was a boss battle
            if (finalEnemyDeath)
            {
                // Gold sequence was started during ApplyDamage()

                // If this was an ordinary battle
                if (!enemyParty.IsBoss)
                {
                    // Play fanfare, except in level 6
                    if (BattleLoader.DungeonLevel != 6)
                    {
                        AudioManager.PlayMusic(AudioClipName.Music_Fanfare);
                    }
                    
                    // Prep for the next fight
                    BattleLoader.GameOver = GameOver.Win; // regular Win
                    MenuManager.GoToMenu(MenuName.GameOver); // displays GameOver menu prefab
                }
                else
                {
                    // HERO WINS THE DUNGEON LEVEL!

                    AudioManager.PlayMusic(AudioClipName.Music_Fanfare);

                    BattleLoader.GameOver = GameOver.BossWin; // BossWin
                    MenuManager.GoToMenu(MenuName.GameOver); // displays GameOver menu prefab
                }
            }
        }
    }
    #endregion

    #region Turns
    /// <summary>
    /// Called by TurnOver() at the start of a HeroTurn
    /// </summary>
    void HeroStartTurn()
    {
        // if not waiting for the final enemy to finish its death sequence
        if (!finalEnemyDeath)
        {
            // Set ID and BattleHero of current hero
            activeHero = TurnCounter.CurrentHeroID;
            BattleHero hero = heroes[activeHero];

            // Resest to neutral mode
            hero.BattleMode = BattleMode.none;
            hero.Sprites.UpdateSprite(Stance.Idle);

            // if poisoned, apply poison damage, using stats of poisoner
            if (hero.PoisonCounter > 0)
            {
                BattleAbility ability = 
                    BattleAbilityData.Data[BattleMode.Magic_Poison];

                Damage damage = BattleMath.Damage(hero.PoisonerBStats,
                            hero.BStats, ability);
                displayDamageEvent.Invoke(hero.BattleID, damage);

                hero.PoisonCounter--;

                // show reaction for 0.2 sec
                hero.Sprites.UpdateSprite(Stance.Struck, 0.2f);

                // did hero die from poison?
                if (hero.IsDead)
                {
                    TurnOver(null, TurnCounter.CurrentID);
                    return;
                }

                // if that was the last PoisonCounter, no longer poisoned
                if (hero.PoisonCounter == 0) 
                {
                    heroObj[activeHero].GetComponent<SpriteRenderer>().color = Color.white;
                }
            }

            // Enable all UI elements for current HeroType, including unique subMenus
            uiEnabler.EnableUI(true, heroes[activeHero].HeroType);
            
            hero.IsHeroTurn = true;

            // Hero steps forward to show user which hero is active.
            Vector2 position = hero.HomePosition;
            position.x += 1f;
            heroObj[activeHero].transform.position = position;

            // Wait for user input
        }
    }

    /// <summary>
    /// Checks for poison damage, and starts the enemyAttackTimer.
    /// Enemy makes its move at the END of enemyAttackTimer.
    /// </summary>
    void EnemyStartTurn()
    {
        if (!finalHeroDeath)
        {
            BattleEnemy enemy = enemyParty.Enemies[TurnCounter.CurrentID];

            // Deal Poison damage if poisoned
            if (enemy.PoisonCounter > 0)
            {
                BattleAbility ability =
                    BattleAbilityData.Data[BattleMode.Magic_Poison];

                Damage damage = BattleMath.Damage(enemy.PoisonerBStats,
                            enemy.Stats, ability);
                displayDamageEvent.Invoke(enemy.BattleID, damage);

                enemy.PoisonCounter--;

                if (enemy.IsDead)
                {
                    TurnOver(null, TurnCounter.CurrentID);
                    return;
                }

                if (enemy.PoisonCounter == 0)
                {
                    enemy.GameObject.GetComponentInChildren<SpriteRenderer>().color = Color.white;
                }
            }

            // Decision-making moved to after attack timer

            // Start timer until move
            enemyAttackTimer.Run();
        }
    }

    /// <summary>
    /// Called at the end of a hero or enemy turn. Completes some end-of-turn behaviors,
    /// but also advances the TurnCounter and begins either a new hero or enemy turn, based
    /// on the new count.
    /// This method can also be invoked, such as by the BattleInventoryMonitor to display healing.
    /// TurnOver() used recursively if an invalid creature comes up.
    /// </summary>
    /// <param name="damage">If the turn ends in taking damage (Ex: healing potions for negative damage), damage is not null.</param>
    /// <param name="turnOverID">battleID of the creature ending its turn</param>
    void TurnOver(Damage damage, int turnOverID)
    {
        if (turnOverID < 0)
        {
            heroes[activeHero].IsHeroTurn = false;

            // display damage if damage is not null (this is rare). 
            // Used by BattleInventoryMonitor for potions.
            // Does not actually change HP/MP, only its display.
            if (damage != null) 
            {
                displayDamageEvent.Invoke(heroes[activeHero].BattleID, damage); // hero.BattleID = -1 to -4
            }
            
            // Choose a sprite, based on condition or action
            if (heroes[activeHero].IsDead)
            {
                heroes[activeHero].Sprites.UpdateSprite(Stance.Dead);
            }
            else if (heroes[activeHero].BattleMode == BattleMode.Defend)
            {
                heroes[activeHero].Sprites.UpdateSprite(Stance.Defend);
            }
            else if (heroes[activeHero].BattleMode == BattleMode.Runic)
            {
                heroes[activeHero].Sprites.UpdateSprite(Stance.RunicBlade, 0.3f, 0);
            }
            else if (heroes[activeHero].BattleMode == BattleMode.Item)
            {
                heroes[activeHero].Sprites.UpdateSprite(Stance.Item, 0.25f);
            }
            else
            {
                //hero[activeHero].Sprites.UpdateSprite(Stance.Idle);
            }
        }


        // Advance to the next turn, next CurrentID 
        TurnCounter.AdvanceToNextTurn();

        // HeroTurn is next
        if (TurnCounter.CurrentID < 0) 
        {
            battleInitializing = false;
            int partyID = BattleMath.ConvertHeroID(TurnCounter.CurrentID);
            
            // Test for a dead next hero
            if ( heroes[partyID] != null && !heroes[partyID].IsDead)
            {
                HeroStartTurn(); // This will enable menu options, using an invoker
            }
            else
            {
                // otherwise, pass the dead hero and go to the next turn
                TurnOver(null, TurnCounter.CurrentID);
            }
        }
        else 
        {
            // EnemyTurn is next, test for a dead enemy
            if (!enemyParty.Enemies[TurnCounter.CurrentID].IsDead)
            {
                // if battleInitializing is true, recursively loop TurnOver until
                // it is a HeroTurn, so that a Hero always is the first to move.
                if (battleInitializing)
                {
                    TurnOver(null, TurnCounter.CurrentID);
                }
                else
                {
                    EnemyStartTurn(); // This will disable menu options, using an invoker
                }
            }
            else
            {
                // Enemy is dead, pass and go to the next turn
                TurnOver(null, TurnCounter.CurrentID);
            }
        }
    }
    #endregion Turns

    #region Invoker_Methods
    // The following methods are used by listener objects
    // for when BattleManager invokes the referenced events.

    public void AddListener_Battle_DisplayDamage(UnityAction<int, Damage> listener)
    {
        displayDamageEvent.AddListener(listener);
    }

    public void AddListener_Battle_RefreshHPMPSliders(UnityAction<int, int,int> listener)
    {
        refreshHPMPSliders.AddListener(listener);
    }

    public void AddListener_Battle_RefreshMPCosts(UnityAction<int> listener)
    {
        refreshMPCosts.AddListener(listener);
    }

    public void AddListener_Battle_EnemyDeathBegin(UnityAction<int, bool> listener)
    {
        enemyDeathBeginEvent.AddListener(listener);
    }
    #endregion

    #region Cheats
    // Cheat buttons are only displayed if the user enables cheats
    
    // Sends info to the console
    public void Click_DebugButton()
    {
        Debug.Log("Enemy0 damage text: " + enemyParty.Enemies[0].GameObject.GetComponentInChildren<DamageDisplay>().DamageAmountText.name);
        Debug.Log("Enemy1 damage text: " + enemyParty.Enemies[1].GameObject.GetComponentInChildren<DamageDisplay>().DamageAmountText.name);
        Debug.Log("Enemy2 damage text: " + enemyParty.Enemies[2].GameObject.GetComponentInChildren<DamageDisplay>().DamageAmountText.name);
        DamageDisplay[] displays = GameObject.FindObjectsOfType<DamageDisplay>();

        Debug.Log("Finding instances of DisplayDamage" + displays.Length);
    }

    // Used by Cheat10xDamage.On_Click(). Multiplies all hero damage by 10.
    // Clicking again multiplies by 10 again (10 x 10 = 100)
    public void Click_Cheat10xDamage()
    {
        cheatDamageMultiplier *= 10;
    }

    // Adds a pack of health minor potions to the party inventory
    public void Click_Cheat_AddPotionPack()
    {
        BattleLoader.Party.Inventory.AddInvItem(InvNames.Potion_Health_Tiny, 5);
        BattleLoader.Party.Inventory.AddInvItem(InvNames.Potion_Health_Small, 5);
        BattleLoader.Party.Inventory.AddInvItem(InvNames.Potion_Health_Medium, 5);
    }

    // Increases HP and HPMax of all heroes by 200, displays as healing
    public void Click_Cheat_IncreaseHPMax()
    {
        foreach (BattleHero hero in heroes)
        {
            hero.IncreaseHPMax(200);
            HeroDamageDisplay display = heroObj[hero.PartyID].GetComponent<HeroDamageDisplay>();
            display.SetDamageDisplay(hero.BattleID, hero.HP, hero.HPMax, hero.MP, hero.MPMax);
            Damage damage = new Damage();
            damage.Add(-200);
            displayDamageEvent.Invoke(hero.BattleID, damage);

            display.SetNameAndHPMP(hero.FullName, hero.HP, hero.MP);
        }
    }
    #endregion Cheats
}


