using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// The main container class for each enemy in battle.
/// Current hp/mp are stored here.
/// BattleID is updated as enemies are defeated.
/// Strength and similar unchanging stats are stored in an instance of BattleStats
/// REFACTOR: Convert BattleID/HP/MP/Poisoned/IsDead to a creature-agnostic Battler class
/// REFACTOR: Make BattleHero and BattleEnemy children of Battler
/// </summary>
public class BattleEnemy
{
    #region Fields and Properties

    // Name as an EnemyName. This is not the display name.
    EnemyName enemyName;
    public EnemyName Name { get { return enemyName; } }

    // Original placement on the battlefield
    Vector2 position;
    public Vector2 Position { get { return position; } }
    
    // The gameObject that this class references.
    GameObject gameObject;
    public GameObject GameObject { get { return gameObject; } }

    // BattleID: BattleEnemy IDs are 0 or positive.
    int battleID = 0;
    public int BattleID { get { return battleID; } }

    // The display name is created by combining Name and NameNumber. Ex: Wererat 2
    int NameNumber;

    // Display name
    string fullName;
    public string FullName { get{ return fullName; }}
    

    // Current HP, default 10
    int hp = 10;
    public int HP { get { return hp; } }

    // Reference to HPMax in BattleStats
    public int HPMax { get { return Stats.HPMax; } }

    // Current MP, default 10
    int mp = 10;
    public int MP { get { return mp; } }

    // Reference to MPMax in BattleStats
    public int MPMax { get { return Stats.MPMax; } }
    


    // Unchanging data such as Strength are contained in a BattleStats
    public BattleStats Stats = new BattleStats();



    // Reference to Min/MaxReward in BattleStats
    public int MinReward { get { return Stats.MinReward; } }
    public int MaxReward { get { return Stats.MaxReward; } }
    
    // Reward in gold granted upon defeat of the enemy. Random between Min and Max
    public int Reward { get { return Random.Range(MinReward, MaxReward + 1); } }


    // The enemy is dead when it reaches 0 HP.
    // The gameObject and this class remain while the gameObject fades out.
    bool isDead = false;
    public bool IsDead { get { return isDead; } }

    // Number of turns remaining while poisoned
    public int PoisonCounter { get; set; } = 0;
    // Reference to the BattleStats of the character/enemy that inflicted the poison
    // Null until poisoned.
    public BattleStats PoisonerBStats { get; set; } = null;

    // Number of times the enemy can be stolen from. 3 to zero.
    public int StealsRemaining { get; set; } = 3;
    
    
    // if IsBoss, triggers a long death sequence and completes the dungeon level.
    bool isBoss = false;
    public bool IsBoss { get { return isBoss; } }
    #endregion

    #region Constructor
    /// <summary>
    /// Builds a new BattleEnemy: name, stats, hp/mpMax.
    /// Other properties are set later with a call of LoadObjs.
    /// </summary>
    /// <param name="name">Name as EnemyName</param>
    /// <param name="stats">BattleStats is created outside this class and passed in</param>
    public BattleEnemy(EnemyName name, BattleStats stats)
    {
        enemyName = name;

        this.Stats = stats; 

        hp = stats.HPMax;
        mp = stats.MPMax;
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Loads scene- and gameObject-related information
    /// </summary>
    /// <param name="battleID">0+</param>
    /// <param name="nameSuffix">used when there are multiple enemies of the same kind</param>
    /// <param name="obj">gameObject referenced by this class</param>
    /// <param name="pos">original placement in the scene</param>
    /// <param name="boss">whether or not this enemy is the boss</param>
    public void LoadObjs(int battleID, int nameSuffix, GameObject obj, Vector2 pos, bool boss)
    {
        this.battleID = battleID;        
        
        // create the display name by adding a suffix
        NameNumber = nameSuffix;
        string idAddendum = "";
        if (NameNumber > 0) { idAddendum = " " + NameNumber; }
        fullName = (enemyName.ToString() + idAddendum).Replace("_", " ");

        gameObject = obj;

        position = pos;
        obj.transform.position = position;

        isBoss = boss;
    }

    // Properties such as HP are manipulated through public methods, not directly

    // Sets BattleID to the int provided. 
    // Used when an enemy dies and BattleIDs need recalculating.
    public void ResetID(int battleID)
    {
        this.battleID = battleID;
    }

    // Modify HP up or down. Does not change HPSlider.
    public void TakeDamage(int damage)
    {
        hp -= damage;
        if (hp <= 0)
        { hp = 0; isDead = true; }
        else { isDead = false; }
        if (hp > HPMax) { hp = HPMax; }
    }

    // Modify MP up or down.
    public void TakeMPDamage(int damage)
    {
        mp -= damage;
        if (mp <= 0) { mp = 0; }
        if (mp > MPMax) { mp = MPMax; }
    }

    // Restores hp and mp to max, used only during setup
    public void HealAll()
    {
        hp = HPMax;
        mp = MPMax;
    }
    #endregion
}
