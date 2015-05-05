using UnityEngine;
using System.Collections;

public class synchronizedDatabase : MonoBehaviour {

	float relativeX;
	float relativeY;
	float relativeW;
	float relativeH;
	int msgCount;
	string[] msgs;

	string tempstr;

	[RPC] public void registerMsg(string msg)
	{
		Debug.Log ("Remote procedure call registerMsg()");
		msgs [msgCount++] = "IP "+Network.proxyIP+" : "+msg;

	}

	public void broadcastMsg(string msg)
	{

		networkView.RPC ("registerMsg",RPCMode.AllBuffered, msg);
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
		for (int i=0; i<msgCount; i++)
		{
			resultString += msgs[i]+"\n\n";
		}
		GUI.Box (new Rect (relativeX, relativeY, relativeW, relativeH), resultString);
		tempstr = GUI.TextField (new Rect(relativeX, relativeY+relativeH*1.1f, relativeW, relativeH*0.2f),tempstr);
		if (GUI.Button (new Rect(relativeX, relativeY+relativeH*1.4f, relativeW*0.5f, relativeH*0.1f),"register message"))
		{
			broadcastMsg (tempstr);
		}
	}
}
