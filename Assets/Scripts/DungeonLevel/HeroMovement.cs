using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


/// <summary>
/// Attached to the hero in each dungeon scene. Controls movement and responds to hazards.
/// Vertical movement is handled by Unity's Gravity and Force mechanisms.
/// Horizontal movement is handled directly.
/// </summary>
public class HeroMovement : MonoBehaviour
{
    #region Fields

    // Basic parameters
    public float jumpForce = 10f;
    public float walkSpeed = 6.5f;
    public float gravityScale = 2f;

    // Prevent multi-jump
    bool isIdle = false;
    bool isJumping = true;
    bool verticalTakeoff = false;
    bool isGrounded = false;
    bool isInjured = false;
    bool facingR = true;
    float halfHeight;
    int tryJumpNextFrame = 0;
    const int MaxJumpRetryFrames = 20; // for downhill slopes and cresting hills
    const float MinJumpHeight = 0.12f; // make sure you clear the grounding check

    // Ground Detection
    RaycastHit2D[] checkGround = new RaycastHit2D[32];
    List<RaycastHit2D> checkGroundList = new List<RaycastHit2D>(32);
    const float CastDistanceBuffer = 0.1f;
    float castDistance;
    public LayerMask groundLayerMask; // set in the Inspector
    ContactFilter2D checkGroundFilter;
    const float FreeFallFactor = 0.5f;

    // Body and collider
    Rigidbody2D rb2d;
    CapsuleCollider2D cc2d;

    // Sprite swap support
    SpriteRenderer spriteRenderer;
    EventTimer spriteSwapTimer;
    const float SpriteSwapInterval = 0.08f; // how often the sprite swaps while running
    Sprite[] walkingSprite = new Sprite[4];
    Sprite[] jumpingSprite = new Sprite[2];
    Sprite deathSprite;
    int currentSprite = 0;

    // injury support
    EventTimer injuryTimer;
    const float injuryTimerDuration = 3f;
    const float injuryTickInterval = 0.15f;
    const float maxFade = 0.6f;
    const float minFade = 0.4f;
    const float DamagePercentOfMaxHP = 0.20f; // how much of max is deducted when hitting a trap


    // Awake is called before any Start()
    void Awake()
    {
        // Store common references
        rb2d = GetComponent<Rigidbody2D>();
        cc2d = GetComponent<CapsuleCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Setup a timer for injury
        injuryTimer = gameObject.AddComponent<EventTimer>();
        injuryTimer.Duration = injuryTimerDuration;
        injuryTimer.AddListener_Finished(EndInjury);
        injuryTimer.TickInterval = injuryTickInterval;
        injuryTimer.AddListener_Tick(TickInjury);

        // Set gravity scale (to 2.0)
        rb2d.gravityScale = gravityScale;

        // Used to make sure Cast() does not find its own collider
        checkGroundFilter.useTriggers = false;
        checkGroundFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
        checkGroundFilter.useLayerMask = true;

    }

    /// <summary>
    /// Start is called before the first frame update.
    /// This Start() only loads sprites
    /// Necessary to be in Start() instead of Awake() so ensure that SpriteData()
    /// is initialized first.
    /// </summary>
    private void Start()
    {
        LoadSprites(); 
    }

    // Update is called once per frame. Used for Jump movement, not side movement.
    void Update()
    {
        // Check for Jump input
        if (Input.GetButtonDown("Jump"))
        {
            // Only jump if in contact with the ground
            if (isGrounded)
            {
                Jump();
            }
            else
            {
                // If not isGrounded, check again over the next few frames.
                // This is important because isGrounded can give a false negative on
                // descending slopes.
                if (tryJumpNextFrame <= MaxJumpRetryFrames)
                {
                    tryJumpNextFrame++;
                }
            }
        }

        // Retry a failed jump from a previous frame, but only up to MaxJumpRetryFrames (20)
        if (tryJumpNextFrame > 0)
        {
            if (isGrounded)
            {
                Jump();
            }
            else
            {
                if (tryJumpNextFrame <= MaxJumpRetryFrames)
                {
                    tryJumpNextFrame++;
                }
                else
                {
                    tryJumpNextFrame = 0;
                }
            }
        }

        // Limit jump height when jump button is released
        if (Input.GetButtonUp("Jump"))
        {
            // Modify momentum by FreeFallFactor (decreasing vertical velocity)
            Vector2 velocity = rb2d.velocity;
            velocity.y *= FreeFallFactor;
            rb2d.velocity = velocity;
        }
    }

