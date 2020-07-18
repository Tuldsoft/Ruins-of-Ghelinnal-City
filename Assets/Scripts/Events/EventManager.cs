using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// The EventManager is a system that utilizes Unity Events. It stores all invokers and
/// listeners for every event, and cross registers each with each. Doing this allows objects
/// to call a method without knowing who the recipient of the message will be.
/// 
/// Rather than splitting this class by fields, properties, and methods, it is grouped by
/// the events. Each event is handled with a predictable template, with one or more
/// invokers, and one or more listeners.
/// 
/// 
/// 
/// REFACTOR: Make a generic, annonymous event handler template, and make this static.
/// </summary>
public class EventManager
{
    #region Battle_TurnOver
    // Battle: Tells the BattleManager that the end of a turn has arrived.

    // invokers: TurnInvoker
    static List<TurnInvoker> invokers_Battle_TurnOver = new List<TurnInvoker>();
    // listener: BattleManager
    static UnityAction<Damage, int> listener_Battle_TurnOver;

    public static void AddInvoker_Battle_TurnOver(TurnInvoker turnScript)
    {
        invokers_Battle_TurnOver.Add(turnScript);
        if (listener_Battle_TurnOver != null)
        //foreach (UnityAction<int> listener in listeners_ChoiceSelection)
        {
            turnScript.AddListener_Battle_TurnOver(listener_Battle_TurnOver);
        }
    }

    public static void AddListener_Battle_TurnOver(UnityAction<Damage, int> listener)
    {
        listener_Battle_TurnOver = listener;
        // if (invokers_SpeedUpBrick != null)
        foreach (TurnInvoker turnScript in invokers_Battle_TurnOver)
        {
            turnScript.AddListener_Battle_TurnOver(listener);
        }
    }
    #endregion

    #region Battle_ChoiceSelectionEvent
    // Battle: Sets or unsets an enemy as the intended target

    // invokers: EnemyChoice (menu and sprites)
    static List<EnemyChoice> invokers_Battle_ChoiceSelection = new List<EnemyChoice>();
    // listeners: EnemyChoice (menu and sprites), BattleManager, where int = choice
    static List<UnityAction<int>> listeners_Battle_ChoiceSelection = new List<UnityAction<int>>();

    public static void AddInvoker_Battle_ChoiceSelection(EnemyChoice choiceScript)
    {
        invokers_Battle_ChoiceSelection.Add(choiceScript);
        //if (listeners_SpeedUpFX != null)
        foreach (UnityAction<int> listener in listeners_Battle_ChoiceSelection)
        {
            choiceScript.AddListener_ChoiceSelection(listener);
        }
    }

    public static void AddListener_Battle_ChoiceSelection(UnityAction<int> listener)
    {
        listeners_Battle_ChoiceSelection.Add(listener);
        // if (invokers_SpeedUpBrick != null)
        foreach (EnemyChoice choiceScript in invokers_Battle_ChoiceSelection)
        {
            choiceScript.AddListener_ChoiceSelection(listener);
        }
    }

    #endregion

    #region Battle_FightChoice
    // Battle: The user choses who to target with Fight

    // invokers: FightChoice (on menu option and all enemies if clicked while enabled
    static List<FightChoice> invokers_FightChoice = new List<FightChoice>();
    // listeners: BattleManager (others?)
    static List<UnityAction> listeners_FightChoice = new List<UnityAction>();

    public static void AddInvoker_Battle_FightChoice(FightChoice choiceScript)
    {
        invokers_FightChoice.Add(choiceScript);
        //if (listeners_SpeedUpFX != null)
        foreach (UnityAction listener in listeners_FightChoice)
        {
            choiceScript.AddListener_FightChoice(listener);
        }
    }

    public static void AddListener_Battle_FightChoice(UnityAction listener)
    {
        listeners_FightChoice.Add(listener);
        // if (invokers_SpeedUpBrick != null)
        foreach (FightChoice choiceScript in invokers_FightChoice)
        {
            choiceScript.AddListener_FightChoice(listener);
        }
    }
    #endregion

    #region Battle_PropelHitTarget
    // Battle: A propelled object has hit its intended target

