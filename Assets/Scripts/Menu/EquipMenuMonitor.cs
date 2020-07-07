using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipMenuMonitor : MonoBehaviour
{
    [SerializeField]
    GameObject gridContent = null;

    [SerializeField]
    Text equipButton = null, messageText = null;

    [SerializeField]
    Text currentName = null;
    [SerializeField]
    GameObject currentObject = null;
    [SerializeField]
    Image currentImage = null;

    GameObject parentGameObject;

    Inventory partyStash;
    HeroEquipment heroStash;
    int heroIndex = 0;
    BattleHero hero;
    EquipSlots slot;
    int? selection = null;

    private void Start()
    {
        EventManager.AddListener_Equip_InvSelect(MakeSelection);
    }

    public void SetHero(int index, EquipSlots slot, GameObject parentGameObject)
    {
        this.parentGameObject = parentGameObject;
        parentGameObject.SetActive(false);

        heroIndex = index;
        hero = BattleLoader.Party.Hero[heroIndex];

        this.slot = slot;
        heroStash = BattleLoader.Party.Hero[heroIndex].Equipment;
        
        equipButton.GetComponent<Button>().interactable = false;

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
        GameObject prefabPanel = Resources.Load<GameObject>(@"MenuPrefabs\prefabEquipPanel");


        foreach (InvItem item in partyStash.Contents)
        {
            // only items appropriate to the slot
            if (item.Slot == slot)
            {
                // create instances of the prefab, with the Content panel as its parent
                newPanel = GameObject.Instantiate(prefabPanel, gridContent.transform);

                EquipMenuPanel equipPanel = newPanel.GetComponent<EquipMenuPanel>();

                equipPanel.SetIndex(partyStash.IndexOfItem(item.Name), item.FullName,
                item.Description, item.Sprite, item.Quantity);
            }
        }

        // find hero's current item, and display it
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


        MakeSelection(selection);
    }

    void MakeSelection(int? index)
    {
        if (index != null)
        {
            // if there is an error message displaying, stop the event timer here
            // before changing the message.

            if (selection == index)
            {
                selection = null;
                equipButton.GetComponent<Button>().interactable = false;
                equipButton.color = Color.gray;
                messageText.text = "";
            }
            else
            {
                selection = index;
                equipButton.GetComponent<Button>().interactable = true;
                equipButton.color = Color.white;
                messageText.text = CompareEquipped((int)index);
            }
        }
        else
        {
            //Debug.Log("null index sent through events");
        }

    }
    
    string CompareEquipped(int index)
    {
        Dictionary<BattleStatNames, int> comparison = new Dictionary<BattleStatNames, int>();
        // CreateEquipmentComparison(prospectiveEqBattleStats[index].Export - existingEqBattleStats[hero.slot].Export)
        // messageText = above
        InvEqItem selectedItem = (InvEqItem)partyStash.Contents[(int)index];
        InvEqItem equippedItem = null;
        foreach (InvEqItem item in heroStash.Contents)
        {
            if (item.Slot == selectedItem.Slot)
            {
                equippedItem = item;
                break;
            }
        }

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


        for (int i = 0; i <= 10; i++)
        {
            int value = selectedValues[i] - equippedValues[i];
            if (value != 0)
            {
                comparison.Add((BattleStatNames)i, value);
            }
        }

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

        return output;
    }


    public void Click_EquipButton()
    {
        AudioManager.Chirp();
        if (selection != null)
        {
            hero.Equip(partyStash.Contents[(int)selection].Name);
        }
        PopulateGrid();

    }

    public void Click_CloseButton()
    {
        AudioManager.Close();
        parentGameObject.GetComponent<HeroStatsMenuMonitor>().RefreshHeroStats();
        parentGameObject.GetComponent<HeroStatsMenuMonitor>().RefreshEquipped();
        parentGameObject.SetActive(true);
        Destroy(gameObject);
    }
}
