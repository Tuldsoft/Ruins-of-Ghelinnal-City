using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A container class of BattleHeros, party Gold, and party Inventory. Stored in BattleLoader.
/// </summary>
public class HeroParty
{
    #region Fields and Properties

    // The array of heroes. Can grow from 1 to 4.
    BattleHero[] heroes;
    public BattleHero[] Hero { get { return heroes; } }

    // The gold owned by the party. Used by BattleManager, and several menus.
    public int Gold { get; set; }
    
    // The Inventory of InvItems and EquipInvItems owned by the party.
    // Does not include anything equipped - those are stored in individual hero Inventorys
    public Inventory Inventory { get; set; } = new Inventory();
    #endregion

    #region Constructor

    // Creates a new array of BattleHeros, numMembers length. Other fields areleft empty.
    public HeroParty(int numMembers)
    {
        heroes = new BattleHero[numMembers];
    }

    #endregion

    #region Method

    /// <summary>
    /// Add a BattleHero to the Party. Expands the array if necessary.
    /// Requires the BattleHero to be provided.
    /// There is no RemoveHero() method.
    /// Used by BattleLoader, HirePanelMonitor, and file operations.
    /// Ex: Party.AddHero(new BattleHero(HeroType.Sabin, new BattleStats(), null, "Hero1"));
    /// </summary>
    /// <param name="hero">a constructed BattleHero</param>
    public void AddHero(BattleHero hero)
    {
        // First try to fill any null slots (ex. from Party creation)
        for (int i = 0; i < heroes.Length; i++)
        {
            if (heroes[i] == null)
            {
                // Fill the null slot with the provided hero
                heroes[i] = hero;
                return;
            }
        }

        //  Check for four filled slots
        if (heroes.Length >= 4) { Debug.Log("Already at max party capacity."); return; }


        // Only continue if all slots are full and Length < 4

        // Grow array
        BattleHero[] newArray = new BattleHero[heroes.Length + 1];

        for (int i = 0; i < heroes.Length; i++)
        {
            newArray[i] = heroes[i];
        }
        
        // Fill newly created empty slot
        newArray[newArray.Length - 1] = hero;

        // Replace array
        heroes = newArray;
    }
    #endregion
}
