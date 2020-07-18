using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stores major game state variables.
/// Stores information about the HeroParty for all scenes.
/// Acts as an intermediary between the dungeon scene and battle scene.
/// Stores placement of dungeon enemies for re-creation at the end of the battle.
/// Creates and places enemies in the battle scene.
/// </summary>
public static class BattleLoader
{
    // This class is separated into major regions, with fields/properties
    // nearest to their relevant methods.
    // This class should be split according to its various roles,
    // rather than fulfilling multiple roles.

    #region Initialization
    // The hero party: all heroes, party inventory, accessed throughout the game
    public static HeroParty Party { get; set; } = null;

    static bool initialized = false;
    
    /// <summary>
    /// Called by Initializer at startup
    /// </summary>
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

            // Create a new party if none exist (no longer necessary)
            // if (Party == null)
            {
                NewHeroParty();
            } 
            
            // Set access level to 1 (declared in the Game State region)
            dungeonLevelAccess = 1;
        }
    }

    // Re-initializes BattleLoader, including making a new HeroParty, such as when loading a game
    public static void ResetData()
    {
        initialized = false;
        Initialize();
    }
    #endregion

    #region GameStateVariables
    // Fields and Properties
    // Toggles cheat buttons
    public static bool IsCheating { get; set; } = false; 
    
    // Used for debugging when creating a new party
    //const int DefaultNumPartyMembers = 1;
    static int DefaultNumPartyMembers = 1; // non-const so that I'm not plagued by errors
    
    // Used for tracking where the user was last reading lore
    public static int LastTalkingPoint { get; set; } = 0;

    // Dungeon Access
    public const int MaxDungeonLevelAccess = 7; // increased as new levels are created

    // User access to dungeons, increases as dungeons are completed
    static int dungeonLevelAccess = 1; 
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

    // Used when checking whether to display the final lore
    public static bool GameCompleted = false;

    #endregion

    #region TownMethods

    // No longer used
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
    
    // Creates a default new hero party for a new game
    public static void NewHeroParty ()
    {
        //Create HeroParty
        Party = new HeroParty(1);
        
        // Add default heroes to HeroParty
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
        
        // Add gold and default items to the new HeroParty
        Party.Gold = 275;
        Party.Inventory.AddInvItem(InvNames.Potion_Health_Small, 1);
        Party.Inventory.AddInvItem(InvNames.Potion_Health_Tiny, 2);
    }

    // Used for debugging, provides 4 overpowered
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
    // Fields used for constructing or reconstructing a dungeon    
    static Vector2 heroPosition; // hero position in dungeon
    static List<EnemyInfo> dungeonEnemyInfos = new List<EnemyInfo>();
    static Dictionary<int, List<object>> dungeonEnemyFXsVars 
       = new Dictionary<int, List<object>>();

    // Determines whether a dungeon is being reloaded, or a new dungeon is being loaded
    public static bool NewDungeonLevel { get; set; } = true;
    // Dungeon Level to be constructed, or current level
    static int dungeonLevel = 1;
    public static int DungeonLevel { get { return dungeonLevel; } }
    
    // Called by either DungeonLevelMonitor for New or BattleManager for Reload
    // -1 or 0 signifies Reload, 1+ signifies which level to load fresh
    // This directs to the correct methods.
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

    // Reads the info preset in the scene and stores it
    static void LoadNewLevel ()
    {
        // for debugging only
        if (Party == null)
        {
            NewHeroParty();
        }

        // Capture heroPosition in the scene
        heroPosition = new Vector2();
        heroPosition = GameObject.FindGameObjectWithTag("Hero_Dungeon").transform.position;

        // Capture info on enemies
        RefreshDungeonEnemiesList();
        
    }

    // Loads previously stored info into a fresh dungeon scene
    static void ReloadLevel()
    {
        // place hero at previous location
        GameObject hero = GameObject.FindGameObjectWithTag("Hero_Dungeon");
        hero.transform.position = heroPosition; 
        
        // remove any pre-loaded enemies
        GameObject[] enemyObjs = GameObject.FindGameObjectsWithTag("Enemy_Dungeon");
        if (enemyObjs.Length > 0)
        {
            for (int i = enemyObjs.Length - 1; i >= 0; i--)
            {
                GameObject.Destroy(enemyObjs[i]);
            }
        }

        // place each enemy in storage and set its properties
        for (int i = 0; i < dungeonEnemyInfos.Count; i++)
        {
            // skip the recently defeated dungeonEnemy
            if (!dungeonEnemyInfos[i].IsDead)
            {
                // create a new enemy using a prefab template, and place it
                GameObject newEnemy = GameObject.Instantiate(
                Resources.Load<GameObject>(@"DungeonPrefabs\Enemies\"
                + dungeonEnemyInfos[i].EnemyName.ToString()));
                newEnemy.transform.position = dungeonEnemyInfos[i].Position;

                // for dungeonEnemies that have effects (FX), load the current state of those fx
                // includes movement, explosion, following, etc.
                if (dungeonEnemyFXsVars.ContainsKey(i))
                {
                    newEnemy.TryGetComponent<Enemy_Dungeon_FX>(out Enemy_Dungeon_FX destroyComponent);
                    if (destroyComponent != null) { GameObject.Destroy(destroyComponent); }
                    Enemy_Dungeon_FX newFX = newEnemy.AddComponent<Enemy_Dungeon_FX>();
                    newFX.StoreVariables(dungeonEnemyFXsVars[i]);
                }
            }
        }

        // place camera on hero
        Camera.main.transform.position = new Vector3(heroPosition.x, heroPosition.y, -10);
    }

    // Gather information on existing enemies
    static void RefreshDungeonEnemiesList()
    {
        // Clear previous info
        dungeonEnemyInfos.Clear();
        dungeonEnemyFXsVars.Clear();
        
        // Create array of existing enemies
        GameObject[] enemyObjs = GameObject.FindGameObjectsWithTag("Enemy_Dungeon");
        if (enemyObjs.Length != 0)
        {
            // Set info for each enemy
            for (int i = 0; i < enemyObjs.Length; i++)
            {
                Enemy_Dungeon enemy_Dungeon = enemyObjs[i].GetComponent<Enemy_Dungeon>();
                enemy_Dungeon.SetInfo();
                EnemyInfo info = new EnemyInfo();
                info = enemy_Dungeon.info;

                // Don't add any explodables
                bool explodable = false;
                if (enemyObjs[i].TryGetComponent<Enemy_Dungeon_FX>(out Enemy_Dungeon_FX fx))
                {
                    explodable = fx.explodable;
                }
                
                if (!explodable)
                {
                    // Add info
                    dungeonEnemyInfos.Add(info);

                    // Add FX info
                    if (fx != null)
                    {
                        dungeonEnemyFXsVars.Add(i, fx.ExportVariables());
                    }
                }
            }
        }
    }

    // Select the correct Music to play for the current dungeon
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
                // only loaded on new dungeon, otherwise music has continued through the battle
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

    // Store hero position and enemy positions and infos. Public. Called by Enemy_Dungeon
    public static void UpdatePositions(GameObject hero)
    {
        heroPosition = hero.transform.position;
        RefreshDungeonEnemiesList();
    }

    #endregion

    #region Battle
    // This region loads the battle scene from the dungeon scene

    // Properties

    // Stores all information about enemies in the battle scene, made of BattleEnemys
    static EnemyName battleParty = EnemyName.none;
    public static EnemyName BattleParty { get { return battleParty; } }
    
    // Gold earnings displayed from defeated enemies, added to BattleParty.Gold
    public static int BattleEarnings { get; set; } = 0;
    
    // Used at the end of the battle
    public static GameOver? GameOver { get; set; } = null;

    // Called by Enemy_Dungeon when a collision occurs
    public static void LoadBattle (EnemyName party)
    {
        AudioManager.PlaySound(AudioClipName.Encounter);
        BattleEarnings = 0;
        battleParty = party;
        LoadBattleMusic(party);
        
        // Open battle scene
        MenuManager.GoToMenu(MenuName.Battle);
    }
    
    // Choose the appropriate music to play
    static void LoadBattleMusic(EnemyName party)
    {
        // special cases
        if (dungeonLevel == 6)
        {
            // no music change on battle load in phantom forest
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
        // most cases
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
