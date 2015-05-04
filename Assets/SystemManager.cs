using UnityEngine;
using System.Collections;

/*
 * SYSTEM MANAGER.
 * 
 * System manager stores all your settings and stuff, that persists over change in scenes.
 * System manager is supposed to play a higher role than gameMaster
 * 
 * Please declare everything public and static.
 * Right now, it is in a testing phase.
 * 
 * */


public class SystemManager : MonoBehaviour {

	public static float SPREADRANGE=13f;

	public static string dummyString = "nothing yet";
	public static int numPlayers = 3;
	public static int numCardsDealt = 3;
	public static int numCardsAuction = 1;
	public static int numRounds = 1;
	//public static int numRounds = 5;

	public int tempNumPlayers;
	public int tempNumCards;
	public int tempNumAuction;

	public void changeString()
	{
		dummyString = "TEST SUCCESSFUL";
	}


	public void changeNumPlayersTemp(string val)
	{
		if (int.Parse (val)<6 && int.Parse (val)>0)
			tempNumPlayers=int.Parse (val);
	}

	public void changeNumCardsTemp(string val)
	{
		if (int.Parse (val)<7 && int.Parse (val)>=0)
			tempNumCards=int.Parse (val);
	}

	public void changeNumAuctionTemp(string val)
	{
		if (int.Parse (val)<21 && int.Parse (val)>0)
			tempNumAuction=int.Parse (val);
	}

	public void confirmNumPlayers()
	{
		if (tempNumPlayers!=0)
			numPlayers = tempNumPlayers;
	}
	public void confirmNumCards()
	{
		if (tempNumCards!=0)
			numCardsDealt = tempNumCards;
	}
	public void confirmNumAuction()
	{
		if (tempNumAuction!=0)
			numCardsAuction = tempNumAuction;
	}


	// Use this for initialization
	void Start () {
		int tempNumPlayers = numPlayers;
		int tempNumCards = numCardsDealt;
		int tempNumAuction = numCardsAuction;
		Debug.Log ("setting scene");
	}
	
	// Update is called once per frame
	void Update () {
	
	}


}
