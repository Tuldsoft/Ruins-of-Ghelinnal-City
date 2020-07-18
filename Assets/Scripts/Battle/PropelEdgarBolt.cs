using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A special type of PropelObject that is attached to each bolt fired 
/// from Edgar's AutoCrossbow tool. Includes a minor feature where the bolt sticks
/// into its target before being destroyed.
/// </summary>

public class PropelEdgarBolt : PropelObject
{
    Sprite halfBolt;           // sprite of the feathery end of a bolt
    EventTimer boltDeathTimer; // timer between impact and destroy()

    // Start is run before the GameObject's first frame update
    protected override void Start()
    {
        base.Start();

        // load halfBolt sprite
        halfBolt = Resources.Load<Sprite>(@"Sprites\Hero\EdgarBolt1");
        
        // set up timer
        boltDeathTimer = gameObject.AddComponent<EventTimer>();
        boltDeathTimer.Duration = 1.5f;
        boltDeathTimer.AddListener_Finished(DestroyBolt);
    }

    // Halts momentum (as per PropelObject), but also starts a timer before destruction
    protected override void HitTarget()
    {
        base.HitTarget();
        
        // Swap the sprite from full- to halfBolt
        gameObject.GetComponent<SpriteRenderer>().sprite = halfBolt;

        // Start timer for destruction
        boltDeathTimer.Run();
        
        // Halt movement
        isReturning = false;
        isPropelled = false;
        
        // Attach the bolt to the PropelObject.target, in case the target moves
        transform.SetParent(target.transform);
    }

    // Destroy the bolt at the end of boltDeathTimer
    void DestroyBolt()
    {
        Destroy(gameObject);
    }



}