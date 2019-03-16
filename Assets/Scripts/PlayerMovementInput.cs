using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerMovementInput : MonoBehaviour
{
    public CharacterController2D controller;
    public Animator animator;
    public float runSpeed = 40f;
    float horizontalMove = 0f;
    Rigidbody2D m_rigidbody2D;

    bool jumpInput = false;
    bool disableMovement = false;

    private void OnEnable()
    {
        m_rigidbody2D = GetComponent<Rigidbody2D>();
        disableMovement = false;
        animator.SetBool("Dead", false);

    }

    // Update is called once per frame
    void Update()
    {
        if (GMScript.paused) { return; }
        if (disableMovement) { return; }   
        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
        if (Input.GetButtonDown("Jump")) { jumpInput = true; }
        animator.SetFloat("HVel", Mathf.Abs(horizontalMove));
        animator.SetFloat("YVel", m_rigidbody2D.velocity.y);
        //animator.SetBool("Jump", jump);
    }

    public void OnLanded()
    {
        //jumpInput = false;
    }

    public void OnLaunched()
    {
        //jumpInput = true; //not sure why i was setting this to true just because you got launce, maybe an unerased relic from changing the boolean to only track user input?
    }

    public void OnTurbineCollision()
    {
    }

    /// <summary>

    /// </summary>
    public void OnZipCollision()
    {
        animator.SetBool("Suspended", true);
    }

    public void OnRingCollision()
    {
        animator.SetBool("Suspended", true);
    }

    public void OnKillLineCollision()
    {
        GMScript.instance.StageFailed();
    }

    public void Die()
    {
        m_rigidbody2D.gravityScale = 0;
        disableMovement = true;
        GMScript.state = GMScript.Context.dead;
        animator.SetBool("Dead",true);
    }

    void FixedUpdate()
    {
        if (GMScript.paused) { return; }
        // Move our character i
        controller.Move(horizontalMove * Time.fixedDeltaTime, false, jumpInput);

        ///     these is generally used to reset jump variable when colliding with certain objects
        ///     You can't rely on update method to set jump back to false since multiple update frames can occur before fixed update
        ///     i.e. one frame that would set jump to true and the next sets jump back to false. This can result in lost jump button presses
        jumpInput = false;
    }

}
