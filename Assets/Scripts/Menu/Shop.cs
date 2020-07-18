using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A static class for managing and storing the inventory of the shop. Contains methods 
/// for restocking the store when new dungeonLevelAccess is increased.
/// </summary>
public static class Shop
{
    #region Fields and Properties
    
    // The shop's Inventory
    public static Inventory Stock { get; set; }
    public static int StockLevel { get; set; } = 0;

    static bool initialized = false;
    #endregion

    #region Methods
    // Called by th initializer, after BattleLoader. 
    // Can be re-called to reset the inventory, ex. when load a game from a save file.
    public static void Initialize()
    {
        if (!initialized)
        {
            initialized = true;

            //if (Stock == null)
            {
                StockShop(1); // Add initial inventory
            }
        }
    }

    // Reinitializes the shop
    public static void ResetData()
    {
        initialized = false;
        Initialize();
    }

    // Stocks the shop with new items
    public static void StockShop(int dungeonAccess)
    {
        // No further stocking needed if dungeonAccess is still lower than StockLevel
        if (dungeonAccess <= StockLevel)
        {
            return;
        }

        // Since StockLevel is lower than dungeonAccess, raise StockLevel to the new access.
        StockLevel = dungeonAccess;

        // StockShop should only be called when a new dungeon level is reached.
        // The below switch will add the items to the existing stock.
        // Quantities are increased a when AddInvItem() passes a positive number.
        // Quantities are set to unlimited when AddInvItem() passes a negative number.

        switch (StockLevel)
        {
            case 1:
                // Since this is case 1, create the new Inventory before adding to it.
                Stock = new Inventory();

                // Add some consumables
                Stock.AddInvItem(InvNames.Potion_Health_Tiny, -1, true);
                Stock.AddInvItem(InvNames.Potion_Health_Small, 5, true);
                Stock.AddInvItem(InvNames.Potion_Mana_Tiny, -1, true);
                Stock.AddInvItem(InvNames.Potion_Mana_Small, 5, true);
                Stock.AddInvItem(InvNames.Scroll_Health_Lesser, 3, true);
                Stock.AddInvItem(InvNames.Scroll_Mana_Lesser, 3, true);
                Stock.AddInvItem(InvNames.Knife, -1, true);

                // Add weapons and armor
                AddLeveledWeaponsAndArmorToStock();
                break;

            case 2:
                // Add some consumables
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

                // Add weapons and armor
                AddLeveledWeaponsAndArmorToStock();
                break;

            case 3:
                // Add some consumables
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

                // Add weapons and armor
                AddLeveledWeaponsAndArmorToStock();
                break;

            case 4:
                // Add some consumables
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

                // Add weapons and armor
                AddLeveledWeaponsAndArmorToStock();
                break;

            case 5:
                // Add some consumables
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

                // Add weapons and armor
                AddLeveledWeaponsAndArmorToStock();
                break;

            case 6:
                // Add some consumables
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

                // Add weapons and armor
                AddLeveledWeaponsAndArmorToStock();
                break;

            case 7:
                // When the game is completed, everything becomes available,
                // and the quantities become unlimited.
                AddUnlimitedEverything();
                break;

            default:
                Debug.Log("unknown level when adding stock.");
                break;
        }
    }


    /// <summary>
    /// The weapons and armor that become available in the shop follow a pattern. 
    ///   If an item's Rank is TWO levels LOWER than the StockLevel, 2 copies are added. 
    ///   If the Rank is ONE level LOWER than StockLevel, add 1 copy. 
    ///   If the Rank is EQUAL to the StockLevel, add 1 copy. 
    ///   If the Rank is ONE level higher than the StockLevel, add 0 copies, so that the
    ///     item is displayed, but unavailable for purchase.
    ///   So at StockLevel 3, 2 rank 1 items, 1 rank 2 item, and 1 ranks 3 item are added,
    ///     in addition to what was added at StockLevel 2 and StockLevel 1.
    /// </summary>
    static void AddLeveledWeaponsAndArmorToStock()
    {
        foreach (KeyValuePair<InvNames, InvItem> pair in InvData.Data)
        {
            if (StockLevel >= 3)
            {
                // add 2 rank (level - 2) armor, weapon, each
                if (pair.Value.Rank == StockLevel - 2 &&
                    (pair.Value.Type == InvType.Weapon ||
                    pair.Value.Type == InvType.Armor))
                {
                    Stock.AddInvItem(pair.Key, 2, true);
                } // total (level - 2) is 4
            }
            
            if (StockLevel >= 2)
            {
                // add 1 rank (level - 1) armor, weapon, each
                if (pair.Value.Rank == StockLevel - 1 &&
                    (pair.Value.Type == InvType.Weapon ||
                    pair.Value.Type == InvType.Armor))
                {
                    Stock.AddInvItem(pair.Key, 1, true);
                } // total (level - 1) is 2
            }

            // add 1 rank (level) armor and weapons, each
            if (pair.Value.Rank == StockLevel &&
                (pair.Value.Type == InvType.Weapon ||
                pair.Value.Type == InvType.Armor))
            {
                Stock.AddInvItem(pair.Key, 1, true);
            } // total (level) is 1
                        
            // add 0 rank (level + 1) items for viewing
            if (StockLevel < 7) // there are no rank 8 items for viewing
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

    // Used via a cheatbutton and at the end of the game, makes every item in
    // the shop inventory unlimited.
    public static void AddUnlimitedEverything()
    {
        foreach (KeyValuePair<InvNames, InvItem> pair in InvData.Data)
        {
            Stock.AddInvItem(pair.Key, -1, true);
        }
    }
    #endregion
}
