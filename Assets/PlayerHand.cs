using UnityEngine;
using System.Collections;

public class PlayerHand : Deck {

	private float cash;
	private bool AIControlled;
	private int AIBidValue;

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
		AIBidValue = auctionCard.Rank*5;
	}

	public int getAIBidValue()
	{
		return AIBidValue;
	}

	public void pullAuctionCard(int price)
	{
		if (cash>=price)
		{
			cash -= price;
			GameMaster.requestCardTransfer (100,DeckID, false, true);	//from auction deck to player's deck
		}
		else
		{
			Debug.LogWarning ("Request denied. Not enough cash");
		}
	}

	// Use this for initialization
	void Start () {
		base.Start ();	// access base class
		cash = 100;
		AIControlled = false;
		AIBidValue = 0;
	}
	
	// Update is called once per frame
	void Update () {
		cash += Time.deltaTime;
	}
}
