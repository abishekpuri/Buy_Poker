using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * 
 * GameMaster contains static variables and functions which can be directly called from other classes.
 * It is usually the best place to code for pre-scripted events.
 * 
 * Actual gameplay script is written in this file.
 * 
 *  */

/*
 * Each deck is assigned a unique ID. It is manually assigned so far.
 * Only deck ID from 1 to 99 should be valid for player decks right now.
 * deck ID 0 is preserved for main card deck.
 * deck ID 100 is preserved for auction.
 * deck ID 101 is preserved for dump
 * deck ID 102 is preserved for display table
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
	public static List<Deck> deckList;// = new List<Deck>();	//GameMaster keeps track of all decks in game.
	public static List<PlayerHand> playerList;// = new List<PlayerHand>();		//GameMaster keeps track of all players in game
	private static bool auctionInProgress = false;
	public static bool earlyAuctionEnd = false;
	public static int winnerID;
	public static bool gameBegins = false;
	public static bool roundEnd = true;
	public static bool gameEnd = false;
	public static bool displayGameResult = false;
	public int wins;
	public int auctionCardsLeft;
	public int roundsLeft;
	public int total_games;
	public int gameWinnerID;



	public static void endAuctionEarly() {
		earlyAuctionEnd = true;
	}


	/*************************************Functions below are explicitly called by external classes*******************************/
	public static void reportDeckToGameMaster(Deck currentDeck,bool Player=false)	// Every Decks in the scene report themselves to gameMaster
	{
		deckList.Add (currentDeck);
		Debug.Log ("Deck " + currentDeck.DeckID + " reported to gameMaster, deckListSize="+deckList.Count);
	}
	public static void terminateCurrentAuction()
	{
		auctionInProgress = false;
	}
	public static void requestCardTransfer(int sourceID, int destinationID, bool openCard=false, int rank=0, int suit=0)
	{
		searchDeckByID (sourceID).GetComponent<Deck>().transferCardTo(searchDeckByID (destinationID).GetComponent<Deck>(), openCard, rank, suit);
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
	public static int getGameWinnerID()
	{
		int currentMaxRoundPoints = 0;
		int currentPlayerID = 0;
		for (int i=0; i<playerList.Count; i++)
		{
			if (playerList[i].RoundPoints>=currentMaxRoundPoints)
			{
				currentMaxRoundPoints=playerList[i].RoundPoints;
				currentPlayerID=playerList[i].DeckID;
			}
		}
		return currentPlayerID;
	}

	/*************************************Functions above are explicitly called by external calasses*******************************/

	//NOTE : Delete the commented section if you agree with my change
	public static PlayerHand getWinner(List<PlayerHand> players) {
		/*
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
		*/
		PlayerHand winner = players [0];
		for (int i = 1; i < players.Count; i++) {
			/*if (Types.FindIndex (a => a == players [i].CombinationType) > Types.FindIndex (a => a == winner.CombinationType)) {
				winner = players [i];
			} else if (Types.FindIndex (a => a == players [i].CombinationType) == Types.FindIndex (a => a == winner.CombinationType)) {
				if (players [i].CombinationValue > winner.CombinationValue) {
					winner = players [i];
				}
			}*/
			if(players[i].CombinationRank < winner.CombinationRank) {
				winner = players [i];
			}
			else if (players[i].CombinationRank == winner.CombinationRank && players[i].CombinationValue > winner.CombinationValue) {
				winner = players [i];
			}
		}
		return winner;
	}

	// Use this for initialization. Overrides ***Start() in MonoBehavior***
	void Start () {
		//ResetPrefs();
		Debug.Log (SystemManager.dummyString);	// test static variables.
		// reset static variables
		gameBegins = false;
		roundEnd = true;
		gameEnd = false;
		displayGameResult = false;

		StartCoroutine (coStart ());

	}

	public IEnumerator coStart()
	{
				yield return new WaitForFixedUpdate ();
				// destroy all cards in the game.
				for (int i = 0; i < deckList.Count; i++) {
						deckList [i].destroyAll ();
						if (0 < deckList [i].DeckID && deckList [i].DeckID < 100) {
								deckList.Remove (deckList [i]);
						}
				}



				// set number of rounds
				roundsLeft = SystemManager.numRounds;
		
				// Generate new card deck and shuffle them.
				searchDeckByID (0).generateFullCardDeck ();
				searchDeckByID (0).closeDeck ();
				searchDeckByID (0).shuffle ();


				// setup player hands. Decks 0, 100, 101 and 102 are pre-generated inside the gameScene.
				for (int i = 1; i <= SystemManager.numPlayers; ++i) {
						registerNewPlayerHand (i, new Vector3 (-SystemManager.SPREADRANGE / 2 + (i - 0.5f) * (SystemManager.SPREADRANGE / SystemManager.numPlayers), -2, 0), new Vector3 (0, 0, 0f), 6, true);
				}
				yield return new WaitForFixedUpdate ();
				//enable AI control
				for (int i = 2; i <= SystemManager.numPlayers; ++i) {
						((PlayerHand)searchDeckByID (i)).setAIControl ();
				}
		
				StartCoroutine (startRound ());
	}

	//NOTE : It says I must add yield to the return because of some iterator problem, so I have.
	public IEnumerator startRound() 
	{
		roundEnd = false;
		if (roundsLeft <= 0) {
						StartCoroutine (endGame ());
						yield return true;
				}
		roundsLeft--;

		// hide combination value
		for (int i=0; i<playerList.Count; i++) {
			if (playerList[i].DeckID!=1)
				playerList[i].showCombination=false;
		}

		// recall all cards one by one.
		for (int i=0; i<deckList.Count; i++)
		{
			for (int j=0; j<deckList[i].CARDS.Count+5; j++)
			{
				if (deckList[i].DeckID!=0)
				{
					deckList[i].transferCardTo (searchDeckByID (0), false);
						//playerList[j].evaluateHand();
					searchDeckByID (0).closeDeck ();
					yield return new WaitForSeconds(0.05f);
				}
			}
			searchDeckByID (0).closeDeck ();
		}

		// shuffling animation.
		searchDeckByID (0).setupLayout (2);
		yield return new WaitForSeconds(0.3f);
		searchDeckByID (0).setupLayout (0);
		searchDeckByID (0).shuffle ();
		yield return new WaitForSeconds(0.3f);
		searchDeckByID (0).setupLayout (2);
		yield return new WaitForSeconds(0.3f);
		searchDeckByID (0).shuffle ();
		searchDeckByID (0).setupLayout (0);

		// destroy all cards in the game.
		/*for (int i = 0; i < deckList.Count; i++) {
			deckList [i].destroyAll ();
		}*/

		// set the number of auction cards
		auctionCardsLeft = SystemManager.numCardsAuction;

		// reset player cash.
		/*for (int i = 0; i < playerList.Count; ++i) {
			playerList [i].setCash (200);
		}*/

		StartCoroutine(coStartRound ());
	}
	/************************************* script for each round is written here...*******************************/
	public IEnumerator coStartRound()	//Must be called through StartCoroutine()
	{

		//yield return new WaitForFixedUpdate();
		//wins = PlayerPrefs.GetInt ("wins");
		//total_games = PlayerPrefs.GetInt ("total_games");

		// ==========Deal cards and start the game.================
		yield return StartCoroutine (dealCards (SystemManager.numCardsDealt));	// return startCoroutine(); is same as thread.join(); Waits until the function returns.
		for (int i=0; i<playerList.Count; i++) {
			playerList[i].showGUI=true;
		}
		yield return new WaitForSeconds(0.5f);

		gameBegins = true;

		// =============Starts auction.======================
		while (auctionCardsLeft!= 0 && !earlyAuctionEnd) {
			yield return StartCoroutine (auction ());
			auctionCardsLeft --;
		}
		if (earlyAuctionEnd) {
			earlyAuctionEnd = false;
		}


		// ============End of round.======================
		searchDeckByID (102).setupLayout (4);
		// returns winner hand
		PlayerHand winner = getWinner (playerList);
		//winner.setWinningHand ();
		winnerID = winner.DeckID;
		searchHandByID (winnerID).Winner ();
		if (winnerID == 1) {
			//playerList [0].playerWinner ();
			//PlayerPrefs.SetInt ("wins", wins + 1);
			PlayerPrefs.Save ();
		} else {
			playerList [0].playerLoser ();
		}

		// =============take winners' cards up to the display=============
		for (int i=0; i<searchHandByID (winnerID).winningHand.Count; i++)
		{
			requestCardTransfer (winnerID, 102, true, searchHandByID (winnerID).winningHand[i].Rank, searchHandByID (winnerID).winningHand[i].Suit);
		}
		searchDeckByID (102).sort ();
		for (int i=0; i<playerList.Count; i++) {
			playerList[i].showCombination=true;
		}
		//PlayerPrefs.SetInt ("total_games", total_games + 1);
		roundEnd = true;

	}

	public IEnumerator endGame()
	{
		// hide player GUI
		for (int i=0; i<playerList.Count; i++) {
			playerList[i].showGUI=false;
		}
		roundEnd = false;
		gameEnd = true;
		gameWinnerID = getGameWinnerID ();

		if (gameWinnerID == 1) {
						searchHandByID (1).playerWinner (SystemManager.numPlayers);
				}
		Transform tempParticleSystem = (Transform)Instantiate (Resources.Load <Transform>("prefab/Particle System fadeout"), new Vector3(0,0,0), transform.rotation);
		tempParticleSystem.renderer.sortingLayerName="Particles";

		yield return new WaitForSeconds (1f);
		// destroy all cards in the game.
		for (int i = 0; i < deckList.Count; i++) {
			deckList [i].destroyAll ();
		}
		yield return new WaitForSeconds (1.5f);
		searchDeckByID (102).setupLayout (0);
		searchDeckByID (102).generateFullCardDeck ();
		searchDeckByID (102).shuffle ();
		searchDeckByID (102).setupLayout (6);
		yield return new WaitForSeconds (0.5f);


		displayGameResult = true;
	}


	private IEnumerator dealCards(int numberOfCards)	//must be called through StartCoroutine(dealCards(int));
	{
		yield return new WaitForSeconds(1f);	//DO NOT ERASE THIS PART. DEALING SHOULD NOT START BEFORE HANDS ARE REPORTED TO GAMEMASTER
		Debug.Log("Card dealt to "+(deckList.Count-2)+" hands");
		for (int i=0; i<numberOfCards; i++)
		{
			for (int j=0; j<playerList.Count; j++)
			{
				if (playerList[j].DeckID>0 && playerList[j].DeckID<100)
				{
					searchDeckByID(0).transferCardTo (playerList[j], playerList[j].DeckID==1);
					//playerList[j].evaluateHand();
					yield return new WaitForSeconds(0.2f);
				}
			}
			yield return new WaitForSeconds(0.2f);
		}
		yield return new WaitForSeconds (2f);
		for (int i=0; i<playerList.Count; i++)
			playerList [i].evaluateHand ();
	}

	private IEnumerator auction()
	{
		requestCardTransfer (0,100,true);
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
		requestCardTransfer (100,101,false);
		yield return new WaitForSeconds (1f);
		for (int j=0; j<playerList.Count; j++)
		{
			playerList[j].evaluateHand ();
			//playerList[j].setWinningHand ();
		}
		//Debug.Log (deckList.Count);												

		
	}
	
	// Update is called once per frame ***Overrides Update() from MonoBehavior***
	void Update () {

	}

	void OnGUI()	//Overrided
	{
		GUIStyle style = new GUIStyle(GUI.skin.box);
		style.normal.textColor = Color.green;
		style.fontSize = Utils.adjustUISize (14,true);
		GUIStyle styleBtn = new GUIStyle(GUI.skin.button);
		styleBtn.normal.textColor = Color.green;
		styleBtn.fontSize = Utils.adjustUISize (14,true);
		GUIStyle styleInfo = new GUIStyle(GUI.skin.box);
		styleInfo.fontSize = Utils.adjustUISize (14,true);
		Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.localPosition);
		Vector3 pointBoxScreenPos = Camera.main.WorldToScreenPoint(new Vector3(-7, 4, 0));
		//GUI.Label (new Rect (screenPos.x + 150, Camera.main.pixelHeight - screenPos.y-200, 200, 20), "Number of Wins: " + PlayerPrefs.GetInt("wins"));
		//GUI.Label (new Rect (screenPos.x + 150, Camera.main.pixelHeight - screenPos.y-180, 200, 20), "Total Games: " + PlayerPrefs.GetInt("total_games"));
		//GUI.Box (new Rect (pointBoxScreenPos.x, Camera.main.pixelHeight - pointBoxScreenPos.y, Utils.adjustUISize (200,true), Utils.adjustUISize (20,false)), "Points : " + PlayerPrefs.GetInt ("Points"),style);
		if (!gameEnd) {
			GUI.Box (new Rect (pointBoxScreenPos.x, Camera.main.pixelHeight - pointBoxScreenPos.y, Utils.adjustUISize (200, true), Utils.adjustUISize (50, false)), "Rounds " + (SystemManager.numRounds - roundsLeft) + " / " + SystemManager.numRounds + "\nCards Left : " + auctionCardsLeft, styleInfo);
			if (gameBegins && roundEnd) {
				GUI.Box (new Rect (screenPos.x - Utils.adjustUISize (100, true), Camera.main.pixelHeight - screenPos.y - 160, Utils.adjustUISize (200, true), Utils.adjustUISize (40, false)), "Player " + winnerID + " wins the round!!!", style);
				if (GUI.Button (new Rect (screenPos.x - Utils.adjustUISize (100, true), Camera.main.pixelHeight - screenPos.y - 120, Utils.adjustUISize (200, true), Utils.adjustUISize (40, false)), "Next round", styleBtn)) {
					StartCoroutine (startRound ());
				}

			}
		}
		if (displayGameResult)
		{
			Vector3 resultScreenPos = Camera.main.WorldToScreenPoint(new Vector3(-0.5f, 3, 0));
			style.fontSize = Utils.adjustUISize (18,true);
			GUI.Box (new Rect (screenPos.x - Utils.adjustUISize (125, true), Camera.main.pixelHeight - resultScreenPos.y- Utils.adjustUISize (50, false), Utils.adjustUISize (250, true), Utils.adjustUISize (60, false)), (gameWinnerID==1)?("You won!!"):("You lost!!"), style);
			style.fontSize = Utils.adjustUISize (14,true);
			GUI.Box (new Rect (screenPos.x - Utils.adjustUISize (125, true), Camera.main.pixelHeight - resultScreenPos.y+ Utils.adjustUISize (20, false), Utils.adjustUISize (250, true), Utils.adjustUISize (50, false)), "WINNER : Player " + gameWinnerID + "!!", style);
			if (GUI.Button (new Rect (screenPos.x - Utils.adjustUISize (125, true), Camera.main.pixelHeight - resultScreenPos.y +Utils.adjustUISize (200, false), Utils.adjustUISize (250, true), Utils.adjustUISize (50, false)), "Back to menu", styleBtn)) {
				Application.LoadLevel ("menuScene");
			}
		}
		/*if (GUI.Button (new Rect (screenPos.x-390, Camera.main.pixelHeight - screenPos.y + 150, 80, 20), "Reset")) {
						ResetPrefs ();
				}*/
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

	//Awake is called before start. All static resources in game are loaded here. ***Overrides Awake() in MonoBehavior***
	void Awake() {
		Card.cardSpriteList = Resources.LoadAll <Sprite> ("images/cards_smooth");
		Debug.Log ("Card sprite resourses loaded once and for all");
		deckList = new List<Deck> ();
		playerList = new List<PlayerHand>();
		if (gm == null) {
			gm = GameObject.FindGameObjectWithTag ("GameMaster").GetComponent<GameMaster>();
		}
	}
	
	public static Deck searchDeckByID(int searchID)
	{
		return deckList.Find (x => x.DeckID == searchID);
	}

	// special case for searchDeckByID()
	public static PlayerHand searchHandByID(int searchID)
	{
		return (PlayerHand)deckList.Find (x=>x.DeckID==searchID);
	}


	public void ResetPrefs() {
		PlayerPrefs.SetInt ("wins", 0);
		PlayerPrefs.SetInt ("total_games", 0);
		PlayerPrefs.SetFloat ("bottomCap", 0.1f);
	}
}

