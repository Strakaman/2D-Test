using UnityEngine;
using System.Collections;

public class ShortRangeTeleporter : MonoBehaviour {
	Transform SRTBLocation;
	// Use this for initialization
	void Start () {
		SRTBLocation = GameObject.FindWithTag("SRTB").transform;
		//every SRTA should have an SRTB, which stores the target destination of the teleportation
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnCollisionEnter2D(Collision2D collInfo)
	{
		if (collInfo.gameObject.tag == "Player")
		{
			collInfo.gameObject.BroadcastMessage("SRTACollision",SRTBLocation);
		}
	}
}
