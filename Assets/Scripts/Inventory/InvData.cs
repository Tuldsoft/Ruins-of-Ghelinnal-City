using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// A collection of data to push into InvItems
static public class InvData
{
    static ConfigData invData;
    
    public static Dictionary<InvNames, InvItem> Data { get; } = new Dictionary<InvNames, InvItem>();
    //static Dictionary<InvNames, Sprite> sprites = new Dictionary<InvNames, Sprite>();

    static bool initialized = false;

    public static void Initialize()
    {
        if (!initialized)
        {
            initialized = true;

            invData = new ConfigData(ConfigDataType.InvData);

            foreach (KeyValuePair<InvNames,InvItem> pair in invData.InvStatData)
            {
                Data.Add(pair.Key, pair.Value);
            }


        }
    }


    /*static void LoadSprites()
    {
        foreach (InvNames name in InvNames.GetValues(typeof(InvNames)))
        {
            sprites.Add(name, Resources.Load<Sprite>(@"Sprites\InvItems\" + name.ToString()));
        }
    }*/

    public static InvItem MakeNewInvItem(InvNames name, bool persist = false)
    {
        if (Data[name].Type == InvType.Potion)
        {
            InvItem orig = InvData.Data[name];
            InvItem newItem = new InvItem(name, orig.FullName, orig.Description,
                orig.Sprite, orig.ID, orig.Price, orig.Quantity, orig.Type, orig.Subtype, orig.Slot, orig.Rank, persist);
            return newItem;
        }
        else
        {
            // includes tomes and equipment
            InvEqItem orig = (InvEqItem)InvData.Data[name];
            InvEqItem newItem = new InvEqItem(name, orig.FullName, orig.Description,
                orig.Sprite, orig.ID, orig.Price, orig.Quantity, orig.Type, orig.Subtype, orig.Slot, orig.Rank, orig.BStats, persist);
            return newItem;
        }
        
    }
    
}
