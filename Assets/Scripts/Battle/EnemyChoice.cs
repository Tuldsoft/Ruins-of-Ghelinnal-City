using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class EnemyChoice : FightChoice
{
    
    public int ChoiceNumber { get; set; }
    bool selected = false;

    [SerializeField]
    public GameObject highlighter;

    Battle_ChoiceSelectionEvent battle_ChoiceSelectionEvent = new Battle_ChoiceSelectionEvent();
    UIEnabler uiEnabler;


    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        EventManager.AddInvoker_Battle_ChoiceSelection(this);
        EventManager.AddListener_Battle_ChoiceSelection(MakeSelection);
        selected = false;

        // adds a UIEnabler. This is for enemy sprites and menu items alike.
        // Menu items will self-register
        uiEnabler = gameObject.AddComponent<UIEnabler>();

    }
    

    public void Click_EnemyChoice ()
    {
        if (selected)
        {
            Click_Fight();
        }
        else
        {
            AudioManager.Chirp();
            battle_ChoiceSelectionEvent.Invoke(ChoiceNumber);
        }
        
    }

    public void OnMouseUpAsButton()
    {
        if (uiEnabler.UIEnabled)
        {
            Click_EnemyChoice();
        }
        
    }

    public void AddListener_ChoiceSelection(UnityAction<int> listener)
    {
        battle_ChoiceSelectionEvent.AddListener(listener);
    }

    // listened for if a sprite is clicked, and used with -1 when renumbering
    public void MakeSelection(int choice)
    {
        if (ChoiceNumber == choice)
        {
            selected = !selected;
        }
        else
        {
            // includes all other numbers, including -1
            selected = false;
        }

        highlighter.gameObject.SetActive(selected);
    }

}
