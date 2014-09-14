using UnityEngine;
using System.Collections;

public class Hover : MonoBehaviour {
	private const float VELOCITY = 5;
	private int dir = 1; //used for alternating between forward and backwards
	public Vector2 v2; //set whether horizontal or vertical moving platform
	private float totaldelay; //after a certain period of time, reverse direction of hover
	// Use this for initialization
	void Start () {
		rigidbody2D.velocity = CalV();
	}

	Vector2 CalV()
	{
		return new Vector2 (VELOCITY * v2.x * dir, VELOCITY * v2.y * dir);
	}
	// Update is called once per frame
	void Update () {
		totaldelay += Time.deltaTime;
		if (totaldelay > 7)
		{
			dir *= -1;
			rigidbody2D.velocity = CalV();
			totaldelay = 0;
		}
		else if (totaldelay > 5)
		{
			rigidbody2D.velocity = new Vector2(0,0);
		}
		/*if ((totaldelay > 5) && (rigidbody2D.velocity != new Vector2(0,0)))
		{
			oldv = rigidbody2D.velocity;
			rigidbody2D.velocity = new Vector2(0,0);
		}
		else if (totaldelay > 7)
		{
			rigidbody2D.velocity = oldv * -1;
			totaldelay = 0;
		}*/
	}
	/* don't use
	void OnCollisionEnter2D(Collision2D collInfo)
	{ //if the spring collides with a player, then that's the only time you want 
		//the player's spring collision method to be called.
		if (collInfo.gameObject.tag == "Player")
		{
			collInfo.gameObject.BroadcastMessage("HoverCollision");
		}
	}*/
}