    // invokers: PropelObject (int targetNum) (-1 hero, 0+ enemy)
    static List<PropelObject> invokers_Battle_PropelHitTarget = new List<PropelObject>();
    // listeners: BattleManager (others?)
    static List<UnityAction<int, int, BattleMode>> listeners_Battle_PropelHitTarget = new List<UnityAction<int, int, BattleMode>>();

    public static void AddInvoker_Battle_PropelHitTarget(PropelObject propelScript)
    {
        invokers_Battle_PropelHitTarget.Add(propelScript);
        //if (listeners_SpeedUpFX != null)
        foreach (UnityAction<int, int, BattleMode> listener in listeners_Battle_PropelHitTarget)
        {
            propelScript.AddListener_Battle_PropelHitTarget(listener);
        }
    }

    public static void AddListener_Battle_PropelHitTarget(UnityAction<int, int, BattleMode> listener)
    {
        listeners_Battle_PropelHitTarget.Add(listener);
        // if (invokers_SpeedUpBrick != null)
        foreach (PropelObject propelScript in invokers_Battle_PropelHitTarget)
        {
            propelScript.AddListener_Battle_PropelHitTarget(listener);
        }
    }
    #endregion

    #region Battle_DisplayDamage
    // Battle: Initiates a floating damage display

    // invokers: BattleManager
    static List<BattleManager> invokers_Battle_DisplayDamage = new List<BattleManager>();
    // listeners: DamageDisplays, BattleEnemys, BattleHero?
    static List<UnityAction<int, Damage>> listeners_Battle_DisplayDamage = new List<UnityAction<int, Damage>>();

    public static void AddInvoker_Battle_DisplayDamage(BattleManager managerScript)
    {
        invokers_Battle_DisplayDamage.Add(managerScript);
        //if (listeners_SpeedUpFX != null)
        foreach (UnityAction<int,Damage> listener in listeners_Battle_DisplayDamage)
        {
            managerScript.AddListener_Battle_DisplayDamage(listener);
        }
    }

    public static void AddListener_Battle_DisplayDamage(UnityAction<int,Damage> listener)
    {
        listeners_Battle_DisplayDamage.Add(listener);
        // if (invokers_SpeedUpBrick != null)
        foreach (BattleManager managerScript in invokers_Battle_DisplayDamage)
        {
            managerScript.AddListener_Battle_DisplayDamage(listener);
        }
    }
    #endregion

    #region Battle_ApplyDamage
    // Battle: Change the HP or MP value of a hero or enemy

    // invokers: DamageDisplays
    static List<DamageDisplay> invokers_Battle_ApplyDamage = new List<DamageDisplay>();
    // listeners: BattleManager
    static List<UnityAction<int, int?, int?>> listeners_Battle_ApplyDamage = new List<UnityAction<int, int?, int?>>();

    public static void AddInvoker_Battle_ApplyDamage(DamageDisplay displayScript)
    {
        invokers_Battle_ApplyDamage.Add(displayScript);
        //if (listeners_SpeedUpFX != null)
        foreach (UnityAction<int, int?, int?> listener in listeners_Battle_ApplyDamage)
        {
            displayScript.AddListener_Battle_ApplyDamage(listener);
        }
    }

    public static void AddListener_Battle_ApplyDamage(UnityAction<int, int?, int?> listener)
    {
        listeners_Battle_ApplyDamage.Add(listener);
        // if (invokers_SpeedUpBrick != null)
        foreach (DamageDisplay displayScript in invokers_Battle_ApplyDamage)
        {
            displayScript.AddListener_Battle_ApplyDamage(listener);
        }
    }
    #endregion

    #region Battle_RefreshHPMPSliders
    // Battle: A call to update a display of HP and MP Sliders, either enemy or hero
    // battleid, hpslider reduction, mpslider reduction

    // invokers: BattleManager
    static List<BattleManager> invokers_Battle_RefreshHPMPSliders = new List<BattleManager>();
    // listeners: DamageDisplays, BattleEnemys, BattleHero?
    static List<UnityAction<int, int, int>> listeners_Battle_RefreshHPMPSliders = new List<UnityAction<int, int, int>>();

    public static void AddInvoker_Battle_RefreshHPMPSliders(BattleManager managerScript)
    {
        invokers_Battle_RefreshHPMPSliders.Add(managerScript);
        //if (listeners_SpeedUpFX != null)
        foreach (UnityAction<int, int, int> listener in listeners_Battle_RefreshHPMPSliders)
        {
            managerScript.AddListener_Battle_RefreshHPMPSliders(listener);
        }
    }

