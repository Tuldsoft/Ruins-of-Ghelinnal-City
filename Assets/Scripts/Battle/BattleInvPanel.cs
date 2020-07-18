using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// Attached to prefabBattleInvPanel, a panel in BattleInventory grid.
/// </summary>
public class BattleInvPanel : MonoBehaviour
{
    #region Fields
    [SerializeField]
    Image icon = null, selectionButton = null;
    [SerializeField]
    Text heldText = null;
    //Text nameText = null, descriptionLabel = null;

    int? index = null; // passed as selection number
    bool selected = false;
    int held = 0; // quantity in party stash
    string fullName;
    string description;
    
    Battle_InvSelectEvent invSelect = new Battle_InvSelectEvent();

    Color darkTint = new Color(0f, 0f, 0f, 0.4f);
    Color yellowTint = new Color(1f, 1f, 0f, 0.05f);
    #endregion

    #region Methods
    // Start is called before the first frame update
    void Start()
    {
        EventManager.AddInvoker_Battle_InvSelect(this);
        EventManager.AddListener_Battle_InvSelect(ToggleSelect);
    }

    /// <summary>
    /// Stores item information for this panel
    /// </summary>
    /// <param name="index"></param>
    /// <param name="name"></param>
    /// <param name="description"></param>
    /// <param name="sprite">item sprite</param>
    /// <param name="held">number held</param>
    public void SetIndex(int index, string name, string description, Sprite sprite, int held)
    {
        this.index = index;
        //nameText.text = name;
        fullName = name;
        //descriptionLabel.text = description;
        this.description = description;
        icon.sprite = sprite;
        this.held = held;
        heldText.text = this.held.ToString();
        Deselect();
    }

    /// <summary>
    /// Called when this or another panel is selected
    /// </summary>
    /// <param name="index">index of the panel selected</param>
    /// <param name="message">unused in this method</param>
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

    // Highlight this panel
    void Select()
    {
        selected = true;
        selectionButton.color = yellowTint;
    }
    // De-highlight this panel
    void Deselect()
    {
        selected = false;
        selectionButton.color = darkTint;
    }
    
    // Called by SelectionButton.On_Click()
    public void Click_InvPanelClick()
    {
        AudioManager.Chirp();
        string message = fullName + "\n\n" + description;
        
        invSelect.Invoke(index, message);
    }

    // event handling
    public void AddListener_Battle_InvSelect(UnityAction<int?,string> listener)
    {
        invSelect.AddListener(listener);
    }
    #endregion
}
