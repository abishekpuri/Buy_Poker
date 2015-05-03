using UnityEngine;
using System.Collections;

public class Utils : MonoBehaviour {


	// returns integer size value adjusted for screen size.
	public static int adjustUISize(int UISpaceboxSize, bool isAxisX)
	{
		return (int)(isAxisX?((float)UISpaceboxSize*(float)Camera.main.pixelWidth/1085f) : ((float)UISpaceboxSize*(float)Camera.main.pixelHeight/577f));




	}

	//static 



	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
