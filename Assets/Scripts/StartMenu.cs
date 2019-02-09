using UnityEngine;
using System.Collections;

public class StartMenu : MonoBehaviour {

    public AudioClip mainMenuBGM;
	// Use this for initialization
	void Start () {
        AudioManager.instance.PlayMusic(mainMenuBGM);
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
