using UnityEngine;
using System.Collections;

public class Ring : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void OnTriggerEnter2D(Collider2D collInfo)
	{	//if the ring collides with a player, then that's the only time you want 
		//the player's ring collision method to be called.
		if (collInfo.gameObject.tag == "Player")
		{
			collInfo.gameObject.BroadcastMessage("RingCollision",collider2D.transform);
			//send the transform as well so that player will automatically move to center
		}
	}
	/*void OnCollisionEnter2D(Collision2D collInfo)
		{	//if the ring collides with a player, then that's the only time you want 
			//the player's ring collision method to be called.
			if (collInfo.gameObject.tag == "Player")
			{
				collInfo.gameObject.BroadcastMessage("RingCollision");
			}

	}*/
}
