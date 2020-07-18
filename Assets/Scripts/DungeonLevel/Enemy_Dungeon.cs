using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Attached to every enemy Sprite object in a dungeon scene. Identified by EnemyName,
/// set in the inspector when placed in the scene. Contains EnemyInfo and IsDead, and
/// handles collision events with the Hero.
/// </summary>
public class Enemy_Dungeon : MonoBehaviour
{
    #region Fields
    // A container class containing ID, name, position, and isdead
    public EnemyInfo info = new EnemyInfo();

    // Name of the enemy, set in the inspector when placed in the scene
    [SerializeField]
    public EnemyName enemyName;
    #endregion

    #region Methods
    // Start is called before the first frame update
    void Start()
    {
        // Set name and position in EnemyInfo
        SetInfo();
    }
    

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Hero_Dungeon"))
        {
            // Refresh info
            SetInfo();

            // Label as dead for the next reload of the scene
            info.IsDead = true;

            // Gather current status of all enemies in the dungeon scene, for reload later
            BattleLoader.UpdatePositions(collision.gameObject);

            // Load a battle based on the name of the creature encountered in the dungeon
            BattleLoader.LoadBattle(info.EnemyName);
        }
    }

    // Set EnemyInfo. Called by this class and by BattleLoader.
    public void SetInfo()
    {
        info.EnemyName = enemyName;
        info.Position = gameObject.transform.position;
    }
    #endregion
}
