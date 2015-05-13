using UnityEngine;
using System.Collections;

/**
 * 
 * I had to sacrifice a good class design to make multiplayer game and chat room work at the same time.
 * 
 * this class stores all the values synchronized by all players.
 * Using Remote Procedural Calls.
 * 
 * 
 * */


public class networkBidObject : MonoBehaviour {

	public int[] bidValueByID;
	public int[] randomValues;
	public int[] playersReady;
	public float[] playersCash;
	public int[] playersPoint;
	
	public float auctionCounter;	// auction counter value synchronized from the host side.
	public int transferID;			// card transfer ID after auction, synchronized from the host side.

	// this function gets called remotely.
	// networkView.RPC ("registerBidValue",RPCMode.AllBuffered,values);
	// calls this function in every players in network. In that way, players can share certain values, such as random values.
	//There are also RPCMode.others
	[RPC] virtual public void registerBidValue(Vector3 values)
	{
		bidValueByID [(int)values.x] = (int)values.y;
	}
	// broadcast bid value
	virtual public void broadcastBidValue(Vector3 values)	// chat object overrides this function.
	{
		networkView.RPC ("registerBidValue",RPCMode.All,values);
	}

	//broadcast auction counter. On
	public void broadcastAuctionCounter(float val)
	{
		networkView.RPC ("registerAuctionCounter",RPCMode.All,val);
	}
	// register auction counter
	[RPC] public void registerAuctionCounter(float val)
	{
		auctionCounter = val;
	}

	public void broadcastPlayersCash(Vector3 val)
	{
		networkView.RPC ("registerPlayersCash", RPCMode.All, val);
	}
	[RPC] public void registerPlayersCash(Vector3 val)
	{
		playersCash[(int)(val.x)]=val.y;
	}

	public void broadcastPlayersPoint(Vector3 val)
	{
		networkView.RPC ("registerPlayersPoint", RPCMode.All, val);
	}
	[RPC] public void registerPlayersPoint(Vector3 val)
	{
		playersPoint[(int)(val.x)]=(int)val.y;
	}

	//broadcast transferID
	public void broadcastTransferID(int val)
	{
		networkView.RPC ("registerTransferID",RPCMode.All, val);
	}

	// registers transfer ID
	[RPC] public void registerTransferID(int val)
	{
		transferID = val;
	}

	public void forceEndAuctionForAll()
	{
		networkView.RPC ("forceEndAuction", RPCMode.All);
	}

	[RPC] public void forceEndAuction()
	{
		GameMaster.endAuctionEarly ();
	}

	public void forceRequestCardTransfer(int destID)
	{
		networkView.RPC ("requestCardTransfer",RPCMode.All, destID);
	}

	[RPC] public void requestCardTransfer (int destID)
	{
		GameMaster.requestCardTransfer (0,destID, destID==GameMaster.UserID);
	}

	[RPC] public void registerRandomValues(Vector3 values)
	{
		randomValues[(int)values.x] = (int)values.y;
	}

	// the host-side player creates random values of range 0 to 1000, and sends these values over the network.
	public void broadcastRandomValues()
	{
		for (int i=0; i<100; i++)
		{
			networkView.RPC ("registerRandomValues",RPCMode.All,new Vector3(i,Random.Range (0,1000)));
		}
	}

	[RPC] public void registerPlayerReadyStatus(Vector3 ready)
	{
		playersReady[(int)ready.x] = (int)ready.y;
	}
	public bool isPlayersReady()
	{
		// I was just too lazy to even write one for loop.
		if (playersReady [1] > 0 && playersReady [2] > 0) {
			playersReady[1]=0;
			playersReady[2]=0;
			return true;
		} else
			return false;
	}

	public void playerIsReady(int ID)
	{
		networkView.RPC ("registerPlayerReadyStatus",RPCMode.All,new Vector3(ID,1));
	}

	public int getCommonRandomValue(int index, int mod)
	{
		return randomValues [index] % mod;
	}

	public void debug()
	{
		Debug.Log ("!!!!!referenced");
	}

	public int getHighestBidderIDOverNetwork()
	{
		int currentMax=0;
		int currentPlayerID=1;
		for (int i=1; i<100; i++) {
			if (bidValueByID[i]>=currentMax)
			{
				currentMax=bidValueByID[i];
				currentPlayerID=i;
			}
		}
		return currentPlayerID;
	}

	// super buggy.
	public int getHighestBidValueOverNetwork()
	{
		int currentMax=0;
		int currentPlayerID=1;
		for (int i=1; i<100; i++) {
			if (bidValueByID[i]>=currentMax)
			{
				currentMax=bidValueByID[i];
				currentPlayerID=i;
			}
		}
		return currentMax;
	}

	void Awake() {
		bidValueByID = new int[100];
		randomValues = new int[1000];
		playersReady = new int[100];
		playersCash = new float[100];
		playersPoint = new int[100];
		auctionCounter = 100;
		transferID = 0;
		for (int i=0; i<100; i++) {
			playersReady[i]=1;
		}
	}
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
