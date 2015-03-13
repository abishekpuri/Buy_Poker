using UnityEngine;
using System.Collections;
using System.Collections.Generic;



/*
 * Each deck is assigned a unique ID. It is manually assigned so far.
 * Only deck ID from 1 to 99 are valid right now.
 * deck ID 0 is preserved for main card deck.
 * deck ID 100 is preserved for auction.
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * */
public class GameMaster : MonoBehaviour {
	
	public static GameMaster gm;
	public static List<GameObject> deckList = new List<GameObject>();	//GameMaster keeps track of all decks in game.
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
		transferCards (debugSourceIDField, debugDestinationIDField, false);
	}
	public void debugGenerateNewDeck()
	{
		generateNewDeck (debugDeckIDField, new Vector3(debugXField, debugYField, debugZField), new Vector3(0,0,0), debugOrientationField);
	}
	
	public static void reportDeckToGameMaster(GameObject currentDeck)	// Every Decks in scene report themselves to gameMaster
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

		StartCoroutine (coStart ());
	}

	public IEnumerator coStart()	//Must be called through StartCoroutine()
	{
		yield return new WaitForSeconds(0.01f);
		generateNewDeck (1, new Vector3(0,-3,0), new Vector3(0,0,0),6);
		generateNewDeck (2, new Vector3(-5f,-3,0), new Vector3(0,0,0),6);
		generateNewDeck (3, new Vector3(5f,-3,0), new Vector3(0,0,0),6);
		searchByID (0).GetComponent <Deck> ().generateFullCardDeck ();
		yield return new WaitForFixedUpdate();		// WAIT until sprites in deck 0 are loaded
		searchByID (0).GetComponent <Deck> ().closeDeck ();
		searchByID (0).GetComponent <Deck> ().shuffle ();
		
		StartCoroutine (dealCards (5));

		yield return new WaitForSeconds(10f);

		transferCards (0,100,true);
		yield return new WaitForSeconds(1f);

	}
	
	// Update is called once per frame
	void Update () {

	}
	
	public void generateNewDeck(int id, Vector3 pos, Vector3 rotation, int orientation)
	{
		
		if (id > 0 && id < 100 && searchByID (id)==null)
		{
			GameObject newDeck = (GameObject)Instantiate (new GameObject(),pos, Quaternion.Euler (rotation));
			newDeck.transform.localScale = new Vector3(1.1f, 1.1f, 0);
			Deck newDeckComponent = (Deck)newDeck.AddComponent ("Deck");
			Debug.Log ("NEW Layout "+orientation+" setup for Deck ID = " + newDeck.GetComponent <Deck>().deckID);
			newDeckComponent.deckID = id;
			newDeckComponent.setLayoutType(orientation);
			
		}
		else
		{
			Debug.LogWarning("Operation denied. positive and unique ID value required.");
		}
	}
	
	public IEnumerator dealCards(int numberOfCards)	//must be called through StartCoroutine(dealCards(int));
	{
		yield return new WaitForSeconds(1f);	//DO NOT ERASE THIS PART. DEALING SHOULD NOT START BEFORE HANDS ARE REPORTED TO GAMEMASTER
		Debug.LogWarning("Card dealt to "+deckList.Count+" hands");
		for (int i=0; i<numberOfCards; i++)
			for (int j=0; j<deckList.Count; j++)
				if (deckList[j].GetComponent<Deck>().deckID>0 && deckList[j].GetComponent<Deck>().deckID<100)
				{
					searchByID(0).GetComponent<Deck>().transferTopCardTo (deckList[j].GetComponent<Deck>(), deckList[j].GetComponent<Deck>().deckID==1);
					yield return new WaitForSeconds(0.5f);
					
				}
		
	}
	
	public void transferCards(int sourceID, int destinationID, bool openCard)
	{

		//Debug.Log (" source hand ID = " + sourceID);
		//Debug.Log (" destination hand ID = " + destinationID);
		//Debug.Log (" source hand currently has " + searchByID (sourceID).GetComponent<Deck>().cards.Count + " cards");
		//Debug.Log (" target hand currently has " + searchByID (destinationID).GetComponent<Deck>().cards.Count + " cards");
		//Debug.Log (" current hand currently has " + deckList[2].GetComponent<Deck>().cards.Count + " cards");
		//Debug.Log (" target hand currently has " + deckList[1].GetComponent<Deck>().cards.Count + " cards");

		searchByID (sourceID).GetComponent<Deck>().transferTopCardTo(searchByID (destinationID).GetComponent<Deck>(), openCard);

	}
	
	
	private GameObject searchByID(int searchID)
	{
		return deckList.Find (x=>x.GetComponent<Deck>().deckID == searchID);
	}
	
	
	
}

