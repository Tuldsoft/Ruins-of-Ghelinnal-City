using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PropelObject : MonoBehaviour
{
    protected const float PropelSpeed = 25f;
    protected float propelSpeedModifier = 1f;
    const float KickSpeedModifier = 0.65f;
    const float DrillSpeedModifier = 0.65f;

    protected Vector2 startPosition;
    protected GameObject target;
    protected Collider2D targetCollider;
    protected Rigidbody2D rb2d;
    protected Bounds targetLastPosition;
    protected const float BoundsMargin = 0.5f;
    protected int selfID = 0; // ignored when attached to non-hero non-enemy objects
    protected int BattleID { get
        {
            if (gameObject.CompareTag("Hero_Battle")){ return BattleMath.ConvertHeroID(selfID); }
            else { return selfID; }
        } }
    protected int targetBattleID = 0;
    protected BattleMode mode = BattleMode.Fight;


    protected Battle_PropelHitTargetEvent battle_PropelHitTarget = new Battle_PropelHitTargetEvent();



    protected bool isPropelled = false;
    public bool IsPropelled { get { return isPropelled; } }

    protected bool isReturning = false;
    public bool IsReturning { get { return isReturning; } }



    // Start is called before the first frame update
    protected virtual void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();

        EventManager.AddInvoker_Battle_PropelHitTarget(this);

    }

    // Update is called once per frame
    protected void Update()
    {
        if (isPropelled)
        {
            rb2d.velocity = Vector2.zero;
            Vector2 currentPosition = transform.position;

            if (target != null && !isReturning)
            {
                Vector2 targetPosition = target.transform.position;
                rb2d.velocity = targetPosition - currentPosition;
                targetLastPosition = new Bounds(targetPosition,
                    new Vector2(BoundsMargin, BoundsMargin));
            }
            else
            {
                rb2d.velocity = (Vector2)targetLastPosition.center - currentPosition;
                
                // transform.position = startPosition;
                // isPropelled = false;
            }

            rb2d.velocity = rb2d.velocity.normalized * PropelSpeed * propelSpeedModifier;


            if (targetLastPosition.Contains(transform.position))
            {
                if (!isReturning)
                {
                    HitTarget();
                }
                else
                {
                    rb2d.velocity = Vector2.zero;
                    rb2d.position = startPosition; 
                    isPropelled = false;
                    isReturning = false;
                }
            }
        }
    }


    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == targetCollider && IsPropelled && !isReturning)
        {
            // retrieve current ID of the target,
            // in case it changed since self was propelled

            PropelObject targetPropel;
            bool isOnGameObject = target.TryGetComponent<PropelObject>(out targetPropel);
            if (!isOnGameObject) { targetPropel = target.GetComponentInChildren<PropelObject>(); }
            targetBattleID = targetPropel.BattleID;

            HitTarget();
        }
    }

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
    /// Send a hero or enemy at its foe
    /// </summary>
    /// <param name="startPosition"> Start position of the propelled object</param>
    /// <param name="targetObj"> Target object</param>
    /// <param name="targetID"> Target's numberer</param>
    public void Propel (int battleID, Vector2 startPosition, GameObject targetObj, int targetID, BattleMode mode)
    {
        selfID = gameObject.CompareTag("Hero_Battle") ? BattleMath.ConvertHeroID(battleID) : battleID;
        this.targetBattleID = targetID;
        this.startPosition = startPosition;
        target = targetObj;
        bool isOnGameObject = targetObj.TryGetComponent<Collider2D>(out targetCollider);
        if (!isOnGameObject) { targetCollider = targetObj.GetComponentInChildren<Collider2D>(); }

        targetLastPosition = new Bounds(target.transform.position,
            new Vector2(BoundsMargin, BoundsMargin));
        this.mode = mode;

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

        isPropelled = true;
    }

    public void SetID (int id)
    {
        selfID = id;
    }

    public void AddListener_Battle_PropelHitTarget(UnityAction<int, int, BattleMode> listener)
    {
        battle_PropelHitTarget.AddListener(listener);
    }
}
