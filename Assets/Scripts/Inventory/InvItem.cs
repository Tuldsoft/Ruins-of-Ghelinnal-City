using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An instance of an inventory item, with all of its data. Is IComparible, so that it may be 
/// sorted according to its ID (from the csv). Multiple InvItem objects of the same InvNames 
/// cannot exist in the same Inventory; instead, its quantity increases or decreases.
/// </summary>
public class InvItem : IComparable
{
    #region Auto-Implemented Properties
    public InvNames Name { get; }
    public string FullName { get; }
    public string Description { get; }
    public int ID { get; }
    public Sprite Sprite { get; }
    public int Price { get; }
    public int Quantity { get; set; } // 0 on if persists, -1 if Unlimited
    public InvType Type { get; }
    public InvSubtype Subtype { get; }
    public EquipSlots Slot { get; }
    public int Rank { get; }
    public bool PersistAtZero { get; } = false;
    #endregion

    #region Constructor
    public InvItem(InvNames name, string fullName, string description, Sprite sprite, int id, int price, int quantity, InvType type, InvSubtype subtype, EquipSlots slot, int rank, bool persist = false) 
    {   
        Name = name;
        FullName = fullName;
        Description = description;
        Sprite = sprite;
        ID = id;
        Price = price;
        Quantity = quantity;
        Type = type;
        Subtype = subtype;
        Slot = slot;
        Rank = rank;
        PersistAtZero = persist;
    }
    #endregion

    #region IComparable Implementation
    // Allows use of List<InvItems>.Sort(), comparing according to ID.
    public int CompareTo(object obj)
    {
        // if there's no object, return 0 for equivalency
        if (obj == null)
        {
            return 0;
        }

        // Try to declare obj as a Target
        InvItem otherItem = obj as InvItem;

        // If obj cannot be declared as an InvItem, throw an exception
        if (otherItem != null)
        {
            // Sort by ID, lowest to highest
            // Compare IDs
            if (ID < otherItem.ID) return -1;
            else if (ID > otherItem.ID) return 1;
            else return 0;
        }
        else throw new ArgumentException("Object is not an InvItem.");
    }
    #endregion
}
