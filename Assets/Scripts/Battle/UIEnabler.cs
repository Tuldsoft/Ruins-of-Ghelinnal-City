using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

// keeps a list of buttons associated with a script.
// Each button in the list is one to be disabled or enabled.
// Listens for Enable events to enable UI for the player.
// Is the only invoker and listener for its events.
// Communicates directly to other UIEnablers
// Auto-adds any single button it is attached to

public class UIEnabler : MonoBehaviour
{
    // set this in the inspector if the attached UI button should only
    // be visible to certain members

    [SerializeField]
    public List<HeroType> Exclusive = new List<HeroType>();
    
    
    // list of buttons to disable or enable
    List<Button> buttons = new List<Button>();

    // checked by non-button methods (OnMouseUp calls, etc.)
    bool uiEnabled = false;
    public bool UIEnabled { get { return uiEnabled; } }

    Button selfButton = null;

    // used only by the UIEnabler on the battleManager to return a hero to its position
    BattleManager battleManager = null;

    Battle_UIEnablerEvent uiEnablerEvent = new Battle_UIEnablerEvent();

    



    // Start is called before the first frame update
    void Start()
    {
        EventManager.AddInvoker_Battle_UIEnabler(this);
        EventManager.AddListener_Battle_UIEnabler(SetInteractable);

        // only one selfButton per script, must be a direct component of gameObject
        Button[] selfButtons = gameObject.GetComponents<Button>();
        if (selfButtons.Length == 1) 
        { 
            selfButton = selfButtons[0];
            buttons.Add(selfButton);
        }

        SetInteractable(false, HeroType.none);
        
    }

    // adds a button to be enabled or disabled by the enabler
    public void RegisterButton(Button button)
    {
        buttons.Add(button);
    }

    public void RegisterBattleManager (BattleManager manager)
    {
        battleManager = manager;
    }

    // called by HeroStartTurn, Click_ and other methods, invokes SetInteractable
    public void EnableUI(bool interactable, HeroType type = HeroType.none)
    {
        uiEnablerEvent.Invoke(interactable, type);
        
    }


    // invoked by this or other UIEnablers
    void SetInteractable (bool interactable, HeroType type)
    {

        if (battleManager != null && !interactable)
        {
            battleManager.ReturnHeroToPosition();
        }
        
        foreach (Button button in buttons)
        {
            button.interactable = interactable;
            button.gameObject.SetActive(interactable);

            if (interactable && Exclusive.Count > 0)
            {
                if (!Exclusive.Contains(type)) { button.gameObject.SetActive(false); }
                else { button.gameObject.SetActive(true); }
            }
        }

        uiEnabled = interactable;

        

    }

    public void AddListener_Battle_UIEnabler(UnityAction<bool, HeroType> listener)
    {
        uiEnablerEvent.AddListener(listener);
    }
}
