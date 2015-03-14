using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	private int playerID;	// should be unique
	private int cash;
	private int deckInControl;	// in deck ID

	public int PlayerID
	{
		get{return playerID;}
		set{playerID = value;}
	}

	public void assignDeck(int deckID)
	{
		deckInControl = deckID;
	}

	public void pullAuctionCard(int price)
	{
		if (cash>=price)
		{
			cash -= price;
			GameMaster.requestCardTransfer (100,deckInControl, false, true);	//from auction deck to player's deck
		}
		else
		{
			Debug.LogWarning ("Request denied. Not enough cash");
		}
	}

	// Use this for initialization
	void Start () {
		cash = 100;
		deckInControl = 999;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
