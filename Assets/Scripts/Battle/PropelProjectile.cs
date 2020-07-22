using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stores common traits of both Fireball and Poisonball projectiles, including the 
/// PropelObject of the caster. This can be referenced later, in case selfID needs to change,
/// due to a death since the casting.
/// Inherited by PropelFireball and PropelPoison.
/// REFACTOR: When Combatant or Battler is made, use a reference to that instead of 
/// the caster's PropelObject.
/// </summary>
public class PropelProjectile : PropelObject
{
    // prefab of the animation object, including animation. Ex. prefabPoison or Fireball
    protected GameObject prefabAnimationObject;

    // For retrieving a current BattleID
    PropelObject casterPropelObject = null;
    
    // Stores the caster as well as the usual Propel()
    public void PropelShoot(int battleID, Vector2 startPosition, PropelObject casterPropel, GameObject targetObj, int targetID, BattleMode mode)
    {
        Propel(battleID, startPosition, targetObj, targetID, mode);
        casterPropelObject = casterPropel;
    }

    // Upon impact, create a poison animation and destroy self
    protected override void HitTarget()
    {
        base.HitTarget();
        if (casterPropelObject != null)
        {
            GameObject projectile = Instantiate<GameObject>(prefabAnimationObject);
            projectile.transform.position = gameObject.transform.position;
        }
        Destroy(gameObject);
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        // Verify selfID
        if (BattleID != casterPropelObject.BattleID) 
        { 
            selfID = casterPropelObject.BattleID; 
        }

        base.OnTriggerEnter2D(collision);
    }
}
