using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Attached to the Main Menu. Provides methods for button clicks, and runs the Initializer.
/// The Main Menu is the starting scene, so this call to the Initializer is the one run
/// when compiled. However, if editing, Initializer can be run by the main scene manager
/// for any other scene. Initializer is only run once.
/// The Main Menu provides buttons for New Game, Load Game, Options (cheats and sound),
/// Help, About, and Quit.
/// </summary>
public class MainMenuMonitor : MonoBehaviour
{
    #region UnityMethods
    // Awake() is called before any Start() method
    private void Awake()
    {
        Initializer.Run();
    }

    // Start() is called before the first frame update
    private void Start()
    {
        // Play intro music
        AudioManager.Intro();
    }
    #endregion
    #region Click_ Methods
    // Each of these provides the behavior when its button is clicked (On_Click())
    
    // Launches a NewGame by going straight into town with the default setup
    public void Click_NewGame()
    {
        Initializer.ResetData();
        AudioManager.Chirp();
        MenuManager.GoToMenu(MenuName.MainToTown);
    }

    // Opens a File menu (Load Game)
    public void Click_LoadGame()
    {
        AudioManager.Chirp();
        MenuManager.GoToFileMenu(false);
    }

    // Opens the Options menu for player-based sound and cheat preferences
    public void Click_Options()
    {
        AudioManager.Chirp();
        MenuManager.GoToMenu(MenuName.MainOptions);
    }

    // Opens the Help menu for Useful Information (TM)
    public void Click_Help()
    {
        AudioManager.Chirp();
        MenuManager.GoToMenu(MenuName.Help);
    }

    // Opens the About Menu to show version updates, etc.
    public void Click_About()
    {
        AudioManager.Chirp();
        MenuManager.GoToMenu(MenuName.About);
    }

    // Closes the program (does not work while in editor)
    public void Click_Quit()
    {
        AudioManager.Close();
        MenuManager.GoToMenu(MenuName.Quit);
    }
    #endregion
}
