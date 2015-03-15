using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AgentStateMachine : MonoBehaviour {



	public enum AgentStates
	{
		moving,
		inPosition
	}

	AgentStates currentState = AgentStates.inPosition;

	List<Node> agentPath;

	int currentStep;

	public float stepChangeDistance = 0.5f;

	MoveAgent moveClass;

	private Transform myTrans;
	Vector3 myPos;

	// Use this for initialization
	void Start () 
	{
		moveClass = GetComponent<MoveAgent>();
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

			// set the move target to the next position
			Vector3 nextStep = agentPath[currentStep].position;
			moveClass.moveTarget = nextStep;
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
