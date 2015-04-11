using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public class PlayerHand : Deck {

	public float cash;
	private bool AIControlled;
	// Bidvalue. AI reserves the certain bid value, and player retrieves the bid value by pressing auctionTimer button.
	private int BidValue;
	private int Points = PlayerPrefs.GetInt ("Points");
	private float Multiplier = (PlayerPrefs.HasKey ("Float")?PlayerPrefs.GetFloat ("Multiplier"):1);
	public List<float> WinningBidPercentage = new List<float>();
	// below are non-ingame temporary variables. Feel free to force change the variables anywhere.
	public bool showGUI = false;
	public bool showCombination = true;

	public float Cash
	{
		get{ return cash;}
		//set{playerID = value;}
	}
	public void setCash(float value) {
				cash = value;
		}
	public void setAIControl()
	{
		AIControlled = true;
		Points = 0;
		showCombination = false;
	}
	public void Winner() 
	{
		Points += (5 + (int)(10 * Multiplier));
		PlayerPrefs.SetInt ("Points", Points);
		Multiplier += 0.1f;
		if (Multiplier > 2) {
			Multiplier = 2;
		}
		PlayerPrefs.SetFloat ("Multiplier", Multiplier);
	}
	public void Loser() 
	{
		PlayerPrefs.SetInt ("Multiplier", 1);
	}
	public bool isAIControlled()
	{
		return AIControlled;
	}
	public void buyPrize(int prizeVal) 
	{
				if (Points >= prizeVal) {
						Points -= prizeVal;
						PlayerPrefs.SetInt ("Points", Points);
						//10 prize is getting a free new card
				} 
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
				/*Strategy 1 : If the card will allow me to increase the ranking of my hand, pay 70% of current cash. 
		If it will increase the value of my hand but keep the ranking same  ie go from pair 4's to pair 9's
		 then pay 50% of current cash. Otherwise, only bid 10% of current cash.*/
				System.Random rndm = new System.Random ();
				this.evaluateDeck ();
				int current_rank = CombinationRank;
				int current_score = CombinationValue;
				bool bluff = (rndm.Next (1, 10) <= 5 ? true : false);

				CARDS.Add (auctionCard);
				this.evaluateDeck ();
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
			// if auction fails, AI keeps attempting to bid.
			if (AIControlled)
				BidValue=price;
			else
				BidValue=0;
			return false;
		}
	}

	// Use this for initialization
	void Start () {
		base.Start ();	// access base class
		cash = 200;
		AIControlled = false;
		BidValue = 0;
	}
	
	// Update is called once per frame
	void Update () {
		cash += Time.deltaTime;
	}

	void OnGUI()	//Overrided
	{
		GUIStyle buttonStyle = new GUIStyle (GUI.skin.button);
		buttonStyle.normal.textColor = Color.black;
		buttonStyle.hover.textColor = Color.green;
		Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.localPosition);
		if (Points >= 10) {
						if (GUI.Button (new Rect (screenPos.x-450, Camera.main.pixelHeight - screenPos.y - 330, 70,70), "50 Cash",buttonStyle)) {
								buyPrize (10);
								cash += 50;
						}
				}
		if(Points >= 20) {
			if (GUI.Button (new Rect (screenPos.x-450, Camera.main.pixelHeight - screenPos.y - 250, 70,70), "Stop"+"\n"+"Auction",buttonStyle)) {
				buyPrize (20);
				GameMaster.endAuctionEarly();
			}
		}
		if (Points >= 40) {
						if (GUI.Button (new Rect (screenPos.x - 450, Camera.main.pixelHeight - screenPos.y-170, 70, 70), "Extra" +"\n"+"Card",buttonStyle)){
										buyPrize(40);
										GameMaster.requestCardTransfer (0, 1, false, true);
						}
				}
		if (showGUI){
			GUI.Box (new Rect (screenPos.x, Camera.main.pixelHeight-screenPos.y- 120, 100, (showCombination?60:45)), "Cash = " + (int)cash + "\n" + (AIControlled?"AI":"Player") + " ID = "+DeckID + "\n" +(AIControlled&&!showCombination?"":CombinationType));
		}
		//GUI.Label(new Rect(10,10,200,20),"Here is a block of text\nlalalala\nanother line\nI could do this all day!");
		//Use this function to draw GUI stuff. Google might help. This fucntion is bound to GameMaster object.
		//GUI.Label (new Rect (520,427,100,25),(searchDeckByID (1)).CombinationType);
	}
}
