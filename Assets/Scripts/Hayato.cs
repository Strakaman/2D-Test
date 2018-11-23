using UnityEngine;
using System.Collections;

public class Hayato : MonoBehaviour
{

	private Vector3 s; //box collider size to help with raycasting
	private Vector3 c; //box collider center to help with raycasting
	public KeyCode upKey = new KeyCode (); //Controls
	public KeyCode leftKey = new KeyCode ();
	public KeyCode rightKey = new KeyCode ();
	private float gravity; //gravity sometimes removed for certain physics, need to be able to get it back
	private const float HSPEED = 5; //store magnitude of horizontal speed
	private const float ACCEL = 25;
	private const float MAXSPEED = 10;
	private const float JUMPSPEED = 20;
	private const float SPRBOTDIST = 0.5f; //store sprite distance from center of sprite to bottom, needed for accurate raycasting
	private bool grounded; //stores whether or not player is on the ground

	// Use this for initialization8

	void Start ()
	{
		gravity = GetComponent<Rigidbody2D>().gravityScale; //store off the gravity for later reference
		BoxCollider2D zollider = GetComponent<BoxCollider2D> ();
		s = zollider.size;
		c = zollider.offset;
	}

	// Update is called once per frame

	//if speed is less than max speed absolute value, increment towards max speed
	//if speed and input is the same, combine speed with acceleration, if they are different, substract current speed from acceleration
	//if speed is greater than the max speed, consider decrementing it so player doesn't say, keep that spring speed it got
	void Update ()
	{		
		//Unfortunately, must check under player to see if their is a ground beneath them. 
			//Used in case player runs off platform or platform dissappears under player 
			Vector2 p = transform.position;
			float x = p.x + c.x + s.x / 2 * -1; //left bottom
			float y = (p.y + c.y - s.y / 2); // bottom
			float x2 = x + s.x; //right bottom
			
			RaycastHit2D hit2 = Physics2D.Raycast(new Vector2(x2,y),new Vector2(0,-1),.5f,9);
			RaycastHit2D hit = Physics2D.Raycast(new Vector2(x,y),new Vector2(0,-1),.5f,9); //check under left and right side of player
			if ((!hit.collider) && (!hit2.collider))
			{	
				UnGroundSelf(); //if neither side of player bottom hits anything, they are airborne 
			}
			if (Input.GetKeyDown (upKey)) { 
			int direction;
					if (GMScript.state.Equals(GMScript.Context.normal)) {
						if (grounded){
							Jump ();
						} else if (CanWallJump(out direction)) {
							WallJump(direction);
						}
					} else if (GMScript.state.Equals (GMScript.Context.zipping)) {
						ZipJump();
					} else if (GMScript.state.Equals(GMScript.Context.ringing)) {
							GetComponent<Rigidbody2D>().gravityScale = gravity;
							Jump();
							GMScript.state = GMScript.Context.normal;
							if (Input.GetKey(rightKey))
							{
								GetComponent<Rigidbody2D>().velocity = new Vector2 (MAXSPEED/5, GetComponent<Rigidbody2D>().velocity.y);
							}
							else if (Input.GetKey(leftKey))
							{
								GetComponent<Rigidbody2D>().velocity = new Vector2 (-MAXSPEED/5, GetComponent<Rigidbody2D>().velocity.y);
							}
					}		
			}
			//only really call this if user has pressed the button and is in a normal/wall state
			if ((Input.GetKey (rightKey)) && ((int)GMScript.state < 3)) {
					CalcHVeloctity (Mathf.Sign (1));
					// oldspeed code rigidbody2D.velocity = new Vector2 (HSPEED, rigidbody2D.velocity.y); 
			} else if ((Input.GetKey (leftKey)) && ((int)GMScript.state < 3)) { 
					//else if, don't need to check left key if right key is being pressed
					CalcHVeloctity (Mathf.Sign (-1));
					// old speed code rigidbody2D.velocity = new Vector2 (-HSPEED, rigidbody2D.velocity.y);
			}
	}

