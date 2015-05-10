using UnityEngine;
using System.Collections;

/**
 * 
 * I had to sacrifice a good class design to make multiplayer game and chat room work at the same time.
 * 
 * 
 * 
 * 
 * */


public class networkBidObject : MonoBehaviour {

	public int[] bidValueByID;
	public int[] randomValues;
	public int[] playersReady;

	public float auctionCounter;


	// this function gets called remotely.
	// networkView.RPC ("registerBidValue",RPCMode.AllBuffered,values);
	// calls this function in every players in network. In that way, players can share certain values, such as random values.
	//There are also RPCMode.others
	[RPC] virtual public void registerBidValue(Vector3 values)
	{
		bidValueByID [(int)values.x] = (int)values.y;
	}

	//broadcast auction counter
	public void broadcastAuctionCounter(float val)
	{
		networkView.RPC ("registerAuctionCounter",RPCMode.All,val);
	}
	// register auction counter
	[RPC] public void registerAuctionCounter(float val)
	{
		auctionCounter = val;
	}


	// broadcast bid value
	virtual public void broadcastBidValue(Vector3 values)	// chat object overrides this function.
	{
		networkView.RPC ("registerBidValue",RPCMode.All,values);
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
		auctionCounter = 100;
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
