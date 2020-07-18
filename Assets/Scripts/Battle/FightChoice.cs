using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Attached to the Fight menu option. When enabled and clicked, will cause the current
/// hero to attack the selected enemy (or enemy[0] if none selected). Selection is stored
/// in BattleManager. 
/// EnemyChoice is a variant of FightChoice which is attached to each enemy sprite and 
/// EnemyChoiceText. It handles the selection mechanic.
/// </summary>
public class FightChoice : MonoBehaviour
{
    #region Fields

    // Invoker of FightSelection
    protected Battle_FightSelectionEvent battle_FightSelectionEvent = new Battle_FightSelectionEvent();

    #endregion

    #region Methods
    // Start is called before the first frame update
    protected virtual void Start()
    {
        EventManager.AddInvoker_Battle_FightChoice(this);
    }

    // FightText On_Click() method. Also called by EnemyChoice's "click" method
    public void Click_Fight ()
    {
        AudioManager.Chirp();
        
        // Invoke Battle_FightSelectionEvent to start fight action
        battle_FightSelectionEvent.Invoke();
    }

    // Lets other listen for its invocation
    public void AddListener_FightChoice(UnityAction listener)
    {
        battle_FightSelectionEvent.AddListener(listener);
    }
    #endregion
}
