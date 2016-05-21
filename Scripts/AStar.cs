using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// A standard A* pathfinding algorithm.
/// </summary>

public class AStar : FindPathBase {


	Node currentNode, startNode, endNode;

	/// <summary>
	/// The maximum seek time in milliseconds to prevent an infinite loop.
	/// </summary>
	[Tooltip("The maximum seek time in milliseconds")]
	public float MaximumSeekTime = 500;

	float seekTime, startTime;

	int totalScore = 0;

	AgentStateMachine stateMachine;

	// Use this for initialization
	protected override void Start () 
	{
		stateMachine = GetComponent<AgentStateMachine>();
		mesh.SetupDictionary();
		Debug.LogFormat("The mesh has {0} nodes", mesh.nodes.Count);
	}

	/// <summary>
	/// Sets the agent's destination.
	/// </summary>
	/// <param name="destination">Position.</param>
	public override void SetDestination(Vector3 destination)
	{

		startTime = Time.time;
		Vector3 start = transform.position;

		// I have commented out the calls to the pathfinding plugin I use.
		// I recommend running the following inside a separate thread
		Loom.RunAsync(()=> {

			findPath(start, destination);

		});
	}

	/// <summary>
	/// Finds the path between the two parts.
	/// </summary>
	/// <param name="start">Start.</param>
	/// <param name="end">End.</param>
	protected override void findPath(Vector3 start, Vector3 end)
	{
		Debug.LogFormat("Looking for a path from {0} to {1}", start, end);

		startNode = mesh.getNodeFromPosition(start);
		endNode = mesh.getNodeFromPosition(end);

		Debug.LogFormat("The start and end nodes are {0} and {1}", startNode, endNode );

		totalScore = 0;
		openList.Clear();
		closedList.Clear();
		visitedNodes.Clear();
		path.Clear();

		currentNode = startNode;
		totalScore = calculateDistanceCost(startNode, endNode);
		openList.Add(currentNode, totalScore);

		while (openList.Count > 0)
		{
			// choose the lowest-cost node as the current Node
			currentNode = openList.Aggregate((prev, next)=> next.Value <= prev.Value ? next: prev ).Key;
			Debug.LogFormat("Current node is {0}", currentNode);

			// add exit conditions here.
			if (currentNode == endNode)
			{
				buildPathFrom(currentNode);
				break;
			}


			// get the neighbours of this node
			List<Node> currentNeighbours = mesh.GetNeighboursOfNode(currentNode, true);
			Debug.Log(currentNeighbours.Count);

			for (int i = 0; i < currentNeighbours.Count; i++)
			{
				Node currentNeighbour = currentNeighbours[i];
				Debug.Log("Examining the neighbour at " + currentNeighbour.position);

				// skip this neighbour if it isn't walkable or has already been considered
				if (!currentNeighbour.isWalkable || closedList.Contains(currentNeighbour) )
				{
					continue;
				}

				if (!openList.ContainsKey(currentNeighbour))
				{
					int tempGScore = calculateDistanceCost(currentNode, currentNeighbour);
					int tempFScore = calculateDistanceCost(currentNeighbour, endNode);

					totalScore = tempGScore + tempFScore;

					openList.Add(currentNeighbour, totalScore);

					visitedNodes[currentNeighbour] = currentNode;
				}
			}

			// remove the current node from further consideration.
			openList.Remove(currentNode);
			closedList.Add (currentNode);


		}

	}


	/// <summary>
	/// Builds the path by tracing back along the visited nodes
	/// </summary>
	protected override void buildPathFrom(Node finalNode)
	{
		Debug.LogFormat("Retracing the path from {0} nodes", visitedNodes.Count);
		Node thisNode = finalNode;

		while (thisNode != startNode)
		{
			Node tempNode = visitedNodes[thisNode];
			Debug.LogFormat("Adding node at {0} to path", tempNode.position);
			path.Add (tempNode);
			thisNode = tempNode;
		}

		Debug.LogFormat("Built a path of {0} nodes", path.Count);

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

		if (mesh != null)
		{
			GUI.Box(new Rect(20, 200, 100, 100), "Nav mesh has " + mesh.nodes.Count + " positions" );
		}
	}
#endif
}
