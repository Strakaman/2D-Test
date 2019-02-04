using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuneRotate : MonoBehaviour {

    public Vector3 rotateAroundPoint = new Vector3(0, 0, 5);
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(rotateAroundPoint, 1);

    }
}
