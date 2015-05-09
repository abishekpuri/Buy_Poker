﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/**
 * 
 * PlayerHand class, extends Deck.
 * Contains player specific members and behaviors.
 * 
 * Also contains Hand evaluation and AI scripts.
 * 
 * 
 * */
public class PlayerHand : Deck {

	// Player specific member fields.
	public float cash;
	private bool AIControlled;
	private int roundPoints;
	// Bidvalue. AI reserves the certain bid value at start of auction, and player retrieves the bid value by pressing auctionTimer button.
	private int BidValue;

	//Permanent Upgrades Checker
	// list for Hand value for hand evaluation.
	public List<Card> winningHand = new List<Card> ();
	public string CombinationType = "Evaluating Hand .."; //consider making private later.
	public int CombinationValue; // used to break ties if CombinationType same
	public int FlushValue;
	public int SecondaryCombinationValue; // used if the top value is the same ie if three of a kind same then compare the pair
	public int CombinationRank; // Number that tracks hand value, better than enumerating hand types

	
	// AI related variables.
	private int Multiplier = 1;
	public List<List<Card> > OpponentCards;
	public List<float> WinningBidPercentage = new List<float>();

	// below are non-ingame temporary variables. Feel free to change the variables anywhere.
	public bool showGUI = false;
	public bool showCombination = true;


	public float Cash
	{
		get{ return cash;}
		//set{playerID = value;}
	}
	public int RoundPoints
	{
		get{ return roundPoints;}
	}
	~PlayerHand() {

	}
	public virtual void purchaseUpgrade(int Upgrade)
	{
				if (PlayerPrefs.GetInt ("Points") >= (Upgrade * 3) && PlayerPrefs.GetInt ("Upgrade"+Upgrade)==0) {
						PlayerPrefs.SetInt ("Upgrade" + Upgrade, 1);
						int Points = PlayerPrefs.GetInt ("Points");
						PlayerPrefs.SetInt ("Points", Points - Upgrade * 3);
						PlayerPrefs.Save ();
				}
	}
	public virtual void destroyAll()
	{
		base.destroyAll ();
		while (winningHand.Count>0) {
			Destroy (winningHand [0].gameObject);
			winningHand.Remove (winningHand [0]);
		}
	}

	public void setCash(float value) {
				cash = value;
		}
	public void setAIControl()
	{
		AIControlled = true;
		showCombination = false;
	}
	public void playerWinner(int numPlayers)
	{
		//Debug.Log (deckID + ": " + Points);
		Multiplier = (numPlayers - 1) * 2;
		int Points = PlayerPrefs.GetInt ("Points");
		Points += Multiplier;
		PlayerPrefs.SetInt ("Points", Points);
		PlayerPrefs.Save ();
		Debug.Log (deckID + ": " + PlayerPrefs.GetInt ("Points"));
	}
	public void playerLoser() 
	{
		int Points = PlayerPrefs.GetInt ("Points");
		Points += 1;
		PlayerPrefs.SetInt ("Points", Points);
		PlayerPrefs.Save ();
		Debug.Log (deckID + ": " + PlayerPrefs.GetInt ("Points"));
	}
	public void Winner() 
	{
		roundPoints += (10 - CombinationRank)*2+3;
	}

