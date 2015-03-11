using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameMaster : MonoBehaviour {

	public static GameMaster gm;
	public static List<GameObject> deckList = new List<GameObject>();
	public int debugSourceIDField; 
	public int debugDestinationIDField;
	public int debugDeckIDField;
	public int debugOrientationField;
	public int debugXField;
	public int debugYField;
	public int debugZField;

	public void setDebugDeckIDField(string value)
	{
		debugDeckIDField = int.Parse(value);
	}
	public void setDebugOrientationField(string value)
	{
		debugOrientationField = int.Parse(value);
	}
	public void setDebugXField(string value)
	{
		debugXField = int.Parse(value);
	}
	public void setDebugYField(string value)
	{
		debugYField = int.Parse(value);
	}

	public void setDebugZField(string value)
	{
		debugZField = int.Parse(value);
	}
	public void setDebugSourceIDField(string value)
	{
		debugSourceIDField = int.Parse (value);
	}
	public void setDebugDestinationIDField(string value)
	{
		debugDestinationIDField = int.Parse (value);
	}
	public void debugTransferCards()
	{
		transferCards (debugSourceIDField, debugDestinationIDField);
	}
	public void debugGenerateNewDeck()
	{
		generateNewDeck (debugDeckIDField, new Vector3(debugXField, debugYField, debugZField), new Vector3(0,0,0), debugOrientationField);
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

		//gm.generateNewDeck (2, 0, 0, 0);
		//deck = new Deck ();
		//deck.new_card (3,3);
		//deck.new_card (2,1);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void generateNewDeck(int id, Vector3 pos, Vector3 rotation, int orientation)
	{
		if (id > 0)
		{
			GameObject newDeck = (GameObject)Instantiate (new GameObject(),pos, Quaternion.Euler (rotation));
			newDeck.transform.localScale = new Vector3(1.1f, 1.1f, 0);
			newDeck.AddComponent ("Deck");
			newDeck.GetComponent <Deck>().deckID = id;
			newDeck.GetComponent<Deck>().setupLayout(orientation);
		}
		else
		{
			Debug.LogWarning("Operation denied. positive ID value required.");
		}
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