    // FixedUpdate is used for side movement, not vertical.
    private void FixedUpdate()
    {
        // Starts false, becomes true if the ground is found.
        isGrounded = false;

        // For debugging only
        Vector2 normal = Vector2.zero;

        // Use RayCasts to check for ground contact
        int numCollisions = rb2d.Cast(Vector2.down, checkGroundFilter, checkGround, CastDistanceBuffer);
        checkGroundList.Clear();
        for (int i = 0; i < numCollisions; i++)
        {
            checkGroundList.Add(checkGround[i]);
            Debug.DrawLine(rb2d.position, checkGroundList[i].point, Color.red);
            normal = checkGroundList[i].normal;
            Debug.DrawLine(checkGroundList[i].point, checkGroundList[i].point + normal, Color.yellow);
        }

        // If ground contact is found, isGrounded (permits Jumping)
        if (numCollisions > 0)
        {
            isGrounded = true;
            isJumping = false;
            verticalTakeoff = false;
        }

        //Debug.DrawRay(rb2d.position, new Vector2(0,-castDistance), Color.green);

        // Retrieve horizontal input
        float horizontalInput = Input.GetAxis("Horizontal");

        if (horizontalInput != 0)
        {
            // if trying to move sideways and isGrounded
            if (isGrounded) 
            {
                // Move in the provided direction
                rb2d.velocity = new Vector2(horizontalInput * walkSpeed, 0);

                // Flip sprite according to direction
                if ((horizontalInput > 0f && !facingR) ||
                    (horizontalInput < 0f && facingR))
                {
                    facingR = !facingR;
                    spriteRenderer.flipX = !spriteRenderer.flipX;
                }
                isIdle = false;
                SpriteSwap();
            }
            else if (!isJumping)
            {
                // if neither jumping nor grounded, is presently in FreeFall.
                // Horizontal movement is decreased to allow partial control over descent.
                Vector2 velocity = rb2d.velocity;
                velocity.x *= FreeFallFactor;
                rb2d.velocity = velocity;
            }
            else if (verticalTakeoff ||
                (horizontalInput > 0 && !facingR) ||
                (horizontalInput < 0 && facingR))
            {
                // if isJumping, but trying to modify a jump (upward) from standing still, 
                // OR if trying to reverse the direction of a jump already started,
                // allow only partial control
                rb2d.velocity = new Vector2(horizontalInput * walkSpeed * FreeFallFactor * 0.5f, rb2d.velocity.y);
            }
        }

        // if no input and on the ground, show an idle sprite.
        if (horizontalInput == 0 && isGrounded && !isIdle)
        {
            isIdle = true;
            SpriteSwap();
            spriteSwapTimer.Run();
        }
    }

    // Jump propels the hero upwards. This assumes isGrounded.
    void Jump()
    {
        // Start the jump above the length of the RayCast used to detect grounded.
        Vector2 position = new Vector2(0, MinJumpHeight);
        rb2d.position += position;

        // Propel up
        rb2d.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        
        // If moving neither left nor right, verticalTakeoff.
        if (Mathf.Abs(rb2d.velocity.x) < 0.2)
        {
            verticalTakeoff = true;
        }

        // Disable retries in the next frame
        tryJumpNextFrame = 0;
        isJumping = true;
        isIdle = false;

        // Swap in the first Jump[0] sprite.
        currentSprite = 0;
        spriteSwapTimer.Stop();
        SpriteSwap();
    }

    /// <summary>
    /// Refreshes the sprite. This is called directly or at the end of a timer. By rapidly 
    /// swapping sprites, it can creates a pseudoanimation of walking or jumping.
    /// 
    /// This code is reused and expanded for swapping sprites in battle, 
    /// via HeroSprites.UpdateSprite()
    /// </summary>
    void SpriteSwap()
    {
        // Only run if the timer is NOT already running. 
        // This prevents repeated calls as a movement button is held.
        if (spriteSwapTimer.Running == false)
        {
            if (isJumping)
            {
                // if on 0, show 1 and start a timer
                if (currentSprite == 0 && !spriteSwapTimer.Finished)
                {
                    spriteSwapTimer.Duration = 0.15f;
                    spriteSwapTimer.Run();
                }
                else
                // if on 1 (or anything else), stop the timer and leave as 1
                {
                    currentSprite = 1;
                    spriteSwapTimer.Stop();
                }
                // display 0 or 1
                spriteRenderer.sprite = jumpingSprite[currentSprite];
                currentSprite++;
            }
            else if (!isIdle)
            {
                // For walking. Advance in a cycle through the four walk sprites.
                if (currentSprite > 3) { currentSprite = 0; }
                spriteRenderer.sprite = walkingSprite[currentSprite];
                spriteSwapTimer.Duration = SpriteSwapInterval;
                spriteSwapTimer.Run();
                currentSprite++;
            }
            else
            {
                // if idle, just show idle
                spriteSwapTimer.Stop();
                spriteRenderer.sprite = walkingSprite[0];
                currentSprite = 0;
            }
        }
    }

