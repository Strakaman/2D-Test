using UnityEngine;
using System.Collections;

public class Spring : MonoBehaviour {
	public Vector3 dir;
	private GMScript.Springer sprop; //need to pass a struct to player to get all the properties of the spring, i think
    private Collider2D m_Collider;
    [SerializeField]
    private Animator animator;

    // Use this for initialization
	void Start () {
		Vector3 mag = new Vector3(transform.position.x,transform.position.y,transform.position.z);
		sprop.elRay = new Ray(mag,dir);
		if (this.tag == "Spring") //it will either be a spring or a big spring that has this script
		{
			sprop.intensity = 2;
		}
		else 
		{
			sprop.intensity = 3;
		}
        if (dir.x == dir.y)
        {
            sprop.intensity *= 1.5f; //for some reason when the springs are diagonal, their power is diminished. TODO: proper vector math to figure out why this is happening
        }
        m_Collider = GetComponent<Collider2D>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	//void OnCollisionEnter2D(Collision2D collInfo) //makes more sense to use a trigger
    void OnTriggerEnter2D(Collider2D collInfo)
	{ //if the spring collides with a player, then that's the only time you want 
	  //the player's spring collision method to be called.
		if (collInfo.gameObject.tag == "Player")
		{
            
			collInfo.gameObject.BroadcastMessage("SpringCollision",sprop);
		}
        m_Collider.enabled = false;
        animator.SetBool("Hit", true);
        Invoke("Renable", .75f);
	}

    void Renable()
    {
        m_Collider.enabled = true;
        animator.SetBool("Hit", false);
    }
}
