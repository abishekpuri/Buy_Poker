using UnityEngine;
using System.Collections;

public class DebugDeckBehavior : MonoBehaviour {

	Deck theDeckToBeChallenged;
	int rankTextField;
	int suitTextField;

	// Use this for initialization
	void Start () {
		theDeckToBeChallenged = GetComponent<Deck>();
		//theDeckToBeChallenged.generateFullCardDeck ();
		//theDeckToBeChallenged.shuffle ();
		//theDeckToBeChallenged.setupOrientation (1);
		//theDeckToBeChallenged.openDeck ();
		//theDeckToBeChallenged.animation_merge ();
		Debug.Log ("cardCount = " + theDeckToBeChallenged.cards.Count);

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void add_new()
	{
		theDeckToBeChallenged.new_card (rankTextField, suitTextField);
	}
}
