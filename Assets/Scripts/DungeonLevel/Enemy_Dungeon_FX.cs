using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Attached to certain enemy prefabs. Controls the behavior of dungeon enemies, such
/// as flying, chasing, etc. Most of the behavior is set in the Inspector when the 
/// enemy is placed in the scene.
/// 
/// A copy of this is stored in BattleLoader during battle. It later re-attaches it to
/// newly-created enemies on reload, so that their behavior matches those from before the battle.
/// 
/// Export and Store variables are used to recreate the FX during unload and load.
/// REFACTOR: Create an FX state class to store the current state, rather than a List<object>
/// </summary>
public class Enemy_Dungeon_FX : MonoBehaviour
{
    #region Fields
    // Whether or not to flip the sprite on start (sprite facing right or left)
    [SerializeField]
    bool flipOnStart = false;

    // Whether or not the enemy "rolls" (dynamic Rigidbody2D, etc.)
    [SerializeField]
    bool rollable = false;
    
    // Whether the enemy will explode a certain while after creation
    [SerializeField]
    public bool explodable = false;
    [SerializeField]
    float timerDuration = 3f;
    
    // Whether the creature paces right and left, and which direction it faces
    [SerializeField]
    bool pacing = false;
    [SerializeField]
    float pacingPace = 1.5f;
    bool flipX = true;

    // Whether the creater paces up and down, and the direction it is moving
    [SerializeField]
    bool verticalPacing = false;
    bool ascend = true;

    // Whether the creature will charge at the hero
    [SerializeField]
    bool chargable = false;
    [SerializeField]
    float chargingPace = 7f;
    
    // Whether or not the creature is flying in the air (disable gravity/mass)
    [SerializeField]
    public bool isFlying = false;

    // Whether the creature will periodically jump in the air
    [SerializeField]
    bool jumper = false;
    [SerializeField]
    float jumpForce = 7f, jumpForceVariance = 0.5f;
    [SerializeField]
    float jumpInterval = 4f, jumpIntervalVariance = 0.5f;
    EventTimer jumpTimer;

    // Local state variables for charging function
    bool isCharging = false;
    bool isReturning = false;
    float enemyMaxHalfWidth = 0f;     // used for calcuating size
    CircleCollider2D homeLocation;    // a collider created for detecting when an enemy is "home"
    Vector2 homePosition;             // a creature's original location (for charging)
    CircleCollider2D targetLocation;  // a collider created at the destination a creature charges to
    Vector2 targetPosition;           // the position of a charger's target
    EdgeCollider2D targetingScope;    // a collider used for Line-of-sight, triggers charge when crossed

    
    // Body and collider of the enemy
    Rigidbody2D rb2d;
    PolygonCollider2D pc2d;

    // Timer for explosions
    EventTimer explosionTimer;
    // Animated object produced in an explosion
    GameObject prefabExplosion;
    #endregion

    #region Methods
    // Start is called before the first frame update
    void Start()
    {
        // Store local objects and state
        rb2d = GetComponent<Rigidbody2D>();
        pc2d = GetComponent<PolygonCollider2D>();
        flipX = gameObject.GetComponent<SpriteRenderer>().flipX;

        // Create a friction-less material for pacing
        PhysicsMaterial2D zeroFrictionMaterial = new PhysicsMaterial2D();
        zeroFrictionMaterial.friction = 0f;

        // Flip if flipX
        if (flipOnStart) { FlipX(); }

        // Enable pacing in a random direction
        if (pacing)
        {
            bool flip = Random.value > 0.5f ? true : false;
            if (flip) { FlipX(); }
            rb2d.sharedMaterial = zeroFrictionMaterial;
            rb2d.gravityScale = 0;
        }
        
        // Enable vertical pacing in a random direction
        if (verticalPacing) {ascend = Random.value > 0.5f ? true : false; }

        // Set up Rigidbody2D for rolling, pick a random initial rotation, and enable
        if (rollable)
        {
            rb2d.constraints = RigidbodyConstraints2D.None;
            float rotation = Random.Range(0f, 360f);
            Vector3 rotateVector = new Vector3(0, 0, rotation);
            transform.Rotate(rotateVector);
        }

        // Store a prefab to create at the end of the timer, and start timer
        if (explodable)
        {
            prefabExplosion = Resources.Load<GameObject>(@"DungeonPrefabs\Explosion");

            explosionTimer = gameObject.AddComponent<EventTimer>();
            explosionTimer.Duration = timerDuration;
            explosionTimer.Run();
            explosionTimer.AddListener_Finished(Explode);
        }

        // Set up charging
        if (chargable)
        {
            // Get a max halfWidth. This is the farthest polygon point from center.
            foreach (Vector2 point in pc2d.points)
            {
                float pointX = Mathf.Abs(point.x) * transform.localScale.x;
                enemyMaxHalfWidth = pointX > enemyMaxHalfWidth ? pointX : enemyMaxHalfWidth;
            }
            // Store reference to attached EdgeCollider2D
            targetingScope = GetComponent<EdgeCollider2D>();
            // Disable the scope while charging, enable otherwise
            targetingScope.enabled = !isCharging;
        }

        // Set up jumping
        if (jumper)
        {
            // Set timer for the next jump to a random interval, then start the timer
            jumpTimer = gameObject.AddComponent<EventTimer>();
            jumpTimer.Duration = Random.Range(jumpInterval - jumpIntervalVariance,
                jumpInterval + jumpIntervalVariance);
            jumpTimer.Run();
            jumpTimer.AddListener_Finished(Jump);
        }
    }


