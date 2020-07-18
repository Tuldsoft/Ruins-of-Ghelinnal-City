using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

/// <summary>
/// FightChoice is attached to the "Fight" option in the lower-left menu. EnemyChoice comes in pairs, 
/// with one attached to an EnemyChoiceText in the lower right, and the other to a monster sprite.
/// It will toggle a user's selection of an enemy, or, if already selected, will trigger the Fight 
/// action (via FightChoice).
/// </summary>
public class EnemyChoice : FightChoice
{
    #region Fields
    // The ID of the enemy this object refers to. This is identical between the EnemyChoice
    // attached to the sprite and the EnemyChoice attached to the name display.
    public int ChoiceNumber { get; set; }
    
    // Whether or not the attached enemy is selected. When selected, its partner is also selected.
    bool selected = false;

    // The object used to show selection. For a sprite, this is a pointer icon. For an EnemyChoiceText,
    // this is a yellow glow. It is active when selected.
    [SerializeField]
    public GameObject highlighter;

    // Event to communicate with all other EnemyChoice objects
    Battle_ChoiceSelectionEvent battle_ChoiceSelectionEvent = new Battle_ChoiceSelectionEvent();
    
    // Component that enables or disables this object based on whether the user can input
    UIEnabler uiEnabler;
    #endregion


    #region Methods
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        // Both a listener and an invoker of ChoiceSelection
        EventManager.AddInvoker_Battle_ChoiceSelection(this);
        EventManager.AddListener_Battle_ChoiceSelection(MakeSelection);
        selected = false;

        // adds a UIEnabler. This is for enemy sprites and menu items alike.
        // Menu items will self-register when created.
        uiEnabler = gameObject.AddComponent<UIEnabler>();

    }
    
    // On_Click() method of EnemyChoiceText's Button component
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
    
    // Used with an enemy sprite's Collider component.
    public void OnMouseUpAsButton()
    {
        if (uiEnabler.UIEnabled)
        {
            Click_EnemyChoice();
        }
        
    }

    // Registers this as an invoker for ChoiceSelection
    public void AddListener_ChoiceSelection(UnityAction<int> listener)
    {
        battle_ChoiceSelectionEvent.AddListener(listener);
    }

    /// <summary>
    /// Invoked by this and other EnemyChoice objects, when any is "clicked".  
    /// </summary>
    /// <param name="choice">ID of the Choice that was "clicked". Use -1 to de-select all</param>
    public void MakeSelection(int choice)
    {
        if (ChoiceNumber == choice)
        {
            // Toggle selected if ID matches
            selected = !selected;
        }
        else
        {
            // IF another ID or -1, deselect
            selected = false;
        }

        // Display or hide highlighter or pointer
        highlighter.gameObject.SetActive(selected);
    }
    #endregion
}
