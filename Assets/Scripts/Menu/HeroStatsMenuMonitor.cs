using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Attached to prefabHeroStatsMenu. Displays a hero's current equipment or BattleStats
/// through toggle buttons. Accessed from the Manage Party menu. Provides access to the
/// Equip menu. Inherits PartyMenuPanel, which contains the code for displaying
/// name, HP and MP sliders.
/// </summary>
public class HeroStatsMenuMonitor : PartyMenuPanel
{
    #region Fields
    
    // Container objects for the two displays
    [SerializeField]
    GameObject equipmentPanel = null, statsPanel = null;

    // Text of the button that serves to toggle the display
    [SerializeField]
    Text toggleText = null;
    
    // Text labels for each stat value
    [SerializeField]
    Text strengthText = null, defenseText = null, magicText = null, resistanceText = null,
        staminaText = null, agilityText = null,
        hitPercentText = null, evadePercentText = null, critChancePercentText = null,
        weaponText = null, weaponStatsText = null, helmText = null, helmStatsText = null,
        armorText = null, armorStatsText = null, glovesText = null, glovesStatsText = null,
        beltText = null, beltStatsText = null, bootsText = null, bootsStatsText = null;

    // Image objects for each of the equipment items, for setting sprites
    [SerializeField]
    Image weaponImage = null, helmImage = null, armorImage = null,
        glovesImage = null, beltImage = null, bootsImage = null;

    // which of two displays is showing
    bool showingEquipment = true;

    // Inventory of hero's equipment
    HeroEquipment equipment;
    #endregion

    #region Methods
    // Called by ManagePartyMenu during creation, or by a panel, doing the same.
    // Creates the displays, given the id of the selected hero
    public override void SetID(int id, GameObject parentGameObject)
    {
        // Set the id and HP/MP info and assign the caller of the menu
        base.SetID(id, parentGameObject);

        // Load and display all
        RefreshHeroStats();
        RefreshEquipped();
    }

    // Load all battle stats
    public void RefreshHeroStats()
    {
        BattleStats stats = BattleLoader.Party.Hero[ID].BStats;

        strengthText.text = stats.Strength.ToString();
        defenseText.text = stats.Defense.ToString();
        magicText.text = stats.Magic.ToString();
        resistanceText.text = stats.Resistance.ToString();
        staminaText.text = stats.Stamina.ToString();
        agilityText.text = stats.Agility.ToString();
        hitPercentText.text = stats.HitPercent.ToString();
        evadePercentText.text = stats.EvadePercent.ToString();
        critChancePercentText.text = stats.CritChance.ToString();
    }

    // Refresh each of the 6 slots of equipment
    public void RefreshEquipped()
    {
        // Retrieve hero's personal inventory of equipped items
        equipment = BattleLoader.Party.Hero[ID].Equipment;

        // If the slot property returns null, nothing is equipped and the 
        // display should be left blank, as it is at instantiation

        // If a slot property returns a non-null InvNames field, retrieve the
        // InvEqItem stored in the hero's inventory and set the display
        if (equipment.Weapon != null)
        {
            InvEqItem weapon = (InvEqItem)(equipment.GetItem((InvNames)equipment.Weapon));
            weaponText.text = weapon.FullName;
            weaponStatsText.text = CreateSummary(weapon.BStats);
            weaponImage.sprite = weapon.Sprite;
            weaponImage.enabled = true;
        }
        if (equipment.Helm != null)
        {
            InvEqItem helm = (InvEqItem)equipment.GetItem((InvNames)equipment.Helm);
            helmText.text = helm.FullName;
            helmStatsText.text = CreateSummary(helm.BStats);
            helmImage.sprite = helm.Sprite;
            helmImage.enabled = true;
        }
        if (equipment.Armor != null)
        {
            InvEqItem armor = (InvEqItem)equipment.GetItem((InvNames)equipment.Armor);
            armorText.text = armor.FullName;
            armorStatsText.text = CreateSummary(armor.BStats);
            armorImage.sprite = armor.Sprite;
            armorImage.enabled = true;
        }
        if (equipment.Gloves != null)
        {
            InvEqItem gloves = (InvEqItem)equipment.GetItem((InvNames)equipment.Gloves);
            glovesText.text = gloves.FullName;
            glovesStatsText.text = CreateSummary(gloves.BStats);
            glovesImage.sprite = gloves.Sprite;
            glovesImage.enabled = true;
        }
        if (equipment.Belt != null)
        {
            InvEqItem belt = (InvEqItem)equipment.GetItem((InvNames)equipment.Belt);
            beltText.text = belt.FullName;
            beltStatsText.text = CreateSummary(belt.BStats);
            beltImage.sprite = belt.Sprite;
            beltImage.enabled = true;
        }
        if (equipment.Boots != null)
        {
            InvEqItem boots = (InvEqItem)equipment.GetItem((InvNames)equipment.Boots);
            bootsText.text = boots.FullName;
            bootsStatsText.text = CreateSummary(boots.BStats);
            bootsImage.sprite = boots.Sprite;
            bootsImage.enabled = true;
        }
        
        // Refresh HP/MP, from PartyMenuPanel
        RefreshPanel();
    }

