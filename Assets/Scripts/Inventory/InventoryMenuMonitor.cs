using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryMenuMonitor : MonoBehaviour
{
    [SerializeField]
    GameObject gridContent = null;
    [SerializeField]
    Text goldText = null, useButton = null, messageText = null;

    GameObject parentGameObject = null;

    Inventory partyStash;
    int heroIndex = 0;
    BattleHero hero;
    List<EquipSlots> slots = new List<EquipSlots>();

    int? selection = null;


    // timed message handler, for when hp/mp are at max
    EventTimer messageTimer;
    const float messageTimerDuration = 1.5f;
    string previousMessage = "";

    private void Start()
    {
        EventManager.AddListener_Inventory_Select(MakeSelection);

        messageTimer = gameObject.AddComponent<EventTimer>();
        messageTimer.Duration = messageTimerDuration;
        messageTimer.AddListener_Finished(ResetMessage);

        goldText.text = BattleLoader.Party.Gold.ToString();
    }

    public void SetInventory(int index, List<EquipSlots> slots, GameObject parentGameObject)
    {
        this.parentGameObject = parentGameObject;
        parentGameObject.SetActive(false);

        heroIndex = index;
        hero = BattleLoader.Party.Hero[heroIndex];

        this.slots = slots;

        useButton.GetComponent<Button>().interactable = false;

        messageText.text = "";
        partyStash = BattleLoader.Party.Inventory;

        PopulateGrid();
    }

    void PopulateGrid()
    {
        // empty any pre-existing children
        for (int i = 0; i < gridContent.transform.childCount; i++)
        {
            Destroy(gridContent.transform.GetChild(i).gameObject);
        }
        
        // create a grid item
        GameObject newPanel;
        GameObject prefabPanel = Resources.Load<GameObject>(@"MenuPrefabs\prefabInventoryPanel");

        // show all item types in slots list
        foreach (InvItem item in partyStash.Contents)
        {
            if (slots.Contains(item.Slot))
            {
                // create instances of the prefab, with the Content panel as its parent
                newPanel = GameObject.Instantiate(prefabPanel, gridContent.transform);

                InventoryPanel invPanel = newPanel.GetComponent<InventoryPanel>();

                bool isConsumable = item.Slot == EquipSlots.none ? true : false;
                invPanel.SetIndex(partyStash.IndexOfItem(item.Name), item.FullName,
                    item.Description, item.Sprite, item.Quantity, isConsumable);
            }
        }
        MakeSelection(null, "");
    }

    void MakeSelection(int? index, string message)
    {
        if (index != null)
        {
            // if there is an error message displaying, stop the event timer here
            // before changing the message.
            if (messageTimer.Running) { messageTimer.Stop(); }

            if (selection == index )
            {
                selection = null;
                useButton.GetComponent<Button>().interactable = false;
                useButton.color = Color.gray;

                messageText.text = "";
            }
            else
            {
                // only activate Use button if it is a consumable
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

    void ResetMessage()
    {
        messageText.text = previousMessage;
    }

    public void Click_UseButton()
    {
        if (selection == null) { return; }

        InvItem item = partyStash.Contents[(int)selection];

        if (item.Slot != EquipSlots.none)
        {
            Debug.Log("Cannot use a non-consumable");
        }

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
                        #endregion Use_Mana_Pot
                }
                break;

            case InvType.Tome:
                AudioManager.PlaySound(AudioClipName.UseTome);
                hero.BStats.Equip((item as InvEqItem).BStats);
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

        partyStash.RemoveInvItem(item.Name, 1);
        PopulateGrid();
    }


    public void Click_CloseButton()
    {
        AudioManager.Close();
        //hero.HealAll();
        HeroStatsMenuMonitor statMenu = parentGameObject.GetComponent<HeroStatsMenuMonitor>();

        statMenu.RefreshHeroStats();
        statMenu.RefreshEquipped();
        statMenu.RefreshPanel();
        
        parentGameObject.SetActive(true);
        

        Destroy(gameObject);
    }
}
