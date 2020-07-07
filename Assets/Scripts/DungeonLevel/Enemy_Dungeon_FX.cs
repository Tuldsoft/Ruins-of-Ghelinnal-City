using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Dungeon_FX : MonoBehaviour
{
    [SerializeField]
    bool flipOnStart = false;

    [SerializeField]
    bool rollable = false;
    
    [SerializeField]
    public bool explodable = false;
    [SerializeField]
    float timerDuration = 3f;
    
    [SerializeField]
    bool pacing = false;
    [SerializeField]
    float pacingPace = 1.5f;
    bool flipX = true;

    [SerializeField]
    bool verticalPacing = false;
    bool ascend = true;

    [SerializeField]
    bool chargable = false;
    [SerializeField]
    float chargingPace = 7f;
    
    [SerializeField]
    public bool isFlying = false;

    [SerializeField]
    bool jumper = false;
    [SerializeField]
    float jumpForce = 7f, jumpForceVariance = 0.5f;
    [SerializeField]
    float jumpInterval = 4f, jumpIntervalVariance = 0.5f;
    EventTimer jumpTimer;

    bool isCharging = false;
    bool isReturning = false;
    float enemyMaxHalfWidth = 0f;
    CircleCollider2D homeLocation;
    Vector2 homePosition;
    CircleCollider2D targetLocation;
    Vector2 targetPosition;
    EdgeCollider2D targetingScope;

    

    Rigidbody2D rb2d;
    PolygonCollider2D pc2d;

    EventTimer explosionTimer;
    GameObject prefabExplosion;


    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        pc2d = GetComponent<PolygonCollider2D>();
        flipX = gameObject.GetComponent<SpriteRenderer>().flipX;

        PhysicsMaterial2D zeroFrictionMaterial = new PhysicsMaterial2D();
        zeroFrictionMaterial.friction = 0f;

        if (flipOnStart) { FlipX(); }

        if (pacing)
        {
            bool flip = Random.value > 0.5f ? true : false;
            if (flip) { FlipX(); }
            rb2d.sharedMaterial = zeroFrictionMaterial;
            rb2d.gravityScale = 0;
        }
        
        if (verticalPacing) {ascend = Random.value > 0.5f ? true : false; }

        if (rollable)
        {
            rb2d.constraints = RigidbodyConstraints2D.None;
            float rotation = Random.Range(0f, 360f);
            Vector3 rotateVector = new Vector3(0, 0, rotation);
            transform.Rotate(rotateVector);
        }

        if (explodable)
        {
            prefabExplosion = Resources.Load<GameObject>(@"DungeonPrefabs\Explosion");

            explosionTimer = gameObject.AddComponent<EventTimer>();
            explosionTimer.Duration = timerDuration;
            explosionTimer.Run();
            explosionTimer.AddListener_Finished(Explode);
        }

        if (chargable)
        {
            foreach (Vector2 point in pc2d.points)
            {
                float pointX = Mathf.Abs(point.x) * transform.localScale.x;
                enemyMaxHalfWidth = pointX > enemyMaxHalfWidth ? pointX : enemyMaxHalfWidth;
            }
            targetingScope = GetComponent<EdgeCollider2D>();
            targetingScope.enabled = !isCharging;
        }

        if (jumper)
        {
            jumpTimer = gameObject.AddComponent<EventTimer>();
            jumpTimer.Duration = Random.Range(jumpInterval - jumpIntervalVariance,
                jumpInterval + jumpIntervalVariance);
            jumpTimer.Run();
            jumpTimer.AddListener_Finished(Jump);
        }

    }


    // Update is called once per frame
    void FixedUpdate()
    {
        // flipX == true = facing left
        
        if (pacing || isFlying)
        {
            int direction = flipX ? -1 : 1;
                        
            // locomotion - flat surfaces only
            
            rb2d.velocity = new Vector2((float)direction * pacingPace, 0);
        }

        if (verticalPacing)
        {
            int direction = ascend ? 1 : -1; 
            rb2d.velocity = new Vector2(0, (float)direction);
        }

        if (isCharging)
        {
            /*Vector2 direction = new Vector2(targetLocation.transform.position.x - transform.position.x,
                targetLocation.transform.position.y - transform.position.y).normalized;*/
            Vector2 direction = new Vector2(targetPosition.x - transform.position.x,
                0).normalized;

            rb2d.velocity = direction * chargingPace;
            if (targetLocation == null || homePosition == null)
            {
                targetLocation = MakeTarget(targetPosition);
                homeLocation = MakeTarget(homePosition);
            }

        }

        if (isReturning)
        {
            /*Vector2 direction = new Vector2(homeLocation.transform.position.x - transform.position.x,
                homeLocation.transform.position.y - transform.position.y).normalized;*/
            Vector2 direction = new Vector2(homePosition.x - transform.position.x,
                0).normalized;

            rb2d.velocity = direction * pacingPace;
            if (targetLocation == null || homePosition == null)
            {
                targetLocation = MakeTarget(targetPosition);
                homeLocation = MakeTarget(homePosition);
                FlipX();
            }
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isFlying && collision.gameObject.CompareTag("KillBoundary_Dungeon"))
        {
            Destroy(gameObject);
        }
        
        if (pacing && collision.gameObject.CompareTag("PacingBoundary_Dungeon"))
        {
            // turn around when hitting an edgecollider
            FlipX();
        }
        
        if (verticalPacing && collision.gameObject.CompareTag("PacingBoundary_Dungeon"))
        {
            // turn around when hitting an edgecollider
            ascend = !ascend;
        }

        if (chargable && collision.gameObject.CompareTag("Hero_Dungeon") && !isCharging && !isReturning)
        {
            isCharging = true;
            targetingScope.enabled = false;
            Vector2 position = gameObject.transform.position;
            position.x += enemyMaxHalfWidth * (flipX ? -1 : 1); //flipx false means + (right), true means - (left)
            homePosition = position;
            homeLocation = MakeTarget(position);

            position = collision.gameObject.transform.position;
            position.y = homeLocation.transform.position.y;
            targetPosition = position;
            targetLocation = MakeTarget(position);
        }

        /*if (isCharging && (collision.gameObject.CompareTag("PacingBoundary_Dungeon") 
            || collision == targetLocation))*/
        /*if (isCharging && (collision.gameObject.CompareTag("PacingBoundary_Dungeon")
            ))*/

        if (isCharging && collision == targetLocation)
        {
            isCharging = false;
            targetingScope.enabled = true;
            isReturning = true;
            FlipX();
        }

        if (collision == homeLocation && !isCharging)
        {
            isReturning = false;
            Destroy(targetLocation);
            Destroy(homeLocation);
            FlipX();
        }

        if (collision.gameObject.name == "Kill")
        {
            // enemy is out of bounds
            Destroy(gameObject);
        }

    }

    CircleCollider2D MakeTarget(Vector2 position)
    {
        CircleCollider2D target = new GameObject().AddComponent<CircleCollider2D>();
        target.transform.position = position;
        target.isTrigger = true;
        target.radius = 0.1f;
        return target;
    }

    void FlipX()
    {
        flipX = !flipX;
        gameObject.GetComponent<SpriteRenderer>().flipX = flipX;
        if (pc2d == null) 
        {
            pc2d = gameObject.GetComponent<PolygonCollider2D>(); 
        }
        pc2d.points = FlipXPoints(pc2d.points);

        if (chargable)
        {
            EdgeCollider2D ec2d = gameObject.GetComponent<EdgeCollider2D>();
            ec2d.points = FlipXPoints(ec2d.points);
        }
    }

    Vector2[] FlipXPoints (Vector2[] points)
    {
        for (int i = 0; i < points.Length; i++)
        {
            points[i].x *= -1;
        }
        return points;
    }

    /*private void OnBecameInvisible()
    {
        if (explodable)
        {
            explosionTimer.Stop();
        }
    }*/

    void Explode()
    {
        GameObject explosion = GameObject.Instantiate(prefabExplosion);
        explosion.transform.position = transform.position;
        Destroy(gameObject);
    }

    void Jump()
    {
        rb2d.AddForce(Vector2.up * jumpForce
            * Random.Range(1 - jumpForceVariance, 1 + jumpForceVariance),ForceMode2D.Impulse);
        jumpTimer.Duration = Random.Range(jumpInterval - jumpIntervalVariance,
                jumpInterval + jumpIntervalVariance);
        jumpTimer.Run();
    }

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


        if (rollable)
        {
            gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        }
    }   
}