    public static void AddListener_Battle_RefreshHPMPSliders(UnityAction<int, int, int> listener)
    {
        listeners_Battle_RefreshHPMPSliders.Add(listener);
        // if (invokers_SpeedUpBrick != null)
        foreach (BattleManager managerScript in invokers_Battle_RefreshHPMPSliders)
        {
            managerScript.AddListener_Battle_RefreshHPMPSliders(listener);
        }
    }

    #endregion

    #region Battle_RefreshMPCosts
    // Battle: When opening the Magic SubMenu, show spell MP costs and disable if necessary

    // invokers: BattleManager
    static List<BattleManager> invokers_Battle_RefreshMPCosts = new List<BattleManager>();
    // listeners: DamageDisplays, BattleEnemys, BattleHero?
    static List<UnityAction<int>> listeners_Battle_RefreshMPCosts = new List<UnityAction<int>>();

    public static void AddInvoker_Battle_RefreshMPCosts(BattleManager managerScript)
    {
        invokers_Battle_RefreshMPCosts.Add(managerScript);
        //if (listeners_SpeedUpFX != null)
        foreach (UnityAction<int> listener in listeners_Battle_RefreshMPCosts)
        {
            managerScript.AddListener_Battle_RefreshMPCosts(listener);
        }
    }

    public static void AddListener_Battle_RefreshMPCosts(UnityAction<int> listener)
    {
        listeners_Battle_RefreshMPCosts.Add(listener);
        // if (invokers_SpeedUpBrick != null)
        foreach (BattleManager managerScript in invokers_Battle_RefreshMPCosts)
        {
            managerScript.AddListener_Battle_RefreshMPCosts(listener);
        }
    }

    #endregion Battle_RefreshMPCosts

    /*#region Battle_DisplayDamageEnd (Unused)
    // invokers: DamageDisplays
    static List<DamageDisplay> invokers_Battle_DisplayDamageEnd = new List<DamageDisplay>();
    // listeners: BattleManager
    static List<UnityAction<int>> listeners_Battle_DisplayDamageEnd = new List<UnityAction<int>>();

    public static void AddInvoker_Battle_DisplayDamageEnd(DamageDisplay displayScript)
    {
        invokers_Battle_DisplayDamageEnd.Add(displayScript);
        //if (listeners_SpeedUpFX != null)
        foreach (UnityAction<int> listener in listeners_Battle_DisplayDamageEnd)
        {
            displayScript.AddListener_Battle_DisplayDamageEnd(listener);
        }
    }

    public static void AddListener_Battle_DisplayDamageEnd(UnityAction<int> listener)
    {
        listeners_Battle_DisplayDamageEnd.Add(listener);
        // if (invokers_SpeedUpBrick != null)
        foreach (DamageDisplay displayScript in invokers_Battle_DisplayDamageEnd)
        {
            displayScript.AddListener_Battle_DisplayDamageEnd(listener);
        }
    }
    #endregion*/

    #region Battle_EnemyDeathBegin
    // Battle: When an enemy reach 0 HP, signals the start of an enemy's death fadeout

    // invokers: BattleManager
    static List<BattleManager> invokers_Battle_EnemyDeathBegin = new List<BattleManager>();
    // listeners: EnemyDeath
    static List<UnityAction<int, bool>> listeners_Battle_EnemyDeathBegin = new List<UnityAction<int, bool>>();

    public static void AddInvoker_Battle_EnemyDeathBegin(BattleManager managerScript)
    {
        invokers_Battle_EnemyDeathBegin.Add(managerScript);
        //if (listeners_SpeedUpFX != null)
        foreach (UnityAction<int, bool> listener in listeners_Battle_EnemyDeathBegin)
        {
            managerScript.AddListener_Battle_EnemyDeathBegin(listener);
        }
    }

    public static void AddListener_Battle_EnemyDeathBegin(UnityAction<int, bool> listener)
    {
        listeners_Battle_EnemyDeathBegin.Add(listener);
        // if (invokers_SpeedUpBrick != null)
        foreach (BattleManager managerScript in invokers_Battle_EnemyDeathBegin)
        {
            managerScript.AddListener_Battle_EnemyDeathBegin(listener);
        }
    }
    #endregion

