using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// A standard A* pathfinding algorithm.
/// </summary>

public class AStar : MonoBehaviour {

	NavigationMesh navMesh;

	Node currentNode, startNode, endNode;


	[Tooltip("The maximum seek time in milliseconds")]
	public float MaximumSeekTime = 500;

	float seekTime, startTime;

	int distanceToTarget;

	int distanceFromStart;

	int totalScore = 0;

	Dictionary<Node, int> openList = new Dictionary<Node, int>();

	Dictionary<Node, Node>closedList = new Dictionary<Node, Node>();

	List<Node> path = new List<Node>();

	// Use this for initialization
	void Start () 
	{
		navMesh = GameManager.currentNavMesh;
	}

	/// <summary>
	/// Sets the agent's destination.
	/// </summary>
	/// <param name="position">Position.</param>
	public void SetDestination(Vector3 position)
	{

		startTime = Time.time;

		// I have commented out the calls to the pathfinding plugin I use.
		// I recommend running the following inside a separate thread
//		Loom.RunAsync(()=> {

			findPath(transform.position, position);

//		});
	}

	void findPath(Vector3 start, Vector3 end)
	{
		startNode = navMesh.getNodeFromPosition(start);
		endNode = navMesh.getNodeFromPosition(end);

		totalScore = 0;
		openList.Clear();
		closedList.Clear();

		currentNode = startNode;
		totalScore = calculateDistanceCost(startNode, endNode);
		openList.Add(currentNode, totalScore);

		while (openList.Count > 0)
		{
			if ((Time.time - startTime) > MaximumSeekTime || currentNode == endNode)
			{
				buildPath();
				break;
			}

			currentNode = openList.Aggregate((prev, next)=> next.Value <= prev.Value ? next: prev ).Key;
			List<Node> currentNeighbours = currentNode.neighbours;

			for (int i = 0; i < currentNeighbours.Count; i++)
			{
				Node currentNeighbour = currentNeighbours[i];
				if (currentNeighbour.isWalkable == false || closedList.ContainsKey(currentNeighbour) )
					continue;

				if (openList.ContainsKey(currentNeighbour) == false)
				{
					int tempGScore = calculateDistanceCost(currentNode, currentNeighbour);
					int tempFScore = calculateDistanceCost(currentNeighbour, endNode);

					openList.Add(currentNeighbour, tempGScore + tempFScore);
				}
			}


		}

	}

	void buildPath()
	{

//		Loom.QueueOnMainThread(()=>
//		{
			// send the path to the external class here
			// If using a multithreading package, don't forget
			// to queue that on the main thread

//		});
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
	int calculateDistanceCost(Node firstNode, Node secondNode)
	{
		return Mathf.RoundToInt(Vector3.Distance(firstNode.position, secondNode.position));
	}

	// at the moment, this is just for debugging, confirming that the mesh has been generated
	void OnGUI()
	{
		GUI.skin.box.wordWrap = true;

		if (navMesh != null)
		{
			GUI.Box(new Rect(20, 200, 100, 100), "Nav mesh has " + navMesh.nodes.Keys.Count + " positions" );
		}
	}
}
