using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/*
 * 
 * This class controls behavior of collection of cards.
 * Each deck should be assigned a unique ID for GameMaster to refer to.
 * 
 * 
 * 
 * */
public class Deck : MonoBehaviour {

	public int reserveDeckID;
	private List<Card> cards;			// collection of gameObjects references (cards). List should be quite similar to Vector
	private int deckID;
	public string CombinationType = "Evaluating Hand .."; //consider making private later.
	public int CombinationValue; // used to break ties if CombinationType same
	public int CombinationRank; // Number that tracks hand value, better than enumerating hand types
	private bool initializeFlag = true;			// Once ID is set, it cannot be changed again. I added variable just for the sake of protection, but I might be complicating things.
	private Transform referenceTransform;	// this class includes reference position, scale and rotation of entire deck. Individual cards will be positioned based on this reference transform
	private int currentLayoutType;	//current layout type

	public int DeckID{	//c# simplified getter and setter technique. deckList[i].DeckID is enough! getDeckID() does not work!
		get{return deckID;}
		set{if (initializeFlag){deckID = value; initializeFlag=false;}else{Debug.LogError ("Deck ID assignment denied");}}
	}
	public List<Card> CARDS {
				get{ return cards;}
		}
	public void evaluateDeck()
	{
		int counter = 0;
		bool straight = false;
		List<int> value = new List<int> ();
		List<int> suit = new List<int> ();
		for (int i = 0; i < 15; i++) {
						if (i < 5) {
								suit.Add (0);
						}
						value.Add (0);
		};
				for (int i = 0; i < cards.Count; i++) {
						if(cards[i].Rank == 1) {
							value[14] ++;
						}
						else {
							value [cards [i].Rank]++;
						}
						suit [cards [i].Suit]++;
				}

				for (int i = 0; i < value.Count; i++) {
						if (value [i] > 0) {
								counter++;
						} else {
								counter = 0;
						}
						if (counter == 5) {
								straight = true;
								break;
						}
				}
				int pairs = value.FindAll (a => a == 2).Count;
				int three_kind = value.FindAll (a => a == 3).Count;
				int four_kind = value.FindAll (a => a == 4).Count;
				int flush = suit.FindAll (a => a >= 5).Count;
				counter = 0;
				if ((flush != 0) && straight) {
						CombinationRank = 1;
						CombinationType = "Straight Flush";
				} else if (four_kind >= 1) {
						CombinationRank = 2;
						CombinationValue = value.FindLastIndex (a => a == 4); 
						CombinationType = "Four of a Kind";
				} else if (three_kind > 1 || (three_kind == 1 && pairs >= 1)) {
						CombinationRank = 3;
						CombinationValue = value.FindLastIndex (a => a == 3);
						CombinationType = "Full House";
				} else if (flush >= 1) {
						CombinationRank = 4;
						CombinationType = "Flush";
				} else if (straight) {
						CombinationRank = 5;
						CombinationType = "Straight";
				} else if (three_kind >= 1) {
						CombinationRank = 6;
						CombinationValue = value.FindLastIndex (a => a == 3);
						CombinationType = "Three of a Kind";
				} else if (pairs >= 2) {
						CombinationRank = 7;
						CombinationValue = value.FindLastIndex (a => a == 2);
						CombinationType = "Two Pair";
				} else if (pairs == 1) {
						CombinationRank = 8;
						CombinationValue = value.FindLastIndex (a => a == 2);
						CombinationType = "One Pair";
				} else {
						CombinationRank = 9;
						CombinationValue = value.FindLastIndex (a => a == 1);
						CombinationType = "High Card";
				}
	}

	public Card peekTopCard()
	{
		return cards [cards.Count - 1];
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

	public void setupLayout(int type)	// sets position of every and each cards inside the deck.
	{
		currentLayoutType = type;
		setScaleAndOrder ();
		for (int i=0; i<cards.Count; i++)
			cards[i].setTargetPos(computeIndividualCardTargetPos (type, i));
	}
	public void setLayoutType(int type)	// sets position of every and each cards inside the deck.
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

	private void setScaleAndOrder()		// Sets every card's scale corresponding to its parent's transform, and sets sorting order as well.
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


	/*
	 * Default case: collapse
	 * Case 1: spread by overlapping with each other. To the right.
	 * Case 2: same, spreads from middle
	 * Case 3: spreads with distance. To right.
	 * Case 4: spreads with distance. Spreads from middle.
	 * Case 5: rotates in a weird axis.
	 * Case 6: Spreads in an arc shape. (Shape of 1-cos function to be precise)
	 * 
	 * 
	 * */
	//returns two 3D vectors (position, rotation in eulerian form) for each cards, relative to its index in the list.
	private Vector3[] computeIndividualCardTargetPos(int orientationType, int indexReference)
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
			pos[0] = new Vector3 (indexReference*1.2f,0,0.001f);
			break;
		case 4:
			pos[0] = new Vector3 (indexReference*1.2f - (1.2f*(cards.Count-1))*0.5f,0,0.001f);
			break;
		case 5:
			pos[0] = new Vector3 (indexReference*0.3f,0,0);
			pos[1] = new Vector3 (indexReference*(-120f)/(cards.Count),0,0.001f);
			break;
		case 6:			// DO NOT USE THIS WITH ANY MORE THAN 100 CARDS. You will see (1-cos) wave drawn on Y-axis
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
				for (int i=1; i<=4; ++i) {
						for (int j=1; j<=10; ++j) {
								new_card (j, i);
						}
				}
				for (int i = 1; i <= 4; ++ i) {
						new_card (11, i);
						new_card (12, i);
						new_card (13, i);
				}
		}

	public void shuffle()
	{
		for (int i=0; i<cards.Count; i++)
		{
			int swap_target = Random.Range (i,cards.Count);
			Card temp = cards[i];
			cards[i] = cards[swap_target];
			cards[swap_target] = temp;
		}
		setScaleAndOrder ();
		setupLayout (currentLayoutType);
	}

	// Use this for initialization
	public void Start () {

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
