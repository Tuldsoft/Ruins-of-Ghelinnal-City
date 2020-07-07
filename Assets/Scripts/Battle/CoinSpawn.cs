using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CoinSpawn : MonoBehaviour
{
    Battle_AddCoinsEvent addCoins = new Battle_AddCoinsEvent();
    
    public int Value { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        EventManager.AddInvoker_Battle_AddCoins(this);
    }

    
    
    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "BankTarget")
        {
            addCoins.Invoke(Value);
            Destroy(gameObject);
        }
    }

    public void AddListener_Battle_AddCoins(UnityAction<int> listener)
    {
        addCoins.AddListener(listener);
    }
}
