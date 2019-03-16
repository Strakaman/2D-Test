using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Vanish2018 : MonoBehaviour
{
    Platforms[] platFormsIVanish;
    private float timeTracker;
    public float InitialVanishDelay = 0.5f; //amount of time to wait before on start before triggering Decay
    float livetime = 2; //time before Platforms completely Disapppear
    private WaitForSeconds couroutineDelay;
    void Start()
    {
        couroutineDelay = new WaitForSeconds(.1f); 
        platFormsIVanish = GetComponentsInChildren<Platforms>();
        StartCoroutine(VanishLoop());
    }

    IEnumerator VanishLoop()
    {
        yield return new WaitForSeconds(InitialVanishDelay);
        while (!GMScript.state.Equals(GMScript.Context.dead))
        {
            timeTracker = 0f; //reset time since last vanish
            //fade out
            while (timeTracker < livetime)
            {
                if (GMScript.paused) { continue; }
                foreach(Platforms currPlat in platFormsIVanish)
                {
                    currPlat.SetTransparency(1-timeTracker / livetime);
                }
                timeTracker += .1f;
                yield return couroutineDelay;
            }
            //actually disable collider to match fade out effect
            foreach (Platforms currPlat in platFormsIVanish)
            {
                currPlat.SetColliderStatus(false);
            }
            timeTracker = 0f;
            //reappear after 1 second
            while (timeTracker < 1)
            {
                if (GMScript.paused) { continue; }
                timeTracker += .1f;
                yield return couroutineDelay;
            }
            foreach (Platforms currPlat in platFormsIVanish)
            {
                currPlat.SetColliderStatus(true);
            }
        }
    }
}
