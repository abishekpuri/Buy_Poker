using UnityEngine;
using System.Collections;

/*
 * AuctionTimer has two parts. As soon as this object is instantiated, it starts counting.
 * Also, Button UI is also bound to this object.
 * 
 * This Class contains hardCoded part, and needs to be solved:
 * 1. Button pressing action does not distinguish the player id
 * 2. Button pressing action cannot be done automatically, by AI
 * 
 * 
 * If the counter is interrupted by player or if it counts to 10, it destroys itself.
 * 
 * */
public class AuctionTimer : MonoBehaviour {

	private float timeRemaining;	// time remaining
	private float speedMultiplier;	// countdown speed.
	private bool auctionInProcess;
	private bool buttonClicked;		
	private int transferID;

	private Transform particleEffectPrefab;
	private Texture2D buttonTexture;
	private double timerStopTime;	//when buttonClicked=true, timer stops to 
	const double BUTTON_DELAY = 1;		// delay before destroying itself.

	// Use this for initialization. Default time and speed settings
	void Start () {
		timeRemaining = 100;
		speedMultiplier = 10;	// 10 per second.
		auctionInProcess = true;
		buttonClicked = false;
		particleEffectPrefab = Resources.Load <Transform>("prefab/Particle System");
		buttonTexture = Resources.Load <Texture2D>("images/btnTexture");
	}
	
	// Update is Implicitly called once per frame
	void Update () {
			// If auction is successful, transfer the card to respective playerHand.
			if (!buttonClicked && GameMaster.getHighestBidValue () >= (int)timeRemaining && ((PlayerHand)GameMaster.searchDeckByID (GameMaster.getHighestBidderID ())).bidForAuction ((int)timeRemaining)) {
					buttonClicked = true;
					auctionInProcess = false;
					timerStopTime = Time.time;
					transferID = GameMaster.getHighestBidderID ();
					Transform temp = (Transform)Instantiate (particleEffectPrefab, transform.localPosition, transform.rotation);
					Destroy (temp.gameObject, 1f);
			}

			// Every frame, count down the timer.
			if (timeRemaining >= 10 && auctionInProcess) {
					timeRemaining -= Time.deltaTime * speedMultiplier;

			} else if (buttonClicked && timerStopTime + BUTTON_DELAY < Time.time) {	//After delay time, it transfers card from auction deck to player hand.
					//Debug.Log ("Deliver!!!");
					GameMaster.requestCardTransfer (100, transferID, true);
					((PlayerHand)GameMaster.searchDeckByID (transferID)).takeAuctionCard ((int)timeRemaining);
					buttonClicked = false;
					Destroy (this);
					GameMaster.terminateCurrentAuction ();
			} else if (timeRemaining < 10) {	// if timer counts down to zero, the object is destroyed and tells GameMaster to terminate auction.
					Destroy (this, 0.2f);
					GameMaster.terminateCurrentAuction ();
		}
	}

	// override OnGUI() from monobehavior. Implicitly called by unity.
	// apparently, GUI is refreshed every frame.
	void OnGUI()
	{
		GUI.backgroundColor = new Color(1,1,1,1);
		GUIStyle buttonStyle = new GUIStyle (GUI.skin.button);
		buttonStyle.normal.textColor = new Color (1f, 0.81568632f, 0.3156862754f, 1f);
		buttonStyle.fontSize = Utils.adjustUISize (20,true);
		buttonStyle.hover.textColor = Color.yellow;
		buttonStyle.active.textColor = Color.cyan;
		buttonStyle.fontStyle = FontStyle.Bold;
		buttonStyle.normal.background = buttonTexture;
		buttonStyle.hover.background = buttonTexture;
		buttonStyle.active.background = buttonTexture;

		// Converts localPosition of the transform to position vector in screenSpace, then to GUI space. I found some unidentified bug, and it needs rectification.
		Vector3 pos = transform.localPosition;//(Vector2)Camera.WorldToScreenPoint(pos)
		Vector3 screenPos = Camera.main.WorldToScreenPoint(new Vector3(pos.x, pos.y-1, pos.z));
		if (timeRemaining>=10)
		{
			//If button is clicked, stops timer and waits for BUTTON_DELAY seconds to destroy itself..
			// button clicking action only registers bidValue for player.
			// in case of multiplayer, The function gets re-mapped to network object (networkBidObject)
			if (GUI.Button (new Rect(screenPos.x-75, Camera.main.pixelHeight-screenPos.y, Utils.adjustUISize (150,true), Utils.adjustUISize (90,false)), new GUIContent("BUY! $"+(int)timeRemaining), buttonStyle) && !buttonClicked)
			{
				//Debug.Log ("Pressed!!!");
				//buttonClicked = true;
				((PlayerHand)GameMaster.searchDeckByID (GameMaster.UserID)).setBidValue ((int)timeRemaining);
				if (!(!Network.isClient && (!Network.isServer || Network.connections.Length<1)))
				{
					networkManager.networkObject.broadcastBidValue (new Vector2(GameMaster.UserID,(int)timeRemaining));
				}
				//auctionInProcess=false;
				//timerStopTime = Time.time;
			}
		}
		else{
			GUI.Button (new Rect(screenPos.x-75, Camera.main.pixelHeight-screenPos.y, Utils.adjustUISize (150,true), Utils.adjustUISize (90,false)), "Bid over", buttonStyle);
		}
	}
}
