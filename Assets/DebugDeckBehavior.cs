using UnityEngine;
using System.Collections;

public class DebugDeckBehavior : MonoBehaviour {

	Deck theDeckToBeChallenged;
	int rankTextField;
	int suitTextField;
	int layoutTextField;

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

	public void setLayoutTextField(string value)
	{
		layoutTextField = int.Parse(value);
	}

	public void setRankTextField(string value)
	{
		rankTextField = int.Parse(value);
	}

	public void setSuitTextField(string value)
	{
		suitTextField = int.Parse(value);
	}

	public void spread()
	{
		theDeckToBeChallenged.setupLayout (layoutTextField);
	}


	public void add_new()
	{
		theDeckToBeChallenged.new_card (rankTextField, suitTextField);
	}
}
