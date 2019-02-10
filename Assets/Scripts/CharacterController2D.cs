using System;
using UnityEngine;
using UnityEngine.Events;

public class CharacterController2D : MonoBehaviour
{
    [SerializeField] private float m_JumpForce = 400f;                          // Amount of force added when the player jumps.
    [Range(0, 1)] [SerializeField] private float m_CrouchSpeed = .36f;          // Amount of maxSpeed applied to crouching movement. 1 = 100%
    [Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;  // How much to smooth out the movement
    [SerializeField] private bool m_AirControl = false;                         // Whether or not a player can steer while jumping;
    private float m_AirControlCooldown = 0f;                                    //for controls that prevent air movement, set a time when the player can control movement again
    [SerializeField] private LayerMask m_WhatIsGround;                          // A mask determining what is ground to the character
    [SerializeField] private Transform m_GroundCheck;                           // A position marking where to check if the player is grounded.
    [SerializeField] private Transform m_CeilingCheck;                          // A position marking where to check for ceilings
    [SerializeField] private Collider2D m_CrouchDisableCollider;                // A collider that will be disabled when crouching

    public Animator animator;
    const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
    private bool m_Grounded;            // Whether or not the player is grounded.
    const float k_CeilingRadius = .2f; // Radius of the overlap circle to determine if the player can stand up
    private Rigidbody2D m_Rigidbody2D;
    private bool m_FacingRight = true;  // For determining which way the player is currently facing.
    private Vector3 m_Velocity = Vector3.zero;


    private Vector3 coll_size; //box collider size to help with raycasting
    private Vector3 coll_center; //box collider center to help with raycasting

    private const float MAXSPEED = 50;
    private const float JUMPSPEED = 20;
    private float gravity; //gravity sometimes removed for certain physics, need to be able to get it back

    [Header("Events")]
    [Space]

    public UnityEvent OnLandEvent;

    public UnityEvent OnLaunchEvent; //launched in the area due to some means i.e spring
    public UnityEvent OnTurbineCollEvent; //collided with turbine
    public UnityEvent OnRingCollEvent; //hit grab ring
    public UnityEvent OnGoalCollEvent; //collided with goal
    public UnityEvent OnZipCollEvent; //grabbed zipline handle
    public UnityEvent OnKillCollEvent; //hit kill line

    [System.Serializable]
    public class BoolEvent : UnityEvent<bool> { }

    public BoolEvent OnCrouchEvent;
    private bool m_wasCrouching = false;

    private void Awake()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();

        if (OnLandEvent == null)
            OnLandEvent = new UnityEvent();

        if (OnLaunchEvent == null)
            OnLaunchEvent = new UnityEvent();

        if (OnGoalCollEvent == null)
            OnGoalCollEvent = new UnityEvent();
        if (OnTurbineCollEvent == null)
            OnTurbineCollEvent = new UnityEvent();
        if (OnZipCollEvent == null)
            OnZipCollEvent = new UnityEvent();
        if (OnRingCollEvent == null)
            OnRingCollEvent = new UnityEvent();
        if (OnCrouchEvent == null)
            OnCrouchEvent = new BoolEvent();
        if (OnKillCollEvent == null)
            OnKillCollEvent = new UnityEvent();

    }

    private void Start()
    {
        gravity = m_Rigidbody2D.gravityScale;
        BoxCollider2D zollider = GetComponent<BoxCollider2D>();
        coll_size = zollider.size;
        coll_center = zollider.offset;
    }

