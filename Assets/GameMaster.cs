using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameMaster : MonoBehaviour {

	public static GameMaster gm;
	public static List<GameObject> deckList = new List<GameObject>();

	public static void reportDeckToGameMaster(GameObject currentDeck)
	{
		deckList.Add (currentDeck);
		Debug.Log ("Deck " + (deckList.Count-1) + "reported to gameMaster");
	}


	//Awake is called before start
	void Awake() {
		Card.cardSpriteList = Resources.LoadAll <Sprite> ("images/cards");
		Debug.Log ("Card sprite resourses loaded once and for all");
		//deckList = new List<Deck>();
		if (gm == null) {
			gm = GameObject.FindGameObjectWithTag ("GameMaster").GetComponent<GameMaster>();
		}
	}

	// Use this for initialization
	void Start () {


		//deck = new Deck ();
		//deck.new_card (3,3);
		//deck.new_card (2,1);
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	public void dealCards()
	{
		Debug.Log (" current hand currently has " + deckList[2].GetComponent<Deck>().cards.Count + " cards");
		Debug.Log (" target hand currently has " + deckList[1].GetComponent<Deck>().cards.Count + " cards");
		deckList[2].GetComponent<Deck>().transferTopCardTo(deckList[1].GetComponent<Deck>());
	}






}
