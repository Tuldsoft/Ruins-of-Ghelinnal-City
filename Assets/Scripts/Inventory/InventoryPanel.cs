using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// Attached to a prefabInventoryPanel. Converts InvItem info onto a clickable panel, for
/// use in a grid of panels.
/// </summary>
public class InventoryPanel : MonoBehaviour
{
    #region Fields
    // Item references for the panel, set in the inspector
    [SerializeField]
    Image icon = null, selectionButton = null, bg = null, frame = null;
    [SerializeField]
    Text heldText = null;
    //Text nameText = null, descriptionLabel = null;

    // Fields for display and for use by InventoryMenuMonitor
    int? index = null;
    bool selected = false;
    int held = 0;
    string fullName;
    string description;

    // Event for a panel click
    Inventory_SelectEvent invSelect = new Inventory_SelectEvent();

    // Tints for selected and deselected
    Color darkTint = new Color(0f, 0f, 0f, 0.4f);
    Color yellowTint = new Color(1f, 1f, 0f, 0.1f);
    #endregion

    #region Methods
    // Start is called before the first frame update
    void Start()
    {
        // Panels talk to other panels and to the InventoryMenuMonitor when clicked
        EventManager.AddInvoker_Inventory_Select(this);
        EventManager.AddListener_Inventory_Select(ToggleSelect);
    }

    // Set up the panel
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

        // If consumable, don't show the frame or background (images for weapons and armor require these)
        bg.enabled = !isConsumable;
        frame.enabled = !isConsumable;

        // At the end of set up, make sure this panel is deselected
        Deselect();
    }

    // Select if unselected and clicked. Delected if already selected, or if something else was.
    // Called whenever a panel is clicked, in Click_InvPanelClick()
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

    // Turns yellow and set selected to true
    void Select()
    {
        selected = true;
        selectionButton.color = yellowTint;
    }

    // Turn greyed out and selected to false
    void Deselect()
    {
        selected = false;
        selectionButton.color = darkTint;
    }

    // Called by the invisible Button, On_Click()
    public void Click_InvPanelClick()
    {
        AudioManager.Chirp();
        string message = fullName + "\n\n" + description;

        // Send the name of the item as a message, and tell InventoryMenuMonitor
        // the identity of this panel.
        invSelect.Invoke(index, message);
    }

    // Lets others recognize this script as an invoker.
    public void AddListener_Inventory_Select(UnityAction<int?, string> listener)
    {
        invSelect.AddListener(listener);
    }
    #endregion
}
