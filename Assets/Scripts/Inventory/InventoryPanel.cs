using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class InventoryPanel : MonoBehaviour
{
    [SerializeField]
    Image icon = null, selectionButton = null, bg = null, frame = null;
    [SerializeField]
    Text heldText = null;
    //Text nameText = null, descriptionLabel = null;

    int? index = null;
    bool selected = false;
    int held = 0;
    string fullName;
    string description;

    Inventory_SelectEvent invSelect = new Inventory_SelectEvent();

    Color darkTint = new Color(0f, 0f, 0f, 0.4f);
    Color yellowTint = new Color(1f, 1f, 0f, 0.1f);


    // Start is called before the first frame update
    void Start()
    {
        EventManager.AddInvoker_Inventory_Select(this);
        EventManager.AddListener_Inventory_Select(ToggleSelect);
    }

    public void SetIndex(int index, string name, string description, Sprite sprite, int held, bool isConsumable)
    {
        this.index = index;
        //nameText.text = name;
        fullName = name;
        //descriptionLabel.text = description;
        this.description = description;
        icon.sprite = sprite;
        this.held = held;
        heldText.text = this.held.ToString();

        bg.enabled = !isConsumable;
        frame.enabled = !isConsumable;

        Deselect();
    }

    void ToggleSelect(int? index, string message)
    {
        if (this.index == index)
        {
            if (selected)
            {
                Deselect();
            }
            else
            {
                Select();
            }
        }
        else
        {
            Deselect();
        }
    }

    void Select()
    {
        selected = true;
        selectionButton.color = yellowTint;
    }

    void Deselect()
    {
        selected = false;
        selectionButton.color = darkTint;
    }

    public void Click_InvPanelClick()
    {
        AudioManager.Chirp();
        string message = fullName + "\n\n" + description;

        invSelect.Invoke(index, message);
    }

    public void AddListener_Inventory_Select(UnityAction<int?, string> listener)
    {
        invSelect.AddListener(listener);
    }
}
