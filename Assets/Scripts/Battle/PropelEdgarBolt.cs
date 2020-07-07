using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropelEdgarBolt : PropelObject
{
    Sprite halfBolt;
    EventTimer boltDeathTimer;

    protected override void Start()
    {
        base.Start();
        halfBolt = Resources.Load<Sprite>(@"Sprites\Hero\EdgarBolt1");
        boltDeathTimer = gameObject.AddComponent<EventTimer>();
        boltDeathTimer.Duration = 1.5f;
        boltDeathTimer.AddListener_Finished(DestroyBolt);
    }

    protected override void HitTarget()
    {
        base.HitTarget();
        gameObject.GetComponent<SpriteRenderer>().sprite = halfBolt;
        boltDeathTimer.Run();
        isReturning = false;
        isPropelled = false;
        transform.SetParent(target.transform);
    }

    void DestroyBolt()
    {
        Destroy(gameObject);
    }



}