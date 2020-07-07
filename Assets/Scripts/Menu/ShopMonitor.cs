using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ShopMonitor : MonoBehaviour
{
    // the prefab object to be exposed in the inspector
    
    [SerializeField]
    GameObject gridContent = null;

    [SerializeField]
    Text partyGoldText = null, purchaseAmountText = null, remainingAmountText = null, buyButtonText = null;

    Color goldColor = new Color(1f, 0.9495088f, 0.6235294f);

    int partyGold = 0;
    int totalPurchaseAmount = 0;
    int remainingAmount = 0;

    Shop_MakePurchaseEvent makePurchaseEvent = new Shop_MakePurchaseEvent();

    private void Start()
    {
        EventManager.AddListener_Shop_AdjustPurchaseAmount(AdjustPurchaseAmount);
        EventManager.AddInvoker_Shop_MakePurchase(this);

        RefreshPurchase();
        PopulateGrid();
    }
    

    void PopulateGrid()
    {
        // create a grid item
        GameObject newPanel;
        GameObject prefabPanel = Resources.Load<GameObject>(@"MenuPrefabs\prefabShopPanel");
        Inventory partyStash = BattleLoader.Party.Inventory;

        foreach (InvItem item in Shop.Stock.Contents)
        {
            // create instances of the prefab, with the Content panel as its parent
            newPanel = GameObject.Instantiate(prefabPanel, gridContent.transform);

            ShopPanel shopPanel = newPanel.GetComponent<ShopPanel>();
            int held = 0;
            if (partyStash.ContainsItem(item.Name))
            {
                held = partyStash.Contents[partyStash.IndexOfItem(item.Name)].Quantity;
            }

            shopPanel.SetPanel(item, held);

        }

    }


    void RefreshGrid()
    {
        EmptyGrid();
        RefreshPurchase();
        PopulateGrid();
    }

    void EmptyGrid()
    {
        for (int i = 0; i < gridContent.transform.childCount; i++)
        {
            Destroy(gridContent.transform.GetChild(i).gameObject);
        }
        totalPurchaseAmount = 0;
    }


    void AdjustPurchaseAmount(int amount)
    {
        totalPurchaseAmount += amount;
        RefreshPurchase();
    }

    void RefreshPurchase()
    {
        // totalPurchaseAmount is 0 or negative
        partyGold = BattleLoader.Party.Gold;
        partyGoldText.text = partyGold.ToString();
        remainingAmount = partyGold + totalPurchaseAmount;

        if (totalPurchaseAmount >= 0)
        {
            buyButtonText.GetComponent<Button>().interactable = false; 
            purchaseAmountText.gameObject.SetActive(false);
            remainingAmountText.gameObject.SetActive(false);
        }
        else
        {
            purchaseAmountText.gameObject.SetActive(true);
            remainingAmountText.gameObject.SetActive(true);
            purchaseAmountText.text = totalPurchaseAmount.ToString();
            remainingAmountText.text = remainingAmount.ToString();

            if (remainingAmount < 0)
            {
                buyButtonText.GetComponent<Button>().interactable = false;
                buyButtonText.color = Color.red;
                remainingAmountText.color = Color.red;
            }
            else 
            {
                buyButtonText.GetComponent<Button>().interactable = true; 
                remainingAmountText.color = goldColor;
                buyButtonText.color = Color.green;
            }

        }


    }

    public void Click_BuyButton()
    {
        AudioManager.PlaySound(AudioClipName.ShopPurchase);
        if (remainingAmount >= 0)
        {
            partyGold = remainingAmount;
            BattleLoader.Party.Gold = partyGold;
            remainingAmount = partyGold;
            totalPurchaseAmount = 0;
            RefreshPurchase();
        }

        Dictionary<InvNames, int> purchaseLog = new Dictionary<InvNames, int>();

        makePurchaseEvent.Invoke(purchaseLog);

        foreach (KeyValuePair<InvNames, int> pair in purchaseLog)
        {
            InvItem shopItem = Shop.Stock.GetItem(pair.Key);
            
            // don't deduct from shopInventory if quantity is -1, aka unlimited
            if (shopItem.Quantity > 0)
            {
                Shop.Stock.RemoveInvItem(shopItem.Name, pair.Value);
            }

            InvItem partyItem = InvData.MakeNewInvItem(pair.Key);
            BattleLoader.Party.Inventory.AddInvItem(partyItem.Name, pair.Value);

        }
    }

    public void Click_CloseButton()
    {
        AudioManager.Close();
        Destroy(gameObject);
    }

    public void Click_CheatButton()
    {
        Shop.Stock.ClearInvItems();

        Shop.AddUnlimitedEverything();
        
        BattleLoader.Party.Gold += 500000;
        RefreshGrid();
    }

    public void AddListener_Shop_MakePurchase(UnityAction<Dictionary<InvNames,int>> listener)
    {
        makePurchaseEvent.AddListener(listener);
    }
}
