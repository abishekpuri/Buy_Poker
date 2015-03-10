using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameMaster : MonoBehaviour {

	public static GameMaster gm;
	public static List<Deck> deckList;

	public static void reportDeckToGameMaster(Deck currentDeck)
	{
		deckList.Add (currentDeck);
		Debug.Log ("Deck " + deckList.Count + "reported to gameMaster");
	}


	//Awake is called before start
	void Awake() {
		Card.cardSpriteList = Resources.LoadAll <Sprite> ("images/cards");
		Debug.Log ("Card sprite resourses loaded once and for all");
		deckList = new List<Deck>();
	}

	// Use this for initialization
	void Start () {

		if (gm == null) {
			gm = GameObject.FindGameObjectWithTag ("GameMaster").GetComponent<GameMaster>();
		}
		//deck = new Deck ();
		//deck.new_card (3,3);
		//deck.new_card (2,1);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
