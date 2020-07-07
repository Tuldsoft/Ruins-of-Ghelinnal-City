using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class BattleManager : MonoBehaviour
{
    #region Fields
    // for setting in the inspector what battle to test
    [SerializeField]
    EnemyName defaultBattle = EnemyName.none;
    [SerializeField]
    bool buffParty = false;

    // for cash
    [SerializeField]
    Text rewardText = null;
    [SerializeField]
    GameObject bankTarget = null;
    
    // reward in the "bank" and reward on its way to the bank
    int battleGoldReward = 0;
    int goldToAdd = 0;

    // hero values
    [SerializeField]
    GameObject[] heroStatusBar = new GameObject[4];

    GameObject[] heroObj = new GameObject[4];
    BattleHero[] heroes;
    int activeHero = 0;
    

    // for debugging and cheating
    int cheatDamageMultiplier = 1; 

    // database of current enemies
    EnemyParty enemyParty;

    // time between enemy attacks
    EventTimer enemyAttackTimer;
    const float EnemyAttackTimerDuration = 0.1f;

    // fields for creating text labels in lower right
    [SerializeField]
    Text enemyChoiceText = null; // used as a template 

    // create the requisite enemychoice texts
    float enemyChoiceHeight;
    Vector2 enemyChoicePosition;
    Vector3 enemyChoiceLocalScale;
    List<Text> enemyChoiceTexts = new List<Text>();

    // used to disable menu options in HeroStartTurn during the final enemy death
    bool finalEnemyDeath = false;
    bool finalHeroDeath = false;

    // list of all used battleID numbers
    List<int> usedIDs = new List<int>();

    // submenus
    [SerializeField]
    GameObject magicSubMenuObj = null, blitzSubMenuObj = null, toolsSubMenuObj = null;

    int boltsInFlight = 0;

    // center of hero location
    Vector2 heroPartyPosition = new Vector2(-4.75f, 1.75f);
    Vector2 enemyPartyPosition = new Vector2(4.75f, 1.75f);

    // the current selection
    int choice = 0;

    // for UI enable/disable during player turns
    UIEnabler uiEnabler;

    // invocation events
    Battle_DisplayDamageEvent displayDamageEvent = new Battle_DisplayDamageEvent();
    Battle_EnemyDeathBeginEvent enemyDeathBeginEvent = new Battle_EnemyDeathBeginEvent();
    Battle_RefreshHPMPSlidersEvent refreshHPMPSliders = new Battle_RefreshHPMPSlidersEvent();
    Battle_RefreshMPCostsEvent refreshMPCosts = new Battle_RefreshMPCostsEvent();

    // prefab storage
    GameObject prefabFireball;
    GameObject prefabPoisonball;

    // color storage
    Color poisonColor = new Color(0.32f, 0.63f, 0.24f);

    bool battleInitializing = true;

    #endregion Fields


    #region Awake_Start_Update_Methods
    // for debugging only
    private void Awake()
    {
        Initializer.Run();
    }

    // Start is called before the first frame update
    IEnumerator Start()
    {
        // invoked by TurnInvoker (Inv), and begins the enemyfight timer
        // passes null as damage when from inv
        EventManager.AddListener_Battle_TurnOver(TurnOver); 
        
        EventManager.AddListener_Battle_ChoiceSelection(SetChoice); // targets a target
        EventManager.AddListener_Battle_FightChoice(FightChoice); //later roll this into a renamed Submenuchoice

        EventManager.AddListener_Battle_SubMenuSelection(SubMenuChoice);


        // invoked by a propel object when a fight collision occurs, cues results of action (ex fight)
        EventManager.AddListener_Battle_PropelHitTarget(PropelTargetHit);
        
        
        EventManager.AddInvoker_Battle_DisplayDamage(this);
        EventManager.AddInvoker_Battle_RefreshHPMPSliders(this);
        EventManager.AddInvoker_Battle_RefreshMPCosts(this);
        EventManager.AddListener_Battle_ApplyDamage(ApplyDamage);
        //EventManager.AddListener_Battle_DisplayDamageEnd(EndDamageDisplay);

        EventManager.AddInvoker_Battle_EnemyDeathBegin(this);
        EventManager.AddListener_Battle_EnemyDeathEnd(EnemyDeath);

        EventManager.AddListener_Battle_AddCoins(AddCoinsToPot);

        if (buffParty)
        {
            BattleLoader.NewBuffParty();
        }
    
        heroes = BattleLoader.Party.Hero;

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

        // set up heroes
        for (int i = 0; i < heroes.Length; i++)
        {
            // instantiate heroObjs
            if (heroes[i] != null)
            {
                string path = @"BattlePrefabs\Hero\" + heroes[i].HeroType + "Hero";
                heroObj[i] = GameObject.Instantiate(Resources.Load<GameObject>(path));


                Vector2 position = heroObj[i].transform.position;
                position += positions[i];
                heroObj[i].transform.position = position;
                heroObj[i].name = "Hero " + i;

                // this also sets its position
                heroes[i].LoadObjs(i, heroObj[i]);

                // note - i is the partyID, not battleID
                // battleID = -partyID - 1. Ergo, the first hero(0) is -1 in battle.

                HeroDamageDisplay display = heroObj[i].GetComponent<HeroDamageDisplay>();
                display.SetHeroStatusBar(heroStatusBar[i]);
                display.SetDamageDisplay(heroes[i].BattleID, heroes[i].HP, heroes[i].HPMax,
                    heroes[i].MP, heroes[i].MPMax);
                display.SetNameAndHPMP(heroes[i].FullName, heroes[i].HP, heroes[i].MP);
            }
        }



        // set up enemy party
        if (BattleLoader.BattleParty == EnemyName.none)
        {
            enemyParty = new EnemyParty(defaultBattle);
        }
        else
        {
            enemyParty = new EnemyParty(BattleLoader.BattleParty);
        }
        
        // setup attack timer
        enemyAttackTimer = gameObject.AddComponent<EventTimer>();
        enemyAttackTimer.Duration = EnemyAttackTimerDuration;
        enemyAttackTimer.AddListener_Finished(ChooseEnemyAttack);

        // create and label the bottom right selections
        CreateChoices();
        NumberChoices();

        // set up UIEnabler
        uiEnabler = gameObject.AddComponent<UIEnabler>();
        uiEnabler.RegisterBattleManager(this);

        // load prefabs
        prefabFireball = Resources.Load<GameObject>(@"BattlePrefabs\Fireball");
        prefabPoisonball = Resources.Load<GameObject>(@"BattlePrefabs\Poisonball");

        //HeroStartTurn();
        yield return new WaitForEndOfFrame();

        TurnCounter.GenerateFirstTurns(BattleLoader.Party, enemyParty);

        // skip the first turn, since it's random anyway. to queue the next action
        //uiEnabler.EnableUI(false);
        TurnOver(null, TurnCounter.CurrentID);

        yield return null;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            MenuManager.GoToMenu(MenuName.Pause);
        }

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

    void AddCoinsToPot(int value)
    {
        goldToAdd += value;
    }

    #region User_Input_Methods
    public void SetChoice(int choice)
    {
        this.choice = choice;
    }

    public void Click_DefendOption()
    {
        AudioManager.Chirp();
        heroes[activeHero].BattleMode = BattleMode.Defend;

        uiEnabler.EnableUI(false);
        TurnOver(null, TurnCounter.CurrentID);
    }

    public void Click_RunicOption()
    {
        AudioManager.Chirp();
        AudioManager.PlaySound(AudioClipName.Runic);
        heroes[activeHero].BattleMode = BattleMode.Runic;

        uiEnabler.EnableUI(false);
        TurnOver(null, TurnCounter.CurrentID);
    }

    public void Click_StealOption()
    {
        AudioManager.Chirp();
        heroes[activeHero].BattleMode = BattleMode.Steal;
        heroes[activeHero].Sprites.UpdateSprite(Stance.Defend);
        PropelHero();

        uiEnabler.EnableUI(false);
        //TurnOver(null, TurnCounter.CurrentID);
    }

    public void Click_BlitzOption()
    {
        AudioManager.Chirp();
        // instantiate blitz menu
        // Sabin can Kick (does 0.5 physical damage to two enemies)
        // Sabin can AuraBolt (does 2.0 magic damage to one target)
        // Sabin can FireDance (does 1.0 magic damage to all enemies)
        blitzSubMenuObj.SetActive(true);
    }

    public void Click_ToolsOption()
    {
        AudioManager.Chirp();
        // instantiate blitz menu
        // Edgar can AutoCrossbow (does 0.3 phys damage to four random enemies)
        // Edger can Drill (does 1.2 physical damage to one enemy, no reduce)
        // Edgar can Chainsaw
        toolsSubMenuObj.SetActive(true);
    }

    public void Click_MagicOption()
    {
        AudioManager.Chirp();
        // instantiate magic panel based on HeroType
        refreshMPCosts.Invoke(heroes[activeHero].MP);

        magicSubMenuObj.GetComponent<MagicSubMenu>().Initialize(heroes[activeHero].HeroType);
        magicSubMenuObj.SetActive(true);
        

        // send another call out to enable/disable, for Exclusive magic
        // now that the Magic panel is Active
        //uiEnabler.EnableUI(true, hero[activeHero].HeroType);

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
        // the only enemy attack presently is to launch it at a hero

        BattleEnemy enemy = enemyParty.Enemies[TurnCounter.CurrentID];

        if (!enemy.IsDead)
        {
            // choose a victim
            List<BattleHero> victims = new List<BattleHero>();
            foreach (BattleHero potentialVictim in heroes)
            {
                if (!potentialVictim.IsDead)
                {
                    victims.Add(potentialVictim);
                }
            }

            // don't bother attacking if they are all dead
            if (victims.Count <= 0) { return; }

            victims = Randomize<BattleHero>.List(victims);
            BattleHero victim = victims[0];

            // check for poisoner attack, 25% chance
            List<EnemyName> poisonerList = new List<EnemyName>() { EnemyName.Wererat,
                EnemyName.Grease_Monkey,EnemyName.Cirpius, EnemyName.Poplium,EnemyName.Ninja,
                EnemyName.Apokryphos, EnemyName.Brainpan, EnemyName.Misfit, EnemyName.Atma_Weapon };
            
            if (poisonerList.Contains(enemy.Name) && (Random.value < 0.25f)
                && enemy.MP >= BattleAbilityData.Data[BattleMode.Magic_Poison].MP)
            {
                victim = TestForRunicOnCast(victim, AudioClipName.UseTome);
                
                GameObject poisonball = Instantiate(prefabPoisonball);
                poisonball.transform.position = enemy.GameObject.transform.position;
                poisonball.GetComponent<PropelObject>().Propel(
                    enemy.BattleID, poisonball.transform.position,
                    heroObj[victim.PartyID], victim.BattleID, BattleMode.Magic_Poison);

                // turnover called upon impact
                return;
            }

            // check for fireballer attack 15% chance
            List<EnemyName> fireballerList = new List<EnemyName>() { EnemyName.Magitek_Armor,
            EnemyName.Tunnel_Armor, EnemyName.Bomb,EnemyName.Spit_Fire, EnemyName.Chupon,
                EnemyName.Ghost, EnemyName.Apokryphos, EnemyName.Misfit, EnemyName.Dragon,
                EnemyName.Atma_Weapon};

                        
            if (fireballerList.Contains(enemy.Name) && (Random.value < 0.20f) 
                && enemy.MP >= BattleAbilityData.Data[BattleMode.Magic_Fireball].MP)
            {
                victim = TestForRunicOnCast(victim, AudioClipName.UseTome);
                
                GameObject fireball = Instantiate(prefabFireball);
                fireball.transform.position = enemy.GameObject.transform.position;
                fireball.GetComponent<PropelObject>().Propel(
                    enemy.BattleID, fireball.transform.position,
                    heroObj[victim.PartyID], victim.BattleID, BattleMode.Magic_Fireball);

                // turnover called upon impact
                return;
            }

            // otherwise just attack
            PropelEnemy(TurnCounter.CurrentID, victims[0]);

        }
        //TurnOver(null, TurnCounter.CurrentID);

    }

    // returns the PartyID of a hero currently in BattleMode.Runic, null if none
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

        if (runicID == null) 
        { 
            AudioManager.PlaySound(defaultCastAudio);
            return defaultHero;
        }

        return heroes[(int)runicID];
    }

    void PropelEnemy(int enemyNum, BattleHero victim)
    {
        BattleEnemy enemy = enemyParty.Enemies[enemyNum];

        enemy.GameObject.GetComponentInChildren<PropelObject>().Propel(enemyNum, enemy.Position,
            heroObj[victim.PartyID], victim.BattleID, BattleMode.Fight);

    }
    #endregion

    #region Enemy_Targeting
    void CreateChoices()
    {
        // create the requisite enemychoice texts
        enemyChoiceHeight = enemyChoiceText.rectTransform.rect.height;
        enemyChoicePosition = enemyChoiceText.transform.localPosition;
        enemyChoiceLocalScale = enemyChoiceText.transform.localScale;

        enemyChoiceTexts.Add(enemyChoiceText);
        
        for (int i = 1; i < enemyParty.Enemies.Count; i++)
        {
            GameObject newChoice = GameObject.Instantiate(enemyChoiceText.gameObject);
            newChoice.transform.SetParent(this.transform);
            newChoice.transform.localScale = enemyChoiceLocalScale;

            enemyChoiceTexts.Add(newChoice.GetComponent<Text>());
        }

    }

    void NumberChoices()
    {
        for (int i = 0; i < enemyParty.Enemies.Count; i++)
        {
            enemyChoiceTexts[i].text = enemyParty.Enemies[i].FullName;
            enemyChoiceTexts[i].GetComponent<EnemyChoice>().ChoiceNumber = i;
            enemyChoiceTexts[i].transform.localPosition = enemyChoicePosition + new Vector2(0, -enemyChoiceHeight * i / 2);
            enemyChoiceTexts[i].GetComponent<EnemyChoice>().MakeSelection(-1); // clear text selection
            enemyChoiceTexts[i].enabled = !enemyParty.Enemies[i].IsDead;
            

            GameObject enemy = enemyParty.Enemies[i].GameObject;
            enemy.GetComponentInChildren<EnemyChoice>().ChoiceNumber = i;
            enemy.GetComponentInChildren<EnemyChoice>().MakeSelection(-1);
            enemy.GetComponentInChildren<DamageDisplay>(true).SetID(i);
            enemy.GetComponentInChildren<EnemyDeath>(true).SetID(i);
            enemy.GetComponentInChildren<PropelObject>(true).SetID(i);


            if (enemyParty.Enemies[i].IsDead)
            {
                enemyChoiceTexts[i].enabled = false;
                enemy.GetComponentInChildren<PolygonCollider2D>(true).enabled = false;
            }
        }

        choice = 0;
        for (int i = enemyParty.Enemies.Count-1; i >= 0; i--)
        {
            if (!enemyParty.Enemies[i].IsDead) { choice = enemyParty.Enemies[i].BattleID; }
        }
    }
    #endregion Enemy_Targeting

    #region Hero_Abilities
    public void ReturnHeroToPosition()
    {
        heroObj[activeHero].transform.position = heroes[activeHero].HomePosition;
    }

    void FightChoice()
    {
        uiEnabler.EnableUI(false);
        
        heroes[activeHero].BattleMode = BattleMode.Fight;

        // set up fight sprite
        heroes[activeHero].Sprites.UpdateSprite(Stance.Jump, null, 1); // use jump[1], no timer
        PropelHero();
    }

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

    // invoked by selection an option (besides back) from a submenu
    // will call TurnOver if there are no objects propelled
    void SubMenuChoice (BattleMode mode)
    {
        uiEnabler.EnableUI(false); 
        
        BattleAbility ability = BattleAbilityData.Data[mode];
        BattleHero hero = heroes[activeHero];

        switch (mode)
        {
            case BattleMode.Magic_Cure:
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
                AudioManager.PlaySound(AudioClipName.Boom);
                GameObject fireball = Instantiate(prefabFireball);
                fireball.transform.position = heroObj[activeHero].transform.position;
                fireball.GetComponent<PropelObject>().Propel(hero.BattleID, fireball.transform.position,
                    enemyParty.Enemies[choice].GameObject, choice, BattleMode.Magic_Fireball);
                hero.Sprites.UpdateSprite(Stance.Cast, 0.2f, 0);
                // the rest handled upon impact in PropelObjectHit
                break;

            case BattleMode.Magic_Poison:
                AudioManager.PlaySound(AudioClipName.UseTome);
                GameObject poisonball = Instantiate(prefabPoisonball);
                poisonball.transform.position = heroObj[activeHero].transform.position;
                poisonball.GetComponent<PropelObject>().Propel(
                    hero.BattleID, poisonball.transform.position,
                    enemyParty.Enemies[choice].GameObject, choice, BattleMode.Magic_Poison);
                hero.Sprites.UpdateSprite(Stance.Cast, 0.2f, 0);
                // the rest handled upon impact in PropelObjectHit
                break;

            case BattleMode.Blitz_Kick:
                hero.BattleMode = BattleMode.Blitz_Kick;
                
                // set up kick sprite
                hero.Sprites.UpdateSprite(Stance.Kick);
                PropelHero();

                // results and TurnOver handled upon impact 
                break;

            case BattleMode.Tools_AutoCrossbow:
                hero.BattleMode = BattleMode.Tools_AutoCrossbow;
                hero.Sprites.UpdateSprite(Stance.Defend);

                Tool tool = heroObj[activeHero].GetComponentInChildren<Tool>(true);
                Stack<GameObject> targetStack = new Stack<GameObject>();

                List<BattleEnemy> victims = new List<BattleEnemy>();

                foreach (BattleEnemy enemy in enemyParty.Enemies)
                {
                    if (!enemy.IsDead)
                    {
                        victims.Add(enemy);
                    }
                }
                // get 4 random victims from among the (currently) living
                for (int i = 0; i < 4; i++)
                {
                    int rand = Random.Range(0, victims.Count);
                    targetStack.Push(victims[rand].GameObject);
                }

                boltsInFlight = 4; // tracks the number of targets to hit.
                tool.Aim(hero.BattleID, targetStack);
                tool.UseTool(BattleMode.Tools_AutoCrossbow);

                // results and TurnOver handled by Tool and hit

                break;

            case BattleMode.Tools_Drill:
                hero.BattleMode = BattleMode.Tools_Drill;
                hero.Sprites.UpdateSprite(Stance.Defend);
                heroObj[activeHero].GetComponentInChildren<Tool>(true).UseTool(BattleMode.Tools_Drill);

                // UseTool sets the drill sprite

                PropelHero();
                break;

            default:
                TurnOver(null, TurnCounter.CurrentID);
                break;
        }

    }


    // occurs when a propelled target hits its destination, triggers a TurnOver
    void PropelTargetHit(int attacker, int target, BattleMode mode)
    {
        BattleAbility ability = null;
        if (mode != BattleMode.none &&
            mode != BattleMode.Fight &&
            mode != BattleMode.Defend &&
            mode != BattleMode.Runic &&
            mode != BattleMode.Item)
        {
            ability = BattleAbilityData.Data[mode];
        }

        

        switch (mode)
        {
            case BattleMode.Fight:
                FightDamage(attacker, target);
                break;
            case BattleMode.Magic_Fireball:
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
                if (attacker < 0)
                {
                    BattleEnemy enemy = enemyParty.Enemies[target];
                    BattleStats stats = new BattleStats();
                    stats.Import(heroes[activeHero].BStats.Export());
                    enemy.PoisonerBStats = stats;
                    enemy.PoisonCounter = 4;

                    enemy.GameObject.GetComponentInChildren<SpriteRenderer>().color = poisonColor;

                    // damage occurs on Start Turn methods

                    TestForMPDamage(heroes[activeHero].BattleID, ability.MP);
                    TurnOver(null, TurnCounter.CurrentID);
                }
                else
                {
                    BattleEnemy enemy = enemyParty.Enemies[attacker];

                    if (!TestForRunicOnImpact(ability.MP))
                    {
                        BattleStats stats = new BattleStats();
                        stats.Import(enemy.Stats.Export());
                        BattleHero hero = heroes[BattleMath.ConvertHeroID(target)];

                        hero.PoisonerBStats = stats;
                        hero.PoisonCounter = 4;

                        heroObj[BattleMath.ConvertHeroID(target)].GetComponent<SpriteRenderer>().color = poisonColor;

                        // damage occurs on Start Turn methods
                    }

                    TestForMPDamage(enemy.BattleID, ability.MP);
                    TurnOver(null, TurnCounter.CurrentID);
                }
                break;

            case BattleMode.Blitz_Kick:
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



                // choose secondary kick victim
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
                // one of the bolts has hit a target
                {
                    BattleEnemy enemy = enemyParty.Enemies[target];
                    BattleHero hero = heroes[activeHero];
                    
                    if (enemy != null && !enemy.IsDead)
                    {
                        Damage damage = BattleMath.Damage(hero.BStats,
                                    enemy.Stats, ability);
                        damage.Modify(cheatDamageMultiplier);
                        displayDamageEvent.Invoke(enemy.BattleID, damage);
                    }

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

                {
                    AudioManager.PlaySound(AudioClipName.ThiefSteal);
                    BattleEnemy enemy = enemyParty.Enemies[target];
                    BattleHero hero = heroes[activeHero];

                    if (enemy.StealsRemaining > 0)
                    {
                        // 10 - 25% of min reward
                        float rand = Random.Range(0.1f, 0.25f);
                        int goldStolen = Mathf.CeilToInt(enemy.Stats.MinReward * rand);

                        rewardText.gameObject.SetActive(true);
                        CoinSpawner spawner = gameObject.AddComponent<CoinSpawner>();
                        spawner.StartSpawn(enemy.GameObject.transform.localPosition,
                            bankTarget.transform.localPosition, goldStolen, false);

                        enemy.StealsRemaining--;
                    }
                    else
                    {
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

    bool TestForRunicOnImpact (int? mp)
    {
        foreach (BattleHero hero in heroes)
        {
            if (hero.BattleMode == BattleMode.Runic)
            {
                hero.Sprites.UpdateSprite(Stance.RunicBlade, 0.3f, 0);
                hero.BattleMode = BattleMode.none;
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
    // called from PropelTargetHit in BattleManager, not from the invoker
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


            // modify damage
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

    void ApplyDamage(int battleID, int? damage, int? mpDamage)
    {
        // enemy takes damage
        if (battleID >= 0)
        {
            BattleEnemy enemy = enemyParty.Enemies[battleID];
            if (damage != null) { enemy.TakeDamage((int)damage); }
            if (mpDamage != null) { enemy.TakeMPDamage((int)mpDamage); }

            // if multi-hit, don't do this on combo and secondary hits
            if (enemy.IsDead && !finalEnemyDeath)
            {
                // cash in the reward and start the death timer

                // cash
                rewardText.gameObject.SetActive(true);
                CoinSpawner spawner = gameObject.AddComponent<CoinSpawner>();
                spawner.StartSpawn(enemy.GameObject.transform.localPosition,
                    bankTarget.transform.localPosition, enemy.Reward, enemy.IsBoss);

                // begin death fade
                enemyDeathBeginEvent.Invoke(battleID, enemyParty.Enemies[battleID].IsBoss);

                // test for GameOverWin happens upon actual enemy death
                // if no enemies are alive, signal to the EnemyDeathTimer to call GameOverWin
                // final enemy death is tested in HeroStartTurn as well.
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
        else
        {
            int id = BattleMath.ConvertHeroID(battleID);
            if (damage != null) { heroes[id].TakeDamage((int)damage); }
            if (mpDamage != null) { heroes[id].TakeMPDamage((int)mpDamage); }

            // check for hero death?
            if (heroes[id].IsDead)
            {
                heroes[id].Sprites.UpdateSprite(Stance.Dead);
                TurnCounter.RefreshIDs();
            }
            
            // test for GameOver
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

                // player is dead
                BattleLoader.GameOver = GameOver.Death;
                MenuManager.GoToMenu(MenuName.GameOver);
            }

        }
    }

    // invoked at the end of the death timer
    void EnemyDeath(int enemy)
    {
        // Destroy the correct enemy
        Destroy(enemyParty.Enemies[enemy].GameObject);
        enemyParty.Enemies.RemoveAt(enemy);

        // Destroy the last choice text
        Destroy(enemyChoiceTexts[enemyChoiceTexts.Count - 1].gameObject);
        enemyChoiceTexts.RemoveAt(enemyChoiceTexts.Count - 1);

        foreach (BattleEnemy enemyRecount in enemyParty.Enemies)
        {
            enemyRecount.ResetID(enemyParty.Enemies.IndexOf(enemyRecount));
        }

        TurnCounter.RefreshIDs();

        // Renumber and relabel the choices
        NumberChoices();

        // only trigger end if the last enemy has been removed
        if (enemyParty.Enemies.Count < 1) 
        {
            // return to dungeon if it was an ordinary battle
            // return to town if it was a boss battle
            if (finalEnemyDeath)
            {
                //BattleLoader.BattleEarnings = battleGoldReward + goldToAdd;
                //BattleLoader.Party.Gold += BattleLoader.BattleEarnings;
                

                if (!enemyParty.IsBoss)
                {
                    if (BattleLoader.DungeonLevel != 6)
                    {
                        AudioManager.PlayMusic(AudioClipName.Music_Fanfare);
                    }
                    
                    // prep for the next fight
                    BattleLoader.GameOver = GameOver.Win;
                    MenuManager.GoToMenu(MenuName.GameOver);

                    //MenuManager.EnterDungeonLevel(BattleLoader.DungeonLevel, false);

                }
                else
                {
                    // HERO WINS THE DUNGEON LEVEL!

                    AudioManager.PlayMusic(AudioClipName.Music_Fanfare);

                    BattleLoader.GameOver = GameOver.BossWin;
                    MenuManager.GoToMenu(MenuName.GameOver);
                }


            }
        
        }
    }
    #endregion

    #region Turns

    void HeroStartTurn()
    {
        if (!finalEnemyDeath)
        {
            activeHero = TurnCounter.CurrentHeroID;
            
            BattleHero hero = heroes[activeHero];

            hero.BattleMode = BattleMode.none;
            hero.Sprites.UpdateSprite(Stance.Idle);

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

                if (hero.IsDead)
                {
                    TurnOver(null, TurnCounter.CurrentID);
                    return;
                }

                if (hero.PoisonCounter == 0) 
                {
                    heroObj[activeHero].GetComponent<SpriteRenderer>().color = Color.white;
                }
            }

            uiEnabler.EnableUI(true, heroes[activeHero].HeroType);
            
            hero.IsHeroTurn = true;

            Vector2 position = hero.HomePosition;
            position.x += 1f;
            heroObj[activeHero].transform.position = position;
        }
    }

    void EnemyStartTurn()
    {
        if (!finalHeroDeath)
        {
            BattleEnemy enemy = enemyParty.Enemies[TurnCounter.CurrentID];

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

            // decision-making moved to after attack timer

            enemyAttackTimer.Run();
        }
    }

    // invoked by TurnInvoker to signal completion of a hero's action
    // used also by item use
    // damage is passed to display. 
    // Positive is damage in red,
    // Negative is healing in green.
    // 0 is a Miss
    // null means the turn ended through some other effect
    void TurnOver(Damage damage, int turnOverID)
    {
        if (turnOverID < 0)
        {
            heroes[activeHero].IsHeroTurn = false;
            if (damage != null) // only display damage if the hero applied some effect to self (potion)
            {
                displayDamageEvent.Invoke(heroes[activeHero].BattleID, damage); // hero.BattleID = -1 to -4
            }
            // choose a sprite
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

        // test for hero or enemy, dead or alive
        if (TurnCounter.CurrentID < 0) 
        {
            battleInitializing = false;
            int partyID = BattleMath.ConvertHeroID(TurnCounter.CurrentID);
            if ( heroes[partyID] != null && !heroes[partyID].IsDead)
            {
                HeroStartTurn(); // This will enable menu options, using an invoker
            }
            else
            {
                
                // otherwise, pass the dead hero and go to the next one
                TurnOver(null, TurnCounter.CurrentID);

            }
        }
        else 
        {
            if (!enemyParty.Enemies[TurnCounter.CurrentID].IsDead)
            {
                // make sure the player has the first turn
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
                // otherwise, pass the current dead enemy and go to whomever is next
                TurnOver(null, TurnCounter.CurrentID);
            }
        }
    }
    #endregion Turns

    /*int GetUniqueID()
    {
        if (usedIDs.Count == 0)
        {
            ResetUniqueIDs();
        }
        int output = usedIDs.Count + 1000;
        while (output < int.MaxValue && usedIDs.Contains(output))
        {
            output++;
        }
        if (output == int.MaxValue)
        {
            output = ResetUniqueIDs();
        }

        usedIDs.Add(output);
        return output;

    }

    int ResetUniqueIDs()
    {
        usedIDs.Clear();
        foreach (BattleHero hero in BattleLoader.Party.Hero)
        {
            usedIDs.Add(hero.BattleID);
        }
        foreach (BattleEnemy enemy in enemyParty.Enemies)
        {
            usedIDs.Add(enemy.BattleID);
        }
        return usedIDs.Count + 1000;
    }*/

    #region Invoker_Methods
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
    public void Click_DebugButton()
    {
        Debug.Log("Enemy0 damage text: " + enemyParty.Enemies[0].GameObject.GetComponentInChildren<DamageDisplay>().DamageAmountText.name);
        Debug.Log("Enemy1 damage text: " + enemyParty.Enemies[1].GameObject.GetComponentInChildren<DamageDisplay>().DamageAmountText.name);
        Debug.Log("Enemy2 damage text: " + enemyParty.Enemies[2].GameObject.GetComponentInChildren<DamageDisplay>().DamageAmountText.name);
        DamageDisplay[] displays = GameObject.FindObjectsOfType<DamageDisplay>();

        Debug.Log("Finding instances of DisplayDamage" + displays.Length);
    }

    public void Click_Cheat10xDamage()
    {
        cheatDamageMultiplier *= 10;
    }

    public void Click_Cheat_AddPotionPack()
    {
        BattleLoader.Party.Inventory.AddInvItem(InvNames.Potion_Health_Tiny, 5);
        BattleLoader.Party.Inventory.AddInvItem(InvNames.Potion_Health_Small, 5);
        BattleLoader.Party.Inventory.AddInvItem(InvNames.Potion_Health_Medium, 5);
    }

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


