using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Deck : MonoBehaviour {
	
	public List<GameObject> cards;			// collection of gameObjects references (cards). List should be quite similar to Vector
	public Transform referenceTransform;	// this class includes reference position, scale and rotation of entire deck. Individual cards will be positioned based on this reference transform
	private bool moveEnabled;	// true if the cards should be moving around. I'm Not sure if this is the best way for animation.
	private int currentLayoutType;	//0 or 1 for now. 0 is to merge, and 1 is to spreadout
	private Vector3 currentVelocity;	//Its used just for input parameter for a function SmoothDamp().





	/*
	 * type 0 = collapse
	 * type 1 = spread horizontal, right
	 * type 2 = spread horizontal, from middle
	 * type 3 = complete seperation, to the right
	 * type 4 = complete seperation, from middle
	 * type 4 = hand spread, with angle. Spread to right.
	 * */
	public void setupLayout(int type)
	{
		setScaleAndOrder ();
		currentLayoutType = type;
		for (int i=0; i<cards.Count; i++)
			cards[i].GetComponent<Card>().setTargetPos(computeIndividualCardTargetPos (type, i));
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
		setupLayout (currentLayoutType);
	}

	public void openDeck()	// show face of all cards in deck
	{
		setScaleAndOrder ();
		for (int i=0; i<cards.Count; i++)
			cards[i].GetComponent<Card>().showFace ();
	}

	public void hideDeck()	// show background for all cards in deck
	{
		setScaleAndOrder ();
		for (int i=0; i<cards.Count; i++)
			cards[i].GetComponent<Card>().showBackground ();
	}

	public void generateFullCardDeck()
	{
		for (int i=1; i<=4; ++i)
			for (int j=1; j<=13; ++j)
				new_card(j,i);
	}

	private void setScaleAndOrder()
	{
		for (int i=0; i<cards.Count; i++)
		{
			cards [i].GetComponent<Card> ().setSortingOrder (i);
			cards[i].GetComponent<Transform>().localScale = referenceTransform.localScale;
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

	public void destroyAll()
	{
		while (cards.Count>0)
		{
			Destroy (cards[0].gameObject);
			cards.Remove (cards[0]);
			//cards [i].GetComponent<Card> ().setSortingOrder (i);
			//cards[i].GetComponent<Transform>().localScale = referenceTransform.localScale;
		}
		Debug.Log ("After destroying everything, "+cards.Count + " cards Left ");
	}
	
	private Vector4 computeIndividualCardTargetPos(int orientationType, int indexReference)	//returns target vector for each cards. Temporary solution
	{
		// 4th vector represents rotation.
		switch (orientationType)
		{
		case 1:
			return new Vector3 (indexReference*0.3f,0,0);
		case 2:
			return new Vector3 (indexReference*0.3f - (0.3f*(cards.Count-1))*0.5f,0,0);
		case 3:
			return new Vector3 (indexReference*1f,0,0);
		case 4:
			return new Vector3 (indexReference*1f - (1f*(cards.Count-1))*0.5f,0,0);
		case 5:
			return new Vector4 (indexReference*0.2f,0,0,indexReference*5);
		default:
			return new Vector3(0,0,0);
		}
	}

	// Use this for initialization
	void Start () {
		cards = new List<GameObject>();
		referenceTransform = GetComponent<Transform> ();
		GameMaster.reportDeckToGameMaster (this);
	}
	
	// Update is called once per frame
	void Update () {
	}


}
