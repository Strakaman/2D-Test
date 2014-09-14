using UnityEngine;
using System.Collections;

public class Turbine : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerStay2D(Collider2D collInfo)
	{
		if(collInfo.gameObject.tag == "Player")
		{
			collInfo.gameObject.BroadcastMessage("WindCollision",transform);
		}
					
	}
}
