using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameMaster : MonoBehaviour {

	public static GameMaster gm;
	public static List<GameObject> deckList = new List<GameObject>();
	public int debugSourceIDField; 
	public int debugDestinationIDField;

	public void setDebugSourceIDField(string value)
	{
		debugSourceIDField = int.Parse(value);
	}

	public void setDebugDestinationIDField(string value)
	{
		debugDestinationIDField = int.Parse (value);
	}

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

	public void generateNewDeck(int id, int x, int y, int z)
	{
		if (id > 0)
		{
			GameObject newDeck = (GameObject)Instantiate (new GameObject(),new Vector3(x,y,z), Quaternion.identity);
			newDeck.transform.localScale = new Vector3(1.1f, 1.1f, 0);
			newDeck.AddComponent ("Deck");
			newDeck.GetComponent <Deck>().deckID = id;
		}
		else
		{
			Debug.LogWarning("Operation denied. positive ID value required.");
		}
	}

	public void debugTransferCards()
	{
		transferCards (debugSourceIDField, debugDestinationIDField);
	}

	public void transferCards(int sourceID, int destinationID)
	{
		Debug.Log (" source hand ID = " + sourceID);
		Debug.Log (" destination hand ID = " + destinationID);
		Debug.Log (" source hand currently has " + searchByID (sourceID).GetComponent<Deck>().cards.Count + " cards");
		Debug.Log (" target hand currently has " + searchByID (destinationID).GetComponent<Deck>().cards.Count + " cards");
		//Debug.Log (" current hand currently has " + deckList[2].GetComponent<Deck>().cards.Count + " cards");
		//Debug.Log (" target hand currently has " + deckList[1].GetComponent<Deck>().cards.Count + " cards");
		searchByID (sourceID).GetComponent<Deck>().transferTopCardTo(searchByID (destinationID).GetComponent<Deck>());
	}


	private GameObject searchByID(int searchID)
	{
		return deckList.Find (x=>x.GetComponent<Deck>().deckID == searchID);
	}



}