    private void FixedUpdate()
    {
        //bool wasGrounded = m_Grounded;
        m_Grounded = false;

        // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
        // This can be done using layers instead but Sample Assets will not overwrite your project settings.
        Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                m_Grounded = true;
                /* if (!wasGrounded)
                     OnLandEvent.Invoke();*/
            }
        }
    }

    void Update()
    {
        if (!m_AirControl)
        {
            m_AirControlCooldown -= Time.deltaTime;
            if (m_AirControlCooldown <= 0)
            {
                Debug.Log("Resetting air control");
                m_AirControl = true;
            }
        }
    }

    public void Move(float move, bool crouch, bool jump)
    {
        EvalCrouchSituation(crouch);

        if (m_wasCrouching)
        {
            // Reduce the speed by the crouchSpeed multiplier
            move *= m_CrouchSpeed;
        }
        //only control the player if grounded or airControl is turned on
        if (m_Grounded || m_AirControl)
        {
            // If crouching

            CalcHorizontalMovement(move);

            EvalFlipSituation(move);
        }
        // If the player should jump...
        if (jump)
        {
            if (m_Grounded)
            {
                // Add a vertical force to the player.
                animator.SetBool("Jump", true);
                m_Grounded = false;
                m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
            }
            else if (GMScript.state.Equals(GMScript.Context.zipping))
            {
                Debug.Log("here");
                ZipJump();
            }
            else if (GMScript.state.Equals(GMScript.Context.ringing))
            {
                RingJump(move);
            }
            else
            {
                int direction;
                if (CanWallJump(out direction))
                {
                    WallJump(direction);
                }
            }
                
        }
    }

    private void CalcHorizontalMovement(float move)
    {
        // Move the character by finding the target velocity
        Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.velocity.y);
        // And then smoothing it out and applying it to the character
        m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);
    }

    public void EvalCrouchSituation(bool crouch)
    {
        // If crouching, check to see if the character can stand up
        if (!crouch)
        {
            // If the character has a ceiling preventing them from standing up, keep them crouching
            if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround))
            {
                Debug.LogWarning(Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround).name);
                crouch = true;
            }
        }
        if (crouch)
        {
            if (!m_wasCrouching)
            {
                m_wasCrouching = true;
                OnCrouchEvent.Invoke(true);
            }

            // Disable one of the colliders when crouching
            if (m_CrouchDisableCollider != null)
                m_CrouchDisableCollider.enabled = false;
        }
        else
        {
            // Enable the collider when not crouching
            if (m_CrouchDisableCollider != null)
                m_CrouchDisableCollider.enabled = true;

            if (m_wasCrouching)
            {
                m_wasCrouching = false;
                OnCrouchEvent.Invoke(false);
            }
        }
    }

    public void EvalFlipSituation(float move)
    {
        // If the input is moving the player right and the player is facing left...
        if (move > 0 && !m_FacingRight)
        {
            // ... flip the player.
            Flip();
        }
        // Otherwise if the input is moving the player left and the player is facing right...
        else if (move < 0 && m_FacingRight)
        {
            // ... flip the player.
            Flip();
        }
    }

    private void Flip()
    {
        // Switch the way the player is labelled as facing.
        m_FacingRight = !m_FacingRight;

        // Multiply the player's x local scale by -1.
        /*Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;*/
        transform.Rotate(0, 180f, 0);
    }

    //Omari Stuff
    private bool CanWallJump(out int dir)
    {
        Vector2 p = transform.position;
        // Check collisions left and right to see if user is next to a wall
        for (int i = 0; i < 3; i++)
        { //use loop to create three rays equally distributed of player
            float x = p.x + coll_center.x + coll_size.x / 2 * -1; //check the left of the player for a wall first
            float y = (p.y + coll_center.y - coll_size.y / 2) + coll_size.y / 2 * i; // shoot a ray from the top, middle, and bottom
            float x2 = x + coll_size.x; //gets other side of player 
                                //Ray ray = new Ray (new Vector3(x,y,0), new Vector3 (-1,0,0)); //create ray and cast it left
                                //RaycastHit2D hit = Physics2D.Raycast(ray.origin,ray.direction,.1f,9); //.1f to be more or less touching wall
            RaycastHit2D hit = Physics2D.Raycast(new Vector2(x, y), new Vector2(-1, 0), .1f, 9); //check left side for wall jump, then right side
            RaycastHit2D hit2 = Physics2D.Raycast(new Vector2(x2, y), new Vector2(1, 0), .1f, 9);

            if (hit && hit.collider && hit.collider.tag.Equals("Wall"))
            {
                dir = 1; //because there is a wall on the left, the wall jump should be to the right
                return true;
            }
            else if (hit2 && hit2.collider && hit2.collider.tag.Equals("Wall"))
            {
                dir = -1; //because there is a wall on the right, wall jump should be to the left
                return true;
            }
        }
        dir = 0;
        return false;
    }

    private void WallJump(int dir)
    {
        Flip();
        m_Rigidbody2D.velocity = new Vector2(.375f*MAXSPEED * dir, .9f * JUMPSPEED);
        m_AirControl = false;
        SetAirControlCooldown(.25f);
    }

    private void Stop()
    {
        m_Rigidbody2D.velocity = new Vector2(0, 0);
        //m_Rigidbody2D.gravityScale = 0;
    }

    Vector3 straightUp = new Vector3(0, 0.52f, 0);

    public void SpringCollision(GMScript.Springer sprop) //todo: figure out why jumping on spring and running into spring result in different launches
    {   //called when player collides with spring from spring script
        //springs launch you pretty hard
        transform.position = sprop.elRay.origin + straightUp; //move to center of spring so that you can hit it from any side and get the same effect
        float mag = JUMPSPEED * sprop.intensity*.9f; //combine ray's direction with player speed default. differentiate between spring and big spring with intensity
        float dirx = sprop.elRay.direction.x;
        float diry = sprop.elRay.direction.y;
        m_Rigidbody2D.velocity = new Vector2(dirx * MAXSPEED*.5f, diry * mag);
        Debug.Log("Velocity " +m_Rigidbody2D.velocity);
        m_AirControl = false;
        SetAirControlCooldown(.25f * sprop.intensity); //the more powerful the spring the greater time the user is incapacitated
        if (!GMScript.state.Equals(GMScript.Context.dead))
        {
            GMScript.state = GMScript.Context.normal; //just in case you zipline into the teleporter, don't want to be stuck in that state
        }
        animator.SetBool("Jump", true);
        OnLaunchEvent.Invoke();
    }

    /// <summary>
    /// Controls the amount of time that has to pass before player can control their air movement again
    /// This is better than using an invoke because the player may interact with multiple objects that reset the cooldown time
    /// </summary>
    /// <param name="coolDownTime"></param>
     void SetAirControlCooldown(float coolDownTime)
     {
         m_AirControlCooldown = coolDownTime;
     }

    public void SRTACollision(Transform teleportTo)
    {
        transform.position = teleportTo.position;
        if (!GMScript.state.Equals(GMScript.Context.dead))
        {
            GMScript.state = GMScript.Context.normal; //just in case you zipline into the teleporter, don't want to be stuck in that state
        }
        //just go to whereever
    }

    public void ZipCollision(Transform attachto)
    {
        GMScript.state = GMScript.Context.zipping;
        transform.position = attachto.position;
        //Vector2 jVel = new Vector2(MAXSPEED * Mathf.Cos(-0.174532925f), MAXSPEED * Mathf.Sin(-0.174532925f));
        //m_Rigidbody2D.velocity = jVel;
        m_Rigidbody2D.velocity = new Vector2(0, 0);
        m_Rigidbody2D.gravityScale = 0;
        //Invoke("ZipJump", 4.4f); //after 4.2 seconds (based on zipline length), you have reached the end of the zipline if you are still on it
        m_AirControl = false;
        SetAirControlCooldown(float.MaxValue); //set to some high number to prevent player from being able to control x-axis movement while on zipline
        OnZipCollEvent.Invoke();
    }

    public void ZipJump()
    {
        if (GMScript.state.Equals(GMScript.Context.zipping))
        {
            m_Rigidbody2D.velocity = new Vector2(.75f*MAXSPEED, 0);
            GMScript.state = GMScript.Context.normal;
            m_Rigidbody2D.gravityScale = gravity;
            //m_AirControl = true;
            transform.parent = null;
            SetAirControlCooldown(.5f);
            OnLaunchEvent.Invoke();
        }
    }


    public void HoverCollision()
    {
        //rigidbody2D.velocity = new Vector2 (0, 0);
        //rigidbody2D.gravityScale = 0;
    }

    public void GoalCollision()
    {
        OnGoalCollEvent.Invoke();
        Stop();
    }

    public void KillLineCollision()
    {
        Stop();
        OnKillCollEvent.Invoke();
    }

    public void RingCollision(Transform ringTransform)
    {
        // user should lose x-axis movement as long as they are on attached to ring
        GMScript.state = GMScript.Context.ringing;
        m_Rigidbody2D.velocity = new Vector2(0, 0);
        m_AirControl = false;
        SetAirControlCooldown(float.MaxValue); //set high to prevent accidental re-granting of air control
        transform.position = ringTransform.position;
        m_Rigidbody2D.gravityScale = 0;
        OnRingCollEvent.Invoke();
    }

    private void RingJump(float move)
    {
        m_AirControl = true;
        m_Rigidbody2D.gravityScale = gravity;
        m_Rigidbody2D.velocity = new Vector2(Mathf.Sign(move) * MAXSPEED * .2f, 0);
        m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
        GMScript.state = GMScript.Context.normal;
        animator.SetBool("Jump", true);
        OnLaunchEvent.Invoke();
    }

    public void WindCollision(Transform lol)
    {
        //rigidbody2D.AddForce(new Vector2(0,.1f));	
        //if (rigidbody2D.velocity.y > 9)
        //{
        animator.SetBool("Jump", true);
        m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, 6f);
        OnTurbineCollEvent.Invoke();
        //float x = (transform.position.y - lol.position.y);
        //x /= 3200;
        //x /= 999.75f;
        //Debug.Log(rigidbody2D.velocity.y);
        //rigidbody2D.AddForce(new Vector2(0,x));	
        //rigidbody2D.AddForce(new Vector2(0,.01f));	
        //}
    }

    public void GroundSelf()
    {
        bool wasGrounded = m_Grounded;
        m_Grounded = true;
        animator.SetBool("Jump", false);
        if (!wasGrounded)
            OnLandEvent.Invoke();
    }

}
