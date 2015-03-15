using UnityEngine;
using System.Collections;

public class MoveAgent : MonoBehaviour {

	[Tooltip("Whether or not the agent can move")]
	public bool canMove = false;

	[Tooltip("The agent's movement speed")]
	public float speed = 1f;

	/// <summary>
	/// The agent's move target is their next step.
	/// </summary>
	[HideInInspector]
	public Vector3 moveTarget;

	Rigidbody myBody;

	Transform myTrans;



	Vector3 myPos;

	// Use this for initialization
	void Start () 
	{
		myTrans = transform;
		myPos = myTrans.position;
		myBody = GetComponent<Rigidbody>();
		moveTarget = myPos;
	}

	void FixedUpdate () 
	{
		myPos = myTrans.position;

		if (canMove)
		{
			myBody.velocity = (myPos - moveTarget).normalized * speed;
		}

	}
}
