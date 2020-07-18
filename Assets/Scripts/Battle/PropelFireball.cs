using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A special PropelObject that triggers a fiery explosion upon impact, then destroys its gameObject.
/// </summary>
public class PropelFireball : PropelObject
{
    // prefabExplosion, including animation
    GameObject prefabExplosion;

    // In addition to launching towards its target, load the explosion for later and move slower
    protected override void Start()
    {
        base.Start();
        prefabExplosion = Resources.Load<GameObject>(@"Battleprefabs\Explosion");
        propelSpeedModifier = 0.4f;
    }

    // Upon impact, create an explosion and destroy self
    protected override void HitTarget()
    {
        base.HitTarget();
        GameObject explosion = Instantiate<GameObject>(prefabExplosion);
        explosion.transform.position = gameObject.transform.position;
        Destroy(gameObject);
    }



}
