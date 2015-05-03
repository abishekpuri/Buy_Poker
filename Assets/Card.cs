using UnityEngine;
using System.Collections;

public class Card : MonoBehaviour {
	static public Sprite[] cardSpriteList;// = Resources.LoadAll <Sprite> ("images/cards");	//preLoaded array of sprites from Assets/Resources/images folder
	const float rotationSpeed=3f;
	const float movementSpeed = 3f;

	// rank and suit of the card. Each Rank and suit maps to diffferent sprite resources.
	private int rank;					// 1 to 13, 1 is Ace
	private int suit;					// 1 to 4, Clover, Heart, Spade and Diamond respectively.

	// card sprite and generic-animation related.
	private bool isOpen;				// whether card is open or closed
	private Animator anim;				// for setting transitions for any card animation
	private int sortingOrder;			// order in 2D graphics layer. Sprite with higher order is drawn on top.

	// Position and movement control. Card can also be treated as 3D particle.
	private Vector3 targetLocalPos;		// target position of the card, in local coordinate system. Animation script is Moved from Deck class to Card class.
	private Vector3 targetLocalRotation;// target rotation of the card, in local coordinate system. Euler rotation.
	private bool movementEnabled;		//if enabled, card smoothly moves towards to the targetPosition

	// System variable, mainly for protection
	private bool initializeFlag = true;	// once the values are initialized, flag is set to false and rejects any re-initialization.

	//===================================== Getter and Setter functions =====================================
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



	//===================================== Sprite rendering and animation control =====================================
	private Sprite getCardSprite()	//returns card sprite, mapped by rank and suit
	{
		return (suit<5 && suit>0 && rank<14 && rank>0)? (cardSpriteList[(rank-1)+((suit-1)*13)]) : (null);
	}

	public void startBlinkAnim()
	{
		if (isOpen) {

			anim.SetBool ("blinkEnabled", true);
			//if (anim.animation!=null)
			//	anim.animation.Rewind();// no idea how to reset animation
		}
	}

	public void stopBlinkAnim()
	{
		anim.SetBool ("blinkEnabled", false);
	}

	public void showFace()
	{
		isOpen = true;
		//renderer.enabled=true;
		GetComponent<SpriteRenderer> ().sprite = getCardSprite ();
		//GetComponent<SpriteRenderer> ().color = Color.white;
	}
	
	public void showBackground()
	{
		isOpen = false;
		//renderer.enabled=true;
		//GetComponent<SpriteRenderer>().color = Color.red;
		//GetComponent<SpriteRenderer> ().sprite = cardSpriteList[13];
		GetComponent<SpriteRenderer> ().sprite = Resources.Load <Sprite> ("images/playing-card-back");
		//anim.SetBool ("blinkEnabled", false);
	}

	public void setSortingOrder(int value)
	{
		sortingOrder = value;
		GetComponent<SpriteRenderer>().sortingOrder = sortingOrder;
	}



	//===================================== Movement control and position tracking =====================================
	public void setTargetPos(Vector3[] pos)
	{
		movementEnabled = true;
		targetLocalPos = pos[0];
		targetLocalRotation = pos [1];
	}


	//===================================== Override Monobehavior. Update() and Start() =====================================
	// Awake is called before Start()
	void Awake(){
		if (GetComponent<SpriteRenderer> () == null)
			this.gameObject.AddComponent ("SpriteRenderer");	//Adds component to the gameObject if sprite is null
		GetComponent<SpriteRenderer> ().sprite = Resources.Load <Sprite> ("images/playing-card-back");//getCardSprite ();
		anim = GetComponent<Animator>();
		isOpen = false;
		movementEnabled = true;
	}

	// Use this for initialization. Function is called when the object is instantiated
	void Start () {
		
		//Debug.Log ("Card initialized");
	}
	// Update is called once per frame. Every single cards move towards the target location every frame. This is a simplied PID control.
	// Minor bug found. Local coordinate system of cards does not follow its Parent's rotation
	void Update () {
		if (movementEnabled)
		{
			Vector3 deltaPos = targetLocalPos-GetComponent<Transform>().localPosition;
			GetComponent<Transform>().localPosition += movementSpeed*deltaPos*Time.deltaTime;

			transform.rotation = Quaternion.Slerp(GetComponent<Transform>().localRotation, Quaternion.Euler (targetLocalRotation),Time.deltaTime * rotationSpeed);
		}
	}
}
