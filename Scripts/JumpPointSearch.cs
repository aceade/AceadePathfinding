using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Finds a path using Jump Point Search.
/// </summary>

public class JumpPointSearch : FindPathBase {


	Node currentNode, startNode, endNode, previousNode;

	List<Node> jumpPoints = new List<Node>();

	protected override void Start()
	{
		mesh = GameManager.getCurrentNavMesh();
	}

	public override void SetDestination(Vector3 position)
	{

		findPath(transform.position, position);
	}

	protected override void findPath(Vector3 start, Vector3 end)
	{
		startNode = mesh.getNodeFromPosition(start);
		endNode = mesh.getNodeFromPosition(end);
		previousNode = startNode;
		currentNode = startNode;
		jumpPoints.Clear();

		Loom.RunAsync(()=>
		{
			while (currentNode != endNode)
			{
				jumpPoints = findSuccessorsOf(currentNode);
			}
			buildPathFrom(currentNode);
		});

	}

	protected override void buildPathFrom(Node finalNode)
	{
		Debug.Log(string.Format("BUilding back from end node at {0}", finalNode.position));
	}

	protected override int calculateDistanceCost(Node firstNode, Node secondNode)
	{
		return Mathf.RoundToInt(Vector3.Distance(firstNode.position, secondNode.position));
	}

	/// <summary>
	/// Finds the neighbours of the specified node.
	/// </summary>
	/// <returns>The neighbours.</returns>
	/// <param name="theNode">The node.</param>
	List<Node> findNeighbours(Node theNode)
	{
		Debug.Log(string.Format("Finding the neighbours of node at {0}", theNode.ToString()));
		List<Node> neighbours = mesh.GetNeighboursOfNode(theNode, false);
		List<Node> pruned = new List<Node>();

		for (int i = 0; i < neighbours.Count; i++)
		{
			Node neighbour = neighbours[i];
			Vector3 dir = ((Vector3)neighbour.position - (Vector3)theNode.position);
			if (Mathf.Abs(dir.x) > 0 && Mathf.Abs(dir.z) > 0)
			{
				int tempCost = calculateDistanceCost(startNode, theNode) + calculateDistanceCost(theNode, endNode);
				int pathCost = calculateDistanceCost(startNode, theNode) + calculateDistanceCost(theNode, neighbour)
					+ calculateDistanceCost(neighbour, endNode);
				if (tempCost <= pathCost && neighbour.isWalkable == true)
				{
					Debug.Log(string.Format("The node at {0} is not a natural neighbour of the node at {1}", 
					                        neighbour.position, theNode.position));
					pruned.Add (neighbour);
				}
			}
			else
			{

				// if the length of a path (theNode -> end)
				// is <= length of a path (theNode -> neighbour -> endpoint)
				// remove it
				int tempCost = calculateDistanceCost(startNode, theNode) + calculateDistanceCost(theNode, endNode);
				int pathCost = calculateDistanceCost(startNode, theNode) + calculateDistanceCost(theNode, neighbour)
					+ calculateDistanceCost(neighbour, endNode);
				if (tempCost < pathCost && neighbour.isWalkable == true)
				{
					Debug.Log(string.Format("The node at {0} is not a natural neighbour of the node at {1}", 
					                        neighbour.position, theNode.position));
					pruned.Add (neighbour);
				}
			}
		}

		neighbours.RemoveAll(d=> pruned.Contains(d));

		return neighbours;
	}


	List<Node> findSuccessorsOf(Node thisNode)
	{
		List<Node> successors = new List<Node>();
		List<Node> neighbours = findNeighbours(thisNode);
		Vector3 thisPos = thisNode.position;
		for (int i = 0; i < neighbours.Count; i++)
		{
			Vector3 neighbourPos = neighbours[i].position;
			Node jumpPoint = jump (neighbours[i], neighbourPos - thisPos);

			if (jumpPoint != null)
			{
				successors.Add (jumpPoint);
			}

		}
		return successors;
	}

	Node jump(Node theNode, Vector3 direction)
	{
		Vector3 position = theNode.position + direction;
		Node newNode = mesh.getNodeFromPosition(position);

		if (newNode == null || newNode.isWalkable == false)
		{
			return null;
		}

		if (newNode == endNode)
		{
			Debug.Log("Found the end node");
			return newNode;
		}

		return (jump (newNode, (Vector3) newNode.position - (Vector3) theNode.position));

	}

}
