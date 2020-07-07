using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
