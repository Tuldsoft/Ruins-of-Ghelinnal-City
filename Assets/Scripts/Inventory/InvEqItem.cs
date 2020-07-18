using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The only difference between InvItem and InvEqItem is the inclusion of a BattleStats.
/// 
/// InvEqItem includes weapons, armor, and tomes, but not potions.
/// </summary>
public class InvEqItem : InvItem
{
    public BattleStats BStats { get; }

    public InvEqItem(InvNames name, string fullName, string description, Sprite sprite, int id,
        int price, int quantity, InvType type, InvSubtype subtype, EquipSlots slot, int rank, BattleStats stats, bool persist = false) 
        : base( name, fullName, description, sprite, id, price, quantity, type, subtype, slot, rank, persist)
    {
        BStats = stats;
    }
}
