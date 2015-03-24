using UnityEngine;
using System.Collections;

/*
 * Use this code on the card object to test card behavior.
 * 
 * 
 * 
 * 
 * */
public class DebugCardBehavior : MonoBehaviour {
		
	Card theCardToBeChallenged;
	int state=0;

	// Use this for initialization
	void Start () {

		theCardToBeChallenged = GetComponent<Card>();


	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnMouseDown()	// this function is called whenever you click. YOU NEED "collider" component on gameObject to make this work.
	{
		//Debug.Log ("mouse up");
		state++;
		switch (state) {
		case 2:
			theCardToBeChallenged.showBackground ();
			state = 0;
			break;
		case 1:
			theCardToBeChallenged.showFace ();
			break;
		}
	}

}
