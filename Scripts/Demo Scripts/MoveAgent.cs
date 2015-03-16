using UnityEngine;
using System.Collections;

/// <summary>
/// Moves an agent by setting the velocity of the Rigidbody.
/// </summary>

public class MoveAgent : MonoBehaviour {

	/// <summary>
	/// Determines if the agent will move or not
	/// </summary>
	[Tooltip("Whether or not the agent can move")]
	public bool canMove = false;


	/// <summary>
	/// The agent's speed.
	/// </summary>
	[Tooltip("The agent's movement speed")]
	public float speed = 1f;

	/// <summary>
	/// The agent's move target is their next step.
	/// </summary>
	[HideInInspector]
	public Vector3 moveTarget;

	// caching the agent's Rigidbody component
	Rigidbody myBody;

	// cashing the agent's Transform
	Transform myTrans;

	// caching the agent's current position
	Vector3 myPos;

	// cache the private variables on startup
	void Start () 
	{
		myTrans = transform;
		myPos = myTrans.position;
		myBody = GetComponent<Rigidbody>();
		moveTarget = myPos;
	}

	// FixedUpdate is called on the physics timestep
	void FixedUpdate () 
	{
		myPos = myTrans.position;

		// if the agent can move, create a movement vector and multiply that by the speed
		if (canMove)
		{
			myBody.velocity = (moveTarget - myPos).normalized * speed;
		}

	}
}
