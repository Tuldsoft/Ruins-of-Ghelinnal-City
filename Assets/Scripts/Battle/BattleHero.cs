using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHero
{

    HeroType heroType = HeroType.Sabin;
    public HeroType HeroType { get { return heroType; } }


    GameObject gameObject;
    //public GameObject GameObject { get { return gameObject; } }

    int battleID;
    public int BattleID { get { return battleID; } }

    int partyID;
    public int PartyID { get { return partyID; } }

    string fullName = "Hero";
    public string FullName { get { return fullName; } }

    HeroSprites sprites = new HeroSprites();
    public HeroSprites Sprites { get { return sprites; } }

    // home position in battle, used by Propel
    Vector2 homePosition;
    public Vector2 HomePosition { get { return homePosition; } }

    public BattleStats BStats { get; set; } = new BattleStats();
    public HeroEquipment Equipment { get; set; } = new HeroEquipment();

    int hp = 50;
    public int HP { get { return hp; } }
    public int HPMax { get { return BStats.HPMax; } }

    int mp = 50;
    public int MP { get { return mp; } }
    public int MPMax { get { return BStats.MPMax; } }

    public int Strength { get { return BStats.Strength; } }
    public int Defense { get { return BStats.Defense; } }
    public int Magic { get { return BStats.Magic; } }
    public int Resistance { get { return BStats.Resistance; } }
    public int Stamina { get { return BStats.Stamina; } }
    public int Agility { get { return BStats.Agility; } }
    public int HitPercent { get { return BStats.HitPercent; } }
    public int EvadePercent { get { return BStats.EvadePercent; } }
    public int CritChance { get { return BStats.CritChance; } }

    // Stats while in battle
    public BattleMode BattleMode { get; set; } = BattleMode.none;
    public bool IsHeroTurn { get; set; }
    bool isDead = false;
    public bool IsDead { get { return isDead; } }
    public int PoisonCounter { get; set; } = 0;
    public BattleStats PoisonerBStats { get; set; } = null;



    public BattleHero(HeroType type, BattleStats stats, HeroEquipment equipment = null, string name = "Hero1")
    {
        heroType = type;
        sprites = HeroSpriteData.MakeNewHeroSprites(heroType);

        fullName = name;
        BStats = stats;
        BStats.UpdateMax();
        HealAll();
        if (equipment != null)
        {
            Equipment = equipment;
        }
    }


    // Used by the BattleManager
    //public void LoadObjs(int id, GameObject obj, string name)
    public void LoadObjs(int partyID, GameObject obj)
    {
        gameObject = obj;
        homePosition = obj.transform.position;
        this.partyID = partyID;
        battleID = -partyID - 1;
        obj.GetComponent<PropelObject>().SetID(partyID);
        //fullName = name;

        // sprites = HeroSpriteData.MakeNewHeroSprites(heroType);
        sprites.SetObj(gameObject, this);
    }

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

    public void HealAll()
    {
        hp = HPMax;
        mp = MPMax;
        isDead = false;
    }

    public void IncreaseHPMax(int increase)
    {
        int tempMax = HPMax;
        //hp += increase;
        BStats.UpdateMax(increase, 0);
        hp += HPMax - tempMax;

    }

    public void Equip(InvNames name)
    {
        // Calculate new stats first

        // find bstats of item to remove
        BattleStats remove = null;
        InvItem returnToStashItem = null;
        foreach (InvItem item in Equipment.Contents)
        {
            if (item.Slot == InvData.Data[name].Slot) 
            {
                remove = ((InvEqItem)InvData.Data[item.Name]).BStats;
                returnToStashItem = item;
                break; 
            }
        }

        // find bstats of item to add
        BattleStats add = (InvData.Data[name] as InvEqItem).BStats;

        // hero.bstats -= remove( or 0 if null)
        // hero.bstats += add)
        BStats.Equip(add, remove);


        // move items from inventories second

        // remove 1 of EQUIPPED item from partyStash
        if (BattleLoader.Party.Inventory.ContainsItem(name))
        {
            BattleLoader.Party.Inventory.RemoveInvItem(name, 1);
        }

        // remove UNEQUIPPED item from Equipment, add 1 to partyStash
        // 
        if (returnToStashItem != null)
        {
            Equipment.RemoveInvItem(returnToStashItem.Name, 1); 
            BattleLoader.Party.Inventory.AddInvItem(returnToStashItem.Name, 1);
        }
        
        // add EQUIPPED item to Equipment
        Equipment.AddInvItem(name, 1);
    }

    public void Rename(string name)
    {
        fullName = name;
    }
}