	public bool isAIControlled()
	{
		return AIControlled;
	}
	public void buyPrize(int prizeVal) 
	{
				if (roundPoints >= prizeVal) {
						roundPoints -= prizeVal;
						//PlayerPrefs.SetInt ("Points", roundPoints);
						//10 prize is getting a free new card
				} 
		}
	public void evaluateHand()
	{
		int counter = 0;
		bool straight = false;
		List<int> value = new List<int> ();
		List<int> suit = new List<int> ();
		for (int i = 0; i < 15; i++) {
			if (i < 5) {
				suit.Add (0);
			}
			value.Add (0);
		}
		for (int i = 0; i < cards.Count; i++) {
			if (cards [i].Rank == 1) {
				value [14] ++;
			} else {
				value [cards [i].Rank]++;
			}
			suit [cards [i].Suit]++;
		}
		//Check for A to 5 straight
		if (value [14] > 0) {
			for (int i = 2; i < 6; ++i) {
				if (value [i] > 0) {
					counter++;
				}
				if (counter == 4) {
					CombinationValue = 1;
					straight = true;
				}
			}
		}
		counter = 0;
		for (int i = 0; i < value.Count; i++) {
			if (value [i] > 0) {
				CombinationValue = (counter == 0 ? i : CombinationValue);
				counter++;
				if (deckID == 1) {
					Debug.Log (counter);
				}
			} else {
				counter = 0;
			}
			if (counter == 5) {
				straight = true;
				break;
			}
		}
		int pairs = value.FindAll (a => a == 2).Count;
		int three_kind = value.FindAll (a => a == 3).Count;
		int four_kind = value.FindAll (a => a == 4).Count;
		int flush = suit.FindAll (a => a >= 5).Count;
		if ((flush != 0) && straight) {
			CombinationRank = 1;
			//WARNING : THIS WILL NOT WORK IF PLAYER HAS TWO FLUSHES
			FlushValue = suit.FindLastIndex(a=> a>=5);
			CombinationType = "Straight Flush";
		} else if (four_kind >= 1) {
			CombinationRank = 2;
			CombinationValue = value.FindLastIndex (a => a == 4); 
			CombinationType = "Four of a Kind";
		} else if (three_kind > 1 || (three_kind == 1 && pairs >= 1)) {
			CombinationRank = 3;
			CombinationValue = value.FindLastIndex (a => a == 3);
			SecondaryCombinationValue = value.FindLastIndex (a => a == 2);
			CombinationType = "Full House";
		} else if (flush >= 1) {
			int maxCard = 0;
			for (int i = 0; i < suit.Count; ++i) {
				if (suit [i] >= 5) {
					if (cards.FindLastIndex (a => a.Suit == i) >= maxCard) {
						maxCard = cards.FindLastIndex (a => a.Suit == i);
						FlushValue = i;
					}
				}
			}
			CombinationRank = 4;
			CombinationValue = maxCard;
			CombinationType = "Flush";
		} else if (straight) {
			CombinationRank = 5;
			CombinationType = "Straight";
		} else if (three_kind >= 1) {
			CombinationRank = 6;
			CombinationValue = value.FindLastIndex (a => a == 3);
			CombinationType = "Three of a Kind";
		} else if (pairs >= 2) {
			CombinationRank = 7;
			CombinationValue = value.FindLastIndex (a => a == 2);
			//This is to find the second value, so we temporarily make the first one 0 
			value [CombinationValue] = 0;
			SecondaryCombinationValue = value.FindLastIndex (a => a == 2);
			value [CombinationValue] = 2;
			CombinationType = "Two Pair";
		} else if (pairs == 1) {
			CombinationRank = 8;
			CombinationValue = value.FindLastIndex (a => a == 2);
			CombinationType = "One Pair";
		} else {
			CombinationRank = 9;
			CombinationValue = value.FindLastIndex (a => a == 1);
			CombinationType = "High Card";
		}
		setWinningHand ();
	}
	//Warning : setWinningHand not working for flush/straight, for flush it is showing all cards
	private void setWinningHand() 
	{
		string[] single = {"High Card","One Pair","Three of a Kind","Four of a Kind"};
		string[] doublee = {"Two Pair","Full House"};
		bool Ace = false;
		List<String> singleRank = new List<String> ();
		List<String> doubleRank = new List<String> ();
		doubleRank.AddRange (doublee);
		singleRank.AddRange (single);
		if (CombinationValue == 14) {
				Ace = true;
				CombinationValue = 1;
		}
		if (singleRank.FindLastIndex (a => a == CombinationType) != -1) {
				winningHand = cards.FindAll (a => a.Rank == CombinationValue);
		} else if (doubleRank.FindLastIndex (a => a == CombinationType) != -1) {
				winningHand = cards.FindAll (a => a.Rank == CombinationValue);
				List<Card> secondPart = cards.FindAll (a => a.Rank == SecondaryCombinationValue);
				winningHand.AddRange (secondPart);
		} else if (CombinationType == "Flush") {
				Debug.Log ("FlushValue = "+FlushValue);
				List<Card> flushCards = cards.FindAll (a => a.Suit == FlushValue);
				Debug.Log ("FlushCardsCount = "+flushCards.Count);
				int count = 4;
				while (count >= 0) {
				winningHand.Add (flushCards[0]);
				flushCards.Remove (flushCards[0]);
				count--;
				}
		} else {
				for (int i = CombinationValue; i < CombinationValue+5; ++i) {
						if (CombinationType == "Straight") {
								winningHand.Add (cards.Find (a => a.Rank == i));
						} else {
								winningHand.Add (cards.Find (a => (a.Rank == i && a.Suit == FlushValue)));
						}
				}
		}
		//cards = winningHand;
		if (Ace) {
				CombinationValue = 14;
		}
		if (DeckID==GameMaster.UserID && PlayerPrefs.GetInt ("Upgrade1")==1) {		// THIS IF STATEMENT IS ONLY A TEMPORARY SOLUTION
				for (int i=0; i<cards.Count; ++i) {
						cards [i].stopBlinkAnim ();
				}
				for (int i=0; i<winningHand.Count; ++i) {
						winningHand [i].startBlinkAnim ();
				}
		}
		//Debug.Log ("Winning hand count = " + winningHand.Count);
	}

