using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class PanelSelection : MonoBehaviour
{
    [SerializeField]
    Text choiceText = null;
    [SerializeField]
    Image tintImage = null;

    int selectionNum;
    bool selected = false;

    Color darkTint = new Color(0f, 0f, 0f, 0.4f);
    Color yellowTint = new Color(1f, 1f, 0f, 0.1f);

    PanelSelectEvent panelSelectEvent = new PanelSelectEvent();


    public void SetupPanel (int selectionNum, string displayText)
    {
        this.selectionNum = selectionNum;
        choiceText.text = displayText;

        EventManager.AddInvoker_PanelSelect(this);
        EventManager.AddListener_PanelSelect(ToggleSelect);

        Deselect();
    }

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

    void Select()
    {
        selected = true;
        tintImage.color = yellowTint;
    }

    void Deselect()
    {
        selected = false;
        tintImage.color = darkTint;
    }

    public void Click_Panel()
    {
        AudioManager.Chirp();
        panelSelectEvent.Invoke(selectionNum);
    }

    public void AddListener_PanelSelect(UnityAction<int?> listener)
    {
        panelSelectEvent.AddListener(listener);
    }
}
