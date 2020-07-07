using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropelFireball : PropelObject
{
    GameObject prefabExplosion;

    protected override void Start()
    {
        base.Start();
        prefabExplosion = Resources.Load<GameObject>(@"Battleprefabs\Explosion");
        propelSpeedModifier = 0.4f;
    }

    protected override void HitTarget()
    {
        base.HitTarget();
        GameObject explosion = Instantiate<GameObject>(prefabExplosion);
        explosion.transform.position = gameObject.transform.position;
        Destroy(gameObject);
    }



}
