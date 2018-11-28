using UnityEngine;
using System.Collections;

public class Hover : MonoBehaviour {
	private const float VELOCITY = 5;
	private int dir = 1; //used for alternating between forward and backwards
	public Vector2 v2; //set whether horizontal or vertical moving platform
	private float totaldelay; //after a certain period of time, reverse direction of hover

    private void FixedUpdate()
    {
        //3 stages to movement, move in dir v2, stop, then move in reverse v2 direction
        totaldelay += Time.fixedDeltaTime;
        if (totaldelay > 7)
        {
            dir *= -1;
            transform.position += CalPosChange();
            totaldelay = 0;
        }
        else if (totaldelay < 5)
        {
            transform.position += CalPosChange();
        }
    }

    Vector3 CalPosChange()
    {
        return new Vector3(VELOCITY * v2.x * dir*Time.fixedDeltaTime, VELOCITY * v2.y * dir*Time.fixedDeltaTime,0);
    }

    /// <summary>
    /// Colliding with the player sets the player to be a child of this object. This allows for any positional
    /// change to automatically be registered  as a change for the player too, so that player will move with the platform.
    /// This is to solve issue where player was sliding off of moving platforms
    /// </summary>
    /// <param name="collInfo"></param>
    void OnCollisionEnter2D(Collision2D collInfo)
	{ 
		if (collInfo.gameObject.tag == "Player")
		{
            collInfo.transform.parent = transform ;
        }
	}

    void OnCollisionExit2D(Collision2D collInfo)
    { //if the spring collides with a player, then that's the only time you want 
      //the player's spring collision method to be called.
        if (collInfo.gameObject.tag == "Player")
        {
            collInfo.transform.parent = null;
        }
    }
}
