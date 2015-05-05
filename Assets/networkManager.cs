using UnityEngine;
using System.Collections;

public class networkManager : MonoBehaviour {

	public static string GAMENAME="networking_example";
	public static float REFRESH_HOST_TIMEOUT = 3f;
	public static float SEARCH_TIMEOUT=30f;
	float search_wait_time=0;
	float refresh_start_time;
	float search_start_time;

	string statusMsg;

	public Transform samplePref;

	float btnX;
	float btnY;
	float btnW;
	float btnH;

	bool searching;
	bool refreshing;
	HostData[] hostData;

	private Transform networkObject;

	// Use this for initialization
	void Start () {
		search_wait_time = 7f;
		btnX=Screen.width*0.05f;
		btnY=Screen.height*0.05f;
		btnW=Screen.width*0.15f;
		btnH=Screen.height*0.2f;
		refreshing = false;
		refresh_start_time = 0;
		hostData = new HostData[0];
		searching = false;
		search_start_time = 0;
		statusMsg = "";
	}
	
	// Update is called once per frame
	void Update () {
		// refresh for existing host, and retrieves host data.
		if (refreshing && (MasterServer.PollHostList ().Length > 0 || refresh_start_time+REFRESH_HOST_TIMEOUT<Time.time )) {
			refreshing=false;
			Debug.Log ("Host count = " + MasterServer.PollHostList ().Length);
			hostData = MasterServer.PollHostList ();
		}
		// If no connection after SEARCH_TIMEOUT, stops searching.
		if (searching && search_start_time + SEARCH_TIMEOUT < Time.time && Network.connections.Length<1) {
			Debug.Log ("Search failed!!");
			statusMsg="search failed!";
			searching =false;
			MasterServer.UnregisterHost();
			Network.Disconnect ();
		}
		// if you get to keep waiting, just print dot dot dot to keep you from being impatient
		if (searching && search_start_time + search_wait_time < Time.time && Network.connections.Length<1)
		{
			if (((int)search_wait_time)%4==0)
				statusMsg="waiting for clients";
			else
				statusMsg+=".";
			search_wait_time+=1f;
		}
	}

	public IEnumerator startNetworkingSearch()
	{
		searching = true;
		search_start_time = Time.time;
		bool hostFound = false;
		search_wait_time = 7f;

		// first step : refresh the host list.
		refreshHostList ();
		statusMsg="searching for host..";
		while (refreshing) {
			yield return new WaitForSeconds(0.1f);
			if (!searching)
				break;
		}

		// second step : for all list of hosts, search for empty room and connect.
		if (searching)
		{
			for (int i=0; i<hostData.Length; i++) {
				if (hostData[i].connectedPlayers<2 && searching)
				{
					statusMsg="host ID "+i+" found!";
					Network.Connect(hostData[i]);
					hostFound=true;
					searching = false;
					break;
				}
			}
		}
		// third step : create and register new host, and wait for new players coming in.
		if (!hostFound && searching)
		{
			statusMsg="("+hostData.Length+") registering as host";
			Debug.Log ("Host not found! Creating a new host...");
			Network.InitializeServer (2,25001,!Network.HavePublicAddress ());
			MasterServer.RegisterHost (GAMENAME,"host ID "+hostData.Length);
		}
	}


	void StartServer()
	{

	}

	void refreshHostList()
	{
		MasterServer.RequestHostList (GAMENAME);
		refreshing = true;
		refresh_start_time = Time.time;
	}

	void OnServerInitialized()
	{
		Debug.Log ("Server initialized!");
	}

	void OnConnectedToServer()
	{
		Debug.Log ("connected to host!");
		statusMsg="connected to host!";
		searching = false;
		networkObject=(Transform)(Instantiate (samplePref, new Vector3(8,3,0), Quaternion.identity));
	}

	void OnPlayerConnected()
	{
		Debug.Log ("connected to player!");
		statusMsg="connected to player!";
		searching = false;
		networkObject=(Transform)(Instantiate (samplePref, new Vector3(8,3,0), Quaternion.identity));
	}

	void OnDisconnectedFromServer()
	{
		Debug.Log ("disconnected from host!");
		Destroy (networkObject.gameObject);

	}

	void OnPlayerDisconnected()
	{
		Debug.Log ("player disconnected!");
		Destroy (networkObject.gameObject);
	}

	void OnMasterServerEvent(MasterServerEvent mse){
		if (mse == MasterServerEvent.RegistrationSucceeded)
		{
			Debug.Log ("Registered Server");
			statusMsg="waiting for clients";
		}
	}

	void OnGUI()
	{
		if (!Network.isClient && (!Network.isServer || Network.connections.Length<1)) {
			if (!searching)
			{
				if (GUI.Button (new Rect (btnX, btnY, btnW, btnH), "Start Search")) {
					Debug.Log ("Start searching for opponent");
					statusMsg="accessing server..";
					StartCoroutine(startNetworkingSearch ());
				}
			}
			else if (GUI.Button (new Rect (btnX, btnY, btnW, btnH), "Cancel Search"+"\n\n"+statusMsg))
			{
				Debug.Log ("Search cancel");
				searching=false;
				MasterServer.UnregisterHost();
				Network.Disconnect ();
			}
		}
		else if (GUI.Button (new Rect (btnX, btnY, btnW, btnH), "Disconnect!"))
		{
			Debug.Log ("Disconnected");
			MasterServer.UnregisterHost();
			Network.Disconnect ();
			Destroy (networkObject);
		}/*
		for (int i=0; i<hostData.Length; i++) {
			GUI.Box (new Rect (btnX+Screen.height*0.4f, btnY*1.2f+btnH*i, btnW*3f, btnH), hostData[i].gameName);
			
		}*/

	}
}
