using UnityEngine;
using System.Collections;

public class DebugCardBehavior : MonoBehaviour {
		
	Card cardController;
	int state=0;

	// Use this for initialization
	void Start () {

		cardController = GetComponent<Card>();


	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnMouseDown()
	{
		Debug.Log ("mouse up");
		state++;
		switch (state) {
		case 1:
				cardController.showBackground ();
				break;
		case 2:
				cardController.showFace ();
				state = 0;
				break;
		case 3:
				cardController.hideCard ();
				
				break;
		}
	}

}
