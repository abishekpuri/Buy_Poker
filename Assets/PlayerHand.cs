using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class PlayerHand : Deck {

	private float cash;
	private bool AIControlled;
	// Bidvalue. AI reserves the certain bid value, and player retrieves the bid value by pressing auctionTimer button.
	private int BidValue;

	// below are non-ingame temporary variables. Feel free to force change the variables anywhere.
	public bool showGUI = false;
	public bool showCombination = true;

	public float Cash
	{
		get{return Cash;}
		//set{playerID = value;}
	}
	public void setCash(float value) {
				cash = value;
		}
	public void setAIControl()
	{
		AIControlled = true;
		showCombination = false;
	}
	public bool isAIControlled()
	{
		return AIControlled;
	}

	public void setBidValue(int val)
	{
		BidValue = val;
	}
	
	public int getBidValue()
	{
		return BidValue;
	}



	public void CalculateAIBid(Card auctionCard)
	{
		this.evaluateDeck ();
		int current_rank = CombinationRank;
		int current_score = CombinationValue;
		CARDS.Add (auctionCard);
		this.evaluateDeck ();
		if (current_rank > CombinationRank) {
						BidValue = (int)(0.7 * cash);
				} else if ((current_rank == CombinationRank) && (current_score > CombinationValue)) {
						BidValue = (int)(0.5 * cash);
				}
		else {
						BidValue = (int)(0.1 * cash);
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
		cash = 100;
		AIControlled = false;
		BidValue = 0;
	}
	
	// Update is called once per frame
	void Update () {
		cash += Time.deltaTime;
	}

	void OnGUI()	//Overrided
	{
		if (showGUI){
			Vector3 pos = transform.localPosition;//(Vector2)Camera.WorldToScreenPoint(pos)
			Vector3 screenPos = Camera.main.WorldToScreenPoint(pos);
			GUI.Box (new Rect (screenPos.x, Camera.main.pixelHeight-screenPos.y- 120, 100, (showCombination?60:45)), "Cash = " + (int)cash + "\n" + (AIControlled?"AI":"Player") + " ID = "+DeckID + "\n" +(AIControlled&&!showCombination?"":CombinationType));
		}
		//GUI.Label(new Rect(10,10,200,20),"Here is a block of text\nlalalala\nanother line\nI could do this all day!");
		//Use this function to draw GUI stuff. Google might help. This fucntion is bound to GameMaster object.
		//GUI.Label (new Rect (520,427,100,25),(searchDeckByID (1)).CombinationType);
	}
}
