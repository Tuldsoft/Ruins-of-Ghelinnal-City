using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class EquipMenuPanel : MonoBehaviour
{
    [SerializeField]
    Image icon = null, selectionButton = null;
    [SerializeField]
    Text nameText = null, descriptionLabel = null, heldText = null;

    int? index = null;
    bool selected = false;
    int held = 0;

    Equip_InvSelectEvent invSelect = new Equip_InvSelectEvent();

    Color halfWhiteTint = new Color(1f, 1f, 1f, 0.5f);
    Color whiteTint = new Color(1f, 1f, 1f, 0.25f);
    Color black = new Color(0f, 0f, 0f, 1f);
    Color yellowTint = new Color(1f, 1f, 0f, 0.1f);
    Color halfYellowTint = new Color(1f, 1f, 0f, 0.25f);

    // Start is called before the first frame update
    void Start()
    {
        EventManager.AddInvoker_Equip_InvSelect(this);
        EventManager.AddListener_Equip_InvSelect(ToggleSelect);

    }

    // Update is called once per frame
    void Update()
    {

    }

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

    void Select()
    {
        selected = true;
        selectionButton.color = halfWhiteTint;
        ColorBlock colors = selectionButton.GetComponent<Button>().colors;
        colors.normalColor = yellowTint;
        colors.highlightedColor = halfYellowTint;
        selectionButton.GetComponent<Button>().colors = colors;
    }

    void Deselect()
    {
        selected = false;
        selectionButton.color = whiteTint;
        ColorBlock colors = selectionButton.GetComponent<Button>().colors;
        colors.normalColor = black;
        colors.highlightedColor = yellowTint;
        selectionButton.GetComponent<Button>().colors = colors;
        
    }

    public void Click_InvPanelClick()
    {
        AudioManager.Chirp();
        invSelect.Invoke(index);
    }

    public void AddListener_Equip_InvSelect(UnityAction<int?> listener)
    {
        invSelect.AddListener(listener);
    }
}
