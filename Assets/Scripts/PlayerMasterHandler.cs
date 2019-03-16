using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerMasterHandler : MonoBehaviour
{
    public PlayerMovementInput inputMovement;
    public Animator animator;

    //string pName = "Hayato";


    public void GameOver()
    {

    }

    void DeactivateYourMovement()
    {
        inputMovement.enabled = false;
    }

    public void OnGoalCollision()
    {
        animator.SetBool("Success", true);
        DeactivateYourMovement();
    }

    public void RpcResetForRematch()
    {
        inputMovement.enabled = true;
    }
}
