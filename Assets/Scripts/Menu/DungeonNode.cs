using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonNode : MonoBehaviour
{
    [SerializeField]
    public int level;
    

    public void SetLevel(int level)
    {
        this.level = level;
    }

    public void Click_DungeonLevelNode()
    {
        AudioManager.Chirp();
        MenuManager.EnterDungeonLevel(this.level, true);
    }
}
