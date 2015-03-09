using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Deck : MonoBehaviour {
	
	public List<GameObject> cards;			// collection of gameObjects references (cards). List should be quite similar to Vector
	public Transform referenceTransform;	// this class includes reference position, scale and rotation of entire deck. Individual cards will be positioned based on this reference transform
	private bool inTransition;	// true if the cards should be moving around. I'm Not sure if this is the best way for animation.
	private int transitionType;	//0 or 1 for now. 0 is to merge, and 1 is to spreadout
	private Vector3 currentVelocity;	//Its used just for input parameter for a function SmoothDamp().
	//public Card[] cards;
	//public int cardCount;

	// Use this for initialization
	void Start () {
		cards = new List<GameObject>();
		referenceTransform = GetComponent<Transform> ();

		//generateFullCardDeck ();
		//shuffle ();
		setScaleAndOrder ();
		//animation_spreadHorizontal ();
		//animation_merge();

		//Debug.Log ("cardCount = " + cards.Count);
	}
	
	// Update is called once per frame
	void Update () {
		
		if (inTransition)	// does not work well
		{
			for (int i=0; i<cards.Count; i++) {
				Vector3 targetLocalPos = targetCardPos (transitionType, i);
				Vector3 newPos = Vector3.SmoothDamp(cards[i].GetComponent<Transform>().localPosition, targetLocalPos, ref currentVelocity, 1f);
				// I don't really get how SmoothDamp() works. We should manually implement vector translation. Temporary solution only.
				cards[i].GetComponent<Transform>().localPosition = newPos;
			}
			
		}

	}

	public void openDeck()
	{
		for (int i=0; i<cards.Count; i++)
		{
			cards[i].GetComponent<Card>().SortingOrder = i;
			cards[i].GetComponent<Card>().showFace ();
		}
	}

	public void closeDeck()
	{
		for (int i=0; i<cards.Count; i++)
		{
			cards[i].GetComponent<Card>().SortingOrder = i;
			cards[i].GetComponent<Card>().showBackground ();
		}
	}

	private Vector3 targetCardPos(int type, int indexReference)	//returns target vector for each cards. Temporary solution
	{
		switch (type)
		{
		case 1:
			return new Vector3 (indexReference*0.3f,0,0);
		default:
			return new Vector3(0,0,0);
		}
	}

	public void animation_spreadHorizontal()
	{
		for (int i=0; i<cards.Count; i++) {
			cards [i].GetComponent<Card> ().SortingOrder = i;
		}
		inTransition = true;
		transitionType = 1;
	}
	public void animation_merge()
	{
		for (int i=0; i<cards.Count; i++) {
			cards [i].GetComponent<Card> ().SortingOrder = i;
		}
		inTransition = true;
		transitionType = 0;
	}


	private void setScaleAndOrder()
	{
		for (int i=0; i<cards.Count; i++)
		{
			cards [i].GetComponent<Card> ().SortingOrder = i;
			cards[i].GetComponent<Transform>().localScale = referenceTransform.localScale;
			//cards[i].GetComponent<Transform>().localPosition = targetCardPos (type, i);
			
		}
	}

	public void setupOrientation(int type = 0)	// later on it should be private
	{
		for (int i=0; i<cards.Count; i++) {
			cards [i].GetComponent<Card> ().SortingOrder = i;
			//cards [i].GetComponent<Card> ().SortingOrder = i;
			//cards[i].GetComponent<Transform>().localScale = referenceTransform.localScale;
			cards[i].GetComponent<Transform>().localPosition = targetCardPos (type, i);

		}
	}



	public void new_card(int rank, int suit)	// create a new card object into the scene, and adds its reference to the cardlist.
	{
		GameObject newCard = (GameObject)Instantiate (Resources.Load ("prefab/card"), referenceTransform.localPosition, Quaternion.identity);
		// instantiate() creates an object into the scene. Then, it returns Object class.
		newCard.GetComponent<Card>().rank = rank;	//set rank and suit
		newCard.GetComponent<Card>().suit = suit;
		newCard.GetComponent <Transform>().parent = (Transform)this.GetComponent <Transform>();// let all new cards become a child of this deck.
		newCard.GetComponent<Card>().showFace ();
		cards.Add (newCard);	// add newCard to list of cards.
		setScaleAndOrder ();
	}

	public void generateFullCardDeck()
	{
		for (int i=1; i<=4; ++i)
			for (int j=1; j<=13; ++j)
				new_card(j,i);
		setScaleAndOrder ();
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
		setScaleAndOrder ();
	}






}