	public void setBidValue(int val)
	{
		BidValue = val;
	}
	
	public int getBidValue()
	{
		return BidValue;
	}

	//Debugging Function to check if winning bids are being entered
	public void showWinningBids() {
				for (int i = 0; i < WinningBidPercentage.Count; ++i) {
						Debug.Log (WinningBidPercentage [i]);
				}
		}

	public void CalculateAIBid(Card auctionCard)
	{
		/*To prevent players from catching on to AI's lower point for useless cards, we will track all bets made below 50% of 
		 * total cash, and remember the percentge, adjusting ours accordingly*/

		float avgBetPercentage = 0;
		float bottomCap = PlayerPrefs.GetFloat("bottomCap");
		for (int i = 0; i < WinningBidPercentage.Count; ++i) {
						if (WinningBidPercentage [i] < 0.5) {
								avgBetPercentage += WinningBidPercentage [i];
						}
				}
		avgBetPercentage /= WinningBidPercentage.FindAll(a => a <0.5).Count;
		if (WinningBidPercentage.FindAll (a => a < 0.5).Count > 3) {
						if (avgBetPercentage > bottomCap) {
								bottomCap += 0.05f;
								PlayerPrefs.SetFloat ("bottomCap", bottomCap);
						}
						if (avgBetPercentage < bottomCap) {
								bottomCap -= 0.05f;
								PlayerPrefs.SetFloat ("bottomCap", bottomCap);
						}
				}
		/*Strategy 1 : Calculate the Value of the current card using a weighted vector system that increases semi exponentially to
		 * factor in the difficulty in moving up a level in the later stages. We then find the power value using this vector system
		 * and normalize it using total power. This gives us a raw value of the card we can then adjust according to aggresion,
		 * bluffing and so on.*/
		this.evaluateHand ();

				/*Strategy 2 : If the card will allow me to increase the ranking of my hand, pay 70% of current cash. 
		If it will increase the value of my hand but keep the ranking same  ie go from pair 4's to pair 9's
		 then pay 50% of current cash. Otherwise, only bid 10% of current cash.*/
				System.Random rndm = new System.Random ();
				int current_rank = CombinationRank;
				int current_score = CombinationValue;
				bool bluff = (rndm.Next (1, 10) <= 5 ? true : false);

				CARDS.Add (auctionCard);
				this.evaluateHand ();
				if (current_rank > CombinationRank) {
			float rankVal = (float)(current_rank - CombinationRank);
			rankVal /= 10;
			rankVal += 0.50f;
			BidValue = (int)(cash *  rankVal);
		} else if ((current_rank == CombinationRank) && (current_score > CombinationValue)) {
						BidValue = (int)(0.3 * cash);
		} else {
						//Create a bluff, this card cannot help at all, but 10% of the time, the program will act like its an important card.
						if (bluff) {
								BidValue = (int)((0.4+rndm.Next(1,3)/10) * cash);
						} else {
								BidValue = (int)(bottomCap * cash);
						}
		}
		if (BidValue >= 90) {
						BidValue = 90;
				}
		CARDS.Remove (auctionCard);
	}

	public void takeAuctionCard(int price)
	{
		cash -= price;
		BidValue = 0;
		if (!(!Network.isClient && (!Network.isServer || Network.connections.Length<1)))
		{
			networkManager.networkObject.broadcastBidValue (new Vector2(DeckID, (int)0));
		}
		Debug.Log ("Player "+DeckID+ "'s current cash = "+cash + "!!");
	}

	public bool bidForAuction(int price)
	{
		if (cash>=price)
		{
			//cash -= price;
			return true;
		}
		else
		{
			// if auction fails, AI continues attempting to bid for lower price.
			if (AIControlled)
				BidValue=price;
			else
			{
				BidValue=0;
				if (!(!Network.isClient && (!Network.isServer || Network.connections.Length<1)))
				{
					networkManager.networkObject.broadcastBidValue (new Vector2(DeckID, (int)0));
				}
			}
			return false;
		}
	}

