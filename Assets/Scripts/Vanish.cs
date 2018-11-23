using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Vanish : MonoBehaviour {
	private float totaldelay; //after a certain period of time, reverse direction of hover
	public float livetime; //set on the prefab, allows for changing of vanish time per platform if necessary
	GameObject[] vplats;
	bool appear = false; //holds where to make the objects appear or dissappear
	// Use this for initialization
	void Start () {
		vplats = GameObject.FindGameObjectsWithTag("VPlatform"); //get all the vanishing platforms in the stage
	}
	// Update is called once per frame
	void Update () {
		totaldelay += Time.deltaTime;
		if (totaldelay > livetime) //do this every time x seconds pass
		{
			totaldelay = 0;
			foreach (GameObject ob in vplats)
				{
					ob.transform.GetComponent<Collider2D>().enabled = appear; //make the collider dissapear
					ob.transform.GetComponent<Renderer>().enabled = appear; //make the visual of the platform dissappear
				}
			appear = !appear; //if they dissappeared this time, next time they will appear, and vice versa
			}
		}
	}