	private void Jump ()
	{//for modularity purposes
			GetComponent<Rigidbody2D>().velocity = new Vector2 (GetComponent<Rigidbody2D>().velocity.x, JUMPSPEED);
			UnGroundSelf();
	}
	private bool CanWallJump(out int dir)
	{
		Vector2 p = transform.position;
		// Check collisions left and right to see if user is next to a wall
		for (int i = 0; i<3; i ++) { //use loop to create three rays equally distributed of player
			float x = p.x + c.x + s.x / 2 * -1; //check the left of the player for a wall first
			float y = (p.y + c.y - s.y / 2) + s.y /2 * i; // shoot a ray from the top, middle, and bottom
			float x2 = x + s.x; //gets other side of player 
			//Ray ray = new Ray (new Vector3(x,y,0), new Vector3 (-1,0,0)); //create ray and cast it left
			//RaycastHit2D hit = Physics2D.Raycast(ray.origin,ray.direction,.1f,9); //.1f to be more or less touching wall
			RaycastHit2D hit = Physics2D.Raycast(new Vector2(x,y),new Vector2(-1,0),.1f,9); //check left side for wall jump, then right side
			RaycastHit2D hit2 = Physics2D.Raycast(new Vector2(x2,y),new Vector2(1,0),.1f,9);

			if (hit && hit.collider && hit.collider.tag.Equals("Wall")) {
				dir = 1; //because there is a wall on the left, the wall jump should be to the right
					return true;
			}
			else if(hit2 && hit2.collider && hit2.collider.tag.Equals("Wall")){
				dir = -1; //because there is a wall on the right, wall jump should be to the left
				return true;
			}
		}
		dir = 0;
		return false;
	}

	private void WallJump(int dir)
	{
		GetComponent<Rigidbody2D>().velocity = new Vector2(MAXSPEED * dir,.9f *JUMPSPEED);
	}

	private void CalcHVeloctity (float dir)
	{ //dir variable passed in as indication of whether player pressed left or right
			if (GetComponent<Rigidbody2D>().velocity.x != (MAXSPEED * dir)) { //only accelerate if player hasn't already hit it's max velocity in that direction
					float newV = GetComponent<Rigidbody2D>().velocity.x + (ACCEL * Time.deltaTime * dir); //increase speed in whatever direction player pressed
					if (Mathf.Abs (newV) > MAXSPEED) {
							newV = MAXSPEED * dir; //account for if velocity change would send user over max horizontal speed
					}
					GetComponent<Rigidbody2D>().velocity = new Vector2 (newV, GetComponent<Rigidbody2D>().velocity.y); //set speed to value established by equations
			}
	}

	private void Stop()
	{
		GetComponent<Rigidbody2D>().velocity = new Vector2(0,0);
		GetComponent<Rigidbody2D>().gravityScale = 0;
	}

	public void SpringCollision (GMScript.Springer sprop)
	{	//called when player collides with spring from spring script
			//springs launch you pretty hard
			//rigidbody2D.velocity = new Vector2 (rigidbody2D.velocity.x, JUMPSPEED * sprop.intensity);
		transform.position = sprop.elRay.origin; //move to center of spring so that you can hit it from any side and get the same effect
		float mag = JUMPSPEED * sprop.intensity; //combine ray's direction with player speed default. differentiate between spring and big spring with intensity
		float dirx = sprop.elRay.direction.x;
		float diry = sprop.elRay.direction.y;
		if (Mathf.Abs(dirx) != Mathf.Abs(diry)){
			GetComponent<Rigidbody2D>().velocity = new Vector2 (dirx * MAXSPEED, diry * mag);
		}
		else { //want the 45 degree angle spring to have more horizontal power
			GetComponent<Rigidbody2D>().velocity = new Vector2(Mathf.Sign(dirx) * MAXSPEED, diry * mag);
		}	
			UnGroundSelf();
		if (!GMScript.state.Equals(GMScript.Context.dead))
		{
			GMScript.state = GMScript.Context.normal; //just in case you zipline into the teleporter, don't want to be stuck in that state
		}
	}

	public void SRTACollision (Transform teleportTo)
	{
			transform.position = teleportTo.position;
			if (!GMScript.state.Equals(GMScript.Context.dead))
			{
				GMScript.state = GMScript.Context.normal; //just in case you zipline into the teleporter, don't want to be stuck in that state
			}
			//just go to whereever
	}