	// Use this for initialization
	void Start () {
		base.Start ();
	}

	void Awake(){
		base.Awake ();	// access base class
		cash = SystemManager.startCash;
		AIControlled = false;
		showCombination = GameMaster.UserID == DeckID;
		BidValue = 0;
		roundPoints = 0;
	}

	// Update is called once per frame
	void Update () {
		if (GameMaster.gameBegins && !GameMaster.roundEnd)
			cash += ((Time.deltaTime)*SystemManager.cashIncome)/60f;
	}

	void OnGUI()	//Overrided
	{

		GUIStyle buttonStyle = new GUIStyle (GUI.skin.button);
		buttonStyle.normal.textColor = Color.cyan;
		buttonStyle.hover.textColor = Color.cyan;
		buttonStyle.fontSize = Utils.adjustUISize (14,true);
		int buttonStyleAdjustedUISizeX = Utils.adjustUISize (70,true);
		int buttonStyleAdjustedUISizeY = Utils.adjustUISize (70,false);

		GUIStyle boxStyle = new GUIStyle (GUI.skin.box);
		boxStyle.normal.textColor = Color.white;
		boxStyle.fontSize = Utils.adjustUISize (14,true);
		// Vector3 screenPosition => You can set Position of GUI in world space and then convert it into screenPos(GUI pos)
		Vector3 screenPos = Camera.main.WorldToScreenPoint(new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z));
		Vector3 StatBoxscreenPos = Camera.main.WorldToScreenPoint (new Vector3 (transform.localPosition.x, transform.localPosition.y-1, transform.localPosition.z));
		Vector3 awardButtonScreenPos;
		if (DeckID == GameMaster.UserID && showGUI) {
			if (roundPoints >= 5) {
				awardButtonScreenPos = Camera.main.WorldToScreenPoint (new Vector3 (6, 3, transform.localPosition.z));
				if (GUI.Button (new Rect (awardButtonScreenPos.x, Camera.main.pixelHeight - awardButtonScreenPos.y, buttonStyleAdjustedUISizeX, buttonStyleAdjustedUISizeY), "50 Cash\n(10)", buttonStyle)) {
					buyPrize (10);
					cash += 50;
				}
			}
			if (roundPoints >= 10) {
				awardButtonScreenPos = Camera.main.WorldToScreenPoint (new Vector3 (6, 1.5f, transform.localPosition.z));
				if (GUI.Button (new Rect (awardButtonScreenPos.x, Camera.main.pixelHeight - awardButtonScreenPos.y, buttonStyleAdjustedUISizeX, buttonStyleAdjustedUISizeY), "Stop" + "\n" + "Auction\n(10)", buttonStyle)) {
					buyPrize (15);
					GameMaster.endAuctionEarly ();
				}
			}
			if (roundPoints >= 15) {
				awardButtonScreenPos = Camera.main.WorldToScreenPoint (new Vector3 (6, 0, transform.localPosition.z));
				if (GUI.Button (new Rect (awardButtonScreenPos.x, Camera.main.pixelHeight - awardButtonScreenPos.y, buttonStyleAdjustedUISizeX, buttonStyleAdjustedUISizeY), "Extra" + "\n" + "Card\n(15)", buttonStyle)) {
					buyPrize (15);
					GameMaster.requestCardTransfer (0, 1, true);
				}
			}
		}
		if (showGUI){
			GUI.Box (new Rect (StatBoxscreenPos.x-Utils.adjustUISize (50, true), Camera.main.pixelHeight-StatBoxscreenPos.y, Utils.adjustUISize (100,true), ((GameMaster.UserID==DeckID||showCombination)?Utils.adjustUISize (80,false):Utils.adjustUISize (60,false))),
			         "Cash = $" + (int)cash + "\n" + (AIControlled?"AI":"Player") + " ID = "+DeckID + "\n"
			         +(((GameMaster.UserID==DeckID || showCombination) && PlayerPrefs.GetInt ("Upgrade1")==1)?(CombinationType+"\n"):"")+"Points = "+roundPoints, boxStyle);
		}
		//GUI.Label(new Rect(10,10,200,20),"Here is a block of text\nlalalala\nanother line\nI could do this all day!");
		//Use this function to draw GUI stuff. Google might help. This fucntion is bound to GameMaster object.
		//GUI.Label (new Rect (520,427,100,25),(searchDeckByID (1)).CombinationType);
	}
}
