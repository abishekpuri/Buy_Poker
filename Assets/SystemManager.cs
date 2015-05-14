using UnityEngine;
using System.Collections;
using System.Collections.Generic;


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
	public bool settingPage;
	public bool storePage;

	public static string dummyString = "nothing yet";
	public static int numPlayers = 3;
	public static int numCardsDealt = 3;
	public static int numCardsAuction = 2;
	public static int numRounds = 3;
	public static int startCash = 200;	//
	public static int cashIncome = 60;	// per minute
	public static bool isCustom = false;
	public int tempNumPlayers;
	public int tempNumCards;
	public int tempNumAuction;
	public int tempNumRounds;
	public int tempStartCash;
	public int tempCashIncome;

	public static void reset() 
	{
		numPlayers = 3;
		numCardsDealt = 3;
		numCardsAuction = 2;
		numRounds = 3;
		startCash = 200;
		cashIncome = 60;
	}

	public void setForMultiplayer()
	{
		numPlayers = 2;
		numCardsDealt = 2;
		numCardsAuction = 5;
		numRounds = 5;
		startCash = 100;
		cashIncome = 60;
	}

	public void forDemo() 
	{
				PlayerPrefs.SetInt ("Points", 10);
		for (int i = 0; i < 5; ++i) {
			PlayerPrefs.SetInt ("Upgrade" + i, 0);
		}
		PlayerPrefs.Save ();
		}
	public void resetButton() 
	{
				PlayerPrefs.SetInt ("wins", 0);
				PlayerPrefs.SetInt ("Points", 0);
				for (int i = 0; i < 5; ++i) {
						PlayerPrefs.SetInt ("Upgrade" + i, 0);
				}
				PlayerPrefs.Save ();
				numPlayers = 3;
				numCardsDealt = 3;
				numCardsAuction = 2;
				numRounds = 3;
				startCash = 200;
				cashIncome = 60;
		}
	public void changeString()
	{
		dummyString = "TEST SUCCESSFUL";
	}

	public void CustomGame(bool state)
	{
		isCustom = state;
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

	public void confirmAll()
	{
		if (tempNumPlayers!=0)
			numPlayers = tempNumPlayers;
		if (tempNumCards!=0)
			numCardsDealt = tempNumCards;
		if (tempNumAuction!=0)
			numCardsAuction = tempNumAuction;
		if (tempNumRounds!=0)
			numRounds = tempNumRounds;
		startCash = tempStartCash;
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

		GUIStyle boxStyle = new GUIStyle (GUI.skin.box);
		boxStyle.normal.textColor = Color.white;
		//boxStyle.normal.background = Resources.Load <Texture2D> ("images/woodTexture");
		boxStyle.fontSize = Utils.adjustUISize (18,true);
		// Vector3 screenPosition => You can set Position of GUI in world space and then convert it into screenPos(GUI pos)
		Vector3 screenPos = Camera.main.WorldToScreenPoint(new Vector3(5, 3.2f, 0));

		if (settingPage)
		{
			GUI.Box (new Rect (screenPos.x-Utils.adjustUISize (50, true), Camera.main.pixelHeight-screenPos.y, Utils.adjustUISize (100,true), Utils.adjustUISize (400,false)), numPlayers+"\n\n\n"+numCardsDealt+"\n\n\n"+numCardsAuction+"\n\n\n"+numRounds+"\n\n\n"+startCash+"\n\n\n"+cashIncome,boxStyle);
		}
		if (storePage) 
		{
			GUI.Box (new Rect (screenPos.x-Utils.adjustUISize (500, true), Camera.main.pixelHeight-screenPos.y, Utils.adjustUISize (400,true), Utils.adjustUISize (50,false)),"Number of Points : "+ PlayerPrefs.GetInt ("Points"),boxStyle);
		}
	}
}
