using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroStatsMenuMonitor : PartyMenuPanel
{
    [SerializeField]
    GameObject equipmentPanel = null, statsPanel = null;

    [SerializeField]
    Text toggleText = null;
    
    [SerializeField]
    Text strengthText = null, defenseText = null, magicText = null, resistanceText = null,
        staminaText = null, agilityText = null,
        hitPercentText = null, evadePercentText = null, critChancePercentText = null,
        weaponText = null, weaponStatsText = null, helmText = null, helmStatsText = null,
        armorText = null, armorStatsText = null, glovesText = null, glovesStatsText = null,
        beltText = null, beltStatsText = null, bootsText = null, bootsStatsText = null;

    [SerializeField]
    Image weaponImage = null, helmImage = null, armorImage = null,
        glovesImage = null, beltImage = null, bootsImage = null;

    bool showingEquipment = true;

    HeroEquipment equipment;

    public override void SetID(int id, GameObject parentGameObject)
    {
        base.SetID(id, parentGameObject);

        RefreshHeroStats();
        RefreshEquipped();

    }

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

    public void RefreshEquipped()
    {
        equipment = BattleLoader.Party.Hero[ID].Equipment;

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

        RefreshPanel();
    }

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

    public void Click_Toggle()
    {
        AudioManager.Chirp();
        showingEquipment = !showingEquipment;
        equipmentPanel.SetActive(showingEquipment);
        statsPanel.SetActive(!showingEquipment);
        toggleText.text = showingEquipment ? "Statistics" : "Equipment";

    }

    public void Click_Inventory()
    {
        AudioManager.Chirp();
        GameObject inventoryMenu = GameObject.Instantiate(
            Resources.Load<GameObject>(@"MenuPrefabs\prefabInventoryMenu"));
        EquipSlots[] slotArray = (EquipSlots[])Enum.GetValues(typeof(EquipSlots));
        List<EquipSlots> slots = new List<EquipSlots> (slotArray);
        inventoryMenu.GetComponent<InventoryMenuMonitor>().SetInventory(ID, slots, this.gameObject);
    }

    public void Click_Close()
    {
        AudioManager.Close();
        Destroy(gameObject);
        PartyMenuPanel[] panels = parentMenuObject.GetComponentsInChildren<PartyMenuPanel>();
        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].RefreshPanel();
        }
        parentMenuObject.SetActive(true);
    }

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
}
