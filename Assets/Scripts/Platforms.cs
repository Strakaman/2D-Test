using UnityEngine;
using System.Collections;

public class Platforms : MonoBehaviour {

    BoxCollider2D m_Collider;
    SpriteRenderer[] m_Sprites;
	//private Vector3 s; //box collider size to help with collision checks
	// Use this for initialization
	void Start () {
        m_Sprites = GetComponentsInChildren<SpriteRenderer>();
        m_Collider = GetComponentInChildren<BoxCollider2D>();
        //BoxCollider2D zollider = GetComponentInChildren<BoxCollider2D>();
        //s = zollider.size;
    }

    // Update is called once per frame
    void Update () {
	
	}

    public void SetColliderStatus(bool enableCollider)
    {
        m_Collider.enabled = enableCollider;
        if (enableCollider)
        {
            //SetTransparency(1);
        }
    }

    public void SetTransparency(float alphaPercentage)
    {
        Color c;
        foreach(SpriteRenderer spr in m_Sprites)
        {
            c = spr.color;
            c.a = alphaPercentage;
            spr.color = c;
        }
    }

    void OnCollisionEnter2D(Collision2D collInfo)
	{
		if (collInfo.gameObject.tag == "Player")
		{
			if (collInfo.gameObject.transform.position.y > (transform.position.y + m_Collider.size.y/2 )) 
			{//player has to be higher than the top of the platform, which should 
			//equal the middle of the platform plus its top half in distance
				collInfo.gameObject.BroadcastMessage("GroundSelf");
			}
		}
	}
}
