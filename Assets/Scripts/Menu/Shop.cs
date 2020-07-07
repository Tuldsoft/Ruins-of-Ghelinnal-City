using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Shop
{
    public static Inventory Stock { get; set; }

    public static int StockLevel { get; set; } = 0;

    static bool initialized = false;

    public static void Initialize()
    {
        if (!initialized)
        {
            initialized = true;

            //if (Stock == null)
            {
                StockShop(1);
            }
            
        }
    }

    public static void ResetData()
    {
        initialized = false;
        Initialize();
    }

    public static void StockShop(int dungeonAccess)
    {
        // use this later
        
        if (dungeonAccess <= StockLevel)
        {
            return;
        }

        StockLevel = dungeonAccess;
        switch (StockLevel)
        {
            case 1:
                Stock = new Inventory();
                // add some limited stock items
                Stock.AddInvItem(InvNames.Potion_Health_Tiny, -1, true);
                Stock.AddInvItem(InvNames.Potion_Health_Small, 5, true);
                Stock.AddInvItem(InvNames.Potion_Mana_Tiny, -1, true);
                Stock.AddInvItem(InvNames.Potion_Mana_Small, 5, true);
                Stock.AddInvItem(InvNames.Scroll_Health_Lesser, 3, true);
                Stock.AddInvItem(InvNames.Scroll_Mana_Lesser, 3, true);
                Stock.AddInvItem(InvNames.Knife, -1, true);

                AddLeveledWeaponsAndArmorToStock();
                break;

            case 2:
                // add some limited stock items
                Stock.AddInvItem(InvNames.Potion_Health_Small, -1, true);
                Stock.AddInvItem(InvNames.Potion_Health_Medium, 1, true);
                Stock.AddInvItem(InvNames.Potion_Mana_Small, -1, true);
                Stock.AddInvItem(InvNames.Potion_Mana_Medium, 1, true);
                Stock.AddInvItem(InvNames.Scroll_Health_Lesser, 3, true);
                Stock.AddInvItem(InvNames.Scroll_Mana_Lesser, 3, true);
                Stock.AddInvItem(InvNames.Scroll_Strength_Lesser, 3, true);
                Stock.AddInvItem(InvNames.Scroll_Defense_Lesser, 3, true);
                Stock.AddInvItem(InvNames.Scroll_Magic_Lesser, 3, true);
                Stock.AddInvItem(InvNames.Scroll_Resistance_Lesser, 3, true);

                AddLeveledWeaponsAndArmorToStock();
                break;

            case 3:
                // add some limited stock items
                Stock.AddInvItem(InvNames.Potion_Health_Medium, 5, true);
                Stock.AddInvItem(InvNames.Potion_Health_Large, 1, true);
                Stock.AddInvItem(InvNames.Potion_Mana_Medium, 5, true);
                Stock.AddInvItem(InvNames.Potion_Mana_Large, 1, true);
                Stock.AddInvItem(InvNames.Scroll_Health_Lesser, 5, true);
                Stock.AddInvItem(InvNames.Scroll_Health_Greater, 2, true);
                Stock.AddInvItem(InvNames.Scroll_Mana_Lesser, 5, true);
                Stock.AddInvItem(InvNames.Scroll_Health_Greater, 2, true);
                Stock.AddInvItem(InvNames.Scroll_Strength_Lesser, 3, true);
                Stock.AddInvItem(InvNames.Scroll_Defense_Lesser, 3, true);
                Stock.AddInvItem(InvNames.Scroll_Magic_Lesser, 3, true);
                Stock.AddInvItem(InvNames.Scroll_Resistance_Lesser, 3, true);
                Stock.AddInvItem(InvNames.Tome_Stamina, 1, true);
                Stock.AddInvItem(InvNames.Tome_Agility, 1, true);

                AddLeveledWeaponsAndArmorToStock();
                break;

            case 4:
                // add some limited stock items
                Stock.AddInvItem(InvNames.Potion_Health_Medium, -1, true);
                Stock.AddInvItem(InvNames.Potion_Health_Large, 5, true);
                Stock.AddInvItem(InvNames.Potion_Health_Huge, 1, true);
                Stock.AddInvItem(InvNames.Potion_Mana_Medium, -1, true);
                Stock.AddInvItem(InvNames.Potion_Mana_Large, 5, true);
                Stock.AddInvItem(InvNames.Potion_Health_Huge, 1, true);
                Stock.AddInvItem(InvNames.Scroll_Health_Lesser, 5, true);
                Stock.AddInvItem(InvNames.Scroll_Health_Greater, 3, true);
                Stock.AddInvItem(InvNames.Scroll_Mana_Lesser, 5, true);
                Stock.AddInvItem(InvNames.Scroll_Mana_Lesser, 3, true);
                Stock.AddInvItem(InvNames.Scroll_Strength_Lesser, 5, true);
                Stock.AddInvItem(InvNames.Scroll_Strength_Greater, 2, true);
                Stock.AddInvItem(InvNames.Scroll_Defense_Lesser, 5, true);
                Stock.AddInvItem(InvNames.Scroll_Defense_Greater, 2, true);
                Stock.AddInvItem(InvNames.Scroll_Magic_Lesser, 5, true);
                Stock.AddInvItem(InvNames.Scroll_Magic_Greater, 2, true);
                Stock.AddInvItem(InvNames.Scroll_Resistance_Lesser, 5, true);
                Stock.AddInvItem(InvNames.Scroll_Resistance_Greater, 2, true);
                Stock.AddInvItem(InvNames.Tome_Stamina, 2, true);
                Stock.AddInvItem(InvNames.Tome_Agility, 2, true);

                AddLeveledWeaponsAndArmorToStock();
                break;

            case 5:
                // add some limited stock items
                Stock.AddInvItem(InvNames.Potion_Health_Large, -1, true);
                Stock.AddInvItem(InvNames.Potion_Health_Huge, 4, true);
                Stock.AddInvItem(InvNames.Potion_Health_Epic, 1, true);
                Stock.AddInvItem(InvNames.Potion_Mana_Large, -1, true);
                Stock.AddInvItem(InvNames.Potion_Mana_Huge, 4, true);
                Stock.AddInvItem(InvNames.Potion_Mana_Epic, 1, true);
                Stock.AddInvItem(InvNames.Scroll_Health_Lesser, 10, true);
                Stock.AddInvItem(InvNames.Scroll_Health_Greater, 5, true);
                Stock.AddInvItem(InvNames.Rune_Health, 4, true);
                Stock.AddInvItem(InvNames.Scroll_Mana_Lesser, 10, true);
                Stock.AddInvItem(InvNames.Scroll_Mana_Greater, 5, true);
                Stock.AddInvItem(InvNames.Rune_Mana, 4, true);
                Stock.AddInvItem(InvNames.Scroll_Strength_Lesser, 10, true);
                Stock.AddInvItem(InvNames.Scroll_Strength_Greater, 5, true);
                Stock.AddInvItem(InvNames.Scroll_Defense_Lesser, 10, true);
                Stock.AddInvItem(InvNames.Scroll_Defense_Greater, 5, true);
                Stock.AddInvItem(InvNames.Scroll_Magic_Lesser, 10, true);
                Stock.AddInvItem(InvNames.Scroll_Magic_Greater, 5, true);
                Stock.AddInvItem(InvNames.Scroll_Resistance_Lesser, 10, true);
                Stock.AddInvItem(InvNames.Scroll_Resistance_Greater, 5, true);
                Stock.AddInvItem(InvNames.Tome_Stamina, 2, true);
                Stock.AddInvItem(InvNames.Tome_Agility, 2, true);

                AddLeveledWeaponsAndArmorToStock();
                break;

            case 6:
                // add some limited stock items
                Stock.AddInvItem(InvNames.Potion_Health_Huge, 15, true);
                Stock.AddInvItem(InvNames.Potion_Health_Epic, 2, true);
                Stock.AddInvItem(InvNames.Potion_Mana_Huge, 15, true);
                Stock.AddInvItem(InvNames.Potion_Mana_Epic, 2, true);
                Stock.AddInvItem(InvNames.Scroll_Health_Lesser, 15, true);
                Stock.AddInvItem(InvNames.Scroll_Health_Greater, 10, true);
                Stock.AddInvItem(InvNames.Rune_Health, 5, true);
                Stock.AddInvItem(InvNames.Scroll_Mana_Lesser, 15, true);
                Stock.AddInvItem(InvNames.Scroll_Mana_Greater, 10, true);
                Stock.AddInvItem(InvNames.Rune_Mana, 5, true);
                Stock.AddInvItem(InvNames.Scroll_Strength_Lesser, 15, true);
                Stock.AddInvItem(InvNames.Scroll_Strength_Greater, 10, true);
                Stock.AddInvItem(InvNames.Rune_Strength, 4, true);
                Stock.AddInvItem(InvNames.Scroll_Defense_Lesser, 15, true);
                Stock.AddInvItem(InvNames.Scroll_Defense_Greater, 10, true);
                Stock.AddInvItem(InvNames.Rune_Defense, 4, true);
                Stock.AddInvItem(InvNames.Scroll_Magic_Lesser, 15, true);
                Stock.AddInvItem(InvNames.Scroll_Magic_Greater, 10, true);
                Stock.AddInvItem(InvNames.Rune_Magic, 4, true);
                Stock.AddInvItem(InvNames.Scroll_Resistance_Lesser, 15, true);
                Stock.AddInvItem(InvNames.Scroll_Resistance_Greater, 10, true);
                Stock.AddInvItem(InvNames.Rune_Resistance, 4, true);
                Stock.AddInvItem(InvNames.Tome_Stamina, 3, true);
                Stock.AddInvItem(InvNames.Tome_Stamina_Greater, 1, true);
                Stock.AddInvItem(InvNames.Tome_Agility, 3, true);
                Stock.AddInvItem(InvNames.Tome_Agility_Greater, 1, true);

                AddLeveledWeaponsAndArmorToStock();
                break;

            case 7:
                AddUnlimitedEverything();
                break;

            default:
                Debug.Log("unknown level when adding stock.");
                break;
        }

    }

    static void AddLeveledWeaponsAndArmorToStock()
    {
        foreach (KeyValuePair<InvNames, InvItem> pair in InvData.Data)
        {
            if (StockLevel >= 3)
            {
                // add 2 rank 1 (level - 2) armor, weapon, each
                if (pair.Value.Rank == StockLevel - 2 &&
                    (pair.Value.Type == InvType.Weapon ||
                    pair.Value.Type == InvType.Armor))
                {
                    Stock.AddInvItem(pair.Key, 2, true);
                } // total (level - 2) is 4
            }
            
            if (StockLevel >= 2)
            {
                // add 1 rank 2 (level - 1) armor, weapon, each
                if (pair.Value.Rank == StockLevel - 1 &&
                    (pair.Value.Type == InvType.Weapon ||
                    pair.Value.Type == InvType.Armor))
                {
                    Stock.AddInvItem(pair.Key, 1, true);
                } // total (level - 1) is 2
            }

            // add 1 rank 3 (level) armor and weapons, each
            if (pair.Value.Rank == StockLevel &&
                (pair.Value.Type == InvType.Weapon ||
                pair.Value.Type == InvType.Armor))
            {
                Stock.AddInvItem(pair.Key, 1, true);
            } // total (level) is 1
                        
            // rank 4 for viewing
            if (StockLevel < 7)
            {
                if (pair.Value.Rank == StockLevel + 1 &&
                (pair.Value.Type == InvType.Weapon ||
                pair.Value.Type == InvType.Armor))
                {
                    Stock.AddInvItem(pair.Key, 0, true);
                } // total (level + 1) is 0
            }
        }
    }

    public static void AddUnlimitedEverything()
    {
        foreach (KeyValuePair<InvNames, InvItem> pair in InvData.Data)
        {
            Stock.AddInvItem(pair.Key, -1, true);
        }
    }
}
