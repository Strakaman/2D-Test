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

    bool jump = false;

    private void OnEnable()
    {
        m_rigidbody2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
        if (Input.GetButtonDown("Jump"))
        {
            Debug.LogWarning("come on man");
            jump = true;
            // CmdSetJumpVal(true);
        }
        animator.SetFloat("HVel", Mathf.Abs(horizontalMove));
        animator.SetFloat("YVel", m_rigidbody2D.velocity.y);
        animator.SetBool("Jump", jump);
    }

    public void OnLanded()
    {
        Debug.Log("somehow hit");
        jump = false;
    }

    public void OnLaunched()
    {
        Debug.Log("shoulda fucking debugged this");
        jump = true;
    }

    void FixedUpdate()
    {
        // Move our character i
        controller.Move(horizontalMove * Time.fixedDeltaTime, false, jump);
    }

}