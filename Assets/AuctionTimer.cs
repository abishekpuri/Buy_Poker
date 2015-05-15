using UnityEngine;
using System.Collections;

/*
 * AuctionTimer has two parts. As soon as this object is instantiated, it starts counting.
 * Also, Button UI is also bound to this object.
 * 
 * 
 * If any player wins the bid or if it counts to 10, it requests card transfer to GameMaster, and destroys itself.
 * 
 * */
public class AuctionTimer : MonoBehaviour {
	
	private int ticks;
	
	private float timeRemaining;	// time remaining
	private float networkHostTimeRemaining;	// network host time remaining.
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
		gameObject.AddComponent ("AudioSource");
		ticks = 0;
		timeRemaining = 100;
		networkHostTimeRemaining = 100;
		speedMultiplier = 10;	// 10 per second.
		if (Network.connections.Length>0)
			networkManager.networkObject.transferID = 0;
		auctionInProcess = true;
		buttonClicked = false;
		particleEffectPrefab = Resources.Load <Transform>("prefab/Particle System");
		buttonTexture = Resources.Load <Texture2D>("images/btnTexture");
		
		// For example, a server might not yet have broadcasted its auction timer. Then, clients would probably use the value of previous auction.
		// Client resets network register value to timeRemaining, until fresh value from the host arrives.
		if (Network.isClient && Network.connections.Length >= 1) {
			networkManager.networkObject.auctionCounter=100;
			networkManager.networkObject.broadcastBidValue (new Vector2(GameMaster.UserID,0));
		}
	}
	
	// Update is Implicitly called once per frame
	void Update () {
		// increase ticks by one
		ticks++;
		// Every frame, count down the timer.
		if (timeRemaining >= 10 && auctionInProcess) {
			timeRemaining -= Time.deltaTime * speedMultiplier;
		}
		
		/**
		 * 
		 * Network related section => timer synchronization code.
		 * 
		 * Extrapolation and intrapolation techniques are used
		 * 
		 * Host broadcasts timer value, and clients receives them.
		 * 
		 * */
		// server broadcasts current auction timer. That is, the host dominates the timer value
		if (ticks%10 == 0 && Network.isServer && Network.connections.Length>=1)
		{
			networkManager.networkObject.broadcastAuctionCounter (timeRemaining);
			//Debug.Log ("counter sync host side");
		}
		// clients receives the timer value, with extrapolation and interpolation.
		if (Network.isClient && Network.connections.Length>=1)
		{
			// interpolation technique used. Client's auction counter value generally converges to the host value, instead of jumping to the value.
			if (networkManager.networkObject.transferID==0)
				networkHostTimeRemaining-=Time.deltaTime * speedMultiplier;
			timeRemaining = timeRemaining+(networkHostTimeRemaining-timeRemaining)/5;
			if (ticks%10 == 5)
			{
				networkHostTimeRemaining=networkManager.networkObject.auctionCounter;// - (Network.GetLastPing(Network.connections[0])/1000f)*speedMultiplier;
				Debug.Log ("counter sync client side, raw value= "+ networkHostTimeRemaining);
				// little extrapolation technique used for better time synchronization at client side.
				// condition to extrapolate : it must be counting down in the future.
				if (networkHostTimeRemaining >= 10 && auctionInProcess)
					networkHostTimeRemaining-= (Network.GetAveragePing(Network.connections[0])/1000f)*speedMultiplier;
				Debug.Log ("counter sync client side, audjusted to latency= "+ networkHostTimeRemaining);
				Debug.Log ("Average ping= "+ Network.GetAveragePing(Network.connections[0]));
			}
			
		}
		// If auction is successful, transfer the card to respective playerHand.
		if (!buttonClicked &&((GameMaster.getHighestBidValue () >= (int)timeRemaining && ((PlayerHand)GameMaster.searchDeckByID (GameMaster.getHighestBidderID ())).bidForAuction ((int)timeRemaining)) || (Network.connections.Length>0 && networkManager.networkObject.transferID!=0))) {
			GameMaster.gm.audio.clip = Resources.Load <AudioClip>("audio/swoosh");
			GameMaster.gm.audio.Play ();
			buttonClicked = true;
			auctionInProcess = false;
			timerStopTime = Time.time;
			transferID = GameMaster.getHighestBidderID ();
			Transform temp = (Transform)Instantiate (particleEffectPrefab, transform.localPosition, transform.rotation);
			Destroy (temp.gameObject, 1f);
			// For multiplayer purpose.
			// If button click is detected, that is, if there is a player with highest bidding value register over the network,
			// host broadcasts the transferID value
			if (Network.isServer && Network.connections.Length>=1)
			{
				networkManager.networkObject.broadcastTransferID (transferID);
				//Debug.Log ("broadcastTransferID");
			}
		}
		
		
		//A second delay after one player wins the bid, it transfers card from auction deck to player hand.
		// if transferID=0 (network mode), then wait until the data arrive
		else if (transferID!=0 && buttonClicked && timerStopTime + BUTTON_DELAY < Time.time) {	
			//Debug.Log ("Deliver!!!");
			
			GameMaster.requestCardTransfer (100, transferID, true);
			((PlayerHand)GameMaster.searchDeckByID (transferID)).takeAuctionCard ((int)timeRemaining);
			buttonClicked = false;
			if (Network.connections.Length>=1)
				networkManager.networkObject.transferID=0;
			GameMaster.terminateCurrentAuction ();
			Destroy (this);
		}
		// For multiplayer purpose.
		// If button click is detected, that is, if there is a player with highest bidding value register over the network,
		// at the same time as host is broadcasting transferID value, client is trying to receive it.
		else if (buttonClicked && Network.isClient && Network.connections.Length>=1)
		{
			transferID = networkManager.networkObject.transferID;
			//Debug.Log ("Receive transfer ID = "+transferID);
		}
		else if (timeRemaining < 10) {	// if timer counts down to zero, the object is destroyed and tells GameMaster to terminate auction.
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
		if (GameMaster.searchHandByID (GameMaster.UserID).Cash >= timeRemaining) {
			buttonStyle.normal.textColor = Color.yellow;
			buttonStyle.active.textColor = Color.cyan;
		}
		else {
			buttonStyle.normal.textColor = Color.red;
			buttonStyle.hover.textColor = Color.red;
			buttonStyle.active.textColor = Color.red;
		}
		
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
			//Debug.Log (timeRemaining);
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
