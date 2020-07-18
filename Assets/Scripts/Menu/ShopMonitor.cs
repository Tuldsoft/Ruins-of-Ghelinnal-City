using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

/// <summary>
/// Whereas Shop is a static class, sustained from scene to scene, and saved, 
/// in save files, ShopMonitor is a component attached to prefabShopMenu.
/// </summary>
public class ShopMonitor : MonoBehaviour
{
    #region Fields

    // Where to put shop panels
    [SerializeField]
    GameObject gridContent = null;

    // Text labels for various purposes
    [SerializeField]
    Text partyGoldText = null, purchaseAmountText = null, remainingAmountText = null, buyButtonText = null;

    // Color preset for the numerical value of gold
    Color goldColor = new Color(1f, 0.9495088f, 0.6235294f);

    // Amounts used in transactions
    int partyGold = 0;
    int totalPurchaseAmount = 0;
    int remainingAmount = 0;

    // Invoker for the makePurchase event
    Shop_MakePurchaseEvent makePurchaseEvent = new Shop_MakePurchaseEvent();
    #endregion

    #region Methods
    // Start() is called before the first frame update
    private void Start()
    {
        // Listens for AdjustPurchase events, and Invokes MakePurchase events
        EventManager.AddListener_Shop_AdjustPurchaseAmount(AdjustPurchaseAmount);
        EventManager.AddInvoker_Shop_MakePurchase(this);

        // Lay out shop grid panels
        RefreshPurchase();
        PopulateGrid();
    }
    
    // Fills the main part of the shop screen with inventory for purchase
    void PopulateGrid()
    {
        // Load a shop panel template
        GameObject newPanel;
        GameObject prefabPanel = Resources.Load<GameObject>(@"MenuPrefabs\prefabShopPanel");
        
        // Retrieve the Inventory of the Party
        Inventory partyStash = BattleLoader.Party.Inventory;

        // Shop.Stock was populated during file load or during Initializing
        foreach (InvItem item in Shop.Stock.Contents)
        {
            // Create instances of the shopPanel, naming Content object as its parent
            newPanel = GameObject.Instantiate(prefabPanel, gridContent.transform);
            ShopPanel shopPanel = newPanel.GetComponent<ShopPanel>();
            
            // Retrieve the number of identical items from the partyStash
            int held = 0;
            if (partyStash.ContainsItem(item.Name))
            {
                held = partyStash.Contents[partyStash.IndexOfItem(item.Name)].Quantity;
            }

            // Set up the shop Panel with the InvItem from Shop.Stock
            shopPanel.SetPanel(item, held);
        }
    }

    // After a purchase is made and inventorys are updated, refresh the shop menu display
    void RefreshGrid()
    {
        EmptyGrid();
        RefreshPurchase();
        PopulateGrid();
    }

    // Remove all panels from the shop menu (this does not affect Shop.Stock)
    void EmptyGrid()
    {
        for (int i = 0; i < gridContent.transform.childCount; i++)
        {
            Destroy(gridContent.transform.GetChild(i).gameObject);
        }
        totalPurchaseAmount = 0;
    }

    // When a + or - is clicked on a panel, it invokes this method, which adjusts totalPurchaseAmount
    void AdjustPurchaseAmount(int amount)
    {
        totalPurchaseAmount += amount;
        RefreshPurchase();
    }

    // Produces and displays a string calculating the cost of the proposed purchase.
    // Also shows if the proposed purchase is greater than available party gold, and
    // makes the Buy button only interactable if the purchase amount is <= party gold.
    void RefreshPurchase()
    {
        
        partyGold = BattleLoader.Party.Gold;
        partyGoldText.text = partyGold.ToString();
        remainingAmount = partyGold + totalPurchaseAmount;

        if (totalPurchaseAmount >= 0)
        {
            // totalPurchaseAmount should never be less than zero.
            // if totalPurchaseAmount is zero, there are no purchases queued,
            // so make the BuyButton non-interactable and hide the calculation texts
            buyButtonText.GetComponent<Button>().interactable = false; 
            purchaseAmountText.gameObject.SetActive(false);
            remainingAmountText.gameObject.SetActive(false);
        }
        else
        {
            // A purchase is being proposed, so display the calculations
            purchaseAmountText.gameObject.SetActive(true);
            remainingAmountText.gameObject.SetActive(true);
            purchaseAmountText.text = totalPurchaseAmount.ToString();
            remainingAmountText.text = remainingAmount.ToString();

            // If after the purchase, the party would have negative gold, prevent the purchase
            if (remainingAmount < 0)
            {
                buyButtonText.GetComponent<Button>().interactable = false;
                buyButtonText.color = Color.red;
                remainingAmountText.color = Color.red;
            }
            else 
            {
                // A valid purchase is being made, so Buy is interactable
                buyButtonText.GetComponent<Button>().interactable = true; 
                remainingAmountText.color = goldColor;
                buyButtonText.color = Color.green;
            }
        }
    }

    // For BuyButton On_Click(). Makes the proposed purchase, adjusting gold and Inventorys
    public void Click_BuyButton()
    {
        AudioManager.PlaySound(AudioClipName.ShopPurchase); // ka-ching
        
        // Calculate gold
        if (remainingAmount >= 0)
        {
            partyGold = remainingAmount;
            BattleLoader.Party.Gold = partyGold;
            remainingAmount = partyGold;
            
            // Clear the proposed purchase display
            totalPurchaseAmount = 0;
            RefreshPurchase();
        }

        // Make a dictionary to store the quantity of each item in the purchase
        Dictionary<InvNames, int> purchaseLog = new Dictionary<InvNames, int>();

        // Tells all the shopPanels to put their quantity and itemName info into the log
        makePurchaseEvent.Invoke(purchaseLog);

        // For everything in the log with a quantity > 0
        foreach (KeyValuePair<InvNames, int> pair in purchaseLog)
        {
            // Retrieve the InvItem from the Shop.Stock
            InvItem shopItem = Shop.Stock.GetItem(pair.Key);
            
            // Deduct quantity from Shop.Stock, but 
            // don't deduct from shopInventory if quantity is -1, aka unlimited
            if (shopItem.Quantity > 0)
            {
                Shop.Stock.RemoveInvItem(shopItem.Name, pair.Value);
            }

            // Make a new copy of the item and add some of it to the party inventory
            InvItem partyItem = InvData.MakeNewInvItem(pair.Key);
            BattleLoader.Party.Inventory.AddInvItem(partyItem.Name, pair.Value);
        }
    }

    // Used by the CloseButton On_Click()
    public void Click_CloseButton()
    {
        AudioManager.Close();
        Destroy(gameObject);
    }

    // Used by the cheat button On_Click(). Adds unlimited quantities of all items
    // to the Shop, plus 500k gold
    public void Click_CheatButton()
    {
        Shop.Stock.ClearInvItems();

        Shop.AddUnlimitedEverything();
        
        BattleLoader.Party.Gold += 500000;
        RefreshGrid();
    }

    // Allows other objects to become listeners of this invoker.
    public void AddListener_Shop_MakePurchase(UnityAction<Dictionary<InvNames,int>> listener)
    {
        makePurchaseEvent.AddListener(listener);
    }
    #endregion
}
