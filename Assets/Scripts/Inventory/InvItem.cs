using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvItem : IComparable
{
    public InvNames Name { get; }
    public string FullName { get; }
    public string Description { get; }
    public int ID { get; }
    public Sprite Sprite { get; }
    public int Price { get; }
    public int Quantity { get; set; }
    public InvType Type { get; }
    public InvSubtype Subtype { get; }
    public EquipSlots Slot { get; }
    public int Rank { get; }
    public bool PersistAtZero { get; } = false;

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
    
    public int CompareTo(object obj)
    {
        // if there's no object, return 0 for equivalency
        if (obj == null)
        {
            return 0;
        }

        // try to declare obj as a Target
        InvItem otherItem = obj as InvItem;

        // if obj cannot be declared as an InvItem, throw an exception
        if (otherItem != null)
        {
            // sort by ID, lowest to highest
            // compare IDs
            if (ID < otherItem.ID) return -1;
            else if (ID > otherItem.ID) return 1;
            else return 0;
        }
        else throw new ArgumentException("Object is not an InvItem.");
    }
}
