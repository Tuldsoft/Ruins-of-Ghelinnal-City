using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuMonitor : MonoBehaviour
{
    [SerializeField]
    Text partyGoldText = null;

    private void Start()
    {
        // pause the game as soon as the object is added to the scene
        Time.timeScale = 0;
        partyGoldText.text = BattleLoader.Party.Gold.ToString();
    }

    public void Click_Resume()
    {
        AudioManager.Close();
        // unpause the game and destroy menu
        Time.timeScale = 1;
        Destroy(gameObject);
    }

    public void Click_ManageParty()
    {
        AudioManager.Chirp();
        GameObject manageMenu = GameObject.Instantiate(
                    Resources.Load<GameObject>(@"MenuPrefabs\prefabManagePartyMenu"));
        manageMenu.GetComponentInChildren<Canvas>().worldCamera = Camera.main;
        manageMenu.GetComponent<ManagePartyMenuMonitor>().SetPauseMenu(this.gameObject);
        gameObject.SetActive(false);
    }
        
    public void Click_Help()
    {
        AudioManager.Chirp();
        MenuManager.GoToMenu(MenuName.Help);
    }

    public void Click_ReturnToTown()
    {
        AudioManager.Close();
        // unpause the game, destroy this menu, go to town menu
        Time.timeScale = 1;
        Destroy(gameObject);
        MenuManager.GoToMenu(MenuName.BattleToTown);
    }

    public void Click_Quit()
    {
        AudioManager.Close();
        MenuManager.GoToMenu(MenuName.Quit);
    }
}
