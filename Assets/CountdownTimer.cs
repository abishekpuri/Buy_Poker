using UnityEngine;
using System.Collections;

/*
 * 
 * This Class contains hardCoded part:
 * 1. Button pressing action does not distinguish the player
 * 2. Button pressing action cannot be done automatically
 * 
 * 
 * */
public class CountdownTimer : MonoBehaviour {

	public float timeRemaining;
	public float speedMultiplier;
	public bool auctionInProcess;
	public bool buttonClicked;
	private double timerStopTime;
	const double BUTTON_DELAY = 1;

	// Use this for initialization
	void Start () {
		timeRemaining = 100;
		speedMultiplier = 8;
		auctionInProcess = true;
		buttonClicked = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (timeRemaining > 1 && auctionInProcess) {
			timeRemaining -= Time.deltaTime * speedMultiplier;

		}
		else if (buttonClicked && timerStopTime+BUTTON_DELAY < Time.time)
		{
			Debug.Log ("Deliver!!!");
			GameMaster.requestCardTransfer (100,1, true);	// THIS PART IS HARD CODED
			buttonClicked=false;
			Destroy (this,0.2f);
			GameMaster.terminateCurrentAuction();
		}
		else if (timeRemaining <= 1)
		{
			Destroy (this,0.2f);
			GameMaster.terminateCurrentAuction();
		}
	}

	void OnGUI()
	{
		Vector3 pos = transform.localPosition;//(Vector2)Camera.WorldToScreenPoint(pos)
		Vector3 screenPos = Camera.main.WorldToScreenPoint(pos);
		Vector2 convertedGUIPos = GUIUtility.ScreenToGUIPoint(screenPos);
		if (timeRemaining>1)
		{
			if (GUI.Button (new Rect(convertedGUIPos.x, convertedGUIPos.y-100, 70, 35), "Bid for "+(int)timeRemaining) && !buttonClicked)
			{
				Debug.Log ("Pressed!!!");
				buttonClicked = true;
				auctionInProcess=false;
				timerStopTime = Time.time;
			}
		}
		else{
			GUI.Button (new Rect(convertedGUIPos.x, convertedGUIPos.y-100, 70, 35), "Bid over");
		}
	}
}
