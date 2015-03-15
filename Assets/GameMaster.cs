using UnityEngine;
using System.Collections;
using System.Collections.Generic;



/*
 * Each deck is assigned a unique ID. It is manually assigned so far.
 * Only deck ID from 1 to 99 are valid right now.
 * deck ID 0 is preserved for main card deck.
 * deck ID 100 is preserved for auction.
 * deck ID 101 is preserved for dump
 * 
 * 
 * 
 * 
 * 
 * 
 * */
public class GameMaster : MonoBehaviour {
	
	public static GameMaster gm;
	private static List<Deck> deckList = new List<Deck>();	//GameMaster keeps track of all decks in game.
	private static List<Player> playerList = new List<Player>();		//GameMaster keeps track of all players in game
	private static bool auctionInProgress = false;
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
		requestCardTransfer (debugSourceIDField, debugDestinationIDField);
	}
	public void debugGenerateNewDeck()
	{
		generateNewDeck (debugDeckIDField, new Vector3(debugXField, debugYField, debugZField), new Vector3(0,0,0), debugOrientationField);
	}
	
	public static void reportDeckToGameMaster(Deck currentDeck)	// Every Decks in scene report themselves to gameMaster
	{
		deckList.Add (currentDeck);
		Debug.Log ("Deck " + currentDeck.DeckID + " reported to gameMaster");
	}
	public static void terminateCurrentAuction()
	{
		auctionInProgress = false;
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
		yield return new WaitForFixedUpdate();
		generateNewDeck (1, new Vector3(0,-3,0), new Vector3(0,0,0),6);
		generateNewDeck (2, new Vector3(-5f,-3,0), new Vector3(0,0,0),6);
		generateNewDeck (3, new Vector3(5f,-3,0), new Vector3(0,0,0),6);
		searchByID (0).generateFullCardDeck ();
		yield return new WaitForFixedUpdate();		// WAIT until sprites in deck 0 are loaded
		searchByID (0).closeDeck ();
		searchByID (0).shuffle ();
		
		StartCoroutine (dealCards (3));

		yield return new WaitForSeconds(7f);


		for (int i=0; i<5; i++) {
			requestCardTransfer (0,100,false, true);
			yield return new WaitForSeconds (1f);
			auctionInProgress = true;
			searchByID (100).gameObject.AddComponent ("CountdownTimer");
			while (auctionInProgress){yield return new WaitForSeconds (1f);}	// while auction is in progress
			requestCardTransfer (100,101,false, false);
			yield return new WaitForSeconds (1f);
		}
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
			newDeckComponent.DeckID = id;
			Debug.Log ("NEW DECK => Deck ID = "+newDeckComponent.DeckID+", Layout = " + orientation);
			newDeckComponent.setLayoutType(orientation);
			
		}
		else
		{
			Debug.LogWarning("Operation denied. positive and unique ID value required.");
		}
	}

	public void startTimer()
	{
		//AddCom
	}
	
	public IEnumerator dealCards(int numberOfCards)	//must be called through StartCoroutine(dealCards(int));
	{
		yield return new WaitForSeconds(1f);	//DO NOT ERASE THIS PART. DEALING SHOULD NOT START BEFORE HANDS ARE REPORTED TO GAMEMASTER
		Debug.Log("Card dealt to "+(deckList.Count-2)+" hands");
		for (int i=0; i<numberOfCards; i++)
			for (int j=0; j<deckList.Count; j++)
				if (deckList[j].DeckID>0 && deckList[j].DeckID<100)
				{
					searchByID(0).transferTopCardTo (deckList[j], deckList[j].DeckID==1);
					yield return new WaitForSeconds(0.5f);
					
				}
	}
	
	public static void requestCardTransfer(int sourceID, int destinationID, bool searchByPlayerID=false, bool openCard=false)
	{
		searchByID (sourceID).GetComponent<Deck>().transferTopCardTo(searchByID (destinationID).GetComponent<Deck>(), openCard);
	}
	
	
	private static Deck searchByID(int searchID)
	{
		return deckList.Find (x=>x.DeckID == searchID);
	}
	
	
	
}

