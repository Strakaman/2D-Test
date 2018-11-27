using UnityEngine;
using System.Collections;

public class ZipLine2018 : MonoBehaviour {
    // Use this for initialization

    [SerializeField]
    float zipTime = 3f; //time between when player starts zipping and when zipline hits the end

    float elapsedTime = 0f;
    [SerializeField]
    Transform startPosition;
    [SerializeField]
    Transform endPosition;
    bool zipping = false;

    public Collider2D m_collider;
    CharacterController2D playerZipping;
	void Start () {
        //StartCoroutine(StartZipLining());

    }

    // Update is called once per frame
    void Update () {
	
	}

   IEnumerator StartZipLining()
    {
        zipping = true;
        elapsedTime = 0;
        while (elapsedTime < zipTime)
        {
            transform.position =  Vector3.Lerp(startPosition.position, endPosition.position, (elapsedTime/zipTime));
            elapsedTime += .01f;
            yield return new WaitForSeconds(.01f);
        }

        playerZipping.ZipJump();

        yield return new WaitForSeconds(2f);
        ResetZipLine();
    }

    void ResetZipLine()
    {
        zipping = false;
        transform.position = startPosition.position;
        m_collider.enabled = true;
    }

	void OnTriggerEnter2D(Collider2D collInfo)
	{
        //if the zipline collides with a player, then that's the only time you want 
        //the player's spring collision method to be called.
        m_collider.enabled = false; //only trigger once player activates zipline
        if (collInfo.gameObject.tag == "Player")
		{
            playerZipping = collInfo.gameObject.GetComponent<CharacterController2D>();
            playerZipping.ZipCollision(transform);
            //collInfo.gameObject.BroadcastMessage("ZipCollision", transform); //disable player movement and gravity and such
            collInfo.transform.parent = transform;
            if (!zipping)
            { //if player manages to get back on zipline while its falling, don't restart courotine
                StartCoroutine(StartZipLining());
            }
        } 
	}

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
           // collision.transform.parent = null;
        }
    }
}