    // Creates a string summary of all the stats a piece of equipment confers
    string CreateSummary(BattleStats stats)
    {
        string format = "+#;-#;(0)";
        string output = "";
        output += stats.BaseHPMax != 0 ? "HP " + stats.BaseHPMax.ToString(format) + " " : "";
        output += stats.BaseMPMax != 0 ? "MP " + stats.BaseMPMax.ToString(format) + " " : "";
        output += stats.Strength != 0 ? "Str " + stats.Strength.ToString(format) + " " : "";
        output += stats.Defense != 0 ? "Def " + stats.Defense.ToString(format) + " " : "";
        output += stats.Magic != 0 ? "Mag " + stats.Magic.ToString(format) + " " : "";
        output += stats.Resistance != 0 ? "Res " + stats.Resistance.ToString(format) + " " : "";
        output += stats.Stamina != 0 ? "Sta " + stats.Stamina.ToString(format) + " " : "";
        output += stats.Agility != 0 ? "Agi " + stats.Agility.ToString(format) + " " : "";
        output += stats.BaseHit != 0 ? "Hit " + stats.BaseHit.ToString(format) + " " : "";
        output += stats.BaseEvade != 0 ? "Eva " + stats.BaseEvade.ToString(format) + " " : "";
        output += stats.CritChance != 0 ? "Crit " + stats.CritChance.ToString(format) : "";
        return output;


    }

    #region Equipment Click Methods
    // Each of these contains a method called by a On_Click() method of an invisble button,
    // laid over the display of a piece of weapon or armor. It launches the Equip Menu,
    // so that the weapon or armor can be replaced by another.
    public void Click_Weapon()
    {
        AudioManager.Chirp();
        GameObject equipSelectMenu = GameObject.Instantiate(
            Resources.Load<GameObject>(@"MenuPrefabs\prefabEquipMenu"));
        equipSelectMenu.GetComponent<EquipMenuMonitor>().SetHero(ID, EquipSlots.Weapon, this.gameObject);
    }

    public void Click_Helm()
    {
        AudioManager.Chirp();
        GameObject equipSelectMenu = GameObject.Instantiate(
            Resources.Load<GameObject>(@"MenuPrefabs\prefabEquipMenu"));
        equipSelectMenu.GetComponent<EquipMenuMonitor>().SetHero(ID, EquipSlots.Helm, this.gameObject);
    }

    public void Click_Armor()
    {
        AudioManager.Chirp();
        GameObject equipSelectMenu = GameObject.Instantiate(
            Resources.Load<GameObject>(@"MenuPrefabs\prefabEquipMenu"));
        equipSelectMenu.GetComponent<EquipMenuMonitor>().SetHero(ID, EquipSlots.Armor, this.gameObject);
    }

    public void Click_Gloves()
    {
        AudioManager.Chirp();
        GameObject equipSelectMenu = GameObject.Instantiate(
            Resources.Load<GameObject>(@"MenuPrefabs\prefabEquipMenu"));
        equipSelectMenu.GetComponent<EquipMenuMonitor>().SetHero(ID, EquipSlots.Gloves, this.gameObject);
    }

    public void Click_Belt()
    {
        AudioManager.Chirp();
        GameObject equipSelectMenu = GameObject.Instantiate(
            Resources.Load<GameObject>(@"MenuPrefabs\prefabEquipMenu"));
        equipSelectMenu.GetComponent<EquipMenuMonitor>().SetHero(ID, EquipSlots.Belt, this.gameObject);
    }

    public void Click_Boots()
    {
        AudioManager.Chirp();
        GameObject equipSelectMenu = GameObject.Instantiate(
            Resources.Load<GameObject>(@"MenuPrefabs\prefabEquipMenu"));
        equipSelectMenu.GetComponent<EquipMenuMonitor>().SetHero(ID, EquipSlots.Boots, this.gameObject);
    }
    #endregion Equipment Click Methods
    
    // Swaps the display between the current equipment and the battle stats of the selected hero.
    // Called by the ToggleButton On_Click()
    public void Click_Toggle()
    {
        AudioManager.Chirp();
        showingEquipment = !showingEquipment;
        equipmentPanel.SetActive(showingEquipment);
        statsPanel.SetActive(!showingEquipment);
        toggleText.text = showingEquipment ? "Statistics" : "Equipment";
    }

    // Opens an inventory menu of all items in the party stash. Consumables can be used on the 
    // current character this way.
    public void Click_Inventory()
    {
        AudioManager.Chirp();
        GameObject inventoryMenu = GameObject.Instantiate(
            Resources.Load<GameObject>(@"MenuPrefabs\prefabInventoryMenu"));
        EquipSlots[] slotArray = (EquipSlots[])Enum.GetValues(typeof(EquipSlots));
        List<EquipSlots> slots = new List<EquipSlots> (slotArray);
        inventoryMenu.GetComponent<InventoryMenuMonitor>().SetInventory(ID, slots, this.gameObject);
    }

    // Closes the HeroStatsMenu and reactivates the prior menu. Called by the CloserButton On_Click()
    public void Click_Close()
    {
        AudioManager.Close();
        Destroy(gameObject);

        // Refresh all panels of the ManagePartyMenu, in case of changes made
        PartyMenuPanel[] panels = parentMenuObject.GetComponentsInChildren<PartyMenuPanel>();
        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].RefreshPanel();
        }
        
        parentMenuObject.SetActive(true);
    }

    // Cheat accessed through a cheatbutton. Turns the current hero into a super hero
    // by adding obscenely high values to its BattleStats.
    public void Click_Cheat_SuperHuman()
    {
        BattleStats stats = new BattleStats();
        int[] bonuses = new int[] { 2000, 2000, 500, 500, 500, 500,
            50, 50, 50, 50, 50, 0, 0 };
        stats.Import(bonuses);
        BattleLoader.Party.Hero[ID].BStats.Equip(stats);
        BattleLoader.Party.Hero[ID].HealAll();
        RefreshHeroStats();
    }
    #endregion
}
