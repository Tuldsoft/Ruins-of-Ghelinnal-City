using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A special PropelObject that triggers a poisony explosion upon impact, then destroys its gameObject.
/// </summary>
public class PropelPoison : PropelProjectile
{
    // In addition to launching towards its target, load the poison animation for later and move slower
    protected override void Start()
    {
        base.Start();
        prefabAnimationObject = Resources.Load<GameObject>(@"Battleprefabs\Poison");
        propelSpeedModifier = 0.4f;
    }
}
