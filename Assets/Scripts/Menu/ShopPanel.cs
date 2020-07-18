using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

/// <summary>
/// Attached as a component to each ShopPanel. Represents a single item and its quantity from 
/// Shop.Stock. Controls all display pieces of the panel, and keeps track of the propsoed purchase 
/// quantity of the item. Communicates with the ShopMonitor via the MakePurchase event, by adding its
/// item name and quantity to a provided purchase log Dictionary.
/// </summary>
public class ShopPanel : MonoBehaviour
{
    #region Fields
    // Display objects that need updating
    [SerializeField]
    Text nameText = null, costText = null, stockText = null, 
        heldText = null, quantityText = null, descriptionText = null;
    [SerializeField]
    Image icon = null;

    // Gold color preset for numerical money
    Color goldColor = new Color(1f, 0.9495088f, 0.6235294f);

    // Name of the InvItem that this panel represents. Does not store the InvItem directly,
    // to prevent unintended alteration of Inventorys
    InvNames itemName; // Name of the item in Shop.Stock
    int quantity = 0;  // Quantity of the item proposed as a purchase
    int cost = 50;     // Cost of the item
    int stock = -1;    // Quantity of the item in Shop.Stock
    int held = 0;      // QUantity of the item in the partyStash

    // Invoker for the adjustPurchaseAmount event
    Shop_AdjustPurchaseAmountEvent adjustPurchaseAmount = new Shop_AdjustPurchaseAmountEvent();
    #endregion

    #region Methods
    // Start is called before the first frame update
    void Start()
    {
        // Invoker for AdjustPurchaseAmount event, and listener for MakePurchase
        EventManager.AddInvoker_Shop_AdjustPurchaseAmount(this);
        EventManager.AddListener_Shop_MakePurchase(LogPanelPurchase);
    }

    /// <summary>
    /// Sets the panel's display
    /// </summary>
    /// <param name="item">The InvItem in SHop.Stock that the panel represents</param>
    /// <param name="held">The quantity held in the partyStash</param>
    public void SetPanel(InvItem item, int held)
    {
        // Use info from the InvItem in Shop.Stock to store locally
        itemName = item.Name;
        icon.sprite = item.Sprite;
        nameText.text = item.FullName;
        descriptionText.text = item.Description;
        cost = item.Price;
        stock = item.Quantity;
        
        // Use provided int as the number held by the party
        this.held = held;

        // Update the display with stored variables
        RefreshPanel();
    }

    // Sequence of methods needed to display current information
    void RefreshPanel()
    {
        SetPrice(cost);
        SetStock(stock);
        SetHeld(held);
        SetQuantity();
    }

    // Update amount showing in costText object, and its color
    public void SetPrice(int cost)
    {
        costText.text = cost.ToString();
        if (cost > BattleLoader.Party.Gold)
        {
            costText.color = Color.red;
        }
        else
        {
            costText.color = goldColor;
        }
    }

    // Update amount showing in the shop stock, and its color
    public void SetStock(int stock) 
    {
        if (stock < 0)
        {
            stockText.text = "Unlimited";
        }
        else
        {
            stockText.text = stock.ToString();
            if (stock == 0)
            {
                stockText.color = Color.red;
            }
            else
            {
                stockText.color = Color.white;
            }
        }
    }

    // Update amount showing as held by the party, and chooses whether to bother displaying it
    public void SetHeld(int held)
    {
        heldText.text = held.ToString();
        if (held <= 0)
        {
            heldText.gameObject.SetActive(false);
        }
        else
        {
            heldText.gameObject.SetActive(true);
        }
    }

    // Update amount showing as the intended purchase quantity, and picks a color
    public void SetQuantity()
    {
        quantityText.GetComponent<InputField>().text = quantity.ToString();
        if (quantity <= 0)
        {
            quantityText.color = Color.gray;
        }
        else
        {
            quantityText.color = Color.white;
        }
    }
 
    // Used buy PlusButton On_Click(). Increases the quantity of the proposed purchase,
    // up to the amount available in Shop.Stock. Invokes adjustPurchaseAmount, to update
    // the ShopMonitor's purchase display.
    public void Click_PlusButton()
    {
        AudioManager.Chirp();
        if (stock < 0 || quantity < stock)
        {
            quantity++;
            RefreshPanel();
            adjustPurchaseAmount.Invoke(-cost);
        }
    }

    // Used buy PlusButton On_Click(). Decreases the quantity of the proposed purchase,
    // down to minimum of zero. Invokes adjustPurchaseAmount, to update
    // the ShopMonitor's purchase display.
    public void Click_MinusButton()
    {
        AudioManager.Chirp(); 
        if (quantity > 0)
        {
            quantity--;
            RefreshPanel();
            adjustPurchaseAmount.Invoke(cost);
        }
    }

    // Used by the Quantitytext object, which serves as an InputField
    // to accept keyboard adjustments of quantity. Clamps between zero and ShopStock
    // REFACTOR: Assumes the user behaves and enters an int. Little error handling.
    public void EnterQuantity(string entry)
    {
        int newQuantity = int.Parse(entry);

        // Minimum of 0
        if (newQuantity < 0) { newQuantity = 0; }

        // maximum of stock if limited supply
        if (stock >= 0 && newQuantity > stock) { newQuantity = stock; }

        int costDifference = cost * (quantity - newQuantity);

        quantity = newQuantity;
        
        // Update colorations
        RefreshPanel();
        // Notify ShopMonitor of new costs
        adjustPurchaseAmount.Invoke(costDifference);
    }

    // Used to make other classes as listeners to this invoker.
    public void AddListener_Shop_AdjustPurchaseAmount(UnityAction<int> listener)
    {
        adjustPurchaseAmount.AddListener(listener);
    }

    // Invoked by the MakePurchase event, this adds the name of the item represented
    // by this panel and the proposed purchase quantity (if > 0) to a purchase log. All panels
    // add to this same log. This does not make any adjustments to shop or party Inventorys
    void LogPanelPurchase(Dictionary<InvNames,int> log)
    {
        // Only make a log entry if quantity is > 0
        if (quantity > 0)
        {
            // Add name and quantity to the log
            log.Add(itemName, quantity);

            // Deduct quantity from the local stock int
            if (stock > 0)
            {
                stock -= quantity;
            }

            // Increase quantity to the local held int
            held += quantity;

            // Set new purchase quantity to zero
            quantity = 0;
            
            // Update labels
            RefreshPanel();
        }
    }
    #endregion
}
