using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// A standard A* pathfinding algorithm.
/// </summary>

public class AStar : FindPathBase {

	NavigationMesh navMesh;

	Node currentNode, startNode, endNode;

	/// <summary>
	/// The maximum seek time in milliseconds to prevent an infinite loop.
	/// </summary>
	[Tooltip("The maximum seek time in milliseconds")]
	public float MaximumSeekTime = 500;

	float seekTime, startTime;

	int distanceToTarget;

	int distanceFromStart;

	int totalScore = 0;

	AgentStateMachine stateMachine;

	// Use this for initialization
	protected override void Start () 
	{
		navMesh = GameManager.getCurrentNavMesh();
		stateMachine = GetComponent<AgentStateMachine>();
		Debug.Log(navMesh + ", " + stateMachine);
	}

	/// <summary>
	/// Sets the agent's destination.
	/// </summary>
	/// <param name="position">Position.</param>
	public override void SetDestination(Vector3 position)
	{

		startTime = Time.time;
		Vector3 start = transform.position;

		// I have commented out the calls to the pathfinding plugin I use.
		// I recommend running the following inside a separate thread
		Loom.RunAsync(()=> {

			findPath(start, position);

		});
	}

	/// <summary>
	/// Finds the path between the two parts.
	/// </summary>
	/// <param name="start">Start.</param>
	/// <param name="end">End.</param>
	protected override void findPath(Vector3 start, Vector3 end)
	{
		Debug.Log("Looking for a path from " + start + " to " + end);

		startNode = navMesh.getNodeFromPosition(start);
		endNode = navMesh.getNodeFromPosition(end);

		Debug.Log("The start and end nodes are " +startNode + " and " + endNode );

		totalScore = 0;
		base.openList.Clear();
		closedList.Clear();
		visitedNodes.Clear();
		path.Clear();

		currentNode = startNode;
		totalScore = calculateDistanceCost(startNode, endNode);
		base.openList.Add(currentNode, totalScore);

		while (base.openList.Count > 0)
		{
			// choose the lowest-cost node as the current Node
			currentNode = base.openList.Aggregate((prev, next)=> next.Value <= prev.Value ? next: prev ).Key;
			Debug.Log("Current node is at " + currentNode.position);

			// add exit conditions here.
			if (currentNode == endNode)
			{
				buildPathFrom(currentNode);
				break;
			}


			// get the neighbours of this node
			List<Node> currentNeighbours = navMesh.GetNeighboursOfNode(currentNode, true);
			Debug.Log(currentNeighbours);

			for (int i = 0; i < currentNeighbours.Count; i++)
			{
				Node currentNeighbour = currentNeighbours[i];
				Debug.Log("Examining the neighbour at " + currentNeighbour.position);

				// skip this neighbour if it isn't walkable or has already been considered
				if (currentNeighbour.isWalkable == false || closedList.Contains(currentNeighbour) )
					continue;

				if (base.openList.ContainsKey(currentNeighbour) == false)
				{
					int tempGScore = calculateDistanceCost(currentNode, currentNeighbour);
					int tempFScore = calculateDistanceCost(currentNeighbour, endNode);

					totalScore = tempGScore + tempFScore;

					base.openList.Add(currentNeighbour, totalScore);

					visitedNodes[currentNeighbour] = currentNode;
				}
			}

			// remove the current node from further consideration.
			base.openList.Remove(currentNode);
			closedList.Add (currentNode);


		}

	}


	/// <summary>
	/// Builds the path by tracing back along the visited nodes
	/// </summary>
	protected override void buildPathFrom(Node finalNode)
	{
		Debug.Log("Retracing the path from " + visitedNodes.Count + " nodes");
		Node thisNode = finalNode;

		while (thisNode != startNode)
		{
			Node tempNode = visitedNodes[thisNode];
			Debug.Log("Adding node at " + tempNode.position.ToString() + " to path");
			path.Add (tempNode);
			thisNode = tempNode;
		}

		Debug.Log("Built a path of " + path.Count + " noeds");

		// send the path to the external class here
		// If using a multithreading package, don't forget
		// to queue that on the main thread
		Loom.QueueOnMainThread(()=>
		{
			
		stateMachine.setPath(path);

		});
	}

	/*
	 * For each factor, create a calculation method here.
	 * Each method must be of type int, and take at least
	 * one Node as a parameter
	 */ 


	/// <summary>
	/// Calculates the distance cost between two nodes.
	/// </summary>
	/// <returns>The distance cost.</returns>
	/// <param name="firstNode">First node.</param>
	/// <param name="secondNode">Second node.</param>
	protected override int calculateDistanceCost(Node firstNode, Node secondNode)
	{
		return Mathf.RoundToInt(Vector3.Distance(firstNode.position, secondNode.position));
	}

#if UNITY_EDITOR

	// this is only called in the Editor for debugging purposes,
	// confirming that the navigation mesh has been generated.
	void OnGUI()
	{
		GUI.skin.box.wordWrap = true;

		if (navMesh != null)
		{
			GUI.Box(new Rect(20, 200, 100, 100), "Nav mesh has " + navMesh.nodes.Keys.Count + " positions" );
		}
	}
#endif
}
