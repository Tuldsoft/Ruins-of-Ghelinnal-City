using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyDeath : MonoBehaviour
{
    public int ID { get; set; }

    EventTimer enemyDeathTimer;
    float enemyDeathTimerDuration = 1.5f; // make sure this is greater than the coinspawner
    float enemyDeathFadePercentPerSecond;
    const float bossDeathTimerDuration = 4.0f;
    float opacityPercent = 1f;
    float redPercent = 1f;

    bool isDying;
    bool isBoss;

    SpriteRenderer spriteRenderer;

    Battle_EnemyDeathEndEvent enemyDeathEnd = new Battle_EnemyDeathEndEvent();

    // Start is called before the first frame update
    void Start()
    {
        // set up enemy death fade timer
        enemyDeathTimer = gameObject.AddComponent<EventTimer>();
        
        enemyDeathTimer.AddListener_Finished(EndDeath);

        // set up sprite renderer
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();

        EventManager.AddListener_Battle_EnemyDeathBegin(StartDeathTimer);
        EventManager.AddInvoker_Battle_EnemyDeathEnd(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (isDying)
        {
            float fadeAmount = enemyDeathFadePercentPerSecond * Time.deltaTime;
            opacityPercent = Mathf.Max(opacityPercent - fadeAmount, 0);
            redPercent = Mathf.Max(redPercent - 1.5f * fadeAmount, 0);

            float r=1f, g=1f, b=1f, a=1f;
                        
            if (isBoss)
            {
                g *= redPercent;
                b *= redPercent;
            }
            a *= opacityPercent;

            Color faded = new Color(r,g,b,a);
            spriteRenderer.color = faded;
        }
    }


    void StartDeathTimer(int id, bool boss)
    {
        if (this.ID == id)
        {
            isBoss = boss; 
            if (isBoss) { enemyDeathTimerDuration = bossDeathTimerDuration; }
            isDying = true; 
            
            enemyDeathTimer.Duration = enemyDeathTimerDuration;
            enemyDeathFadePercentPerSecond = 1 / enemyDeathTimerDuration;
            enemyDeathTimer.Run();
        }
    }

    void EndDeath()
    {
        enemyDeathEnd.Invoke(ID);
    }

    public void SetID(int enemyID)
    {
        ID = enemyID;
    }

    public void AddListener_Battle_EnemyDeathEnd(UnityAction<int> listener)
    {
        enemyDeathEnd.AddListener(listener);
    }

}
