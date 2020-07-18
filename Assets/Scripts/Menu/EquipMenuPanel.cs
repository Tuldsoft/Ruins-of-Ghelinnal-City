using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

/// <summary>
/// Attached to the prefabEquipItem. Selects and de-selects itself and others of its kind
/// through its select method, which also tells the EquipMenuMonitor the index of the selected one.
/// </summary>
public class EquipMenuPanel : MonoBehaviour
{
    // Included labels and sprites, set in the prefab via inspector
    [SerializeField]
    Image icon = null, selectionButton = null;
    [SerializeField]
    Text nameText = null, descriptionLabel = null, heldText = null;

    int? index = null;     // The index of this instance of EquipMenuuPanel
    bool selected = false; // Whether or not it is selected (highlighted)
    int held = 0;          // The number of this kind of item in the partyStash

    // Used as an invoker of InvSelectEvent
    Equip_InvSelectEvent invSelect = new Equip_InvSelectEvent();

    // Color presets
    Color halfWhiteTint = new Color(1f, 1f, 1f, 0.5f);
    Color whiteTint = new Color(1f, 1f, 1f, 0.25f);
    Color black = new Color(0f, 0f, 0f, 1f);
    Color yellowTint = new Color(1f, 1f, 0f, 0.1f);
    Color halfYellowTint = new Color(1f, 1f, 0f, 0.25f);

    // Start is called before the first frame update
    void Start()
    {
        // Used as both an invoker and listener for Equip_InvSelect
        EventManager.AddInvoker_Equip_InvSelect(this);
        EventManager.AddListener_Equip_InvSelect(ToggleSelect);

    }

    // Used by EquipMenuMonitor to load its values, after this is created
    public void SetIndex(int index, string name, string description, Sprite sprite, int held)
    {
        this.index = index;
        nameText.text = name;
        descriptionLabel.text = description;
        this.held = held;
        heldText.text = this.held.ToString();
        icon.sprite = sprite;
        Deselect();
    }

    // Toggles the selection or off. Called by Equip_InvSelect event (which all panels hear)
    void ToggleSelect(int? index)
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

    // Turn the highlighting on
    void Select()
    {
        selected = true;
        selectionButton.color = halfWhiteTint;
        ColorBlock colors = selectionButton.GetComponent<Button>().colors;
        colors.normalColor = yellowTint;
        colors.highlightedColor = halfYellowTint;
        selectionButton.GetComponent<Button>().colors = colors;

        // Comparison is done by EquipMenuMonitor when Equip_InvSelect event is 
        // invoked, using the index stored in this class.
    }

    // Turn highlighting off
    void Deselect()
    {
        selected = false;
        selectionButton.color = whiteTint;
        ColorBlock colors = selectionButton.GetComponent<Button>().colors;
        colors.normalColor = black;
        colors.highlightedColor = yellowTint;
        selectionButton.GetComponent<Button>().colors = colors;
        
    }

    // Used by the panel's invisible button, On_Click()
    public void Click_InvPanelClick()
    {
        AudioManager.Chirp();
        invSelect.Invoke(index);
    }

    // All listeners use this method to recognize this as an invoker
    public void AddListener_Equip_InvSelect(UnityAction<int?> listener)
    {
        invSelect.AddListener(listener);
    }
}