    // called in Start().
    void LoadSprites()
    {
        // After HeroSprite are initialized:
        // Setup the spriteswaptimer
        spriteSwapTimer = gameObject.AddComponent<EventTimer>();
        spriteSwapTimer.Duration = SpriteSwapInterval;
        spriteSwapTimer.AddListener_Finished(SpriteSwap);
        spriteSwapTimer.Run();

        // Populate sprites, only need Walk[], Jump[] and Dead
        BattleLoader.Party.Hero[0].Sprites.SetObj(gameObject); 
        HeroSprites sprites = BattleLoader.Party.Hero[0].Sprites;
        walkingSprite[0] = sprites.Walk[0];
        walkingSprite[1] = sprites.Walk[1];
        walkingSprite[2] = sprites.Walk[2];
        walkingSprite[3] = sprites.Walk[3];

        jumpingSprite[0] = sprites.Jump[0];
        jumpingSprite[1] = sprites.Jump[1];

        deathSprite = sprites.Dead;
    }

    /// <summary>
    /// Called whenever the hero encounters another collider, including the ground.
    /// Encounters with enemies is handled by Enemy_Dungeon.
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // There's multiple Tile and Grid objects. They can only be differentiated by name.
        // When the hero takes damage from touching lava or a spike, he isInjured, 
        // and prevented from taking further damage for a time.
        if (collision.gameObject.name == "Damage" && !isInjured)
        {
            // Pop upward as if pricked or stung
            Vector2 direction = Vector2.up;
            rb2d.velocity = Vector2.zero;
            rb2d.AddForce(direction.normalized * jumpForce * 1.2f, ForceMode2D.Impulse);
            verticalTakeoff = true;
            isJumping = true;

            // Prevent reinjury while timer is running.
            isInjured = true;
            injuryTimer.Run();

            // All heroes take damage. Evaluate if all the heroes are dead.
            bool gameOver = true;
            for (int i = 0; i < BattleLoader.Party.Hero.Length; i++)
            {
                BattleLoader.Party.Hero[i].TakeDamage(Mathf.FloorToInt(BattleLoader.Party.Hero[i].HPMax * DamagePercentOfMaxHP));
                if (BattleLoader.Party.Hero[i].HP > 0) { gameOver = false; } // if one hero.hp > 0, continue
            }

            // Show Gameover.Death menu prefab to return to town
            if (gameOver)
            {
                cc2d.enabled = false;
                
                // player is dead
                BattleLoader.GameOver = GameOver.Death;
                MenuManager.GoToMenu(MenuName.GameOver);
            }
        }

        // "Kill" objects instantly mean death. This is used as tiles underneath open pits, out of sight.
        if (collision.gameObject.name == "Kill")
        {
            // player is dead
            BattleLoader.GameOver = GameOver.Death;
            MenuManager.GoToMenu(MenuName.GameOver);

            rb2d.velocity = Vector2.zero;
            rb2d.simulated = false;
        }

    }

    // "Kill" objects work better than OnBecameInvisible(), because there are situations
    // where the hero can be momentarily off camera. Also, OnBecameInvisible() won't trigger if 
    // often fails to the gameobject is visible in the editor (even if not on camera), and
    // sometimes just fails to trigger when it should.
    /*private void OnBecameInvisible()
    {
        
        *//*if (transform.position.y < 0 && BattleLoader.DungeonLevel != 6)
        {
            rb2d.velocity = Vector2.zero;
            rb2d.simulated = false;
        }*//*
        
        
        
        // OnBecameInvisible triggers on scene loads, so I need to find another way to
        // detect falling out of the scene. Otherwise, this'll trigger when battle starts.

        // use an edge collider below the screen to detect death

        *//*if (BattleLoader.Hero.HP > 0)
        {
            cc2d.enabled = false;

            // player is dead
            BattleLoader.GameOver = GameOver.Death;
            MenuManager.GoToMenu(MenuName.GameOver);
        }*//*
    }*/

    // Called at the end of the injuryTimer
    void EndInjury()
    {
        isInjured = false;
        SetFader(1f);
    }

    // As the timer continues, flicker the hero on and off by adjusting its fade
    void TickInjury(int remainingIntervals)
    {
        if (remainingIntervals % 2 == 0)
        {
            SetFader(minFade);
        }
        else
        {
            SetFader(maxFade);
        }
    }

    // Sets the alpha color of the SpriteRenderer
    void SetFader(float fade)
    {
        SpriteRenderer faderRenderer = gameObject.GetComponent<SpriteRenderer>();
        Color fader = faderRenderer.color;
        fader.a = fade;
        faderRenderer.color = fader;
    }
    #endregion
}
