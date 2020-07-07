using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BattleLoader
{
    public static HeroParty Party { get; set; } = null;

    static bool initialized = false;
    public static void Initialize()
    {
        if (!initialized)
        {
            initialized = true;

            // Load Player Preferences
            if (PlayerPrefs.HasKey(PlayerPrefsKeys.IsCheating.ToString()))
            {
                int isCheating = PlayerPrefs.GetInt(PlayerPrefsKeys.IsCheating.ToString());

                // Convert int to bool. -1 if false, 0 if true;
                bool cheatingBool = isCheating < 0 ? false : true;
                BattleLoader.IsCheating = cheatingBool;
            }

            // Create a new party if none exist
            //if (Party == null)
            {
                NewHeroParty();
            }

            dungeonLevelAccess = 1;
        }
    }

    public static void ResetData()
    {
        initialized = false;
        Initialize();
    }

    #region GameStateVariables
    public static bool IsCheating { get; set; } = false;
    //public static BattleHero Hero { get; set; } = null;
    public const int MaxDungeonLevelAccess = 7;
    static int dungeonLevelAccess = 1;
    //const int DefaultNumPartyMembers = 1;
    static int DefaultNumPartyMembers = 1; // non-const so that I'm not plagued by errors

    public static int LastTalkingPoint { get; set; } = 0;

    public static int DungeonLevelAccess
    {

        get
        {
            return dungeonLevelAccess;
        }
        set
        {
            // dungeonLevelAccess is limited to the constant MaxDungeonLevelAccess above
            if (value <= MaxDungeonLevelAccess)
            {
                dungeonLevelAccess = value;
            }
            else
            {
                dungeonLevelAccess = MaxDungeonLevelAccess;
            }
        }
    }

    public static bool GameCompleted = false;

    #endregion

    #region Town


    public static void RefreshHeroParty ()
    {
        if (Party == null)
        {
            NewHeroParty();
        }
        else
        {
            Party.Hero[0].HealAll();
        }
    }
    
    public static void NewHeroParty ()
    {
        //Hero = new BattleHero();
        Party = new HeroParty(1);
        Party.AddHero(new BattleHero(HeroType.Sabin, new BattleStats(), null, "Hero1"));
        if (DefaultNumPartyMembers > 1)
        {
            Party.AddHero(new BattleHero(HeroType.Edgar, new BattleStats(), null, "Hero2"));
        }
        if (DefaultNumPartyMembers > 2)
        {
            Party.AddHero(new BattleHero(HeroType.Locke, new BattleStats(), null, "Hero3"));
        }
        if (DefaultNumPartyMembers > 3)
        {
            Party.AddHero(new BattleHero(HeroType.Celes, new BattleStats(), null, "Hero4"));
        }
        Party.Gold = 275;
        Party.Inventory.AddInvItem(InvNames.Potion_Health_Small, 1);
        Party.Inventory.AddInvItem(InvNames.Potion_Health_Tiny, 2);
    }

    public static void NewBuffParty()
    {
        int gold = Party.Gold;
        Party = new HeroParty(4);
        Party.AddHero(new BattleHero(HeroType.Sabin, new BattleStats(), null, "Hero1"));
        Party.AddHero(new BattleHero(HeroType.Edgar, new BattleStats(), null, "Hero2"));
        Party.AddHero(new BattleHero(HeroType.Locke, new BattleStats(), null, "Hero3"));
        Party.AddHero(new BattleHero(HeroType.Celes, new BattleStats(), null, "Hero4"));
        BattleStats stats = new BattleStats();
        int[] bonuses = new int[] { 2000, 2000, 750, 750, 750, 750,
            50, 50, 50, 50, 50, 0, 0 };
        stats.Import(bonuses);
        foreach (BattleHero hero in Party.Hero)
        {
            hero.BStats.Equip(stats);
            hero.HealAll();
        }
        Party.Gold = gold;
    }

    #endregion

    #region DungeonLevels
    static Vector2 heroPosition;
    static List<EnemyInfo> dungeonEnemyInfos = new List<EnemyInfo>();
    static Dictionary<int, List<object>> dungeonEnemyFXsVars 
       = new Dictionary<int, List<object>>();

    public static bool NewDungeonLevel { get; set; } = true;
    static int dungeonLevel = 1;
    public static int DungeonLevel { get { return dungeonLevel; } }
    
    // called by either DungeonLevelMonitor for New or BattleManager for Reload
    // -1 or 0 signifies Reload, 1+ signifies which level to load fresh
    public static void LoadLevel (int level)
    {
        dungeonLevel = level;
        if (NewDungeonLevel)
        {
            LoadNewLevel();
            DungeonMusic();
            NewDungeonLevel = false;
        }
        else
        {
            DungeonMusic();
            ReloadLevel();
        }
    }

    // reads the info preset in the scene and stores it
    static void LoadNewLevel ()
    {
        // for debugging only
        if (Party == null)
        {
            NewHeroParty();
        }

        heroPosition = new Vector2();
        heroPosition = GameObject.FindGameObjectWithTag("Hero_Dungeon").transform.position;

        RefreshDungeonEnemiesList();
        
    }

    // loads the stored info into a loaded scene
    static void ReloadLevel()
    {
        // place hero
        GameObject hero = GameObject.FindGameObjectWithTag("Hero_Dungeon");
        hero.transform.position = heroPosition; 
        
        GameObject[] enemyObjs = GameObject.FindGameObjectsWithTag("Enemy_Dungeon");
        if (enemyObjs.Length > 0)
        {
            for (int i = enemyObjs.Length - 1; i >= 0; i--)
            {
                GameObject.Destroy(enemyObjs[i]);
            }
        }


        for (int i = 0; i < dungeonEnemyInfos.Count; i++)
        //foreach (EnemyInfo info in dungeonEnemyInfos)
        {
            if (!dungeonEnemyInfos[i].IsDead)
            {
                GameObject newEnemy = GameObject.Instantiate(
                Resources.Load<GameObject>(@"DungeonPrefabs\Enemies\"
                + dungeonEnemyInfos[i].EnemyName.ToString()));
                newEnemy.transform.position = dungeonEnemyInfos[i].Position;

                if (dungeonEnemyFXsVars.ContainsKey(i))
                {
                    newEnemy.TryGetComponent<Enemy_Dungeon_FX>(out Enemy_Dungeon_FX destroyComponent);
                    if (destroyComponent != null) { GameObject.Destroy(destroyComponent); }
                    Enemy_Dungeon_FX newFX = newEnemy.AddComponent<Enemy_Dungeon_FX>();
                    newFX.StoreVariables(dungeonEnemyFXsVars[i]);
                }
            }
        }

        Camera.main.transform.position = new Vector3(heroPosition.x, heroPosition.y, -10);
    }

    static void RefreshDungeonEnemiesList()
    {
        dungeonEnemyInfos.Clear();
        dungeonEnemyFXsVars.Clear();
        GameObject[] enemyObjs = GameObject.FindGameObjectsWithTag("Enemy_Dungeon");

        if (enemyObjs.Length != 0)
        {
            for (int i = 0; i < enemyObjs.Length; i++)
            {
                Enemy_Dungeon enemy_Dungeon = enemyObjs[i].GetComponent<Enemy_Dungeon>();
                enemy_Dungeon.SetInfo();
                EnemyInfo info = new EnemyInfo();
                info = enemy_Dungeon.info;

                // don't add any explodables
                bool explodable = false;
                if (enemyObjs[i].TryGetComponent<Enemy_Dungeon_FX>(out Enemy_Dungeon_FX fx))
                {
                    explodable = fx.explodable;
                }
                
                if (!explodable)
                {
                    dungeonEnemyInfos.Add(info);

                    if (fx != null)
                    {
                        dungeonEnemyFXsVars.Add(i, fx.ExportVariables());
                    }
                }
            }
        }
    }

    static void DungeonMusic()
    {
        switch (dungeonLevel)
        {
            case 1:
                AudioManager.PlayMusic(AudioClipName.Music_Terra);
                break;
            case 2:
                AudioManager.PlayMusic(AudioClipName.Music_The_Mines_Of_Narshe);
                break;
            case 3:
                AudioManager.PlayMusic(AudioClipName.Music_Devils_Lab);
                break;
            case 4:
                AudioManager.PlayMusic(AudioClipName.Music_Mt_Koltz);
                break;
            case 5:
                AudioManager.PlayMusic(AudioClipName.Music_Save_Them);
                break;
            case 6:
                // only loaded on new dungeon
                if (NewDungeonLevel)
                {
                    AudioManager.PlayMusic(AudioClipName.Music_The_Phantom_Forest);
                }
                break;
            case 7:
                AudioManager.PlayMusic(AudioClipName.Music_New_Continent);
                break;

        }
    }


    public static void UpdatePositions(GameObject hero)
    {
        heroPosition = hero.transform.position;
        RefreshDungeonEnemiesList();
    }

    #endregion


    #region Battle
    static EnemyName battleParty = EnemyName.none;
    public static EnemyName BattleParty { get { return battleParty; } }
    public static int BattleEarnings { get; set; } = 0;

    public static GameOver? GameOver { get; set; } = null;


    public static void LoadBattle (EnemyName party)
    {
        AudioManager.PlaySound(AudioClipName.Encounter);
        BattleEarnings = 0;
        battleParty = party;
        LoadBattleMusic(party);
        
        MenuManager.GoToMenu(MenuName.Battle);
    }
    
    static void LoadBattleMusic(EnemyName party)
    {
        if (dungeonLevel == 6)
        {
            // no change on battle load in phantom forest
        }
        else if (dungeonLevel == 3) // only change in 3 if tunnelarmor
        {
            // no change if regular enemy, but do change for the boss
            if (party == EnemyName.Tunnel_Armor)
            {
                AudioManager.PlayMusic(AudioClipName.Music_The_Unforgiven);
            }
        }
        else
        {
            switch (party)
            {
                case EnemyName.Ultros:
                    AudioManager.PlayMusic(AudioClipName.Music_The_Decisive_Battle);
                    break;
                case EnemyName.Whelk:
                    AudioManager.PlayMusic(AudioClipName.Music_The_Decisive_Battle);
                    break;
                case EnemyName.Vargas:
                    AudioManager.PlayMusic(AudioClipName.Music_Wild_West);
                    break;
                case EnemyName.Chupon:
                    AudioManager.PlayMusic(AudioClipName.Music_The_Wedding_Waltz_Duel_4);
                    break;
                case EnemyName.Atma_Weapon:
                    AudioManager.PlayMusic(AudioClipName.Music_The_Fierce_Battle);
                    break;
                default:
                    AudioManager.PlayMusic(AudioClipName.Music_Battle_Theme);
                    break;
            }
        }
    }

    #endregion
}
