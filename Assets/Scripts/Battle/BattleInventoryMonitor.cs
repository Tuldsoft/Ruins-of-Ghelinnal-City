using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

/// <summary>
/// Attached to prefabBattleInventory menu prefab.
/// </summary>
public class BattleInventoryMonitor : MonoBehaviour
{
    #region Fields
    [SerializeField]
    GameObject gridContent = null;
    [SerializeField]
    Text goldText = null, useButton = null, messageText = null;
    
    // reference to HeroParty's Inventory
    Inventory partyStash;
    // index of highlighted item, null if none are selected
    int? selection = null;

    // timed message handler, used while displaying "You are at full..."
    EventTimer messageTimer;
    const float messageTimerDuration = 1.5f;
    string previousMessage = "";

    // turns UI elements off when a choice is made
    UIEnabler uiEnabler;

    // turnOver component, used when the menu closes
    TurnInvoker turnInvoker;
    #endregion

    #region Monobehavior Methods
    // Start is called before the first frame update
    private void Start()
    {
        // Make the menu capable of invoking the end of a turn
        turnInvoker = gameObject.AddComponent<TurnInvoker>();
        
        EventManager.AddListener_Battle_InvSelect(MakeSelection);

        // for displaying "You are at full."
        messageTimer = gameObject.AddComponent<EventTimer>();
        messageTimer.Duration = messageTimerDuration;
        messageTimer.AddListener_Finished(ResetMessage);

        // show gold
        goldText.text = BattleLoader.Party.Gold.ToString();
        // cannot use until a selection is made
        useButton.GetComponent<Button>().interactable = false;

        // retrieve inventory from HeroParty
        partyStash = BattleLoader.Party.Inventory;

        messageText.text = "";

        // Only used for invoking disable. No button.
        uiEnabler = gameObject.AddComponent<UIEnabler>();

        PopulateGrid();
    }
    #endregion

    #region Methods
    /// <summary>
    /// Populates the usable inventory
    /// </summary>
    void PopulateGrid()
    {
        // Create a grid item
        GameObject newPanel;
        GameObject prefabPanel = Resources.Load<GameObject>(@"MenuPrefabs\prefabBattleInvPanel");

        foreach (InvItem item in partyStash.Contents)
        {
            // Only populate potions
            if (item.Type == InvType.Potion)
            {
                // Create instances of the prefab, with the Content panel as its parent
                newPanel = GameObject.Instantiate(prefabPanel, gridContent.transform);

                BattleInvPanel battleInvPanel = newPanel.GetComponent<BattleInvPanel>();

                // Load item into the panel
                battleInvPanel.SetIndex(partyStash.IndexOfItem(item.Name), item.FullName,
                    item.Description, item.Sprite, item.Quantity);
            }
        }

    }

    /// <summary>
    /// Event triggered when a user makes a selection
    /// </summary>
    /// <param name="index">Index of panel/inventory. Null when called during reset.</param>
    /// <param name="message">Message that appears when selection is attempted.</param>
    void MakeSelection(int? index, string message)
    {
        // if null, ignore
        if (index != null)
        {
            // if there is an error message displaying, stop the event timer here
            // before changing the message.
            if (messageTimer.Running) { messageTimer.Stop(); }

            if (selection == index)
            {
                // deselect
                selection = null;
                useButton.GetComponent<Button>().interactable = false;
                useButton.color = Color.gray;

                messageText.text = "";
            }
            else
            {
                // set new selection with message
                selection = index;
                useButton.GetComponent<Button>().interactable = true;
                useButton.color = Color.white;
                
                messageText.text = message;
            }
        }
        else
        {
            // skip it because it is null (sent through other mechanic like during a reset)
        }

    }

    // Returns message to previous.
    void ResetMessage()
    {
        messageText.text = previousMessage;
    }

    // Uses the selected item
    public void Click_UseButton()
    {
        if (selection == null) { return; }

        AudioManager.PlaySound(AudioClipName.UsePotion);

        // reference to hero and item
        InvItem item = partyStash.Contents[(int)selection];
        BattleHero hero = BattleLoader.Party.Hero[BattleMath.ConvertHeroID(TurnCounter.CurrentID)];

        // check for full hp/mp, display message and do not use.
        int hp = hero.HP;
        int hpMax = hero.HPMax;
        if (hp == hpMax && item.Type == InvType.Potion && item.Subtype == InvSubtype.Health)
        {
            previousMessage = messageText.text;
            messageText.text = "You are already at full health.";
            messageTimer.Run();
            return;
        }
        int mp = hero.MP;
        int mpMax = hero.MPMax;
        if (mp == mpMax && item.Type == InvType.Potion && item.Subtype == InvSubtype.Mana)
        {
            previousMessage = messageText.text;
            messageText.text = "You are already at full mana.";
            messageTimer.Run();
            return;
        }

        // create negative damage for restorative item
        Damage damage = new Damage();

        switch (item.Type)
        {
            case InvType.Potion:

                int healing;
                switch (item.Subtype)
                {
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
                        
                        // remove item from stash and queue healing in Damage
                        partyStash.RemoveInvItem(item.Name, 1);
                        damage.Add(-healing, false, true, false); // amount, not crit, isItem, not MP
                        break;

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

                        // remove item from stash and queue healing in Damage
                        partyStash.RemoveInvItem(item.Name, 1);
                        damage.Add(-healing, false, true, true); // amount, not crit, isItem, is MP
                        break;
                }
                break;

            default:
                damage.Add(null);
                break;
        }
        // disable ui once a choice is made
        uiEnabler.EnableUI(false);

        // Invoke a Player End Turn event
        // Use this event instead of triggering off a fight collision with enemy
        // TurnOver applies the damage queued in Damage
        turnInvoker.TurnOver(damage, TurnCounter.CurrentID);
        
        // Exit menu
        Destroy(gameObject);
    }

    // Exit menu without ending turn
    public void Click_CloseButton()
    {
        AudioManager.Close();
        Destroy(gameObject);
    }
    #endregion
}
