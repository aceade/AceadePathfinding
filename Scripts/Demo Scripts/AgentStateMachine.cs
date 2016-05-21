using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// A sample finite-state machine for the agents.
/// </summary>

public class AgentStateMachine : MonoBehaviour {


	///<summary>
	/// The possible states in which an agent can be
	/// </summary>
	public enum AgentStates
	{
		moving,
		inPosition
	}

	AgentStates currentState = AgentStates.inPosition;

	List<Node> agentPath;

	int currentStep;

	[Tooltip("How far should an agent be from their next step before they change")]
	public float stepChangeDistance = 0.5f;

	// the class that handles movement
	MoveAgent moveClass;

	FindPathBase pathfindingClass;

	private Transform myTrans;
	Vector3 myPos;

	// Use this for initialization
	void Start () 
	{
		moveClass = GetComponent<MoveAgent>();
		pathfindingClass = GetComponent<FindPathBase>();
		myTrans = transform;
		myPos = myTrans.position;
	}
	
	// Update is called once per frame
	void Update () 
	{
		switch(currentState)
		{
		case AgentStates.inPosition:
			break;
		case AgentStates.moving:

			// show the path if inside the Editor
#if UNITY_EDITOR
			showPath();
#endif

			// set the move target to the next position
			Vector3 nextStep = agentPath[currentStep].position;
			moveClass.moveTarget = nextStep;
			myTrans.forward = (nextStep - myPos);

			if (Vector3.Distance(nextStep, myPos) < stepChangeDistance)
			{
				if (currentStep < agentPath.Count)
				{
					currentStep++;

					if (currentStep == agentPath.Count)
						setState(AgentStates.inPosition);
				}
			}

			break;
		}
	}

	void showPath()
	{
		Vector3 pathZero = agentPath[0].position;
		foreach (Node theNode in agentPath)
		{
			Debug.DrawLine(pathZero, theNode.position);
			pathZero = theNode.position;
		}
	}

	/// <summary>
	/// Sets the agent's path path.
	/// </summary>
	/// <param name="newPath">New path.</param>
	public void setPath(List<Node> newPath)
	{
		if (newPath.Count > 0)
		{
			agentPath = newPath;
			setState(AgentStates.moving);
		}

	}

	public void SetDestination(Vector3 position)
	{
		pathfindingClass.SetDestination(position);
	}

	/// <summary>
	/// Changes the agent's state.
	/// </summary>
	/// <param name="newState">New state.</param>
	public void setState(AgentStates newState)
	{
		currentState = newState;

		if (newState == AgentStates.moving)
			moveClass.canMove = true;

		if (newState == AgentStates.inPosition)
			moveClass.canMove = false;
	}
}
