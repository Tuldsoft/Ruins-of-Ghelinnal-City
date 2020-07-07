using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BattleInvPanel : MonoBehaviour
{
    [SerializeField]
    Image icon = null, selectionButton = null;
    [SerializeField]
    Text heldText = null;
    //Text nameText = null, descriptionLabel = null;

    int? index = null;
    bool selected = false;
    int held = 0;
    string fullName;
    string description;
    
    Battle_InvSelectEvent invSelect = new Battle_InvSelectEvent();

    Color darkTint = new Color(0f, 0f, 0f, 0.4f);
    Color yellowTint = new Color(1f, 1f, 0f, 0.05f);

    
    // Start is called before the first frame update
    void Start()
    {
        EventManager.AddInvoker_Battle_InvSelect(this);
        EventManager.AddListener_Battle_InvSelect(ToggleSelect);
        //EventManager.AddInvoker_Battle_InvDeselect(this);
        //EventManager.AddListener_Battle_InvDeselect(Deselect);

    }

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

    public void AddListener_Battle_InvSelect(UnityAction<int?,string> listener)
    {
        invSelect.AddListener(listener);
    }
}
