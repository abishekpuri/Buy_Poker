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
	public static bool settingPage=true;

	public static string dummyString = "nothing yet";
	public static int numPlayers = 3;
	public static int numCardsDealt = 2;
	public static int numCardsAuction = 7;
	public static int numRounds = 3;
	public static int startCash = 100;	//
	public static int cashIncome = 60;	// per munite

	public int tempNumPlayers;
	public int tempNumCards;
	public int tempNumAuction;
	public int tempNumRounds;
	public int tempStartCash;
	public int tempCashIncome;

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

	public void changeNumRounds(string val)
	{
		if (int.Parse (val) < 31 && int.Parse (val) > 0)
			tempNumRounds = int.Parse (val);
	}

	public void changeStartCash(string val)
	{
		if (int.Parse (val) > 0)
			tempStartCash = int.Parse (val);
	}

	public void changeCashIncome(string val)
	{
		if (int.Parse (val) > 0)
			tempCashIncome = int.Parse (val);
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
	public void confirmNumRounds()
	{
		if (tempNumRounds!=0)
			numRounds = tempNumRounds;
	}

	public void confirmStartCash()
	{
		startCash = tempStartCash;

	}

	public void confirmCashIncome()
	{
		cashIncome = tempCashIncome;

	}

	// Use this for initialization
	void Start () {
		tempNumPlayers = numPlayers;
		tempNumCards = numCardsDealt;
		tempNumAuction = numCardsAuction;
		tempStartCash = startCash;
		tempCashIncome = cashIncome;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI()
	{
		if (settingPage)
		{
			GUIStyle boxStyle = new GUIStyle (GUI.skin.box);
			boxStyle.normal.textColor = Color.white;
			boxStyle.fontSize = Utils.adjustUISize (18,true);
			// Vector3 screenPosition => You can set Position of GUI in world space and then convert it into screenPos(GUI pos)
			Vector3 screenPos = Camera.main.WorldToScreenPoint(new Vector3(5, 3.2f, 0));

			GUI.Box (new Rect (screenPos.x-Utils.adjustUISize (50, true), Camera.main.pixelHeight-screenPos.y, Utils.adjustUISize (100,true), Utils.adjustUISize (400,false)), numPlayers+"\n\n\n"+numCardsDealt+"\n\n\n"+numCardsAuction+"\n\n\n"+numRounds+"\n\n\n"+startCash+"\n\n\n"+cashIncome,boxStyle);
		}
	}
}
