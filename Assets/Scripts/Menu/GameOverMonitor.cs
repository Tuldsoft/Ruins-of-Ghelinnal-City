using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverMonitor : MonoBehaviour
{
    [SerializeField]
    Image heroImage = null;

    [SerializeField]
    Text earningsText = null;

    [SerializeField]
    Text confirmButton = null;

    private void Start()
    {
        // pause the game as soon as the object is added to the scene
        //Time.timeScale = 0;

        //EventManager.AddListener_Battle_AddCoins(AddCoins);

        string message;
        Color color;

        // death
        if (BattleLoader.GameOver == GameOver.Death)
        {
            heroImage.sprite = BattleLoader.Party.Hero[0].Sprites.Dead;
            //BattleLoader.Party.Gold = Mathf.CeilToInt(BattleLoader.Party.Gold / 2);
            message = BattleLoader.Party.Hero[0].FullName + "'s party has died." +
                    "\n \n The werebears will resurrect you at the temple, using half of your gold.";
            color = Color.red;
        }
        else
        {
            
            if (BattleLoader.GameOver == GameOver.Win)
            {
                // Win
                heroImage.sprite = (BattleLoader.Party.Hero[0].Sprites as SabinSprites).Kick;
                //Time.timeScale = 0;
                message = BattleLoader.Party.Hero[0].FullName + "'s party has defeated the " +
                    BattleLoader.BattleParty.ToString().Replace("_", " ") + "!!";
                color = Color.green;
                confirmButton.text = "Continue";
            }
            else
            {
                // Boss Win
                heroImage.sprite = BattleLoader.Party.Hero[0].Sprites.Victory; 
                
                if (BattleLoader.DungeonLevel == 7)
                {
                    message = BattleLoader.Party.Hero[0].FullName + "'s party has defeated the " +
                    BattleLoader.BattleParty.ToString().Replace("_", " ") + "!!" +
                    "\n \n You've won the game! Head back to town to read the remaining story." +
                    " The shop is completely open to you now.";
                }
                else
                {
                    message = BattleLoader.Party.Hero[0].FullName + "'s party has defeated the " +
                    BattleLoader.BattleParty.ToString().Replace("_", " ") + "!!" +
                    "\n \n The tribe of werebears welcomes you back, and wants to tell you more.";
                }
                
                color = Color.green;

                if (BattleLoader.DungeonLevel >= BattleLoader.DungeonLevelAccess)
                {
                    BattleLoader.DungeonLevelAccess++; // this is clamped by BattleLoader.maxDungeonLevel
                }
            }
            
        }
        
        Text[] findGameOverText = gameObject.GetComponentsInChildren<Text>();
        for (int i = 0; i < findGameOverText.Length - 1; i++)
        {
            if (findGameOverText[i].name == "GameOverText")
            {
                findGameOverText[i].text = message;
                findGameOverText[i].color = color;
            }
        }

        earningsText.text = BattleLoader.BattleEarnings.ToString();

    }

    private void Update()
    {
        earningsText.text = BattleLoader.BattleEarnings.ToString();
    }

    /*void AddCoins(int coins)
    {
        BattleLoader.BattleEarnings += coins;
        earningsText.text = BattleLoader.BattleEarnings.ToString();
    }*/

    public void Click_Confirm()
    {
        AudioManager.Close(); 
        
        // unpause the game, destroy this menu, go to town menu
        Time.timeScale = 1;
        Destroy(gameObject);

        BattleLoader.Party.Gold += BattleLoader.BattleEarnings;
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

    /*public void Click_Quit()
    {
        AudioManager.Close(); 
        MenuManager.GoToMenu(MenuName.Quit);
    }*/
}
