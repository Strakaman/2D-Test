﻿using UnityEngine;
using System.Collections;

public class Goal : MonoBehaviour {

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
			collInfo.gameObject.BroadcastMessage("GoalCollision");
			GameObject.FindGameObjectWithTag("GM").BroadcastMessage("StageClear");
			//broadcast to game master object to stop timer and raise options for next stage
		}
	}


}
