using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Deck : MonoBehaviour {
	
	public List<GameObject> cards;			// collection of card gameObjects.
	public Transform referenceTransform;	// reference position, scale and rotation of entire deck. Positions should be locally coordinated.

	private bool inTransit;	// true if it is in state of change
	private Vector3 currentVelocity;
	//public Card[] cards;
	//public int cardCount;

	// Use this for initialization
	void Start () {
		cards = new List<GameObject>();
		referenceTransform = GetComponent<Transform> ();

		generateFullCardDeck ();
		shuffle ();
		setUpOrientation (1);
		//orientation_toHorizontalLayout ();
		for (int i=0; i<cards.Count; i++)
		{
			//SpriteRenderer sprite = cards[i].GetComponent<SpriteRenderer>();
			//sprite.sortingOrder = i;
			//cards[i].GetComponent<Card>().SortingOrder = i;
			//cards[i].GetComponent<Card>().showBackground ();
			cards[i].GetComponent<Card>().showFace ();
		}
		Debug.Log ("called " + cards.Count);
	}
	
	// Update is called once per frame
	void Update () {
		
		if (inTransit)
		{
			Debug.Log ("moving");
			for (int i=0; i<cards.Count; i++) {
				Vector3 targetLocalPos = new Vector3(i*0.3f,0,0);
				Vector3 newPos = Vector3.SmoothDamp(cards[i].GetComponent<Transform>().localPosition, targetLocalPos, ref currentVelocity, 1f);
				cards[i].GetComponent<Transform>().localPosition = newPos;
				//cards [i].GetComponent<Card> ().SortingOrder = i;
			}
			
		}

	}

	public void orientation_toHorizontalLayout()
	{
		for (int i=0; i<cards.Count; i++) {
			cards [i].GetComponent<Card> ().SortingOrder = i;
		}
		inTransit = true;
	}


	private Vector3 targetCardLocalPosition(int type, int indexReference)
	{
		switch (type)
		{
		case 1:
			return new Vector3 (indexReference*0.3f,0,0);
		default:
			return new Vector3(0,0,0);
		}
	}

	private void setUpOrientation(int type = 0)
	{
		for (int i=0; i<cards.Count; i++) {
			//SpriteRenderer sprite = cards[i].GetComponent<SpriteRenderer>();
			//sprite.sortingOrder = i;
			cards [i].GetComponent<Card> ().SortingOrder = i;
			cards[i].GetComponent<Transform>().localScale = referenceTransform.localScale;
			cards[i].GetComponent<Transform>().localPosition = targetCardLocalPosition (type, i);

		}
	}

	/*
	public void AddCard(int suit, int rank, bool showFaceUp)
	{
		Card cardController = cards [cards.Count].GetComponent<Card> ();
		cardController.rank = rank;
		cardController.suit = suit;

	}
	*/
	/*

	public void new_card(Card card)
	{
		deckTransform = GetComponent<Transform> ();
		//cards = 
	}
	*/

	/*
	public void new_card(int rank, int suit)
	{

		cards.Add ((Card)Instantiate (Resources.Load ("prefab/card"), Vector3.zero, Quaternion.identity));
		cards[cards.Count-1].rank = rank;
		cards [cards.Count - 1].suit = suit;
	}
	*/

	public void new_card(int rank, int suit)
	{
		GameObject newCard = (GameObject)Instantiate (Resources.Load ("prefab/card"), referenceTransform.localPosition, Quaternion.identity);
		newCard.GetComponent<Card>().rank = rank;
		newCard.GetComponent<Card>().suit = suit;
		newCard.GetComponent <Transform>().parent = (Transform)this.GetComponent <Transform>();

		cards.Add (newCard);
	}

	public void generateFullCardDeck()
	{
		for (int i=1; i<=4; ++i)
			for (int j=1; j<=13; ++j)
				new_card(j,i);
	
	}

	public void shuffle()
	{
		for (int i=0; i<cards.Count; i++)
		{
			int swap_target = Random.Range (0,cards.Count);
			GameObject temp = cards[i];
			cards[i] = cards[swap_target];
			cards[swap_target] = temp;
		}
	}






}
