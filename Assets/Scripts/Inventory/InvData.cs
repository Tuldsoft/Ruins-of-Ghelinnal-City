using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// The reference class for all InvItems (including InvEqItems). Constructs a ConfigData object and stores
/// its contents for retrieval by all.
/// </summary>
static public class InvData
{
    // A file-reading object
    static ConfigData invData;
    
    // Provides InvItem data for each name in InvNames
    public static Dictionary<InvNames, InvItem> Data { get; } = new Dictionary<InvNames, InvItem>();


    static bool initialized = false;

    // Called only once by the Initializer
    public static void Initialize()
    {
        if (!initialized)
        {
            initialized = true;

            // Construct a new ConfigData object. Data is created from file upon construction.
            invData = new ConfigData(ConfigDataType.InvData);

            // Copy the contents of the ConfigData into this static class
            foreach (KeyValuePair<InvNames,InvItem> pair in invData.InvStatData)
            {
                Data.Add(pair.Key, pair.Value);
            }
        }
    }

    /// <summary>
    /// Creates a *new* InvItem, using only InvNames. Anything other than a potion has a BattleStats
    /// included, and thus is also an InvEqItem. If the item is to remain in the inventory even when
    /// at zero quantity, pass in persist=true. This occurs for the Shop inventory when out-of-stock.
    /// 
    /// This method is necessary, otherwise all inventories will reference only one of each kind of
    /// item, stored here.
    /// </summary>
    /// <param name="name">InvNames name of the item to be created.</param>
    /// <param name="persist">Whether the InvItem will persist in the Inventory at Quantity = 0.</param>
    /// <returns></returns>
    public static InvItem MakeNewInvItem(InvNames name, bool persist = false)
    {
        // Potions do not require BattleStats, and are not InvEqItems.
        if (Data[name].Type == InvType.Potion)
        {
            InvItem orig = InvData.Data[name];
            InvItem newItem = new InvItem(name, orig.FullName, orig.Description,
                orig.Sprite, orig.ID, orig.Price, orig.Quantity, orig.Type, orig.Subtype, orig.Slot, orig.Rank, persist);
            return newItem;
        }
        else
        {
            // InvEqItem includes tomes and equipment.
            InvEqItem orig = (InvEqItem)InvData.Data[name];
            InvEqItem newItem = new InvEqItem(name, orig.FullName, orig.Description,
                orig.Sprite, orig.ID, orig.Price, orig.Quantity, orig.Type, orig.Subtype, orig.Slot, orig.Rank, orig.BStats, persist);
            return newItem;
        }
        
    }
    
}
