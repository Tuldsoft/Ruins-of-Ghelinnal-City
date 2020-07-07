using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropelPoison : PropelObject
{
    GameObject prefabPoison;

    protected override void Start()
    {
        base.Start();
        prefabPoison = Resources.Load<GameObject>(@"Battleprefabs\Poison");
        propelSpeedModifier = 0.4f;
    }

    protected override void HitTarget()
    {
        base.HitTarget();
        GameObject poison = Instantiate<GameObject>(prefabPoison);
        poison.transform.position = gameObject.transform.position;
        Destroy(gameObject);
    }
}
