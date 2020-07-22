using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// A component class attached to anything that moves in battle, including heros,
/// enemies, spells, and coins. It is set in motion via the Propel() method, and
/// continues in motion while IsPropelled is true. It is (mostly) agnostic to what it
/// is attached to.
/// It defines a target collider and a target Bounds.
/// Speed changes based on Mode, sometimes with a constant here or with a child class.
/// </summary>

public class PropelObject : MonoBehaviour
{
    #region Fields and Properties

    // Speed fields
    protected const float PropelSpeed = 25f;
    protected float propelSpeedModifier = 1f; // Changed in child classes
    const float KickSpeedModifier = 0.65f;    // Overrides propelSpeedModifier
    const float DrillSpeedModifier = 0.65f;   // Overrides propelSpeedModifier

    // Targeting variables
    protected Vector2 startPosition;             // X, Y at start (for returning later)
    protected GameObject target;                 // GameObject of the target
    protected Collider2D targetCollider;         // Collider attached to the target
    protected Rigidbody2D rb2d;                  // rb2d of self gameObject
    protected Bounds targetLastPosition;         // a created area to act as a "net" for the moving object
    protected const float BoundsMargin = 0.5f;   // size of bounds
    protected int selfID = 0;                    // partyID or battleID of object, or the origin object
    public int BattleID { get
        {
            if (gameObject.CompareTag("Hero_Battle")){ return BattleMath.ConvertHeroID(selfID); }
            else { return selfID; }
        } }                                      // BattleID of connected gameObject
    protected int targetBattleID = 0;            // BattleID of target
    
    // Effect being used 
    protected BattleMode mode = BattleMode.Fight;

    // Event invoked by this class when the target has been hit. Handled by BattleManager.PropeltTargetHit()
    protected Battle_PropelHitTargetEvent battle_PropelHitTarget = new Battle_PropelHitTargetEvent();

    // Used in Update() for movement
    protected bool isPropelled = false;
    public bool IsPropelled { get { return isPropelled; } }

    // Used in Update() for returning movement
    protected bool isReturning = false;
    public bool IsReturning { get { return isReturning; } }

    #endregion

    #region Methods
    // Start is called before the first frame update
    protected virtual void Start()
    {
        // Store RigidBody2D of connected 
        rb2d = GetComponent<Rigidbody2D>();
        // PropelHitTarget lets BattleManager know to calculate and display damage, play explosion, etc.
        EventManager.AddInvoker_Battle_PropelHitTarget(this);
    }

    // Update is called once per frame
    protected void Update()
    {
        // Stay inert until isPropelled
        if (isPropelled)
        {
            // halt any existing movement
            rb2d.velocity = Vector2.zero;

            // store currentPosition
            Vector2 currentPosition = transform.position;

            // have target, will travel
            if (target != null && !isReturning)
            {
                // updates target position
                Vector2 targetPosition = target.transform.position;
                rb2d.velocity = targetPosition - currentPosition;
                targetLastPosition = new Bounds(targetPosition,
                    new Vector2(BoundsMargin, BoundsMargin));
            }
            else
            {
                // either target is null, or isReturning, so move towards the center of the Bounds
                // If isReturning, targetLastPosition has been created at startPosition
                rb2d.velocity = (Vector2)targetLastPosition.center - currentPosition;
            }

            // set speed while keeping direction
            rb2d.velocity = rb2d.velocity.normalized * PropelSpeed * propelSpeedModifier;

            // If the current position is within the target Bounds (ex. Coins)
            if (targetLastPosition.Contains(transform.position))
            {
                if (!isReturning)
                {
                    // Object has "hit" its target
                    HitTarget();
                }
                else
                {
                    // Object has arrived at startPosition, so halt Propel activity
                    rb2d.velocity = Vector2.zero;
                    rb2d.position = startPosition; // move from edge of Bounds to center
                    isPropelled = false;
                    isReturning = false;
                }
            }
        }
    }

    // If the target is a GameObject with a collider, this method is triggered instead of in Update()
    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        // Verify target
        if (collision == targetCollider && IsPropelled && !isReturning)
        {
            // Retrieve current ID of the target, in case it changed since
            // self was propelled. ID is current in the PropelObject of the hit target.

            PropelObject targetPropel;
            bool isOnGameObject = target.TryGetComponent<PropelObject>(out targetPropel);
            if (!isOnGameObject) { targetPropel = target.GetComponentInChildren<PropelObject>(); }
            targetBattleID = targetPropel.BattleID;

            HitTarget();
        }
    }

    // Invokes the PropelHitTarget event, and begins return trip.
    // Objects that do not return are destroyed in an override of this method
    protected virtual void HitTarget()
    {
        rb2d.velocity = Vector2.zero;
        targetLastPosition = new Bounds(startPosition,
                    new Vector2(BoundsMargin, BoundsMargin));
        isReturning = true;
        battle_PropelHitTarget.Invoke(BattleID, targetBattleID, mode);
        propelSpeedModifier = 1f;
        mode = BattleMode.none; 
    }


    /// <summary>
    /// Launch the attached GameObject at a target. Ex: a hero towards an enemy or vice versa.
    /// </summary>
    /// <param name="startPosition"> Start position of the propelled object</param>
    /// <param name="targetObj"> Target GameObject</param>
    /// <param name="targetID"> Target's partyID or battleID</param>
    public virtual void Propel (int battleID, Vector2 startPosition, GameObject targetObj, int targetID, BattleMode mode)
    {
        // Convert partyID to battleID if necessary
        selfID = gameObject.CompareTag("Hero_Battle") ? BattleMath.ConvertHeroID(battleID) : battleID;
        this.targetBattleID = targetID;
        
        // Set start position
        this.startPosition = startPosition;
        
        // Set target, and collicder if it exists
        target = targetObj;
        bool isOnGameObject = targetObj.TryGetComponent<Collider2D>(out targetCollider);
        if (!isOnGameObject) { targetCollider = targetObj.GetComponentInChildren<Collider2D>(); }

        // Create a bullseye-like target at the location of the target (in case there is no collider)
        targetLastPosition = new Bounds(target.transform.position,
            new Vector2(BoundsMargin, BoundsMargin));

        // Action being performed by the Propel
        this.mode = mode;

        // Modify speed if Kick or Drill (other effects use override methods to add here)
        switch (mode)
        {
            case BattleMode.Blitz_Kick:
                propelSpeedModifier = KickSpeedModifier;
                break;
            case BattleMode.Tools_Drill:
                propelSpeedModifier = DrillSpeedModifier;
                break;
            default:
                propelSpeedModifier = 1f;
                break;
        }       

        // Allow the Update() method to move this GameObject
        isPropelled = true;
    }

    // Used by BattleManager when IDs need to be reset (due to end of enemy death sequence)
    public void SetID (int id)
    {
        selfID = id;
    }

    // Lets others become listeners for this invocation
    public void AddListener_Battle_PropelHitTarget(UnityAction<int, int, BattleMode> listener)
    {
        battle_PropelHitTarget.AddListener(listener);
    }
    #endregion
}
