#pragma strict
var downKey : KeyCode;
var upKey : KeyCode;
var rightKey : KeyCode;
var leftKey : KeyCode;
function Start () {

}

function Update () {

		if (Input.GetKey(upKey)) {
			Debug.Log("up");
		}
		if (Input.GetKey(rightKey))
		    {
			Debug.Log("right");
			//Rigidbody2D.velocity.x = 12;

		}
		if (Input.GetKey(leftKey))
		{
			Debug.Log("left");
			//Rigidbody2D.velocity.x = -12;
		}
		
}

	


