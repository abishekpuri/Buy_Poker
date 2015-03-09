using UnityEngine;
using System.Collections;

public class GameMaster : MonoBehaviour {


	void Awake() {
		Card.cardSpriteList = Resources.LoadAll <Sprite> ("images/cards");
		Debug.Log ("Card sprite resourses loaded once and for all");
	}

	// Use this for initialization
	void Start () {
		//deck = new Deck ();
		//deck.new_card (3,3);
		//deck.new_card (2,1);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
