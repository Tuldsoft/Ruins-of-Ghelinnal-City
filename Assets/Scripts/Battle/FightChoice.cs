using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// attached to the Fight menu option and to every enemy, if clicked while enabled
// has EnemyChoice as a child class, so that only one OnMouseUpAsButton method is called,
// and so that "enabled" can stay local to that
public class FightChoice : MonoBehaviour
{
    protected Battle_FightSelectionEvent battle_FightSelectionEvent = new Battle_FightSelectionEvent();

    // Start is called before the first frame update
    protected virtual void Start()
    {
        EventManager.AddInvoker_Battle_FightChoice(this);
    }

   
    // See EnemyChoice
    /*private void OnMouseUpAsButton()
    {
        if (enemyChoice.enabled)
        {
            Click_Fight();
        }
    }*/


    // when either monster is clicked a second time
    public void Click_Fight ()
    {
        AudioManager.Chirp();
        // invoke Battle_FightSelectionEvent
        battle_FightSelectionEvent.Invoke();

    }

    public void AddListener_FightChoice(UnityAction listener)
    {
        battle_FightSelectionEvent.AddListener(listener);
    }
}
