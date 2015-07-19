using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Finds a path using Jump Point Search.
/// </summary>

public class JumpPointSearch : FindPathBase {


	Node currentNode, startNode, endNode, previousNode;

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


	}

	protected override void buildPathFrom(Node finalNode)
	{
	}

	protected override int calculateDistanceCost(Node firstNode, Node secondNode)
	{
		return Mathf.RoundToInt(Vector3.Distance(firstNode.position, secondNode.position));
	}

	List<Node> findNeighbours(Node theNode)
	{
		List<Node> neighbours = mesh.GetNeighboursOfNode(theNode, false);

		for (int i = 0; i < neighbours.Count; i++)
		{
			Vector3 dir = ((Vector3)neighbours[i].position - (Vector3)theNode.position);
			if (Mathf.Abs(dir.x) > 0 && Mathf.Abs(dir.z) > 0)
			{
				Debug.Log("THis is a diagonal node");
			}
			else
			{
				Debug.Log("This is not a diagonal node");
			}
		}

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
			return newNode;
		}



		return (jump (newNode, (Vector3) newNode.position - (Vector3) theNode.position));

	}

}
