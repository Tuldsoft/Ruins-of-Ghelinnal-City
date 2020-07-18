using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A special PropelObject that triggers a poisony explosion upon impact, then destroys its gameObject.
/// </summary>
public class PropelPoison : PropelObject
{
    // prefabPoison, including animation
    GameObject prefabPoison;

    // In addition to launching towards its target, load the poison animation for later and move slower
    protected override void Start()
    {
        base.Start();
        prefabPoison = Resources.Load<GameObject>(@"Battleprefabs\Poison");
        propelSpeedModifier = 0.4f;
    }

    // Upon impact, create a poison animation and destroy self
    protected override void HitTarget()
    {
        base.HitTarget();
        GameObject poison = Instantiate<GameObject>(prefabPoison);
        poison.transform.position = gameObject.transform.position;
        Destroy(gameObject);
    }
}
