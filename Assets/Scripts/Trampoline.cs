using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trampoline : MonoBehaviour {

    //bounciness comes from material but animator logic is in script
    private Animator m_animator;
	void Start () {
        m_animator = GetComponentInChildren<Animator>();
	}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            m_animator.SetTrigger("Activated");
        }
    }
}