	public void ZipCollision (Transform attachto)
	{
		GMScript.state = GMScript.Context.zipping;
		transform.position = attachto.position;
		Vector2 jVel = new Vector2 (MAXSPEED * Mathf.Cos(-0.174532925f), MAXSPEED * Mathf.Sin(-0.174532925f));
		GetComponent<Rigidbody2D>().velocity = jVel;
		GetComponent<Rigidbody2D>().gravityScale = 0;
		Invoke("ZipJump",4.4f); //after 4.2 seconds (based on zipline length), you have reached the end of the zipline if you are still on it

	}

	public void ZipJump()
	{
		if (GMScript.state.Equals(GMScript.Context.zipping))
		{
			GetComponent<Rigidbody2D>().velocity = new Vector2 (MAXSPEED, GetComponent<Rigidbody2D>().velocity.y);
			GMScript.state = GMScript.Context.normal;
			GetComponent<Rigidbody2D>().gravityScale = gravity;
		}
	}

	public void HoverCollision ()
	{
		//rigidbody2D.velocity = new Vector2 (0, 0);
		//rigidbody2D.gravityScale = 0;
	}

	public void GoalCollision()
	{
		Stop();
	}

	public void KillLineCollision()
	{
		Stop();
	}

	public void RingCollision(Transform ringTransform)
	{
		GMScript.state = GMScript.Context.ringing;
		GetComponent<Rigidbody2D>().velocity = new Vector2(0,0);
		transform.position = ringTransform.position;
		GetComponent<Rigidbody2D>().gravityScale = 0;

	}

	public void WindCollision(Transform lol)
	{
		//rigidbody2D.AddForce(new Vector2(0,.1f));	
		//if (rigidbody2D.velocity.y > 9)
		//{
			GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x, 6f);
		//float x = (transform.position.y - lol.position.y);
		//x /= 3200;
		//x /= 999.75f;
		//Debug.Log(rigidbody2D.velocity.y);
		//rigidbody2D.AddForce(new Vector2(0,x));	
		//rigidbody2D.AddForce(new Vector2(0,.01f));	
		//}
	}
	public void GroundSelf()
	{
		grounded = true;
	}

	public void UnGroundSelf()
	{
		grounded = false;
	}
}

//Junk Code
/*else {
				Vector2 p = transform.position;
				// Check collisions above and below
				for (int i = 0; i<3; i ++) {
				float x = (p.x + c.x - s.x / 2) + s.x / 2 * i; // Left, centre and then rightmost point of collider
				float y = p.y + c.y + s.y / 2 * -1; // Bottom of collider
				Ray ray = new Ray (new Vector3(x, y,0), new Vector3 (0, -1,0));
				Debug.DrawRay(ray.origin,ray.direction);
				RaycastHit hit;
//				Vector3 vp = ray.origin;
//				Vector3 vt = new Vector3(ray.origin.x,ray.origin.y + 2f,0);
				if ((Physics.Raycast(ray,out hit,2f))) {
					// Get Distance between player and ground
					float dst = Mathf.Abs(ray.origin.y - hit.point.y);
					Debug.Log(dst);
					if ((hit.collider == null) || (hit.collider.sharedMaterial == null))
					{
						break;
					}
					string materialname = hit.collider.sharedMaterial.name;
					if ((dst < .01))	{
						grounded = true;
						Debug.Log(materialname);
						break;
					}
					else {
						grounded = false;
					}
				} */

// Stop player's downwards movement after coming within skin width of a collider

/*Ray groundcheck = new Ray (new Vector3(transform.position.x,transform.position.y - SPRBOTDIST,0), new Vector3 (0, -1,0));
					RaycastHit2D result = Physics2D.Raycast (groundcheck.origin, groundcheck.direction, .01f);
			Debug.DrawRay(groundcheck.origin,groundcheck.direction,Color.green);
					if ((result.collider != null) &&(result.collider.sharedMaterial != null)) { 
							string materialname = result.collider.sharedMaterial.name;
							Debug.Log("yo + Context: " + materialname);	
							if (materialname == "Platform") {
									grounded = true;
							} else {
									grounded = false;
									Debug.Log ("not grounded + Context: " + state);
							}*/
