using UnityEngine;
using System.Collections;

public class StartMenu : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	/**
	Mouse Down Event Handler
*/
	void OnMouseDown()
	{
		GameObject o = GameObject.FindGameObjectWithTag("GM");
		Debug.Log(this.name);
		// if we clicked the play button
		if (this.name == "Play Text")
		
		{Debug.Log("oy");
			// load the game
			this.BroadcastMessage("NextLevel");
			
		}
	}
}