    #region Battle_EnemyDeathEnd
    // Battle: When an enemy has faded out completely from death, remove it from the scene

    // invokers: EneyDeath
    static List<EnemyDeath> invokers_Battle_EnemyDeathEnd = new List<EnemyDeath>();
    // listeners: BattleManager
    static List<UnityAction<int>> listeners_Battle_EnemyDeathEnd = new List<UnityAction<int>>();

    public static void AddInvoker_Battle_EnemyDeathEnd(EnemyDeath deathScript)
    {
        invokers_Battle_EnemyDeathEnd.Add(deathScript);
        //if (listeners_SpeedUpFX != null)
        foreach (UnityAction<int> listener in listeners_Battle_EnemyDeathEnd)
        {
            deathScript.AddListener_Battle_EnemyDeathEnd(listener);
        }
    }

    public static void AddListener_Battle_EnemyDeathEnd(UnityAction<int> listener)
    {
        listeners_Battle_EnemyDeathEnd.Add(listener);
        // if (invokers_SpeedUpBrick != null)
        foreach (EnemyDeath deathScript in invokers_Battle_EnemyDeathEnd)
        {
            deathScript.AddListener_Battle_EnemyDeathEnd(listener);
        }
    }
    #endregion

    #region Battle_AddCoins
    // Battle: Adds coins to the bank when the coin arrives.

    // invokers: CoinSpawn
    static List<CoinSpawn> invokers_Battle_AddCoins = new List<CoinSpawn>();
    // listener: BattleManager
    static UnityAction<int> listener_Battle_AddCoins;

    public static void AddInvoker_Battle_AddCoins(CoinSpawn coinScript)
    {
        invokers_Battle_AddCoins.Add(coinScript);
        if (listener_Battle_AddCoins != null)
        //foreach (UnityAction<int> listener in listeners_Battle_EnemyDeathEnd)
        {
            coinScript.AddListener_Battle_AddCoins(listener_Battle_AddCoins);
        }
    }

    public static void AddListener_Battle_AddCoins (UnityAction<int> listener)
    {
        listener_Battle_AddCoins = listener;
        // if (invokers_SpeedUpBrick != null)
        foreach (CoinSpawn coinScript in invokers_Battle_AddCoins)
        {
            coinScript.AddListener_Battle_AddCoins(listener);
        }
    }
    #endregion

    #region Shop_AdjustPurchaseAmount
    // Shop: The user has adjusted the quantity of a desired item. Update the display.

    // invokers: ShopPanel
    static List<ShopPanel> invokers_Shop_AdjustPurchaseAmount = new List<ShopPanel>();
    // listener: ShopMenuMonitor
    static UnityAction<int> listener_Shop_AdjustPurchaseAmount;

    public static void AddInvoker_Shop_AdjustPurchaseAmount(ShopPanel panelScript)
    {
        invokers_Shop_AdjustPurchaseAmount.Add(panelScript);
        if (listener_Shop_AdjustPurchaseAmount != null)
        //foreach (UnityAction<int> listener in listeners_Battle_EnemyDeathEnd)
        {
            panelScript.AddListener_Shop_AdjustPurchaseAmount(listener_Shop_AdjustPurchaseAmount);
        }
    }

    public static void AddListener_Shop_AdjustPurchaseAmount(UnityAction<int> listener)
    {
        listener_Shop_AdjustPurchaseAmount = listener;
        // if (invokers_SpeedUpBrick != null)
        foreach (ShopPanel panelScript in invokers_Shop_AdjustPurchaseAmount)
        {
            panelScript.AddListener_Shop_AdjustPurchaseAmount(listener);
        }
    }
    #endregion

    #region Shop_MakePurchase
    // Shop: The user has pressed "Buy". Add the items from the cart to the party inventory and deduct gold

    // invoker: ShopMonitor
    static ShopMonitor invoker_Shop_MakePurchase;
    // listeners: ShopPanels
    static List<UnityAction<Dictionary<InvNames, int>>> listeners_Shop_MakePurchase = new List<UnityAction<Dictionary<InvNames, int>>>();

    public static void AddInvoker_Shop_MakePurchase(ShopMonitor monitorScript)
    {
        invoker_Shop_MakePurchase = monitorScript;
        //if (listeners_MakePurchase != null)
        foreach (UnityAction<Dictionary<InvNames, int>> listener in listeners_Shop_MakePurchase)
        {
            monitorScript.AddListener_Shop_MakePurchase(listener);
        }
    }

