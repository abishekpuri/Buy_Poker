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
	public static int numPlayers = 4;
	public static int numCardsDealt = 3;
	public static int numCardsAuction = 10;
	//public static int numRounds = 5;


	public void changeString()
	{
		dummyString = "TEST SUCCESSFUL";
	}

	/*
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	*/
}
