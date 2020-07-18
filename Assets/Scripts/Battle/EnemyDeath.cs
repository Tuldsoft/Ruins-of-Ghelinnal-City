using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Attached as a component to every enemy sprite object. Manipulates the sprite when it hears
/// the EnemyDeathBegin event, and concludes by invoking an EnemyDeathEnd event.
/// REFACTOR: AddComponent via code rather than at start, to eliminate unnecessary Update() calls
/// REFACTOR: Redo as a Coroutine, to eliminate unnecessary Update() calls
/// </summary>
/// 
public class EnemyDeath : MonoBehaviour
{
    #region Fields and Properties
    // BattleID
    public int ID { get; set; }

    // Timer fields
    EventTimer enemyDeathTimer;
    float enemyDeathTimerDuration = 1.5f; // Make sure this is greater than the coinspawner
    float enemyDeathFadePercentPerSecond;
    const float bossDeathTimerDuration = 4.0f;
    
    // Enemies start at 100 %, and fade until 0 %
    float opacityPercent = 1f;
    float redPercent = 1f;

    // isDying is false until initiated
    bool isDying;
    bool isBoss;

    // Reference to enemy sprite renderer (which handles color)
    SpriteRenderer spriteRenderer;

    // invoke end
    Battle_EnemyDeathEndEvent enemyDeathEnd = new Battle_EnemyDeathEndEvent();

    // Start is called before the first frame update
    void Start()
    {
        // set up enemy death fade timer
        enemyDeathTimer = gameObject.AddComponent<EventTimer>();
        enemyDeathTimer.AddListener_Finished(EndDeath);

        // set up sprite renderer
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();

        // Listen for Begin, ready to invoke End
        EventManager.AddListener_Battle_EnemyDeathBegin(StartDeathTimer);
        EventManager.AddInvoker_Battle_EnemyDeathEnd(this);
    }

    // Update is called once per frame
    void Update()
    {
        // Only execute once IsDying
        if (isDying)
        {
            // calculate red and opacity based on time passage since last frame
            float fadeAmount = enemyDeathFadePercentPerSecond * Time.deltaTime;
            opacityPercent = Mathf.Max(opacityPercent - fadeAmount, 0);
            redPercent = Mathf.Max(redPercent - 1.5f * fadeAmount, 0);

            // red, green, blue, alpha (opacity)
            float r=1f, g=1f, b=1f, a=1f;
                        
            // Bosses (only) turn red as they die
            // Reduce green and blue and leave red at 100%
            if (isBoss)
            {
                g *= redPercent;
                b *= redPercent;
            }

            // Reduce opacity
            a *= opacityPercent;

            // change renderer color to modified
            Color faded = new Color(r,g,b,a);
            spriteRenderer.color = faded;
        }
    }

    // Invoked by BattleManager, cues the start of the death sequence
    void StartDeathTimer(int id, bool boss)
    {
        // BattleManager invokes all EnemyDeath, so check that this is the correct one
        if (this.ID == id)
        {
            // use boss duration if boss, otherwise leave as normal
            isBoss = boss; 
            if (isBoss) { enemyDeathTimerDuration = bossDeathTimerDuration; }
            
            // enables Update()
            isDying = true; 
            
            // start the timer
            enemyDeathTimer.Duration = enemyDeathTimerDuration;
            enemyDeathFadePercentPerSecond = 1 / enemyDeathTimerDuration;
            enemyDeathTimer.Run();
        }
    }

    // Invoke EnemyDeathEnd event. Tells the BattleManager to remove this enemy and recalculate battleIDs.
    void EndDeath()
    {
        enemyDeathEnd.Invoke(ID);
    }

    // Used by BattleManager when reassigning IDs
    public void SetID(int enemyID)
    {
        ID = enemyID;
    }

    // Used to register listeners to this invoker
    public void AddListener_Battle_EnemyDeathEnd(UnityAction<int> listener)
    {
        enemyDeathEnd.AddListener(listener);
    }
    #endregion
}
