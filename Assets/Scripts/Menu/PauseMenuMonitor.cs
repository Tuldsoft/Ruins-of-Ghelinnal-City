using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Attached to the prefabPauseMenu. Pauses the game and provides a few menu options.
/// Pause menu is called from the dungeonLevel, when Esc is pressed.
/// </summary>
public class PauseMenuMonitor : MonoBehaviour
{
    // Text field to display current party gold
    [SerializeField]
    Text partyGoldText = null;

    // Start is called before the first frame update
    private void Start()
    {
        // Pause the game as soon as the menu is added to the scene
        Time.timeScale = 0;
        partyGoldText.text = BattleLoader.Party.Gold.ToString();
    }

    // The ResumeButton On_Click() closes the menu and unpauses the game
    public void Click_Resume()
    {
        AudioManager.Close();
        // unpause the game and destroy menu
        Time.timeScale = 1;
        Destroy(gameObject);
    }

    // Calls the ManagePartyMenu, so that equipment can be adjusted and consumables used.
    public void Click_ManageParty()
    {
        AudioManager.Chirp();
        GameObject manageMenu = GameObject.Instantiate(
                    Resources.Load<GameObject>(@"MenuPrefabs\prefabManagePartyMenu"));
        manageMenu.GetComponentInChildren<Canvas>().worldCamera = Camera.main;
        manageMenu.GetComponent<ManagePartyMenuMonitor>().SetPauseMenu(this.gameObject);
        gameObject.SetActive(false);
    }

    // Calls the Help manu when Help button On_Click()
    public void Click_Help()
    {
        AudioManager.Chirp();
        MenuManager.GoToMenu(MenuName.Help);
    }

    // Leaves the dungeonLevel and returns to town. 
    // This is the main mechanism for earning gold when a death in the dungeon is imminent.
    public void Click_ReturnToTown()
    {
        AudioManager.Close();
        // Unpause the game, destroy this menu, go to town menu
        Time.timeScale = 1;
        Destroy(gameObject);
        MenuManager.GoToMenu(MenuName.BattleToTown);
    }

    // Quit Button On_Click(), exits the game entirely
    public void Click_Quit()
    {
        AudioManager.Close();
        MenuManager.GoToMenu(MenuName.Quit);
    }
}
