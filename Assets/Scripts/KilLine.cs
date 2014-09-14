﻿using UnityEngine;
using System.Collections;

public class KilLine : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter2D(Collider2D collInfo)
	{
		if (collInfo.gameObject.tag == "Player")
		{
			collInfo.gameObject.BroadcastMessage("KillLineCollision");
			GameObject.FindGameObjectWithTag("GM").BroadcastMessage("StageFailed");
			//broadcast to game master object to stop timer and raise options for next stage
		}
	}
}
