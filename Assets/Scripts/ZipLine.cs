using UnityEngine;
using System.Collections;

public class ZipLine : MonoBehaviour {
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void OnTriggerEnter2D(Collider2D collInfo)
	{
		//if the zipline collides with a player, then that's the only time you want 
		//the player's spring collision method to be called.
		collider2D.enabled = false; //YOZO - you can only zipline once
		if (collInfo.gameObject.tag == "Player")
		{	
			collInfo.gameObject.BroadcastMessage("ZipCollision",transform);
			Vector2 jVel = new Vector2 (10 * Mathf.Cos(-0.174532925f), 10 * Mathf.Sin(-0.174532925f));
			rigidbody2D.velocity = jVel;
			Invoke("Stop",4.4f);
		} 
	}

	void Stop()
	{
		rigidbody2D.velocity = Vector2.zero;
	}
}
