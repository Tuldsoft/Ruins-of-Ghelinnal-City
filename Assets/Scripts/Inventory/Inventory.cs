using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A container class for InvItems (and InvEqItems). Any place items are stored is considered an
/// Inventory, for example, a hero's equipped items, the party inventory, the shop inventory, the
/// shopping cart, etc. Contains methods for adding and removing items from the collection.
/// 
/// If a Quanity becomes 0, the item is removed, except when it is marked as "persists".
/// </summary>
public class Inventory 
{
    // InvItems in stock
    protected List<InvItem> contents = new List<InvItem>();
    public List<InvItem> Contents { get { return contents; } }

    // Parallel list of just the names of the InvItems
    protected List<InvNames> names = new List<InvNames>();
    
    // Adds an InvItem, given an InvName, quantity, and whether it is to persist at zero
    public virtual void AddInvItem (InvNames name, int quantity, bool persist = false)
    {
        // If the item isn't already there, create a new item and add it to the inventory
        if (!names.Contains(name))
        {
            contents.Add(InvData.MakeNewInvItem(name, persist));
            contents[contents.Count - 1].Quantity = quantity;
            names.Add(name);
            SortInventory();
        }
        else
        {
            // If the Inventory already contains this items, just increase the quantity
            // If a quantity of -1 is passed, the quantity "increases" to "Unlimited"
            if (quantity < 0)
            {
                contents[names.IndexOf(name)].Quantity = -1; // -1 for "Unlimited"
            }
            else
            {
                // If quantity is already -1, leave it at -1, but add quantity if >= 0
                if (contents[names.IndexOf(name)].Quantity >= 0)
                {
                    contents[names.IndexOf(name)].Quantity += quantity;
                }
            }
        }
    }


    // Removes an InvItem from the Inventory, given a name and a quantity.
    public virtual void RemoveInvItem (InvNames name, int quantity)
    {
        // Only proceed if the item exists in the inventory
        if (names.Contains(name))
        {
            // Find the index of the InvItem by referencing names
            int index = names.IndexOf(name);
            InvItem invItem = contents[index];


            // If existing quantity is -1, it is unlimited, and cannot be deducted from.
            // Only remove or deduct if 0 or greater
            if (invItem.Quantity < 0)
            {
                // The quantity doesn't change when < 0
            }
            else
            {
                // If the deduction of quantity would result in a zero or less Quantity
                if (invItem.Quantity - quantity <= 0)
                {

                    if (invItem.PersistAtZero)
                    {
                        // The item persists, and the (shop) inventory has quantity of the item
                        invItem.Quantity = 0;
                    }
                    else
                    {
                        // Fully remove item from the inventory, because it becomes 0 or less.
                        contents.RemoveAt(index);
                        names.RemoveAt(index);
                    }
                }
                else
                {
                    // Deduct from inventory, but leave a remainder of 1+ behind
                    invItem.Quantity -= quantity;
                }
            }

            // No sort is required because the order only changes when adding.
        }
    }

    // Empty the Inventory of all InvItems
    public void ClearInvItems ()
    {
        // redo with .Clear()?
        for (int i = contents.Count - 1; i >= 0; i--)
        {
            contents.RemoveAt(i);
            names.RemoveAt(i);
        }

    }

    // Retrieve a reference to an item, but do not remove it.
    public InvItem GetItem(InvNames name)
    {
        if (names.Contains(name))
        {
            return contents[names.IndexOf(name)];
        }
        else
        {
            return null;
        }
    }

    // Whether or not the Inventory contains an item. Returns true even if Quantity = 0.
    public bool ContainsItem (InvNames name)
    {
        return names.Contains(name);
    }

    // Provides the index of an item in both contents and names
    public int IndexOfItem (InvNames name)
    {
        return names.IndexOf(name);
    }

    // Sorts the Inventory according to the order presented in InvNames (InvItem is IComparible)
    protected void SortInventory ()
    {
        contents.Sort();
        names.Clear();

        foreach (InvItem item in contents)
        {
            names.Add(item.Name);
        }

    }  

}
