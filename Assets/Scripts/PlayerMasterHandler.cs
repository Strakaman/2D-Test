using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerMasterHandler : MonoBehaviour
{
    public PlayerMovementInput inputMovement;




    string pName = "Hayato";


    public void GameOver()
    {

    }

    public void RpcDeactivateYourMovement()
    {
        inputMovement.enabled = false;
    }

    public void RpcResetForRematch()
    {

    }
}