    public static void AddListener_Shop_MakePurchase(UnityAction<Dictionary<InvNames, int>> listener)
    {
        listeners_Shop_MakePurchase.Add(listener);
        if (invoker_Shop_MakePurchase != null)
        //foreach (ShopPanel panelScript in invoker_Shop_MakePurchase)
        {
            invoker_Shop_MakePurchase.AddListener_Shop_MakePurchase(listener);
        }
    }
    #endregion

    #region Battle_InvSelect
    // Battle: The user selects or deselects an item in the battle inventory menu

    // invokers: BattleInvPanel
    static List<BattleInvPanel> invokers_Battle_InvSelect = new List<BattleInvPanel>();
    // listeners: BattleInvPanels, BattleInventoryManager
    static List<UnityAction<int?,string>> listeners_Battle_InvSelect = new List<UnityAction<int?,string>>();

    public static void AddInvoker_Battle_InvSelect(BattleInvPanel panelScript)
    {
        invokers_Battle_InvSelect.Add(panelScript);
        //if (listener_Battle_InvSelect != null)
        foreach (UnityAction<int?,string> listener in listeners_Battle_InvSelect)
        {
            panelScript.AddListener_Battle_InvSelect(listener);
        }
    }

    public static void AddListener_Battle_InvSelect(UnityAction<int?,string> listener)
    {
        listeners_Battle_InvSelect.Add(listener);
        // if (invokers_SpeedUpBrick != null)
        foreach (BattleInvPanel panelScript in invokers_Battle_InvSelect)
        {
            panelScript.AddListener_Battle_InvSelect(listener);
        }
    }
    #endregion

    #region Battle_SubMenuSelection
    // Battle: One of the various SubMenus is opened.

    // invokers: BattleSubMenu
    static List<BattleSubMenu> invokers_Battle_SubMenuSelection = new List<BattleSubMenu>();
    // listeners: BattleManager
    static List<UnityAction<BattleMode>> listeners_Battle_SubMenuSelection = new List<UnityAction<BattleMode>>();

    public static void AddInvoker_Battle_SubMenuSelection(BattleSubMenu panelScript)
    {
        invokers_Battle_SubMenuSelection.Add(panelScript);
        //if (listener_Battle_InvSelect != null)
        foreach (UnityAction<BattleMode> listener in listeners_Battle_SubMenuSelection)
        {
            panelScript.AddListener_Battle_SubMenuSelection(listener);
        }
    }

    public static void AddListener_Battle_SubMenuSelection(UnityAction<BattleMode> listener)
    {
        listeners_Battle_SubMenuSelection.Add(listener);
        // if (invokers_SpeedUpBrick != null)
        foreach (BattleSubMenu panelScript in invokers_Battle_SubMenuSelection)
        {
            panelScript.AddListener_Battle_SubMenuSelection(listener);
        }
    }
    #endregion

    #region Battle_UIEnabler
    // Enables or disables UI elements, for example, during an enemy turn

    // invokers: UIEnabler
    static List<UIEnabler> invokers_Battle_UIEnabler = new List<UIEnabler>();
    // listeners: UIEnabler
    static List<UnityAction<bool, HeroType>> listeners_Battle_UIEnabler = new List<UnityAction<bool, HeroType>>();

    public static void AddInvoker_Battle_UIEnabler(UIEnabler uiEnabler)
    {
        invokers_Battle_UIEnabler.Add(uiEnabler);
        //if (listener_Battle_InvSelect != null)
        foreach (UnityAction<bool, HeroType> listener in listeners_Battle_UIEnabler)
        {
            uiEnabler.AddListener_Battle_UIEnabler(listener);
        }
    }

    public static void AddListener_Battle_UIEnabler(UnityAction<bool, HeroType> listener)
    {
        listeners_Battle_UIEnabler.Add(listener);
        // if (invokers_SpeedUpBrick != null)
        foreach (UIEnabler uiEnabler in invokers_Battle_UIEnabler)
        {
            uiEnabler.AddListener_Battle_UIEnabler(listener);
        }
    }
    #endregion

