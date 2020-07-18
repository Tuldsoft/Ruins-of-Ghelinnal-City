using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Attached to the prefabDungeonNode, a simple button. This script identifies which button is being clicked.

public class DungeonNode : MonoBehaviour
{
    // Level is set by DungeonMenuMonitor in SetLevel()
    [SerializeField]
    public int level;

    // SetLevel is set by DungeonMenuMonitor
    public void SetLevel(int level)
    {
        this.level = level;
    }

    // Called by the button's On_Click() method
    public void Click_DungeonLevelNode()
    {
        AudioManager.Chirp();
        MenuManager.EnterDungeonLevel(this.level, true);
    }
}
