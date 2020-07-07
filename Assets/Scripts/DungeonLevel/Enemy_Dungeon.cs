using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Dungeon : MonoBehaviour
{
    public EnemyInfo info = new EnemyInfo();

    [SerializeField]
    public EnemyName enemyName;

    // Start is called before the first frame update
    void Start()
    {
        SetInfo();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Hero_Dungeon"))
        {
            SetInfo();
            info.IsDead = true;
            BattleLoader.UpdatePositions(collision.gameObject);
            BattleLoader.LoadBattle(info.EnemyName);
        }
    }

    public void SetInfo()
    {
        info.EnemyName = enemyName;
        info.Position = gameObject.transform.position;
    }
}
