using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

/// <summary>
/// A generic PanelSelection. Created by FileMenuMonitor and attached to prefabFileSlotPanel.
/// In theory, can be placed on any prefabPanel object that includes a text display, a button,
/// and an image for tinting.
/// REFACTOR: Inherit this or use as a Interface for any and every "panel" interaction, such 
/// as inventory menus, choosing dungeons, etc.
/// </summary>

public class PanelSelection : MonoBehaviour
{
    // Text label to display on the panel
    [SerializeField]
    Text choiceText = null;
    
    // The Image item whose SpriteRenderer gets tinted depending on selected
    [SerializeField]
    Image tintImage = null;

    int selectionNum;       // Numerical index of this panel
    bool selected = false;  // Whether or not this panel is the one selected panel

    // Color presets for tints
    Color darkTint = new Color(0f, 0f, 0f, 0.4f);
    Color yellowTint = new Color(1f, 1f, 0f, 0.1f);

    // Invoker (and listener) of the panelSelectEvent
    PanelSelectEvent panelSelectEvent = new PanelSelectEvent();

    // Set an index and a display text
    public void SetupPanel (int selectionNum, string displayText)
    {
        this.selectionNum = selectionNum;
        choiceText.text = displayText;

        // Add as both an invoker of PanelSelect, and a listener for all other panels
        EventManager.AddInvoker_PanelSelect(this);
        EventManager.AddListener_PanelSelect(ToggleSelect);

        // Start with nothing selected
        Deselect();
    }

    // Invoked by the PanelSelect event, by this or any other panel
    void ToggleSelect(int? index)
    {
        if (selectionNum == index)
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

    // Select this panel
    void Select()
    {
        selected = true;
        tintImage.color = yellowTint;
    }

    // Deselect this panel
    void Deselect()
    {
        selected = false;
        tintImage.color = darkTint;
    }

    // Called by the panel button's On_Click(). 
    // Calls ToggleSelect() with this panel's identifier
    public void Click_Panel()
    {
        AudioManager.Chirp();
        panelSelectEvent.Invoke(selectionNum);
    }

    // Allows other objects to declare themselves as listeners to this invoker.
    public void AddListener_PanelSelect(UnityAction<int?> listener)
    {
        panelSelectEvent.AddListener(listener);
    }
}
