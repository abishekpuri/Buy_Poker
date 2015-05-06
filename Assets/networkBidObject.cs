using UnityEngine;
using System.Collections;

/**
 * 
 * I had to sacrifice a good class design to make multiplayer game and chat room work at the same time.
 * 
 * 
 * */


public class networkBidObject : MonoBehaviour {

	public int[] bidValueByID;
	
	[RPC] virtual public void registerBidValue(Vector3 values)
	{
		bidValueByID [(int)values.x] = (int)values.y;
	}

	virtual public void broadcastBidValue(Vector3 values)
	{
		networkView.RPC ("registerBidValue",RPCMode.AllBuffered,values);

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
	// Use this for initialization
	void Start () {
		bidValueByID = new int[100];
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
