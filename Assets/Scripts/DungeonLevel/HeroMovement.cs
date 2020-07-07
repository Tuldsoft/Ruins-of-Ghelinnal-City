using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HeroMovement : MonoBehaviour
{
    public float jumpForce = 10f;
    public float walkSpeed = 6.5f;
    public float gravityScale = 2f;

    // prevent multi-jump
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


    // Awake is called before any starts
    void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        cc2d = GetComponent<CapsuleCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        //LoadSprites();

        // halfHeight = cc2d.size.y * transform.localScale.y / 2;
        // castDistance = halfHeight + CastDistanceBuffer;

        injuryTimer = gameObject.AddComponent<EventTimer>();
        injuryTimer.Duration = injuryTimerDuration;
        injuryTimer.AddListener_Finished(EndInjury);
        injuryTimer.TickInterval = injuryTickInterval;
        injuryTimer.AddListener_Tick(TickInjury);

        rb2d.gravityScale = gravityScale;

        // used to make sure Cast() does not find its own collider
        checkGroundFilter.useTriggers = false;
        checkGroundFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
        checkGroundFilter.useLayerMask = true;

    }

    private void Start()
    {
        LoadSprites(); // moved here to make sure spritedata is initialized first
    }

    // Update is called once per frame
    void Update()
    {
        // check for Jump
        if (Input.GetButtonDown("Jump"))
        {
            if (isGrounded)
            {
                Jump();
            }
            else
            {
                // start sequence of retries. This should be a coroutine.
                if (tryJumpNextFrame <= MaxJumpRetryFrames)
                {
                    tryJumpNextFrame++;
                }
            }
        }

        // retry a failed jump, but only up to MaxJumpRetryFrames (20)
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

        // limit jump height when jump button is released
        if (Input.GetButtonUp("Jump"))
        {
            Vector2 velocity = rb2d.velocity;
            velocity.y *= FreeFallFactor;
            rb2d.velocity = velocity;
        }

        
    }

    // FixedUpdate is used for side movement
    private void FixedUpdate()
    {
        // reset by logic below
        isGrounded = false;

        // for debugging only
        Vector2 normal = Vector2.zero;

        // check for ground contact
        int numCollisions = rb2d.Cast(Vector2.down, checkGroundFilter, checkGround, CastDistanceBuffer);
        checkGroundList.Clear();
        for (int i = 0; i < numCollisions; i++)
        {
            checkGroundList.Add(checkGround[i]);
            Debug.DrawLine(rb2d.position, checkGroundList[i].point, Color.red);
            normal = checkGroundList[i].normal;
            Debug.DrawLine(checkGroundList[i].point, checkGroundList[i].point + normal, Color.yellow);
        }

        if (numCollisions > 0)
        {
            isGrounded = true;
            isJumping = false;
            verticalTakeoff = false;
        }

        //float yCorrection = Mathf.Abs(normal.x);
        //if (yCorrection < 0.05) { yCorrection = 1f; }

        //Debug.DrawRay(rb2d.position, new Vector2(0,-castDistance), Color.green);
        /*RaycastHit2D hit = Physics2D.Raycast(rb2d.position, Vector2.down, castDistance, groundLayerMask );
        if (hit.collider != null)
        {
            isGrounded = true;
        }*/

        // move side to side if grounded
        float horizontalInput = Input.GetAxis("Horizontal");

        if (horizontalInput != 0)
        {
            if (isGrounded) 
            {
                rb2d.velocity = new Vector2(horizontalInput * walkSpeed, 0);

                // flip sprite
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
                Vector2 velocity = rb2d.velocity;
                velocity.x *= FreeFallFactor;
                rb2d.velocity = velocity;
            }
            else if (verticalTakeoff ||
                (horizontalInput > 0 && !facingR) ||
                (horizontalInput < 0 && facingR))
            {
                
                 rb2d.velocity = new Vector2(horizontalInput * walkSpeed * FreeFallFactor * 0.5f, rb2d.velocity.y);
            }
        }
        

        // if no horizontal input, not currently jumping it should be idle
        if (horizontalInput == 0 && isGrounded && !isIdle)
        {
            isIdle = true;
            SpriteSwap();
            spriteSwapTimer.Run();
        }
    }

    // propel the hero upwards
    void Jump()
    {
        // make sure to clear the raycast
        Vector2 position = new Vector2(0, MinJumpHeight);
        rb2d.position += position;

        // propel up
        rb2d.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        if (Mathf.Abs(rb2d.velocity.x) < 0.2)
        {
            verticalTakeoff = true;
        }

        // no more retries
        tryJumpNextFrame = 0;
        isJumping = true;
        isIdle = false;

        // reset sprite for jump
        currentSprite = 0;
        spriteSwapTimer.Stop();
        SpriteSwap();
    }

    // refreshes the sprite. Either on call or on timer call
    void SpriteSwap()
    {
        if (spriteSwapTimer.Running == false)
        {
            if (isJumping)
            {
                if (currentSprite == 0 && !spriteSwapTimer.Finished)
                {
                    spriteSwapTimer.Duration = 0.15f;
                    spriteSwapTimer.Run();
                }
                else
                {
                    currentSprite = 1;
                    spriteSwapTimer.Stop();
                }
                spriteRenderer.sprite = jumpingSprite[currentSprite];
                currentSprite++;
            }
            else if (!isIdle)
            {
                if (currentSprite > 3) { currentSprite = 0; }
                spriteRenderer.sprite = walkingSprite[currentSprite];
                spriteSwapTimer.Duration = SpriteSwapInterval;
                spriteSwapTimer.Run();
                currentSprite++;
            }
            else
            {
                spriteSwapTimer.Stop();
                spriteRenderer.sprite = walkingSprite[0];
                currentSprite = 0;
            }
        }
    }

    // called in Start().
    void LoadSprites()
    {
        // setup the spriteswaptimer
        spriteSwapTimer = gameObject.AddComponent<EventTimer>();
        spriteSwapTimer.Duration = SpriteSwapInterval;
        spriteSwapTimer.AddListener_Finished(SpriteSwap);
        spriteSwapTimer.Run();

        // populate sprites
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Damage" && !isInjured)
        {
            Vector2 direction = Vector2.up;
            rb2d.velocity = Vector2.zero;
            rb2d.AddForce(direction.normalized * jumpForce * 1.2f, ForceMode2D.Impulse);
            verticalTakeoff = true;
            isJumping = true;
            isInjured = true;
            injuryTimer.Run();

            bool gameOver = true;
            for (int i = 0; i < BattleLoader.Party.Hero.Length; i++)
            {
                BattleLoader.Party.Hero[i].TakeDamage(Mathf.FloorToInt(BattleLoader.Party.Hero[i].HPMax * DamagePercentOfMaxHP));
                if (BattleLoader.Party.Hero[i].HP > 0) { gameOver = false; }
            }

            if (gameOver)
            {
                cc2d.enabled = false;
                
                // player is dead
                BattleLoader.GameOver = GameOver.Death;
                MenuManager.GoToMenu(MenuName.GameOver);
            }
        }

        if (collision.gameObject.name == "Kill")
        {
            // player is dead
            BattleLoader.GameOver = GameOver.Death;
            MenuManager.GoToMenu(MenuName.GameOver);

            rb2d.velocity = Vector2.zero;
            rb2d.simulated = false;
        }

    }

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

    void EndInjury()
    {
        isInjured = false;
        SetFader(1f);
    }

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

    void SetFader(float fade)
    {
        SpriteRenderer faderRenderer = gameObject.GetComponent<SpriteRenderer>();
        Color fader = faderRenderer.color;
        fader.a = fade;
        faderRenderer.color = fader;
    }
}
