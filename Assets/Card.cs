using UnityEngine;
using System.Collections;

public class Card : MonoBehaviour {

	static public Sprite[] cardSpriteList = Resources.LoadAll <Sprite> ("images/cards");

	public int rank;	// 1 to 13
	public int suit;	// 1 to 4

	private SpriteRenderer sprite;
	private int sortingOrder;
	
	public int Rank{
		get{return rank;}
		set{rank = value;}
	}
	public int Suit{
		get{return suit;}
		set{suit = value;}
	}

	public int SortingOrder{
		set{sortingOrder = value;}
	}




	public Sprite getCardSprite()
	{
		return (suit<5 && suit>0 && rank<14 && rank>0)? (cardSpriteList[(rank-1)+((suit-1)*13)]) : (null);
	}







	public void instantiate()
	{
	}

	public void showFace()
	{
		renderer.enabled=true;
		GetComponent<SpriteRenderer> ().sprite = getCardSprite ();
		GetComponent<SpriteRenderer>().color = Color.white;
		GetComponent<SpriteRenderer>().sortingOrder = sortingOrder;
		Debug.Log ("Card: " + rank + " (" + suit + ") " + "ShowFace");
	}

	public void showBackground()
	{
		renderer.enabled=true;
		GetComponent<SpriteRenderer> ().sprite = cardSpriteList[13];
		GetComponent<SpriteRenderer>().color = Color.red;
		GetComponent<SpriteRenderer>().sortingOrder = sortingOrder;
		Debug.Log ("Card: " + rank + " (" + suit + ") " + "ShowBackground");
	}

	public void hideCard()
	{
		renderer.enabled = false;
		GetComponent<SpriteRenderer>().sortingOrder = sortingOrder;
		Debug.Log ("Card: " + rank + " (" + suit + ") " + "HideCard");
	}


	// Use this for initialization
	void Start () {
		sprite=GetComponent<SpriteRenderer> ();
		if (sprite == null)
			sprite = (SpriteRenderer)this.gameObject.AddComponent ("SpriteRenderer");
		//showFace ();

		//showBackground ();
		//hideCard ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
