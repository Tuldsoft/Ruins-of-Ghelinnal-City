using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory 
{
    protected List<InvItem> contents = new List<InvItem>();
    public List<InvItem> Contents { get { return contents; } }


    protected List<InvNames> names = new List<InvNames>();
    

    public virtual void AddInvItem (InvNames name, int quantity, bool persist = false)
    {
        if (!names.Contains(name))
        {
            contents.Add(InvData.MakeNewInvItem(name, persist));
            contents[contents.Count - 1].Quantity = quantity;
            names.Add(name);
            SortInventory();
        }
        else
        {
            // if -1 is passed, the quantity is "Unlimited"
            if (quantity < 0)
            {
                contents[names.IndexOf(name)].Quantity = -1;
            }
            else
            {
                // if it's already -1, leave it at -1, but add quantity if >= 0
                if (contents[names.IndexOf(name)].Quantity >= 0)
                {
                    contents[names.IndexOf(name)].Quantity += quantity;
                }
            }
        }
    }

    public virtual void RemoveInvItem (InvNames name, int quantity)
    {
        if (names.Contains(name))
        {
            int index = names.IndexOf(name);
            InvItem invItem = contents[index];

            // if existing quantity is -1, it is unlimited, and cannot be deducted from.
            // so only remove or deduct if 0 or greater
            if (invItem.Quantity < 0)
            {
                // the quantity doesn't change when < 0
            }
            else
            {
                
                if (invItem.Quantity - quantity <= 0)
                {

                    if (invItem.PersistAtZero)
                    {
                        // allow the shop inventory to have zero quantity
                        invItem.Quantity = 0;
                    }
                    else
                    {
                        // fully remove item from party inventory, because it becomes 0 or less and limited.
                        contents.RemoveAt(index);
                        names.RemoveAt(index);
                    }
                }
                else
                {
                    // deduct from inventory, but leave remainder behind
                    invItem.Quantity -= quantity;
                }
            }
        }
    }

    public void ClearInvItems ()
    {
        for (int i = contents.Count - 1; i >= 0; i--)
        {
            contents.RemoveAt(i);
            names.RemoveAt(i);
        }

    }

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

    public bool ContainsItem (InvNames name)
    {
        return names.Contains(name);
    }

    /*public bool ContainsItem(InvItem item)
    {
        return names.Contains(item.Name);
    }*/

    public int IndexOfItem (InvNames name)
    {
        return names.IndexOf(name);
    }

    /*public int IndexOfItem(InvItem item)
    {
        return names.IndexOf(item.Name);
    }*/

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
