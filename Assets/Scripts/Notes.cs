using UnityEngine;
using System.Collections;

//Trampoline - simple line over a platform because bounciness can affect side of element as well
//Platform - same problem but with friction, so all celings will either neef seperate material or all platforms will have to have
//a line of real material over a box of fake one, also consider using edge collider for all platform based assets
//ALL COLLISION BASED WITH PLAYER NEED TO SET UP SOME SORT OF CONTEXT ENUMERATOR
//Spring - all springs have a ray to indicate vector of movement
//Big Spring 2x as powerful
//Zipline - velocity of player equals velocity of zipline, so that they move together, remove gravity while zipline
//zipline also comes with a ray to indicate zipline travel direction, on jump off reset gravity and use zipline vector to calculate jump speed vector
//short range teleporterA - stores a transform location in its class, on collison, go to that location
//short range teleporterB - no collision method, location of SRTB is stored in SRTA so player can teleport to SRTB
//Rappel? No Clue
//Spider-man esque grappling hook and swings, additionally 
//climb up a rope or ladder?
//sliding? shrinks hit box in half and player moves at a constantly velocity while sliding to avoid getting stuck under a platform
// jet packs maybe
// maybe repulsive and attactive magnets



/* 
 * stage failure  - stop timer, stop any player movement, option to restart stage, or quit
 * stage success - stop timer, unlock next stage, option to go to next level, restart stage, or quit
 * on gui draw options if we are in option mode
 * figure out how to creation option-esque menu
 * 
 */
public class Notes : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
