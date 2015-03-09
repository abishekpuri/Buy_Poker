using UnityEngine;
using System.Collections;

public class Card : MonoBehaviour {

	static public Sprite[] cardSpriteList;// = Resources.LoadAll <Sprite> ("images/cards");	//preLoaded array of sprites from Assets/Resources/images folder

	public int rank;	// 1 to 13, 1 is Ace
	public int suit;	// 1 to 4, Clover, Heart, Spade and Diamond respectively.

	private SpriteRenderer sprite;	// reference of sprite for the card.
	private int sortingOrder;	// order in 2D graphics layer. Sprite with higher order is drawn on top.
	
	public int Rank{	//c# simplified getter and setter technique.
		get{return rank;}
		set{rank = value;}
	}
	public int Suit{
		get{return suit;}
		set{suit = value;}
	}

	public int SortingOrder{
		set{
			sortingOrder = value;
			GetComponent<SpriteRenderer>().sortingOrder = sortingOrder;
		}
	}




	private Sprite getCardSprite()	//returns card sprite based on rank and suit
	{
		return (suit<5 && suit>0 && rank<14 && rank>0)? (cardSpriteList[(rank-1)+((suit-1)*13)]) : (null);
	}

	
	public void showFace()
	{
		renderer.enabled=true;
		GetComponent<SpriteRenderer> ().sprite = getCardSprite ();	// I think you can do the same thing with sprite.sprite =getCardSprite ();
		GetComponent<SpriteRenderer>().color = Color.white;
		//Debug.Log ("Card: " + rank + " (" + suit + ") " + "ShowFace");//writes to console screen
	}

	public void showBackground()
	{
		renderer.enabled=true;
		GetComponent<SpriteRenderer> ().sprite = cardSpriteList[13];
		GetComponent<SpriteRenderer>().color = Color.red;
		//Debug.Log ("Card: " + rank + " (" + suit + ") " + "ShowBackground");
	}

	public void hideCard()
	{
		renderer.enabled = false;
		//Debug.Log ("Card: " + rank + " (" + suit + ") " + "HideCard");
	}


	// Use this for initialization. Function is called when the object is instantiated
	void Start () {
		sprite=GetComponent<SpriteRenderer> ();	//Finds and returns the component "SpriteRenderer", inside the GameObject.
		if (sprite == null)
			sprite = (SpriteRenderer)this.gameObject.AddComponent ("SpriteRenderer");	//Adds component to the gameObject if sprite is null
		//showFace ();
		//Debug.Log ("Card start");
		//showBackground ();
		//hideCard ();
	}

	void Awake(){

	}

	// Update is called once per frame
	void Update () {
	
	}
}