    // FixedUpdate is called once per fixed interval
    void FixedUpdate()
    {
        // if flipX == true, then facing left
        
        // Move sideways
        if (pacing || isFlying)
        {
            int direction = flipX ? -1 : 1;
                        
            // locomotion - flat surfaces only
            
            rb2d.velocity = new Vector2((float)direction * pacingPace, 0);
        }

        // Move vertically
        if (verticalPacing)
        {
            int direction = ascend ? 1 : -1; 
            rb2d.velocity = new Vector2(0, (float)direction);
        }

        // If charging, move to the target
        if (isCharging)
        {
            // Set normalized direction towards the target
            Vector2 direction = new Vector2(targetPosition.x - transform.position.x,
                0).normalized;

            // Move to target
            rb2d.velocity = direction * chargingPace;
            
            // Create home or destination targets if null (ex: on reload)
            if (targetLocation == null || homePosition == null)
            {
                targetLocation = MakeTarget(targetPosition);
                homeLocation = MakeTarget(homePosition);
            }
        }

        // After charging, an enemy isReturning back to its homePosition
        if (isReturning)
        {
            // Set normalized position towards the home position
            Vector2 direction = new Vector2(homePosition.x - transform.position.x,
                0).normalized;

            // Move to home
            rb2d.velocity = direction * pacingPace;

            // Create home or destination targets if null (ex: on reload)
            if (targetLocation == null || homePosition == null)
            {
                targetLocation = MakeTarget(targetPosition);
                homeLocation = MakeTarget(homePosition);
                // isReturning is in the opposite direction than charging. Flip the sprite.
                FlipX();
            }
        }

    }

    /// <summary>
    /// Several effects can happen when an enemy collides with something. This determines the result.
    /// Collision objects might include "target" colliders created by this script, the hero's collider,
    /// edge colliders used for LOS, and edge colliders to destroy off-camera objects.
    /// 
    /// A special type of EdgeCollider2D with the tag "PacingBoundary_Dungeon" is set up in the editor
    /// as an invisible (to the user) boundary object. It simply reverses the direction of the pacing
    /// enemy, so that it moves back in forth in a defined area. They (obviously) are used in pairs.
    /// 
    /// This is for effects only. A battle-triggering collision between enemy and hero is in EnemyDungeon.
    /// </summary>
    /// <param name="collision">The collider encountered</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        
        // For when an enemy flies out of the scene
        if (isFlying && collision.gameObject.CompareTag("KillBoundary_Dungeon"))
        {
            Destroy(gameObject);
        }
        
        // Reverse direction when hitting a boundary collider
        if (pacing && collision.gameObject.CompareTag("PacingBoundary_Dungeon"))
        {
            // turn around when hitting an edgecollider
            FlipX();
        }

        // Reverse direction when hitting a boundary collider
        if (verticalPacing && collision.gameObject.CompareTag("PacingBoundary_Dungeon"))
        {
            // turn around when hitting an edgecollider
            ascend = !ascend;
        }

        // This is triggered if an enemy's targeting scope EdgeCollider is hit by the hero,
        // and the charger is currently motionless
        if (chargable && collision.gameObject.CompareTag("Hero_Dungeon") && !isCharging && !isReturning)
        {
            // Enable charging during FixedUpdate() and disable the scope
            isCharging = true;
            targetingScope.enabled = false;
            
            // Set up a home target collider half a width away from current position
            Vector2 position = gameObject.transform.position;
            position.x += enemyMaxHalfWidth * (flipX ? -1 : 1); //flipx false (positive) is (right)
            homePosition = position;
            homeLocation = MakeTarget(position);

            // Set up a destination target collider at the hero's position at this moment
            position = collision.gameObject.transform.position;
            position.y = homeLocation.transform.position.y;
            targetPosition = position;
            targetLocation = MakeTarget(position);
        }

