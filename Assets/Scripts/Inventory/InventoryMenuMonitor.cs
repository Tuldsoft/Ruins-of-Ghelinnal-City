using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Attached to the prefabInventoryMenu. Populates a grid of Inventory panels, displays
/// information when an item is selected, allows for the use of potions and tomes.
/// 
/// This Inventory menu is accessed via the prefabHeroStatsMenu.
/// REFACTOR: Make one inventory menu monitor for the shop, equip, use, and battle inventory menus,
/// with children variants.
/// </summary>
public class InventoryMenuMonitor : MonoBehaviour
{
    #region Fields
    // A reference to the grid of InvItems
    [SerializeField]
    GameObject gridContent = null;
    // Reference to various text labels and buttons
    [SerializeField]
    Text goldText = null, useButton = null, messageText = null;

    // A reference to HeroStatsMenuMonitor's gameObject, for disabling the inactive menu
    GameObject parentGameObject = null;

    // A reference to the party inventory
    Inventory partyStash;
    
    // Index and BattleHero of selected hero in BattleLoader.Party.Hero[]
    int heroIndex = 0;
    BattleHero hero;

    List<EquipSlots> slots = new List<EquipSlots>();

    // Which item is currently selected, null for none.
    int? selection = null;

    // Timed message handler, for when hp/mp are at max
    EventTimer messageTimer;
    const float messageTimerDuration = 1.5f;
    string previousMessage = "";
    #endregion

    #region Methods
    // Start is run before the first frame update
    private void Start()
    {
        // Listen for user selections of panels
        EventManager.AddListener_Inventory_Select(MakeSelection);

        // Set up message timer
        messageTimer = gameObject.AddComponent<EventTimer>();
        messageTimer.Duration = messageTimerDuration;
        messageTimer.AddListener_Finished(ResetMessage);

        // Display current gold
        goldText.text = BattleLoader.Party.Gold.ToString();
    }


    /// <summary>
    /// Used by HeroStatsMenuMonitor to pass object references and to disable itself.
    /// </summary>
    /// <param name="index">index of hero in BattleLoader.Party.Hero[]</param>
    /// <param name="slots">A listing of all kinds of slots in EquipSlots</param>
    /// <param name="parentGameObject">A reference to HeroStatsMenuMonitor's gameObject</param>
    public void SetInventory(int index, List<EquipSlots> slots, GameObject parentGameObject)
    {
        // Disable previous menu
        this.parentGameObject = parentGameObject;
        parentGameObject.SetActive(false);

        // Store references
        heroIndex = index;
        hero = BattleLoader.Party.Hero[heroIndex];

        this.slots = slots;

        // Disable Use button until a useable item is selected.
        useButton.GetComponent<Button>().interactable = false;

        messageText.text = "";
        partyStash = BattleLoader.Party.Inventory;

        // Load partystash into the grid
        PopulateGrid();
    }

    // Creates a panel for each item in the party Inventory
    void PopulateGrid()
    {
        // Empty any pre-existing children (panels)
        for (int i = 0; i < gridContent.transform.childCount; i++)
        {
            Destroy(gridContent.transform.GetChild(i).gameObject);
        }
        
        // Create a grid item
        GameObject newPanel;
        GameObject prefabPanel = Resources.Load<GameObject>(@"MenuPrefabs\prefabInventoryPanel");

        // Show all item types in slots list (this is all of them)
        foreach (InvItem item in partyStash.Contents)
        {
            // For each item represented in slots, create a panel
            if (slots.Contains(item.Slot))
            {
                // Create instances of the prefab, with the Content panel as its parent
                newPanel = GameObject.Instantiate(prefabPanel, gridContent.transform);
                InventoryPanel invPanel = newPanel.GetComponent<InventoryPanel>();

                // If its Slot is "none", it is a consumable.
                bool isConsumable = item.Slot == EquipSlots.none ? true : false;
                
                // Set the ID and the display of the panel
                invPanel.SetIndex(partyStash.IndexOfItem(item.Name), item.FullName,
                    item.Description, item.Sprite, item.Quantity, isConsumable);
            }
        }
        // Begin with nothing selected
        MakeSelection(null, "");
    }

    // Highlight an item to display messages or enable the Use button, or de-highlight it.
    // Called from instances of InventoryPanel, each with its own id.
    void MakeSelection(int? index, string message)
    {
        // Index of the panel/item, null to do nothing.
        if (index != null)
        {
            // If there is an error message currently displaying, stop the event timer here
            // before changing the message.
            if (messageTimer.Running) { messageTimer.Stop(); }

            // If the currently selected item was the one just clicked, deselect it.
            if (selection == index )
            {
                selection = null;
                useButton.GetComponent<Button>().interactable = false;
                useButton.color = Color.gray;

                messageText.text = "";
            }
            else
            {
                // Highlight the clicked panel and select it.
                // Only activate Use button if it is a consumable (potion or tome)
                if (partyStash.Contents[(int)index].Slot == EquipSlots.none)
                {
                    useButton.GetComponent<Button>().interactable = true;
                    useButton.color = Color.white;
                }
                
                selection = index;
                messageText.text = message;
            }
        }
        else
        {
            // skip it because it is null (sent through other mechanic like during a reset)
        }
    }

