using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroParty
{
    BattleHero[] heroes;
    public BattleHero[] Hero { get { return heroes; } }


    public int Gold { get; set; }
    
    public Inventory Inventory { get; set; } = new Inventory();

    public HeroParty(int numMembers)
    {
        heroes = new BattleHero[numMembers];
    }


    public void AddHero(BattleHero hero)
    {
        // first try to fill any null slots (ex. from Party creation)
        for (int i = 0; i < heroes.Length; i++)
        {
            if (heroes[i] == null)
            {
                heroes[i] = hero;
                return;
            }
        }

        // all slots are full, so check for four full slots
        if (heroes.Length >= 4) { Debug.Log("Already at max party capacity."); return; }

        // only continue if all slots are full and Length < 4

        // grow array
        BattleHero[] newArray = new BattleHero[heroes.Length + 1];

        for (int i = 0; i < heroes.Length; i++)
        {
            newArray[i] = heroes[i];

            /*if (heroes[i] == null)
            {
                heroes[i] = hero;
                break;
            }*/
        }
        newArray[newArray.Length - 1] = hero;

        heroes = newArray;

    }
    
}
