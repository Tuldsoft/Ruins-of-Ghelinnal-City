using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class BattleInventoryMonitor : MonoBehaviour
{

    [SerializeField]
    GameObject gridContent = null;
    [SerializeField]
    Text goldText = null, useButton = null, messageText = null;
    

    Inventory partyStash;
    int? selection = null;

    // timed message handler
    EventTimer messageTimer;
    const float messageTimerDuration = 1.5f;
    string previousMessage = "";

    // turns UI elements off when a choice is made
    UIEnabler uiEnabler;

    // turnOver component
    TurnInvoker turnInvoker;

    private void Start()
    {
        turnInvoker = gameObject.AddComponent<TurnInvoker>();
        
        EventManager.AddListener_Battle_InvSelect(MakeSelection);

        messageTimer = gameObject.AddComponent<EventTimer>();
        messageTimer.Duration = messageTimerDuration;
        messageTimer.AddListener_Finished(ResetMessage);

        goldText.text = BattleLoader.Party.Gold.ToString();
        useButton.GetComponent<Button>().interactable = false;

        partyStash = BattleLoader.Party.Inventory;

        messageText.text = "";

        // adds to self, but without button. Only used for invoking disable.
        uiEnabler = gameObject.AddComponent<UIEnabler>();

        PopulateGrid();
    }


    void PopulateGrid()
    {
        // create a grid item
        GameObject newPanel;
        GameObject prefabPanel = Resources.Load<GameObject>(@"MenuPrefabs\prefabBattleInvPanel");

        foreach (InvItem item in partyStash.Contents)
        {
            // only populate potions
            if (item.Type == InvType.Potion)
            {
                // create instances of the prefab, with the Content panel as its parent
                newPanel = GameObject.Instantiate(prefabPanel, gridContent.transform);

                BattleInvPanel battleInvPanel = newPanel.GetComponent<BattleInvPanel>();

                battleInvPanel.SetIndex(partyStash.IndexOfItem(item.Name), item.FullName,
                    item.Description, item.Sprite, item.Quantity);
            }
        }

    }

    void MakeSelection(int? index, string message)
    {
        
        
        if (index != null)
        {
            // if there is an error message displaying, stop the event timer here
            // before changing the message.
            if (messageTimer.Running) { messageTimer.Stop(); }

            if (selection == index)
            {
                selection = null;
                useButton.GetComponent<Button>().interactable = false;
                useButton.color = Color.gray;

                messageText.text = "";
            }
            else
            {
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

    void ResetMessage()
    {
        messageText.text = previousMessage;
    }

    public void Click_UseButton()
    {
        if (selection == null) { return; }

        AudioManager.PlaySound(AudioClipName.UsePotion);

        InvItem item = partyStash.Contents[(int)selection];

        BattleHero hero = BattleLoader.Party.Hero[BattleMath.ConvertHeroID(TurnCounter.CurrentID)];

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
                        //hp += healing;
                        partyStash.RemoveInvItem(item.Name, 1);
                        damage.Add(-healing, false, true, false);
                        //damage.Amount[0] = hero.TakeDamage(-healing); ;
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
                        //hp += healing;
                        partyStash.RemoveInvItem(item.Name, 1);
                        damage.Add(-healing, false, true, true);
                        //damage.Amount[0] = hero.TakeMPDamage(-healing); ;
                        break;
                }
                break;

            default:
                damage.Add(null);
                break;
        }
        // disable ui once a choice is made
        uiEnabler.EnableUI(false);

        // invoke a Player End Turn event
        // use this event instead of triggering off a fight collision with enemy
        // integrate event into existing BattleManager code
        turnInvoker.TurnOver(damage, TurnCounter.CurrentID);
        Destroy(gameObject);
    }




    public void Click_CloseButton()
    {
        AudioManager.Close();
        Destroy(gameObject);
    }



}