    #region Equip_InvSelect
    // On the Equip menu, a panel has been selected or deselected. Updates the display

    // invokers: BattleInvPanel
    static List<EquipMenuPanel> invokers_Equip_InvSelect = new List<EquipMenuPanel>();
    // listeners: BattleInvPanels, BattleInventoryManager
    static List<UnityAction<int?>> listeners_Equip_InvSelect = new List<UnityAction<int?>>();

    public static void AddInvoker_Equip_InvSelect(EquipMenuPanel panelScript)
    {
        invokers_Equip_InvSelect.Add(panelScript);
        //if (listener_Battle_InvSelect != null)
        foreach (UnityAction<int?> listener in listeners_Equip_InvSelect)
        {
            panelScript.AddListener_Equip_InvSelect(listener);
        }
    }

    public static void AddListener_Equip_InvSelect(UnityAction<int?> listener)
    {
        listeners_Equip_InvSelect.Add(listener);
        // if (invokers_SpeedUpBrick != null)
        foreach (EquipMenuPanel panelScript in invokers_Equip_InvSelect)
        {
            panelScript.AddListener_Equip_InvSelect(listener);
        }
    }

    #endregion

    #region Inventory_Select
    // When looking at an inventory, an item is selected or deselected. Updates the display.
    // Index, display string

    // invokers: InventoryPanel
    static List<InventoryPanel> invokers_Inventory_Select = new List<InventoryPanel>();
    // listeners: InventoryPanels, InventoryMenuMonitor
    static List<UnityAction<int?, string>> listeners_Inventory_Select = new List<UnityAction<int?, string>>();

    public static void AddInvoker_Inventory_Select(InventoryPanel panelScript)
    {
        invokers_Inventory_Select.Add(panelScript);
        //if (listener_Battle_InvSelect != null)
        foreach (UnityAction<int?, string> listener in listeners_Inventory_Select)
        {
            panelScript.AddListener_Inventory_Select(listener);
        }
    }

    public static void AddListener_Inventory_Select(UnityAction<int?, string> listener)
    {
        listeners_Inventory_Select.Add(listener);
        // if (invokers_SpeedUpBrick != null)
        foreach (InventoryPanel panelScript in invokers_Inventory_Select)
        {
            panelScript.AddListener_Inventory_Select(listener);
        }
    }

    #endregion

    #region PanelSelect
    // A generic panel select/deselect event, such as the ManageParty menu.

    // invokers: PanelSelection
    static List<PanelSelection> invokers_PanelSelect = new List<PanelSelection>();
    // listeners: FileMenuMonitor, PanelSelections
    static List<UnityAction<int?>> listeners_PanelSelect = new List<UnityAction<int?>>();

    public static void AddInvoker_PanelSelect(PanelSelection panelScript)
    {
        invokers_PanelSelect.Add(panelScript);
        //if (listener_Battle_InvSelect != null)
        foreach (UnityAction<int?> listener in listeners_PanelSelect)
        {
            panelScript.AddListener_PanelSelect(listener);
        }
    }

    public static void AddListener_PanelSelect(UnityAction<int?> listener)
    {
        listeners_PanelSelect.Add(listener);
        // if (invokers_SpeedUpBrick != null)
        foreach (PanelSelection panelScript in invokers_PanelSelect)
        {
            panelScript.AddListener_PanelSelect(listener);
        }
    }
    #endregion

    #region Confirmation
    // Used for Confirmation dialogs

    // invokers: Confirmation
    static List<Confirmation> invokers_Confirmation = new List<Confirmation>();
    // listeners: FileMenuMonitors
    static List<UnityAction<bool>> listeners_Confirmation = new List<UnityAction<bool>>();

    public static void AddInvoker_Confirmation(Confirmation confirmScript)
    {
        invokers_Confirmation.Add(confirmScript);
        //if (listener_Battle_InvSelect != null)
        foreach (UnityAction<bool> listener in listeners_Confirmation)
        {
            confirmScript.AddListener_Confirmation(listener);
        }
    }

    public static void AddListener_Confirmation(UnityAction<bool> listener)
    {
        listeners_Confirmation.Add(listener);
        // if (invokers_SpeedUpBrick != null)
        foreach (Confirmation confirmScript in invokers_Confirmation)
        {
            confirmScript.AddListener_Confirmation(listener);
        }
    }
    #endregion

}
