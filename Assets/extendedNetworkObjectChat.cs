using UnityEngine;
using System.Collections;

/**
 * 
 * chat script for unity
 * Class re-design is seriousely required.
 * - it is better re-named
 * - had to messed up with class hierarchy so that multiplayer game on top of chat can be implemented. 
 * 
 * 
 * extends from networkBidObject.
 * Since I am trying to use same networkManager for both chatroom and multiplayer game,
 * there is just no way to access functions of this class in a pretty way.
 * 
 * overrides
 * registerBidValue()
 * broadcastBidValue() from networkBidObject.
 * 
 * 
 * 
 * */


public class extendedNetworkObjectChat : networkBidObject {

	float relativeX;
	float relativeY;
	float relativeW;
	float relativeH;
	int msgCount;
	string[] msgs;

	string tempstr;

	[RPC] public void registerBidValue(string msg)
	{
		Debug.Log ("Remote procedure call registerMsg()");
		msgs [msgCount++] = msg;
	}

	public void broadcastBidValue(string msg)
	{
		networkView.RPC ("registerBidValue",RPCMode.AllBuffered, "IP "+Network.natFacilitatorIP+" : "+msg);
	}


	// Use this for initialization
	void Start () {
		msgs = new string[100];
		tempstr = "";
		relativeX = Screen.width * 0.3f;
		relativeY = Screen.height * 0.05f;
		relativeW = Screen.width * 0.6f;
		relativeH = Screen.height * 0.6f;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI()
	{
		string resultString = "";
		for (int i=msgCount-1; i>=0; i--)
		{
			resultString += msgs[i]+"\n\n";
		}
		GUI.Box (new Rect (relativeX, relativeY, relativeW, relativeH), resultString);
		tempstr = GUI.TextField (new Rect(relativeX, relativeY+relativeH*1.1f, relativeW, relativeH*0.2f),tempstr);
		if (GUI.Button (new Rect(relativeX, relativeY+relativeH*1.4f, relativeW*0.5f, relativeH*0.1f),"register message"))
		{
			broadcastBidValue (tempstr);
		}
	}
}
