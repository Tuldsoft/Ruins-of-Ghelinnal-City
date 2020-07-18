using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Attached to the main camera in every DungeonLevel scene. Calls BattleLoader.LoadLevel()
/// when created, both on Load and Reload
/// </summary>
public class DungeonLevelMonitor : MonoBehaviour
{
    #region Fields
    // Set in the inspector, this identifies which dungeon level BattleLoader is to create.
    [SerializeField]
    public int dungeonLevel = 1;

    // Set in the inspector, contains a reference to the text label for displaying party gold
    [SerializeField]
    Text partyGoldText = null;
    #endregion

    #region Methods
    // Awake runs prior to any Start() method. This runs the Initializer, in case it has not been run yet.
    private void Awake()
    {
        Initializer.Run();
    }

    // Start is called before the first frame update
    void Start()
    {
        // Load or Reload this dungeon
        BattleLoader.LoadLevel(dungeonLevel);

        // Set gold display
        partyGoldText.text = BattleLoader.Party.Gold.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        // Brings up the Pause menu when pressing Esc
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameObject.FindGameObjectWithTag("PauseMenu") == null)
            {
                MenuManager.GoToMenu(MenuName.Pause);
            }
        }
    }
    #endregion
}
