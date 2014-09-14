using UnityEngine;
using System.Collections;

public class Camera_Man : MonoBehaviour {
	private Transform pFollow;

	// Use this for initialization
	void Start () {
	
	}
	void Awake()
	{
		if (GameObject.FindWithTag("Player")){
		pFollow = GameObject.FindWithTag("Player").transform;
	}
		}
	// Update is called once per frame
	void Update () {

	
	}

	void FixedUpdate()
	{
		FollowPlayer();
	}

	void FollowPlayer()
	{
		// By default the target x and y coordinates of the camera are the player's current x and y coordinates.
		if (pFollow){
			float targetX = pFollow.position.x;
			float targetY = pFollow.position.y;
			transform.position = new Vector3(targetX,targetY,transform.position.z);
		}
		// If the difference is significant enough, set the camera's position to the target position with the same z component.
		//Position based movement, move to the exact position
		/*if(Mathf.Abs(transform.position.x - targetX) > .05)
		{
			transform.position = new Vector3(targetX, (targetY + transform.position.y)/2, transform.position.z);
		}
		else if (Mathf.Abs(transform.position.y - targetY) > 2)
		{

			transform.position = new Vector3(targetX, (targetY + transform.position.y)/2, transform.position.z);
		}*/

		//Translation based movement, move towards the exact position
		/*if (Mathf.Abs(targetX - transform.position.x) > .05) {
			transform.Translate((targetX - transform.position.x)/2,0,0);
				}
		else if (Mathf.Abs(targetY - transform.position.y) > 2) {
			transform.Translate(0,(targetY - transform.position.y)/2,0);
		}*/

	}
}
