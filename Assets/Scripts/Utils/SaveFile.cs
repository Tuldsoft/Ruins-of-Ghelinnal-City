using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// A Serializable container class used by FileUtil to create a file (SaveSlot##.RoGC) 
/// in persistentDataPath/. Includes methods for collecting and exporting data.
/// </summary>

[System.Serializable]
public class SaveFile
{
    #region Fields
    // SaveFile unique variables
    int? saveSlot = null;
    public int? SaveSlot { get { return saveSlot; } }
    string versionNum = Application.version;
    public string VersionNum { get { return versionNum; } }



    // Global variables
    bool isCheating = true;
    int dungeonLevelAccess = 1;
    bool gameCompleted = false;
    int lastTalkingPoint = 0;

    // Shop variables
    int shopStockLevel = 0;
    List<InvNames> shopStockInvItemNames = new List<InvNames>();
    List<int> shopStockItemQuantities = new List<int>();

    // HeroParty Variables
    int partyGold = 275;
    int partyNumOfMembers = 1;


    // Inventory variables
    List<InvNames> partyInvItemNames = new List<InvNames>();
    List<int> partyInvItemQuantities = new List<int>();
    List<HeroType> heroTypes = new List<HeroType>();


    // Hero variables
    List<string> heroFullNames = new List<string>();
    List<BattleStats> battleStats = new List<BattleStats>();

    List<InvNames> hero1EquipmentNames = new List<InvNames>();
    List<InvNames> hero2EquipmentNames = new List<InvNames>();
    List<InvNames> hero3EquipmentNames = new List<InvNames>();
    List<InvNames> hero4EquipmentNames = new List<InvNames>();
    #endregion

    #region Constructor
    // Constructor only specifies the slot. The rest is handled by SaveData()
    public SaveFile(int slot)
    {
        saveSlot = slot;
    }
    #endregion

    #region Methods
    // Gathers and saves data from BattleLoader for reconstruction later
    public void SaveData ()
    {
        // Global vars
        isCheating = BattleLoader.IsCheating;
        dungeonLevelAccess = BattleLoader.DungeonLevelAccess;
        gameCompleted = BattleLoader.GameCompleted;
        lastTalkingPoint = BattleLoader.LastTalkingPoint;

        // Shop vars
        shopStockLevel = Shop.StockLevel;
        shopStockInvItemNames.Clear();
        shopStockItemQuantities.Clear();
        foreach (InvItem item in Shop.Stock.Contents)
        {
            shopStockInvItemNames.Add(item.Name);
            shopStockItemQuantities.Add(item.Quantity);
        }

        // HeroParty vars
        partyGold = BattleLoader.Party.Gold;
        partyNumOfMembers = BattleLoader.Party.Hero.Length;

        // Inventory vars
        partyInvItemNames.Clear();
        partyInvItemQuantities.Clear();
        foreach (InvItem item in BattleLoader.Party.Inventory.Contents)
        {
            partyInvItemNames.Add(item.Name);
            partyInvItemQuantities.Add(item.Quantity);
        }

        // Hero vars
        heroFullNames.Clear();
        battleStats.Clear();
        heroTypes.Clear();
        foreach (BattleHero hero in BattleLoader.Party.Hero)
        {
            heroFullNames.Add(hero.FullName);
            battleStats.Add(hero.BStats);
            heroTypes.Add(hero.HeroType);
        }

        hero1EquipmentNames.Clear();
        hero2EquipmentNames.Clear();
        hero3EquipmentNames.Clear();
        hero4EquipmentNames.Clear();
        foreach (InvEqItem item in BattleLoader.Party.Hero[0].Equipment.Contents)
        {
            hero1EquipmentNames.Add(item.Name);
        }
        if (partyNumOfMembers >= 2)
        {
            foreach (InvEqItem item in BattleLoader.Party.Hero[1].Equipment.Contents)
            {
                hero2EquipmentNames.Add(item.Name);
            }
        }
        if (partyNumOfMembers >= 3)
        {
            foreach (InvEqItem item in BattleLoader.Party.Hero[2].Equipment.Contents)
            {
                hero3EquipmentNames.Add(item.Name);
            }
        }
        if (partyNumOfMembers >= 4)
        {
            foreach (InvEqItem item in BattleLoader.Party.Hero[3].Equipment.Contents)
            {
                hero4EquipmentNames.Add(item.Name);
            }
        }
    }

    // Takes the values stored internally (as created when loaded by FileUtil)
    // and propogates them throughout the game (BattleLoader and Shop)
    public void LoadData()
    {
        // Global vars
        BattleLoader.IsCheating = isCheating;
        BattleLoader.DungeonLevelAccess = dungeonLevelAccess;
        BattleLoader.GameCompleted = gameCompleted;
        BattleLoader.LastTalkingPoint = lastTalkingPoint;

        // Shop vars
        Inventory shopStock = Shop.Stock;
        shopStock.ClearInvItems();
        for (int i = 0; i < shopStockInvItemNames.Count; i++)
        {
            shopStock.AddInvItem(shopStockInvItemNames[i], shopStockItemQuantities[i]);
        }

        Shop.StockLevel = shopStockLevel;

        // HeroParty vars
        BattleLoader.Party = new HeroParty(partyNumOfMembers);
        BattleLoader.Party.Gold = partyGold;

        // Inventory vars
        Inventory inventory = BattleLoader.Party.Inventory;
        inventory.ClearInvItems();
        
        for (int i = 0; i < partyInvItemNames.Count; i++)
        {
            inventory.AddInvItem(partyInvItemNames[i], partyInvItemQuantities[i]);
        }

        // Hero vars
        int count = heroFullNames.Count;
        for (int i = 0; i < count; i++)
        {
            BattleLoader.Party.AddHero(new BattleHero(heroTypes[i], battleStats[i], new HeroEquipment(),
                heroFullNames[i]));
        }


        // Hero eq
        foreach (InvNames item in hero1EquipmentNames)
        {
            BattleLoader.Party.Hero[0].Equipment.AddInvItem(item, 1);
        }
        if (count >= 2)
        {
            foreach (InvNames item in hero2EquipmentNames)
            {
                BattleLoader.Party.Hero[1].Equipment.AddInvItem(item, 1);
            }
        }
        if (count >= 3)
        {
            foreach (InvNames item in hero3EquipmentNames)
            {
                BattleLoader.Party.Hero[2].Equipment.AddInvItem(item, 1);
            }
        }
        if (count >= 4)
        {
            foreach (InvNames item in hero4EquipmentNames)
            {
                BattleLoader.Party.Hero[3].Equipment.AddInvItem(item, 1);
            }
        }
    }

    // Creates a summary string of the SaveFile, for use on buttons when saving in 
    // the FileMenu, and as the value for this slot's key in PlayerPrefs
    public override string ToString()
    {
        string saveString = "";

        // Name all heroes
        foreach (string name in heroFullNames)
        {
            saveString += name + ", ";
        }

        // Show gold
        saveString += partyGold + " Gold, ";
        
        // Imprint time onto file
        saveString += System.DateTime.Now.ToString("MMMM dd, yyyy, HH:mm");

        return saveString;
    }
    #endregion
}
