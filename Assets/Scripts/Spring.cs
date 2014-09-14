using UnityEngine;
using System.Collections;

public class Spring : MonoBehaviour {
	public Vector3 dir;
	private GMScript.Springer sprop; //need to pass a struct to player to get all the properties of the spring, i think
	// Use this for initialization
	void Start () {
		Vector3 mag = new Vector3(transform.position.x,transform.position.y,transform.position.z);
		sprop.elRay = new Ray(mag,dir);
		if (this.tag == "Spring") //it will either be a spring or a big spring that has this script
		{
			sprop.intensity = 2;
		}
		else 
		{
			sprop.intensity = 3;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnCollisionEnter2D(Collision2D collInfo)
	{ //if the spring collides with a player, then that's the only time you want 
	  //the player's spring collision method to be called.
		if (collInfo.gameObject.tag == "Player")
		{
			collInfo.gameObject.BroadcastMessage("SpringCollision",sprop);
		}
	}
}
