using UnityEngine;
using System.Collections;

public class Card : MonoBehaviour {
	static public Sprite[] cardSpriteList;// = Resources.LoadAll <Sprite> ("images/cards");	//preLoaded array of sprites from Assets/Resources/images folder
	const float rotationSpeed=3f;
	const float movementSpeed = 3f;


	private int rank;	// 1 to 13, 1 is Ace
	private int suit;	// 1 to 4, Clover, Heart, Spade and Diamond respectively.

	private bool initializeFlag = true;
	private int sortingOrder;	// order in 2D graphics layer. Sprite with higher order is drawn on top.
	private Vector3 targetLocalPos;		// target position of the card, in local coordinate system. Animation script is Moved from Deck class to Card class.
	private Vector3 targetLocalRotation;		// target rotation of the card, in local coordinate system. Euler rotation.
	private bool movementEnabled;		//if enabled, card moves towards to the targetPosition


	public int Rank{	//c# simplified getter and setter technique.
		get{return rank;}
		//set{rank = value;}
	}
	public int Suit{
		get{return suit;}
	}
	public void initializeCard(int rank, int suit)
	{
		if (!initializeFlag)
		{
			Debug.LogError ("Card field assignment denied");
			return;
		}
		this.rank = rank;
		this.suit = suit;
		initializeFlag = false;
	}

	public void showFace()
	{
		//renderer.enabled=true;
		GetComponent<SpriteRenderer> ().sprite = getCardSprite ();
		//GetComponent<SpriteRenderer> ().color = Color.white;
	}
	
	public void showBackground()
	{
		//renderer.enabled=true;
		//GetComponent<SpriteRenderer>().color = Color.red;
		//GetComponent<SpriteRenderer> ().sprite = cardSpriteList[13];
		GetComponent<SpriteRenderer> ().sprite = Resources.Load <Sprite> ("images/playing-card-back");
	}

	public void setSortingOrder(int value)
	{
		sortingOrder = value;
		GetComponent<SpriteRenderer>().sortingOrder = sortingOrder;
	}

	public void setTargetPos(Vector3[] pos)
	{
		movementEnabled = true;
		targetLocalPos = pos[0];
		targetLocalRotation = pos [1];
	}

	private Sprite getCardSprite()	//returns card sprite based on rank and suit
	{
		return (suit<5 && suit>0 && rank<14 && rank>0)? (cardSpriteList[(rank-1)+((suit-1)*13)]) : (null);
	}



	// Use this for initialization. Function is called when the object is instantiated
	void Start () {
		if (GetComponent<SpriteRenderer> () == null)
			this.gameObject.AddComponent ("SpriteRenderer");	//Adds component to the gameObject if sprite is null
		GetComponent<SpriteRenderer> ().sprite = Resources.Load <Sprite> ("images/playing-card-back");//getCardSprite ();
		movementEnabled = true;

		//Debug.Log ("Card initialized");
	}

	// Awake is called before Start()
	void Awake(){

	}

	// Update is called once per frame. Every single cards move towards the target location every frame. This is a simplied PID control.
	// Minor bug found. LocalPosition of cards does not follow its Parent's coordinate system.
	void Update () {
		if (movementEnabled)
		{
			Vector3 deltaPos = targetLocalPos-GetComponent<Transform>().localPosition;
			GetComponent<Transform>().localPosition += movementSpeed*deltaPos*Time.deltaTime;

			transform.rotation = Quaternion.Slerp(GetComponent<Transform>().localRotation, Quaternion.Euler (targetLocalRotation),Time.deltaTime * rotationSpeed);
		}
	}
}
