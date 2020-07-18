using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// A derived version of this is attached to a prefabBattleSubMenu. Includes methods for 
/// On_Click() methods, and invokes a subMenuSelection event to tell the BattleManager what 
/// choice was selected. Variants include BlitzSubMenu, MagicSubMenu, and ToolsSubMenu.
/// </summary>
public abstract class BattleSubMenu : MonoBehaviour
{
    // invoker of subMenuSelection events
    Battle_SubMenuSelectionEvent subMenuSelection = new Battle_SubMenuSelectionEvent();


    public abstract BattleMode Mode { get; }

    // Start is called before the first frame update
    void Start()
    {
        EventManager.AddInvoker_Battle_SubMenuSelection(this);
    }

    // Called by derived class methods
    protected void CloseSubMenu()
    {
        subMenuSelection.Invoke(Mode);
        gameObject.SetActive(false);
    }

    // Called by the Back button
    public void Click_Back()
    {
        AudioManager.Close();
        gameObject.SetActive(false);
    }

    // invoker of subMenuSelection events
    public void AddListener_Battle_SubMenuSelection(UnityAction<BattleMode> listener)
    {
        subMenuSelection.AddListener(listener);
    }
}
