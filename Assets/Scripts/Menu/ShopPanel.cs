using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ShopPanel : MonoBehaviour
{
    [SerializeField]
    Text nameText = null, costText = null, stockText = null, 
        heldText = null, quantityText = null, descriptionText = null;
    [SerializeField]
    Image icon = null;

    Color goldColor = new Color(1f, 0.9495088f, 0.6235294f);

    InvNames itemName;
    int quantity = 0;
    int cost = 50;
    int stock = -1;
    int held = 0;

    Shop_AdjustPurchaseAmountEvent adjustPurchaseAmount = new Shop_AdjustPurchaseAmountEvent();

    // Start is called before the first frame update
    void Start()
    {
        EventManager.AddInvoker_Shop_AdjustPurchaseAmount(this);
        EventManager.AddListener_Shop_MakePurchase(LogPanelPurchase);
    }

    // the item held in shopInventory, and the number held in the Party inventory
    public void SetPanel(InvItem item, int held)
    {
        itemName = item.Name;
        //icon.sprite = Resources.Load<Sprite>(@"Sprites\InvItems\" + itemName.ToString());
        icon.sprite = item.Sprite;


        nameText.text = item.FullName;
        descriptionText.text = item.Description;
        cost = item.Price;
        stock = item.Quantity;
        this.held = held;

        RefreshPanel();
    }

    void RefreshPanel()
    {
        SetPrice(cost);
        SetStock(stock);
        SetHeld(held);
        SetQuantity();
    }

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

    public void EnterQuantity(string entry)
    {
        int newQuantity = int.Parse(entry);

        // minimum of 0
        if (newQuantity < 0) { newQuantity = 0; }

        // maximum of stock if limited supply
        if (stock >= 0 && newQuantity > stock) { newQuantity = stock; }

        int costDifference = cost * (quantity - newQuantity);

        quantity = newQuantity;
        
        RefreshPanel();
        adjustPurchaseAmount.Invoke(costDifference);

    }

    public void AddListener_Shop_AdjustPurchaseAmount(UnityAction<int> listener)
    {
        adjustPurchaseAmount.AddListener(listener);
    }

    void LogPanelPurchase(Dictionary<InvNames,int> log)
    {
        if (quantity > 0)
        {
            log.Add(itemName, quantity);

            if (stock > 0)
            {
                stock -= quantity;
            }

            held += quantity;

            quantity = 0;
            RefreshPanel();
        }
        
    }

}
