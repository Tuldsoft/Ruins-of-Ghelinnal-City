using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A special PropelObject that triggers a fiery explosion upon impact, then destroys its gameObject.
/// </summary>
public class PropelFireball : PropelProjectile
{
    // In addition to launching towards its target, load the explosion for later and move slower
    protected override void Start()
    {
        base.Start();
        prefabAnimationObject = Resources.Load<GameObject>(@"Battleprefabs\Explosion");
        propelSpeedModifier = 0.4f;
    }
}
