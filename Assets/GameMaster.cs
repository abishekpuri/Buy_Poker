using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * 
 * GameMaster contains static variables and functions which can be directly called from other classes.
 * It is usually the best place to code pre-scripted events.
 * 
 * 
 * 
 *  */

/*
 * Each deck is assigned a unique ID. It is manually assigned so far.
 * Only deck ID from 1 to 99 should be valid for player decks right now.
 * deck ID 0 is preserved for main card deck.
 * deck ID 100 is preserved for auction.
 * deck ID 101 is preserved for dump
 * deck ID 102
 * 
 * 
 * 
 * 
 * 
 * */
public class GameMaster : MonoBehaviour {

	// constant player ID value
	public static int UserID = 1;

	public static GameMaster gm;		// Enables gameMaster instance to be referenced from other classes, so that non-static functions can be called. It is currently never used.
	public static List<Deck> deckList = new List<Deck>();	//GameMaster keeps track of all decks in game.
	public static List<PlayerHand> playerList = new List<PlayerHand>();		//GameMaster keeps track of all players in game
	private static bool auctionInProgress = false;
	public static int winnerID;
	public bool gameEnd = false;
	public int debugSourceIDField; 			//Every single function and variables with name "debug" is bound to GUI buttons in gameScene.
	public int wins;
	public int auctionCardsLeft = 7;
	public int total_games;
	/*************************************This part is purely bound to Buttons in gameScene*******************************/
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
		registerNewPlayerHand (debugDeckIDField, new Vector3(debugXField, debugYField, debugZField), new Vector3(0,0,0), debugOrientationField);
	}
	/*************************************This part is purely bound to Buttons in gameScene*******************************/




	/*************************************Functions below are explicitly called by external calasses*******************************/
	public static void reportDeckToGameMaster(Deck currentDeck,bool Player=false)	// Every Decks in the scene report themselves to gameMaster
	{
		deckList.Add (currentDeck);
		Debug.Log ("Deck " + currentDeck.DeckID + " reported to gameMaster");
	}
	public static void terminateCurrentAuction()
	{
		auctionInProgress = false;
	}
	public static void requestCardTransfer(int sourceID, int destinationID, bool searchByPlayerID=false, bool openCard=false)
	{
		searchDeckByID (sourceID).GetComponent<Deck>().transferTopCardTo(searchDeckByID (destinationID).GetComponent<Deck>(), openCard);
	}
	public static int getHighestBidderID()
	{
		int currentMaxBid = 0;
		int currentPlayerID = 0;
		int currentPlayerPos = 0;
		for (int i=0; i<playerList.Count; i++)
		{
			if (playerList[i].getBidValue ()>=currentMaxBid)
			{
				currentMaxBid=playerList[i].getBidValue ();
				currentPlayerID=playerList[i].DeckID;
				currentPlayerPos = i;
			}
		}
		for (int i = 0; i<playerList.Count; ++i) {
			if (playerList [i].isAIControlled ()) {
				float value = playerList[currentPlayerPos].cash;
				playerList[i].WinningBidPercentage.Add(currentMaxBid/value);
						}
				}
		return currentPlayerID;
	}
	// bad design + laziness
	public static int getHighestBidValue()
	{
		int currentMaxBid = 0;
		//int currentPlayerID = 0;
		for (int i=0; i<playerList.Count; i++)
		{
			if (playerList[i].getBidValue ()>=currentMaxBid)
			{
				currentMaxBid=playerList[i].getBidValue ();
				//currentPlayerID=playerList[i].DeckID;
			}
		}
		return currentMaxBid;
	}

	public static PlayerHand getWinner(List<PlayerHand> players) {
		List<string> Types= new List<string>();
		Types.Add ("High Card");
		Types.Add ("One Pair");
		Types.Add ("Two Pair");
		Types.Add ("Three of a Kind");
		Types.Add ("Straight");
		Types.Add ("Flush");
		Types.Add ("Full House");
		Types.Add ("Four of a Kind");
		Types.Add ("Straight Flush");
		PlayerHand winner = players [0];
		for (int i = 1; i < players.Count; i++) {
						if (Types.FindIndex (a => a == players [i].CombinationType) > Types.FindIndex (a => a == winner.CombinationType)) {
								winner = players [i];
						} else if (Types.FindIndex (a => a == players [i].CombinationType) == Types.FindIndex (a => a == winner.CombinationType)) {
								if (players [i].CombinationValue > winner.CombinationValue) {
										winner = players [i];
								}
						}
				}
		return winner;
		}
	/*************************************Functions above are explicitly called by external calasses*******************************/





	
	//Awake is called before start. All static resources in game are loaded here. ***Overrides Awake() in MonoBehavior***
	void Awake() {
		Card.cardSpriteList = Resources.LoadAll <Sprite> ("images/cards_smooth");
		Debug.Log ("Card sprite resourses loaded once and for all");
		//deckList = new List<Deck>();
		if (gm == null) {
			gm = GameObject.FindGameObjectWithTag ("GameMaster").GetComponent<GameMaster>();
		}
	}
	
	// Use this for initialization. Overrides ***Start() in MonoBehavior***
	void Start () {
		//ResetPrefs();
		StartCoroutine (coStart ());
	}
	public void ResetPrefs() {
				PlayerPrefs.SetInt ("wins", 0);
				PlayerPrefs.SetInt ("total_games", 0);
				PlayerPrefs.SetFloat ("bottomCap", 0.1f);
		}
	public IEnumerator coStart()	//Must be called through StartCoroutine()
	{
		yield return new WaitForFixedUpdate();
		wins = PlayerPrefs.GetInt ("wins");
		total_games = PlayerPrefs.GetInt ("total_games");
		// setup player hands. Decks 0, 100, 101 and 102 are pre-generated inside the gameScene.
		registerNewPlayerHand (1, new Vector3(0,-3,0), new Vector3(0,0,0f),6,true);
		registerNewPlayerHand (2, new Vector3(-5f,-3,0), new Vector3(0,0,0),6,true);
		registerNewPlayerHand (3, new Vector3(5f,-3,0), new Vector3(0,0,0),6,true);

		searchDeckByID (0).generateFullCardDeck ();
		yield return new WaitForFixedUpdate();		// WAIT until all sprites in deck 0 are loaded. Otherwise, closeDeck() might not work.

		// enable AI control
		((PlayerHand)searchDeckByID (2)).setAIControl ();
		((PlayerHand)searchDeckByID (3)).setAIControl ();


		searchDeckByID (0).closeDeck ();
		searchDeckByID (0).shuffle ();


		yield return StartCoroutine (dealCards (5));	// return startCoroutine(); is same as thread.join(); Waits until the function returns.

		for (int i=0; i<playerList.Count; i++) {
			playerList[i].showGUI=true;
		}

		yield return new WaitForSeconds(0.5f);

		// Starts auction.
		while (auctionCardsLeft!= 0) {
						yield return StartCoroutine (auction ());
						auctionCardsLeft --;
				}

		searchDeckByID (102).setupLayout (3);

		// returns winner hand
		PlayerHand winner = getWinner (playerList);
		winnerID = winner.DeckID;
		if (winnerID == 1) {
						playerList [0].Winner ();
						PlayerPrefs.SetInt ("wins", wins + 1);
						PlayerPrefs.Save ();
				} else {
						playerList [0].Loser ();
				}
		// take winners' cards up to the table
		for (int i=0; i<20; i++)
			requestCardTransfer (winnerID,102,false, true);

		for (int i=0; i<playerList.Count; i++) {
			playerList[i].showCombination=true;
		}
		PlayerPrefs.SetInt ("total_games", total_games + 1);
		gameEnd = true;
	}

	private IEnumerator dealCards(int numberOfCards)	//must be called through StartCoroutine(dealCards(int));
	{
		yield return new WaitForSeconds(1f);	//DO NOT ERASE THIS PART. DEALING SHOULD NOT START BEFORE HANDS ARE REPORTED TO GAMEMASTER
		Debug.Log("Card dealt to "+(deckList.Count-2)+" hands");
		for (int i=0; i<numberOfCards; i++)
		{
			for (int j=0; j<deckList.Count; j++)
			{
				if (deckList[j].DeckID>0 && deckList[j].DeckID<100)
				{
					searchDeckByID(0).transferTopCardTo (deckList[j], deckList[j].DeckID==1);
					deckList[j].evaluateDeck();
					yield return new WaitForSeconds(0.2f);
				
				}
			}
			yield return new WaitForSeconds(0.2f);
		}
		yield return new WaitForSeconds (2f);
	}

	private IEnumerator auction()
	{
		requestCardTransfer (0,100,false, true);
		yield return new WaitForSeconds (1f);
		auctionInProgress = true;
		searchDeckByID (100).gameObject.AddComponent ("AuctionTimer");

		// show topCard to AI and let them calculate bid value.
		for (int j=0; j<playerList.Count; j++)
		{
			if (playerList[j].isAIControlled ())
				playerList[j].CalculateAIBid (searchDeckByID (100).peekTopCard ());
			//Debug.Log("Player " + j + " bid value = " + playerList[j].getBidValue());
		}

		// start auction
		while (auctionInProgress){yield return new WaitForSeconds (1f);}	// while auction is in progress

		// throws auction card into dump if no one pays for auction.
		requestCardTransfer (100,101,false, false);
		yield return new WaitForSeconds (1f);
		for (int j=0; j<playerList.Count; j++)
		{
			playerList[j].evaluateDeck ();
		}
		//Debug.Log (deckList.Count);												

		
	}
	
	// Update is called once per frame ***Overrides Update() from MonoBehavior***
	void Update () {

	}

	void OnGUI()	//Overrided
	{
		Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.localPosition);
		//GUI.Label (new Rect (screenPos.x + 150, Camera.main.pixelHeight - screenPos.y-200, 200, 20), "Number of Wins: " + PlayerPrefs.GetInt("wins"));
		//GUI.Label (new Rect (screenPos.x + 150, Camera.main.pixelHeight - screenPos.y-180, 200, 20), "Total Games: " + PlayerPrefs.GetInt("total_games"));
		GUI.Label (new Rect (screenPos.x - 310, Camera.main.pixelHeight - screenPos.y - 180, 200, 20), "Points : " + PlayerPrefs.GetInt ("Points"));
		GUI.Label (new Rect (screenPos.x - 310, Camera.main.pixelHeight - screenPos.y - 150, 200, 20), "Cards Left : " + auctionCardsLeft);
		if (GUI.Button (new Rect (screenPos.x - 330, Camera.main.pixelHeight - screenPos.y-50, 50, 80), "40 Pt")) {
						bool prize = playerList [0].buyPrize (40);
						if (prize == true) {
							requestCardTransfer (0,1,false, true);
						}
				}
		if (gameEnd) {
						GUI.Label (new Rect (screenPos.x - 70, Camera.main.pixelHeight - screenPos.y - 120, 200, 20), "The Winner is DECK ID : " + winnerID);
						if (GUI.Button (new Rect (screenPos.x - 70, Camera.main.pixelHeight - screenPos.y - 100, 200, 20), "Play Again")) {
								for (int i = 0; i < deckList.Count; i++) {
										deckList [i].destroyAll ();
								}
								for (int i = 0; i < playerList.Count; ++i) {
										playerList [i].setCash (100);
								}
								gameEnd = false;
								auctionCardsLeft = 7;
								Start ();
						}

				}
		if (GUI.Button (new Rect (screenPos.x-390, Camera.main.pixelHeight - screenPos.y + 150, 80, 20), "Reset")) {
						ResetPrefs ();
				}
		//Use this function to draw GUI stuff. Google might help. This fucntion is bound to GameMaster object.
		//GUI.Label (new Rect (520,427,100,25),(searchDeckByID (1)).CombinationType);
	}
	
	public void registerNewPlayerHand(int id, Vector3 pos, Vector3 rotation, int orientation,bool Player=false)
	{
		
		if (id > 0 && id < 100 && searchDeckByID (id)==null)
		{
			GameObject newDeck = (GameObject)Instantiate (new GameObject(),pos, Quaternion.Euler (rotation));
			newDeck.transform.localScale = new Vector3(1.2f, 1.2f, 0);	//We can change this around.
			PlayerHand newHandComponent = (PlayerHand)newDeck.AddComponent ("PlayerHand");
			newHandComponent.DeckID = id;
			Debug.Log ("NEW DECK => Deck ID = "+newHandComponent.DeckID+", Layout = " + orientation);
			if (Player) {
				playerList.Add (newHandComponent);
			}
			newHandComponent.setLayoutType(orientation);
			
		}
		else
		{
			Debug.LogWarning("Operation denied. positive and unique ID value required.");
		}
	}

	
	public static Deck searchDeckByID(int searchID)
	{
		return deckList.Find (x => x.DeckID == searchID);
	}

	
}

