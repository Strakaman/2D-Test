using UnityEngine;
using UnityEngine.Events;

public class CharacterController2D : MonoBehaviour
{
    [SerializeField] private float m_JumpForce = 400f;                          // Amount of force added when the player jumps.
    [Range(0, 1)] [SerializeField] private float m_CrouchSpeed = .36f;          // Amount of maxSpeed applied to crouching movement. 1 = 100%
    [Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;  // How much to smooth out the movement
    [SerializeField] private bool m_AirControl = false;                         // Whether or not a player can steer while jumping;
    [SerializeField] private LayerMask m_WhatIsGround;                          // A mask determining what is ground to the character
    [SerializeField] private Transform m_GroundCheck;                           // A position marking where to check if the player is grounded.
    [SerializeField] private Transform m_CeilingCheck;                          // A position marking where to check for ceilings
    [SerializeField] private Collider2D m_CrouchDisableCollider;                // A collider that will be disabled when crouching

    const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
    private bool m_Grounded;            // Whether or not the player is grounded.
    const float k_CeilingRadius = .2f; // Radius of the overlap circle to determine if the player can stand up
    private Rigidbody2D m_Rigidbody2D;
    private bool m_FacingRight = true;  // For determining which way the player is currently facing.
    private Vector3 m_Velocity = Vector3.zero;


    private Vector3 s; //box collider size to help with raycasting
    private Vector3 c; //box collider center to help with raycasting

    private const float MAXSPEED = 10;
    private const float JUMPSPEED = 20;
    private float gravity; //gravity sometimes removed for certain physics, need to be able to get it back

    [Header("Events")]
    [Space]

    public UnityEvent OnLandEvent;

    [System.Serializable]
    public class BoolEvent : UnityEvent<bool> { }

    public BoolEvent OnCrouchEvent;
    private bool m_wasCrouching = false;

    private void Awake()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();

        if (OnLandEvent == null)
            OnLandEvent = new UnityEvent();

        if (OnCrouchEvent == null)
            OnCrouchEvent = new BoolEvent();
    }

    private void FixedUpdate()
    {
        bool wasGrounded = m_Grounded;
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
        if (m_Grounded && jump)
        {
            // Add a vertical force to the player.
            m_Grounded = false;
            m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
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
            float x = p.x + c.x + s.x / 2 * -1; //check the left of the player for a wall first
            float y = (p.y + c.y - s.y / 2) + s.y / 2 * i; // shoot a ray from the top, middle, and bottom
            float x2 = x + s.x; //gets other side of player 
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
        GetComponent<Rigidbody2D>().velocity = new Vector2(MAXSPEED * dir, .9f * JUMPSPEED);
    }

    private void Stop()
    {
        GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        GetComponent<Rigidbody2D>().gravityScale = 0;
    }

    public void SpringCollision(GMScript.Springer sprop)
    {   //called when player collides with spring from spring script
        //springs launch you pretty hard
        //rigidbody2D.velocity = new Vector2 (rigidbody2D.velocity.x, JUMPSPEED * sprop.intensity);
        transform.position = sprop.elRay.origin; //move to center of spring so that you can hit it from any side and get the same effect
        float mag = JUMPSPEED * sprop.intensity; //combine ray's direction with player speed default. differentiate between spring and big spring with intensity
        float dirx = sprop.elRay.direction.x;
        float diry = sprop.elRay.direction.y;
        if (Mathf.Abs(dirx) != Mathf.Abs(diry))
        {
            GetComponent<Rigidbody2D>().velocity = new Vector2(dirx * MAXSPEED, diry * mag);
        }
        else
        { //want the 45 degree angle spring to have more horizontal power
            GetComponent<Rigidbody2D>().velocity = new Vector2(Mathf.Sign(dirx) * MAXSPEED, diry * mag);
        }
        if (!GMScript.state.Equals(GMScript.Context.dead))
        {
            GMScript.state = GMScript.Context.normal; //just in case you zipline into the teleporter, don't want to be stuck in that state
        }
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
        Vector2 jVel = new Vector2(MAXSPEED * Mathf.Cos(-0.174532925f), MAXSPEED * Mathf.Sin(-0.174532925f));
        GetComponent<Rigidbody2D>().velocity = jVel;
        GetComponent<Rigidbody2D>().gravityScale = 0;
        Invoke("ZipJump", 4.4f); //after 4.2 seconds (based on zipline length), you have reached the end of the zipline if you are still on it

    }

    public void ZipJump()
    {
        if (GMScript.state.Equals(GMScript.Context.zipping))
        {
            GetComponent<Rigidbody2D>().velocity = new Vector2(MAXSPEED, GetComponent<Rigidbody2D>().velocity.y);
            GMScript.state = GMScript.Context.normal;
            GetComponent<Rigidbody2D>().gravityScale = gravity;
        }
    }


    public void HoverCollision()
    {
        //rigidbody2D.velocity = new Vector2 (0, 0);
        //rigidbody2D.gravityScale = 0;
    }

    public void GoalCollision()
    {
        Stop();
    }

    public void KillLineCollision()
    {
        Stop();
    }

    public void RingCollision(Transform ringTransform)
    {
        GMScript.state = GMScript.Context.ringing;
        GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        transform.position = ringTransform.position;
        GetComponent<Rigidbody2D>().gravityScale = 0;

    }

    public void WindCollision(Transform lol)
    {
        //rigidbody2D.AddForce(new Vector2(0,.1f));	
        //if (rigidbody2D.velocity.y > 9)
        //{
        GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x, 6f);
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
        if (!wasGrounded)
            OnLandEvent.Invoke();
    }

}
