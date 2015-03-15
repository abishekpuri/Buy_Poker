using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Deck : MonoBehaviour {

	public int reserveDeckID;
	private List<Card> cards;			// collection of gameObjects references (cards). List should be quite similar to Vector
	private int deckID;
	private bool initializeFlag = true;
	private Transform referenceTransform;	// this class includes reference position, scale and rotation of entire deck. Individual cards will be positioned based on this reference transform
	private int currentLayoutType;	//current layout type

	public int DeckID{	//c# simplified getter and setter technique.
		get{return deckID;}
		set{if (initializeFlag){deckID = value; initializeFlag=false;}else{Debug.LogError ("Deck ID assignment denied");}}
	}

	public void transferTopCardTo(Deck another, bool cardOpen)
	{
		if (cards.Count>0)
		{
			if (cardOpen)
				cards[cards.Count-1].GetComponent<Card>().showFace ();
			//Debug.Log ("Card count = " + cards.Count);
			//Debug.Log ("card to be transferred : Rank = " + cards[cards.Count-1].GetComponent <Card>().Rank);
			another.addExistingCard (cards[cards.Count-1]);
			cards.Remove (cards [cards.Count - 1]);
			setupLayout(currentLayoutType);
		}
	}

	public void setupLayout(int type)
	{
		currentLayoutType = type;
		setScaleAndOrder ();
		for (int i=0; i<cards.Count; i++)
			cards[i].setTargetPos(computeIndividualCardTargetPos (type, i));
	}
	public void setLayoutType(int type)
	{
		currentLayoutType = type;
	}

	public void openDeck()	// show face of all cards in deck
	{
		for (int i=0; i<cards.Count; i++)
			cards[i].GetComponent<Card>().showFace ();
	}

	public void closeDeck()	// show background for all cards in deck
	{
		for (int i=0; i<cards.Count; i++)
			cards[i].GetComponent<Card>().showBackground ();
	}

	private void setScaleAndOrder()
	{
		for (int i=0; i<cards.Count; i++)
		{
			cards [i].setSortingOrder (i+(deckID%100)*100);
			cards[i].GetComponent<Transform>().localScale = referenceTransform.localScale;
		}
	}

	public void new_card(int rank, int suit)	// create a new card object into the scene, and adds its reference to the cardlist.
	{
		GameObject newCard = (GameObject)Instantiate (Resources.Load ("prefab/card"), referenceTransform.localPosition, Quaternion.identity);
		// instantiate() creates an object into the scene. Then, it returns Object class.
		newCard.GetComponent<Card>().initializeCard (rank,suit);	//set rank and suit
		newCard.GetComponent <Transform>().parent = (Transform)this.GetComponent <Transform>();// let all new cards become a child of this deck.
		newCard.GetComponent<Card>().showFace ();
		cards.Add (newCard.GetComponent<Card>());	// add newCard to list of cards.
		//Debug.Log (cards.Count + " cards count ");
		setupLayout(currentLayoutType);
	}

	public void addExistingCard(Card card)	//add existing card. It should be distinguished from new_card
	{
		cards.Add (card);
		card.GetComponent <Transform>().parent = (Transform)this.GetComponent <Transform>();
		setupLayout (currentLayoutType);
		//Debug.Log ("control transfer successful");
	}


	public void destroyAll()
	{
		while (cards.Count>0)
		{
			Destroy (cards[0].gameObject);
			cards.Remove (cards[0]);
		}
		//Debug.Log ("After destroying everything, "+cards.Count + " cards Left ");
	}
	
	private Vector3[] computeIndividualCardTargetPos(int orientationType, int indexReference)	//returns target vector for each cards. Temporary solution
	{
		// 0th index represents position.
		// 1th index represents Euler rotation.
		Vector3[] pos = new Vector3[2];
		pos[0] = new Vector3(0,0,0);
		pos [1] = new Vector3 (0,0,0);
		switch (orientationType)
		{
		case 1:
			pos[0] = new Vector3 (indexReference*0.3f,0,0.001f);
			break;
		case 2:
			pos[0] = new Vector3 (indexReference*0.3f - (0.3f*(cards.Count-1))*0.5f,0,0.001f);
			break;
		case 3:
			pos[0] = new Vector3 (indexReference*1f,0,0.001f);
			break;
		case 4:
			pos[0] = new Vector3 (indexReference*1f - (1f*(cards.Count-1))*0.5f,0,0.001f);
			break;
		case 5:
			pos[0] = new Vector3 (indexReference*0.3f,0,0);
			pos[1] = new Vector3 (indexReference*(-120f)/(cards.Count),0,0.001f);
			break;
		case 6:			// DO NOT USE THIS WITH ANY MORE THAN 100 CARDS. (1-cos) wave is drawn on Y-axis
			float maximumTilt = 30f;
			maximumTilt *= Mathf.Sqrt (cards.Count)/7;		//maximum angle normalization
			float middleCardIndex = cards.Count/2f-0.5f;		//reference point for yDist=0
			float yDist = Mathf.Abs(indexReference-middleCardIndex);	//Y distance from middle
			yDist = (1-Mathf.Cos(yDist/(30f)))*3f;		// y distance transformed from linear to curve
			pos[0] = new Vector3 (indexReference*0.3f - (0.3f*(cards.Count-1))*0.5f,-yDist,0.001f);
			pos[1] = new Vector3 (0,0,((indexReference+0.5f)*(-maximumTilt))/(cards.Count) + (maximumTilt/2));
			break;
		case 7:			// Non-normalized version of case 6
			float maximumTilt1 = 50f;
			float middleCardIndex1 = cards.Count/2f-0.5f;		//reference point for yDist=0
			float yDist1 = Mathf.Abs(indexReference-middleCardIndex1);	//Y distance from middle
			yDist1 = (1-Mathf.Cos(yDist1/(10f)))*3f;		// y distance transformed from linear to curve
			pos[0] = new Vector3 (indexReference*0.2f - (0.2f*(cards.Count-1))*0.5f,-yDist1,0.001f);
			pos[1] = new Vector3 (0,0,((indexReference+0.5f)*(-maximumTilt1))/(cards.Count) + (maximumTilt1/2));
			break;
		default:
			pos[0] = new Vector3 (indexReference*0.0015f,indexReference*0.0015f,0);
			break;
		}
		return pos;
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
			Card temp = cards[i];
			cards[i] = cards[swap_target];
			cards[swap_target] = temp;
		}
		setScaleAndOrder ();
		setupLayout (currentLayoutType);
	}

	// Use this for initialization
	void Start () {
		if (reserveDeckID>200)
			deckID = 0;
		else if (reserveDeckID >= 100)
			deckID = reserveDeckID;
		cards = new List<Card>();
		referenceTransform = GetComponent<Transform> ();
		GameMaster.reportDeckToGameMaster (this);

	}
	
	// Update is called once per frame
	void Update () {
	}


}
