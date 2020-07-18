using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Stores all battle-related data of a hero.
/// Current HP/MP is stored here.
/// REFACTOR: Convert BattleID/HP/MP/Poisoned/IsDead to a creature-agnostic Battler class
/// REFACTOR: Make BattleHero and BattleEnemy children of Battler
/// </summary>
public class BattleHero
{
    #region Fields and Properties

    // HeroType: Sabin, Edgar, Celes, or Locke (Terra is unused)
    // HeroType is only used internally, and never shown to the user.
    HeroType heroType = HeroType.Sabin;
    public HeroType HeroType { get { return heroType; } }

    // gameObject of the hero during battle
    GameObject gameObject;

    // BattleID of a hero is from -1 to -4 (enemies are 0+)
    int battleID;
    public int BattleID { get { return battleID; } }

    // PartyID is 0 to 3, an inversion of BattleID
    int partyID;
    public int PartyID { get { return partyID; } }

    // Display name of the hero, potentially set in menus by user
    string fullName = "Hero";
    public string FullName { get { return fullName; } }

    // Collection of sprites used by this hero
    HeroSprites sprites = new HeroSprites();
    public HeroSprites Sprites { get { return sprites; } }

    // Home position in battle, used by BattleManager.Propel
    Vector2 homePosition;
    public Vector2 HomePosition { get { return homePosition; } }

    // BattleStats such as Strength and HPMax
    public BattleStats BStats { get; set; } = new BattleStats();
    
    // Inventory of equipped Equipment
    public HeroEquipment Equipment { get; set; } = new HeroEquipment();

    // HP
    int hp = 50;
    public int HP { get { return hp; } }
    public int HPMax { get { return BStats.HPMax; } }

    // MP
    int mp = 50;
    public int MP { get { return mp; } }
    public int MPMax { get { return BStats.MPMax; } }

    // References to the hero's BStats
    public int Strength { get { return BStats.Strength; } }
    public int Defense { get { return BStats.Defense; } }
    public int Magic { get { return BStats.Magic; } }
    public int Resistance { get { return BStats.Resistance; } }
    public int Stamina { get { return BStats.Stamina; } }
    public int Agility { get { return BStats.Agility; } }
    public int HitPercent { get { return BStats.HitPercent; } }
    public int EvadePercent { get { return BStats.EvadePercent; } }
    public int CritChance { get { return BStats.CritChance; } }

    // Variable stats for use in battle

    // Action being executed by the hero
    public BattleMode BattleMode { get; set; } = BattleMode.none;
    // IsHeroTurn = true while waiting for user input
    public bool IsHeroTurn { get; set; }

    // whether at 0 HP. Used to skip the hero's turn.
    bool isDead = false; 
    public bool IsDead { get { return isDead; } }

    // number of turns remaining to take poison damage
    public int PoisonCounter { get; set; } = 0; 
    // stats of poisoner. Used to calculate amount of damage
    public BattleStats PoisonerBStats { get; set; } = null; 
    #endregion

    #region Constructor
    /// <summary>
    /// Builds a hero, including any value used for battle calculations. Persists between battles.
    /// </summary>
    /// <param name="type">as HeroType</param>
    /// <param name="stats">as BattleStats</param>
    /// <param name="equipment">inventory of equipped equipment</param>
    /// <param name="name">fullname</param>
    public BattleHero(HeroType type, BattleStats stats, HeroEquipment equipment = null, string name = "Hero1")
    {
        heroType = type;
        sprites = HeroSpriteData.MakeNewHeroSprites(heroType);

        fullName = name;
        
        // Load BStats
        BStats = stats;
        BStats.UpdateMax(); 
        HealAll();
        if (equipment != null)
        {
            Equipment = equipment;
        }
    }
    #endregion

    #region Methods
    /// <summary>
    /// Loads gameObjects related to this battle scene
    /// </summary>
    /// <param name="partyID">from 0 to 3</param>
    /// <param name="obj">hero's gameObject in this scene</param>
    public void LoadObjs(int partyID, GameObject obj)
    {
        gameObject = obj;
        homePosition = obj.transform.position; // original position
        this.partyID = partyID; // partyID is 0 to 3
        battleID = -partyID - 1; // battleID of a hero is -1 to -4
        obj.GetComponent<PropelObject>().SetID(partyID);

        sprites.SetObj(gameObject, this);
    }

    // Properties such as HP are manipulated only within the class

    // Modify HP/MP (up or down). Does not change any sliders. damage as int, not as Damage
    public int TakeDamage(int damage)
    {
        hp -= damage;
        if (hp <= 0) 
        { hp = 0; isDead = true; }
        else { isDead = false; }

        if (hp > HPMax) { hp = HPMax; }

        return damage;
    }
    public int TakeMPDamage(int damage)
    {
        mp -= damage;
        if (mp <= 0) { mp = 0; }
        if (mp > MPMax) { mp = MPMax; }

        return damage;
    }

    // HP/MP to max
    public void HealAll()
    {
        hp = HPMax;
        mp = MPMax;
        isDead = false;
    }

    // Used by a cheat button in the battle scene
    public void IncreaseHPMax(int increase)
    {
        int tempMax = HPMax;
        BStats.UpdateMax(increase, 0);
        hp += HPMax - tempMax;

    }

    /// <summary>
    /// Alter BattleStats when equipping an item. 
    /// Also called when using for tomes.
    /// All items have BattleStats. 
    /// </summary>
    /// <param name="name">the item to equip as an InvNames</param>
    public void Equip(InvNames name)
    {
        // First: Calculate new stats

        // retrieve bstats of item to remove, if one exists. If not, remains null.
        BattleStats remove = null;
        InvItem returnToStashItem = null;
        // compare kind of equip item to all the equipped items
        foreach (InvItem item in Equipment.Contents)
        {
            if (item.Slot == InvData.Data[name].Slot) 
            {
                remove = ((InvEqItem)InvData.Data[item.Name]).BStats;
                returnToStashItem = item;
                break; 
            }
        }

        // retrieve bstats of item to add
        BattleStats add = (InvData.Data[name] as InvEqItem).BStats;

        // Reduce hero stats by the remove InvItem (if not null). Increase by the add InvItem.
        BStats.Equip(add, remove);


        // Second: move items from inventories

        // remove 1 of EQUIPPED item from partyStash
        if (BattleLoader.Party.Inventory.ContainsItem(name))
        {
            BattleLoader.Party.Inventory.RemoveInvItem(name, 1);
        }

        // remove UNEQUIPPED item from Equipment, add 1 to partyStash
        if (returnToStashItem != null)
        {
            Equipment.RemoveInvItem(returnToStashItem.Name, 1); 
            BattleLoader.Party.Inventory.AddInvItem(returnToStashItem.Name, 1);
        }
        
        // add EQUIPPED item to hero's Equipment
        Equipment.AddInvItem(name, 1);
    }

    // Used by user in menus
    public void Rename(string name)
    {
        fullName = name;
    }
    #endregion
}