        // If the collision is with the target destination, flip and go home
        if (isCharging && collision == targetLocation)
        {
            isCharging = false;
            targetingScope.enabled = true;
            isReturning = true;
            FlipX();
        }

        // if the collision is with the home target, stop, turn the scope back on, and flip
        if (collision == homeLocation && !isCharging)
        {
            isReturning = false;
            Destroy(targetLocation);
            Destroy(homeLocation);
            FlipX();
        }

        // Any collision with something called Kill will kill. Ex: lava tiles
        if (collision.gameObject.name == "Kill")
        {
            // enemy is out of bounds
            Destroy(gameObject);
        }

        
    }

    // Creates a tiny, circular, bulls-eye-like target at the position.
    CircleCollider2D MakeTarget(Vector2 position)
    {
        CircleCollider2D target = new GameObject().AddComponent<CircleCollider2D>();
        target.transform.position = position;
        target.isTrigger = true;
        target.radius = 0.1f;
        return target;
    }

    // Flip the facing of the attached sprite
    void FlipX()
    {
        // Flip the image
        flipX = !flipX;
        gameObject.GetComponent<SpriteRenderer>().flipX = flipX;
        
        // Flip the polygon collider
        if (pc2d == null) 
        {
            pc2d = gameObject.GetComponent<PolygonCollider2D>(); 
        }
        pc2d.points = FlipXPoints(pc2d.points);

        // Flip "scope" colliders
        if (chargable)
        {
            EdgeCollider2D ec2d = gameObject.GetComponent<EdgeCollider2D>();
            ec2d.points = FlipXPoints(ec2d.points);
        }
    }

    // Swaps the X value of all points in a polygon collider
    Vector2[] FlipXPoints (Vector2[] points)
    {
        for (int i = 0; i < points.Length; i++)
        {
            points[i].x *= -1;
        }
        return points;
    }

    // OnBecameInvisible() cannot be used. It doesn't work when visible in the editor (but 
    // not the camera), and often fails to trigger entirely. This is unused.
    /*private void OnBecameInvisible()
    {
        if (explodable)
        {
            explosionTimer.Stop();
        }
    }*/

    // Destroys the object and puts an explosion prefab animation in its place.
    // Called at the end of an explosion timer.
    void Explode()
    {
        GameObject explosion = GameObject.Instantiate(prefabExplosion);
        explosion.transform.position = transform.position;
        Destroy(gameObject);
    }

    // Used by jumpers, pushes the enemy in the air. Called by a randomized jump timer.
    void Jump()
    {
        rb2d.AddForce(Vector2.up * jumpForce
            * Random.Range(1 - jumpForceVariance, 1 + jumpForceVariance),ForceMode2D.Impulse);
        jumpTimer.Duration = Random.Range(jumpInterval - jumpIntervalVariance,
                jumpInterval + jumpIntervalVariance);
        jumpTimer.Run();
    }

    // Exports a list of objects of the current state of the enemy
    public List<object> ExportVariables ()
    {
        List<object> newList = new List<object>();
        newList.Add(flipOnStart);
        newList.Add(rollable);
        newList.Add(explodable);
        newList.Add(timerDuration);
        newList.Add(pacing);
        newList.Add(pacingPace);
        newList.Add(verticalPacing);
        newList.Add(ascend);
        newList.Add(chargable);
        newList.Add(chargingPace);
        newList.Add(isCharging);
        newList.Add(isReturning);
        newList.Add(homePosition);
        newList.Add(targetPosition);
        newList.Add(isFlying);
        newList.Add(jumper);
        newList.Add(jumpForce);
        newList.Add(jumpForceVariance);
        newList.Add(jumpInterval);
        newList.Add(jumpIntervalVariance);

        return newList;
    }

    // Imports a list of variables to set the current behavior of the enemy
    public void StoreVariables(List<object> objs)
    {
        flipOnStart = (bool)objs[0];
        rollable = (bool)objs[1];
        explodable = (bool)objs[2];
        timerDuration = (float)objs[3];
        pacing = (bool)objs[4];
        pacingPace = (float)objs[5];
        verticalPacing = (bool)objs[6];
        ascend = (bool)objs[7];
        chargable = (bool)objs[8];
        chargingPace = (float)objs[9];
        isCharging = (bool)objs[10];
        isReturning = (bool)objs[11];
        homePosition = (Vector2)objs[12];
        targetPosition = (Vector2)objs[13];
        isFlying = (bool)objs[14];
        jumper = (bool)objs[15];
        jumpForce = (float)objs[16];
        jumpForceVariance = (float)objs[17];
        jumpInterval = (float)objs[18];
        jumpIntervalVariance = (float)objs[19];

        // An enemy prefab may not have its rigidbody Dynamic. This makes sure that rollables roll.
        if (rollable)
        {
            gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        }
    }
    #endregion
}
