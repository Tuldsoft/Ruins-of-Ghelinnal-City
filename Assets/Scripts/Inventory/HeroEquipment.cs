﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A container class for a hero's personal equipment. Inherits "Inventory". Only
/// equipped items are shown here.
/// </summary>
public class HeroEquipment : Inventory
{
    #region Fields
    // Inherits Contents and Names from Inventory

    // Tracks what item is currently in each slot
    Dictionary<EquipSlots, InvNames> slots = new Dictionary<EquipSlots, InvNames>();
    public Dictionary<EquipSlots, InvNames> Slots { get { return slots; } }
    #endregion

    #region Public Properties
    // Each returns the item equipped in that slot by referencing Slots. Null if none.

    public InvNames? Weapon 
    { 
        get { if (slots.TryGetValue(EquipSlots.Weapon, out InvNames name))
            { return name; }
            else { return null; } }
    }
    public InvNames? Helm
    {
        get
        {
            if (slots.TryGetValue(EquipSlots.Helm, out InvNames name))
            { return name; }
            else { return null; }
        }
    }
    public InvNames? Armor
    {
        get
        {
            if (slots.TryGetValue(EquipSlots.Armor, out InvNames name))
            { return name; }
            else { return null; }
        }
    }
    public InvNames? Gloves
    {
        get
        {
            if (slots.TryGetValue(EquipSlots.Gloves, out InvNames name))
            { return name; }
            else { return null; }
        }
    }
    public InvNames? Belt
    {
        get
        {
            if (slots.TryGetValue(EquipSlots.Belt, out InvNames name))
            { return name; }
            else { return null; }
        }
    }
    public InvNames? Boots
    {
        get
        {
            if (slots.TryGetValue(EquipSlots.Boots, out InvNames name))
            { return name; }
            else { return null; }
        }
    }
    #endregion

    #region Override Methods
    // Override for Inventory.AddInvItem(). Assigns the item to the slot, then base.AddInvItem()
    public override void AddInvItem(InvNames name, int quantity, bool persist = false)
    {
        EquipSlots addSlot = InvData.Data[name].Slot;

        if (slots.ContainsKey(addSlot))
        {
            Debug.Log(addSlot.ToString() + " slot already occupied.");
        }
        else
        {
            slots.Add(addSlot, name);
            base.AddInvItem(name, quantity, persist);
        }
    }

    // Override for Inventory.RemoveInvItem(). Empties the relevant slot, then base.RemoveInvItem()
    public override void RemoveInvItem(InvNames name, int quantity)
    {
        EquipSlots removeSlot = InvData.Data[name].Slot; 
        
        if (slots.ContainsKey(removeSlot))
        {
            slots.Remove(removeSlot);
            base.RemoveInvItem(name, quantity);
        }
        else
        {
            Debug.Log(removeSlot.ToString() + " slot is empty.");
        }
        
    }
    #endregion
}
