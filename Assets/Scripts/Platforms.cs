using UnityEngine;
using System.Collections;

public class Platforms : MonoBehaviour {
	private Vector3 s; //box collider size to help with collision checks
	// Use this for initialization
	void Start () {
		BoxCollider2D zollider = GetComponentInChildren<BoxCollider2D> ();
		s = zollider.size;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnCollisionEnter2D(Collision2D collInfo)
	{
		if (collInfo.gameObject.tag == "Player")
		{
			if (collInfo.gameObject.transform.position.y > (transform.position.y + s.y/2 )) 
			{//player has to be higher than the top of the platform, which should 
			//equal the middle of the platform plus its top half in distance
				collInfo.gameObject.BroadcastMessage("GroundSelf");
			}
		}
	}
}
