using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Attached to prefabEquipMenu. A selective display of weapons and armor, allowing the
/// user to equip an InvEqItem. A side display shows the potential change in battle statistics.
/// REFACTOR: Combine this and InventoryMenuMonitor and BattleInventoryMonitor, and
/// perhaps even ShopMonitor.
/// </summary>

public class EquipMenuMonitor : MonoBehaviour
{
    #region Fields

    // Reference to the parent of grid objects
    [SerializeField]
    GameObject gridContent = null;

    // Display labels
    [SerializeField]
    Text equipButton = null, messageText = null;

    // Display of the currently equipped item to compare the selected item to
    [SerializeField]
    Text currentName = null;
    [SerializeField]
    GameObject currentObject = null;
    [SerializeField]
    Image currentImage = null;

    // Parent of the attached gameObject (for use when closing) (HeroStatsMenuMonitor)
    GameObject parentGameObject;

    
    Inventory partyStash;     // BattleLoader.Party.Inventory, Inventory to load InvEqItems from
    HeroEquipment heroStash;  // Inventory of the current hero
    int heroIndex = 0;        // Index of the current hero
    BattleHero hero;          // BattleHero class instance of the current hero
    EquipSlots slot;          // Type of weapon or armor that is being selected
    int? selection = null;    // Current panel selected
    #endregion

    #region Methods
    // Start is called before the first frame update
    private void Start()
    {
        // Set as a listener for a panel's Select event
        EventManager.AddListener_Equip_InvSelect(MakeSelection);
    }

    // After HeroStatsMenu instantiates the EquipMenu, it passes relevant values for set up
    public void SetHero(int index, EquipSlots slot, GameObject parentGameObject)
    {
        // Sets HeroStatsMenuMonitor's gameObject as a parent, and displays it so the new menu displays
        this.parentGameObject = parentGameObject;
        parentGameObject.SetActive(false);

        // Index of current hero
        heroIndex = index;
        hero = BattleLoader.Party.Hero[heroIndex];

        // Type of weapon or armor being selected
        this.slot = slot;
        heroStash = BattleLoader.Party.Hero[heroIndex].Equipment;
        
        // Make sure equip button is inactive at start
        equipButton.GetComponent<Button>().interactable = false;


        messageText.text = "";
        partyStash = BattleLoader.Party.Inventory;

        // Fill the grid
        PopulateGrid();
    }

    // Fills the grid of items with the EquipSlots type of weapon or armor
    void PopulateGrid()
    {
        // Empty any pre-existing panel items
        for (int i = 0; i < gridContent.transform.childCount; i++)
        {
            Destroy(gridContent.transform.GetChild(i).gameObject);
        }
        
        // Create a template grid item
        GameObject newPanel;
        GameObject prefabPanel = Resources.Load<GameObject>(@"MenuPrefabs\prefabEquipPanel");

        // Populate the grid with any item in the Party inventory, of the current slot
        foreach (InvItem item in partyStash.Contents)
        {
            // Only items appropriate to the slot
            if (item.Slot == slot)
            {
                // Create instances of the prefab, with the Content panel as its parent
                newPanel = GameObject.Instantiate(prefabPanel, gridContent.transform);

                EquipMenuPanel equipPanel = newPanel.GetComponent<EquipMenuPanel>();

                equipPanel.SetIndex(partyStash.IndexOfItem(item.Name), item.FullName,
                item.Description, item.Sprite, item.Quantity);
            }
        }

        // Find hero's current item, and display it
        bool itemEquipped = false;
        foreach (InvItem item in heroStash.Contents)
        {
            if (item.Slot == slot)
            {
                currentName.text = item.FullName;
                currentImage.sprite = item.Sprite;
                itemEquipped = true;
                break;
            }
        }
        currentObject.SetActive(itemEquipped);

        // Highlight an item
        MakeSelection(selection); // selection is null at start
    }

    // Called either by PopulateGrid() or an individual panel's select event
    void MakeSelection(int? index)
    {
        if (index != null)
        {
            // A panel was deselected, so remove selection and disable Equip button
            if (selection == index)
            {
                selection = null;
                equipButton.GetComponent<Button>().interactable = false;
                equipButton.color = Color.gray;
                messageText.text = "";
            }
            else
            {
                // Set the highlighted panel as the selection
                selection = index;
                equipButton.GetComponent<Button>().interactable = true;
                equipButton.color = Color.white;

                // Update the comparison display
                messageText.text = CompareEquipped((int)index);
            }
        }
        else
        {
            //Debug.Log("null index sent through events");
        }

    }
    
    // Called by MakeSelection(). Produces a string that compares a current item with the selected item
    string CompareEquipped(int index)
    {
        // For each stat, store the change in value between BattleStats
        Dictionary<BattleStatNames, int> comparison = new Dictionary<BattleStatNames, int>();
        
        // Store the selected InvEqItem from the partyStash
        InvEqItem selectedItem = (InvEqItem)partyStash.Contents[(int)index];
        
        // Retrieve the currently equipped InvEqItem from the heroStash
        InvEqItem equippedItem = null;
        foreach (InvEqItem item in heroStash.Contents)
        {
            if (item.Slot == selectedItem.Slot)
            {
                equippedItem = item;
                break;
            }
        }

        // Retrieve an array of ints from the selected item and the current item (if any)
        int[] selectedValues = selectedItem.BStats.Export();
        int[] equippedValues;
        if (equippedItem == null)
        {
            equippedValues = new int[11] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        }
        else
        {
            equippedValues = equippedItem.BStats.Export();
        }

        // For each stat in BattleStats, store the difference between the two sets
        for (int i = 0; i <= 10; i++)
        {
            int value = selectedValues[i] - equippedValues[i];
            if (value != 0)
            {
                comparison.Add((BattleStatNames)i, value);
            }
        }

        // Build a comparison string
        string output = "";
        string format = "+#;-#;(0)";
        foreach (KeyValuePair<BattleStatNames, int> pair in comparison)
        {
            switch (pair.Key)
            {
                case BattleStatNames.BaseHPMax:
                    output += "HP";
                    break;
                case BattleStatNames.BaseMPMax:
                    output += "MP";
                    break;
                case BattleStatNames.BaseHit:
                    output += "Hit";
                    break;
                case BattleStatNames.BaseEvade:
                    output += "Eva";
                    break;
                case BattleStatNames.CritChance:
                    output += "Crit";
                    break;
                default:
                    output += pair.Key.ToString().Substring(0, 3);
                    break;
            }

            output += ": " + pair.Value.ToString(format) + "\n";
        }

        // Return the comparison string
        return output;
    }

    // Used by the Equip button's On_Click()
    public void Click_EquipButton()
    {
        AudioManager.Chirp();
        
        // If there is a selection, equip the item (and unequip whatever else)
        if (selection != null)
        {
            hero.Equip(partyStash.Contents[(int)selection].Name);
        }
        
        // Re-populate the grid, given the newly adjusted Inventorys
        PopulateGrid();
    }

    // Used by the close button On_Click()
    public void Click_CloseButton()
    {
        AudioManager.Close();
        parentGameObject.GetComponent<HeroStatsMenuMonitor>().RefreshHeroStats();
        parentGameObject.GetComponent<HeroStatsMenuMonitor>().RefreshEquipped();
        parentGameObject.SetActive(true);
        Destroy(gameObject);
    }
    #endregion
}