    // Clears an error message. Called at the end of messageTimer.
    void ResetMessage()
    {
        messageText.text = previousMessage;
    }

    // For UseButton's On_Click() event. Deducts the item from party stash and
    // applies the effect to the hero.
    public void Click_UseButton()
    {
        // Only proceed if a valid choice is currently selected
        if (selection == null) { return; }

        // Reference item 
        InvItem item = partyStash.Contents[(int)selection];

        // Display error message if a non-consumable was selected (this should not happen)
        if (item.Slot != EquipSlots.none)
        {
            Debug.Log("Cannot use a non-consumable");
        }

        // Check for full health, send message if so
        int hp = hero.HP;
        int hpMax = hero.HPMax;
        if (hp == hpMax && item.Type == InvType.Potion && item.Subtype == InvSubtype.Health)
        {
            previousMessage = messageText.text;
            messageText.text = "You are already at full health.";
            messageTimer.Run();
            return;
        }
        
        // Check for full MP, send message if so
        int mp = hero.MP;
        int mpMax = hero.MPMax;
        if (mp == mpMax && item.Type == InvType.Potion && item.Subtype == InvSubtype.Mana)
        {
            previousMessage = messageText.text;
            messageText.text = "You are already at full mana.";
            messageTimer.Run();
            return;
        }

        // Apply the effects of a potion or tome
        switch (item.Type)
        {
            case InvType.Potion:
                AudioManager.PlaySound(AudioClipName.UsePotion);
                int healing;
                switch (item.Subtype)
                {
                    #region Use_Health_Pot
                    case InvSubtype.Health:
                        switch (item.Name)
                        {
                            case InvNames.Potion_Health_Tiny:
                                healing = 25;
                                break;
                            case InvNames.Potion_Health_Small:
                                healing = 75;
                                break;
                            case InvNames.Potion_Health_Medium:
                                healing = 250;
                                break;
                            case InvNames.Potion_Health_Large:
                                healing = 600;
                                break;
                            case InvNames.Potion_Health_Huge:
                                healing = 2000;
                                break;
                            case InvNames.Potion_Health_Epic:
                                healing = 100000;
                                break;
                            default:
                                healing = 25;
                                break;
                        }

                        if (hp + healing > hpMax)
                        {
                            healing = hpMax - hp;
                        }
                        //hp += healing;
                        hero.TakeDamage(-healing); ;
                        break;
                        // no call to DisplayDamage or TurnOver here, otherwise same as BattleInventoryMonitor
                    #endregion Use_Health_Pot

                    #region Use_Mana_Potion
                    case InvSubtype.Mana:
                        switch (item.Name)
                        {
                            case InvNames.Potion_Mana_Tiny:
                                healing = 10;
                                break;
                            case InvNames.Potion_Mana_Small:
                                healing = 20;
                                break;
                            case InvNames.Potion_Mana_Medium:
                                healing = 50;
                                break;
                            case InvNames.Potion_Mana_Large:
                                healing = 125;
                                break;
                            case InvNames.Potion_Mana_Huge:
                                healing = 250;
                                break;
                            case InvNames.Potion_Mana_Epic:
                                healing = 100000;
                                break;
                            default:
                                healing = 10;
                                break;
                        }

                        if (mp + healing > mpMax)
                        {
                            healing = mpMax - mp;
                        }
                        //hp += healing;
                        hero.TakeMPDamage(-healing); ;
                        break;
                        // no call to DisplayDamage or TurnOver here, otherwise same as BattleInventoryMonitor
                        #endregion Use_Mana_Pot
                }
                break;

            case InvType.Tome:
                AudioManager.PlaySound(AudioClipName.UseTome);
                // Since a tome has BattleStats, it can be equipped. There is nothing to unequip.
                hero.BStats.Equip((item as InvEqItem).BStats);
                
                // If max hp or mp changed, increase current by a similar amount
                if (item.Subtype == InvSubtype.Health)
                {
                    hero.TakeDamage(-(item as InvEqItem).BStats.BaseHPMax);
                }
                if (item.Subtype == InvSubtype.Mana)
                {
                    hero.TakeDamage(-(item as InvEqItem).BStats.BaseMPMax);
                }
                break;
        }

        // Remove the used item from the party inventory
        partyStash.RemoveInvItem(item.Name, 1);
        
        // Refresh the grid, since index for each item may have changed.
        PopulateGrid();
    }

    // CloseButton's On_Click() method. Returns to the previous HeroStatsMenu.
    public void Click_CloseButton()
    {
        AudioManager.Close();
        //hero.HealAll(); // HealAll disabled to accommodate Dungeon use.
        HeroStatsMenuMonitor statMenu = parentGameObject.GetComponent<HeroStatsMenuMonitor>();

        // Refresh the HeroStatsMenu with hp, etc.
        statMenu.RefreshHeroStats();
        statMenu.RefreshEquipped();
        statMenu.RefreshPanel();
        
        // Enable HeroStatsMenu
        parentGameObject.SetActive(true);
        
        // Destroy this menu
        Destroy(gameObject);
    }
    #endregion
}
