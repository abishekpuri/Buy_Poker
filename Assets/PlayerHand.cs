using UnityEngine;
using System.Collections;

public class PlayerHand : Deck {

	private float cash;
	private bool AIControlled;
	// Bidvalue. AI reserves the certain bid value, and player retrieves the bid value by pressing auctionTimer button.
	private int BidValue;

	public float Cash
	{
		get{return Cash;}
		//set{playerID = value;}
	}
	public void setAIControl()
	{
		AIControlled = true;
	}
	public bool isAIControlled()
	{
		return AIControlled;
	}

	public void CalculateAIBid(Card auctionCard)
	{
		BidValue = auctionCard.Rank*5;
	}

	public int getBidValue()
	{
		return BidValue;
	}
	public void setBidValue(int val)
	{
		BidValue = val;
	}

	public bool pullAuctionCard(int price)
	{
		if (cash>=price)
		{
			cash -= price;
			Debug.LogWarning ("Player " + DeckID + " wins auction!");
			//GameMaster.requestCardTransfer (100,DeckID, false, true);	//from auction deck to player's deck
			return true;
		}
		else
		{
			BidValue=0;
			Debug.LogWarning ("Request denied. Not enough cash");
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
}
