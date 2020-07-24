using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Attached to the prefabGameOverMenu, which either brings the player back to town or
/// back to the dungeon level. GameOver accomodates Win, BossWin, and Death.
/// </summary>
public class GameOverMonitor : MonoBehaviour
{
    #region Fields
    // Image of victory pose or death or kick
    [SerializeField]
    Image heroImage = null;

    // Label displaying party gold 
    [SerializeField]
    Text earningsText = null;

    // OK button
    [SerializeField]
    Text confirmButton = null;
    #endregion

    #region Methods
    // Start() is called before the first frame update
    private void Start()
    {
        // Do not pause so that coins continue being cashed in
        // pause the game as soon as the object is added to the scene
        //Time.timeScale = 0;

        // Message to display and its color
        string message;
        Color color;

        // Death (Battle or Dungeon)
        if (BattleLoader.GameOver == GameOver.Death)
        {
            heroImage.sprite = BattleLoader.Party.Hero[0].Sprites.Dead;
            message = BattleLoader.Party.Hero[0].FullName + "'s party has died." +
                    "\n \n The werebears will resurrect you at the temple, using half of your gold.";
            color = Color.red;
        }
        else
        {
            if (BattleLoader.GameOver == GameOver.Win)
            {
                // Ragular Win
                heroImage.sprite = (BattleLoader.Party.Hero[0].Sprites as SabinSprites).Kick;
                message = BattleLoader.Party.Hero[0].FullName + "'s party has defeated the " +
                    BattleLoader.BattleParty.ToString().Replace("_", " ") + "!!";
                color = Color.green;
                confirmButton.text = "Continue";
            }
            else
            {
                // Boss Win
                heroImage.sprite = BattleLoader.Party.Hero[0].Sprites.Victory; 
                
                if (BattleLoader.DungeonLevel == 7) // Game win if Dungeon 7 complete
                {
                    message = BattleLoader.Party.Hero[0].FullName + "'s party has defeated the " +
                    BattleLoader.BattleParty.ToString().Replace("_", " ") + "!!" +
                    "\n \n You've won the game! Head back to town to read the remaining story." +
                    " The shop is completely open to you now.";
                }
                else // Back to town
                {
                    message = BattleLoader.Party.Hero[0].FullName + "'s party has defeated the " +
                    BattleLoader.BattleParty.ToString().Replace("_", " ") + "!!" +
                    "\n \n The tribe of werebears welcomes you back, and wants to tell you more.";
                }
                
                color = Color.green;

                // Increase access level for next dungeon delve
                if (BattleLoader.DungeonLevel >= BattleLoader.DungeonLevelAccess)
                {
                    BattleLoader.DungeonLevelAccess++; // this is clamped by BattleLoader.maxDungeonLevel
                }
            }
        }
        
        // Retrieve label
        Text[] findGameOverText = gameObject.GetComponentsInChildren<Text>();
        for (int i = 0; i < findGameOverText.Length - 1; i++)
        {
            if (findGameOverText[i].name == "GameOverText")
            {
                findGameOverText[i].text = message;
                findGameOverText[i].color = color;
            }
        }

        // Set display text
        earningsText.text = BattleLoader.BattleEarnings.ToString();
    }

    // Called once per frame
    private void Update()
    {
        // Continue updating earning while coins are still flying in battle scene
        earningsText.text = BattleLoader.BattleEarnings.ToString();
    }

    // Called by the only available button, On_Click()
    public void Click_Confirm()
    {
        AudioManager.Close(); 
        
        // unpause the game, destroy this menu, go to town menu
        Time.timeScale = 1; // time is no longer paused, but just in case
        Destroy(gameObject);

        // Add earning gold from a battle to party 
        BattleLoader.Party.Gold += BattleLoader.BattleEarnings;
        
        // Bring any dead heroes to 1 hp
        ReviveHeroesFromDead();

        // Go to next thing
        if (BattleLoader.GameOver == GameOver.BossWin)
        {
            MenuManager.GoToMenu(MenuName.BattleToTown);
        }
        else if (BattleLoader.GameOver == GameOver.Death)
        {
            // death
            BattleLoader.Party.Gold = Mathf.CeilToInt(BattleLoader.Party.Gold / 2);
            MenuManager.GoToMenu(MenuName.BattleToTown);
        }
        else // non-boss Win
        {
            BattleLoader.GameOver = null;
            MenuManager.EnterDungeonLevel(BattleLoader.DungeonLevel, false);
        }
    }

    // At the end of a battle, make sure everyone has at least 1 hp
    void ReviveHeroesFromDead()
    {
        foreach (BattleHero hero in BattleLoader.Party.Hero)
        {
            if (hero.HP <= 0)
            {
                hero.TakeDamage(-1);
            }
        }
    }
    #endregion
}
