using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Attached to a CoinSpawn gameObject (a spawned coin on its way to the bank)
public class CoinSpawn : MonoBehaviour
{
    // invokes the AddCoinsEvent, which adds the value of this coin to the bank
    Battle_AddCoinsEvent addCoins = new Battle_AddCoinsEvent();
    
    // Value of this coin object (gold)
    public int Value { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        EventManager.AddInvoker_Battle_AddCoins(this);
    }

    // when colliding with the BankTarget object, add the coin's value to the bank
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "BankTarget")
        {
            addCoins.Invoke(Value);
            Destroy(gameObject);
        }
    }

    // used as an invoker to the AddCoins event
    public void AddListener_Battle_AddCoins(UnityAction<int> listener)
    {
        addCoins.AddListener(listener);
    }
}
