using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

/// <summary>
/// UIEnabler is a component attached to any UI element that needs to be turned on and off
/// from user input, for example, when the enemy is attacking.
/// It can be attached in editor, or at runtime.
/// Each UIEnabler keeps a list of buttons associated with its attached script or UI element.
/// When its EnableUI method is called, each button in its list will be disabled or enabled.
/// UIEnabler is the only invoker and listener for its events. It communicates directly 
/// to other UIEnablers.
/// </summary>


public class UIEnabler : MonoBehaviour
{
    #region Fields and Properties
    // Set this in the inspector if the attached UI element should only
    // be visible to certain members. Ex: the Blitz SubMenu is only for Sabin

    [SerializeField]
    public List<HeroType> Exclusive = new List<HeroType>();
    
    // List of buttons in this UIEnabler to disable or enable (typically just one)
    List<Button> buttons = new List<Button>();

    // Checked by non-button methods (OnMouseUp calls, etc.)
    bool uiEnabled = false;
    public bool UIEnabled { get { return uiEnabled; } }

    // A single button represented in this script
    Button selfButton = null;

    // Used only by the UIEnabler on the battleManager to return a hero to its position
    BattleManager battleManager = null;

    Battle_UIEnablerEvent uiEnablerEvent = new Battle_UIEnablerEvent();
    #endregion

    #region Methods
    // Start is called before the first frame update
    void Start()
    {
        // Set as listener and invoker of UIEnabler event
        EventManager.AddInvoker_Battle_UIEnabler(this);
        EventManager.AddListener_Battle_UIEnabler(SetInteractable);

        // Only one selfButton per script, must be a direct component of gameObject
        Button[] selfButtons = gameObject.GetComponents<Button>();
        if (selfButtons.Length == 1) 
        { 
            selfButton = selfButtons[0];
            buttons.Add(selfButton);
        }

        SetInteractable(false, HeroType.none);
        
    }

    // Adds a button to be enabled or disabled by the enabler. (Unused)
    public void RegisterButton(Button button)
    {
        buttons.Add(button);
    }

    // Stores a reference to the BattleManager in this instance
    public void RegisterBattleManager (BattleManager manager)
    {
        battleManager = manager;
    }

    /// <summary>
    /// Called by the BattleManager and other classes, this public method is 
    /// how an exterior class can turn UI elements on and off, via SetInteractable().
    /// Ex: BattleManager.HeroStartTurn(), Click_, other methods
    /// </summary>
    /// <param name="interactable">Whether or not UI element should be interactable</param>
    /// <param name="type">The HeroType of the active hero</param>
    public void EnableUI(bool interactable, HeroType type = HeroType.none)
    {
        // Invoke an EnableUI event for all UIEnablers   
        uiEnablerEvent.Invoke(interactable, type);
    }


    /// <summary>
    /// All UIEnablers listen for this method, which only a UIEnabler can invoke.
    /// </summary>
    /// <param name="interactable">Whether or not UI elements should be interable</param>
    /// <param name="type">The HeroType of the active hero</param>
    void SetInteractable (bool interactable, HeroType type)
    {
        // Return the active hero to home if this call is attached to the BattleManager
        // and UI elements are being turned off
        if (battleManager != null && !interactable)
        {
            battleManager.ReturnHeroToPosition();
        }
        
        // Typically there is only one button
        foreach (Button button in buttons)
        {
            // Set the button or gameObject interactable or not.
            button.interactable = interactable;
            button.gameObject.SetActive(interactable);

            // If this UI element is Exclusive to one or more heroes, enable or disable.
            // Only necessary when enabling, otherwise everything is off regards of hero.
            if (interactable && Exclusive.Count > 0)
            {
                if (!Exclusive.Contains(type)) { button.gameObject.SetActive(false); }
                else { button.gameObject.SetActive(true); }
            }
        }

        // Store interactable for reference later
        uiEnabled = interactable;
    }

    // Add this script as an invoker for other listeners
    public void AddListener_Battle_UIEnabler(UnityAction<bool, HeroType> listener)
    {
        uiEnablerEvent.AddListener(listener);
    }
    #endregion
}
