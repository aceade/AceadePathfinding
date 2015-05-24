using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// An abstract base class for finding paths.
/// </summary>

public abstract class FindPathBase : MonoBehaviour {

	protected NavigationMesh mesh;

	protected Dictionary<Node, int> openList = new Dictionary<Node, int>();
	
	protected List<Node> closedList = new List<Node>();
	
	protected Dictionary<Node, Node>visitedNodes = new Dictionary<Node, Node>();
	
	protected List<Node> path = new List<Node>();

	public abstract void SetDestination(Vector3 destination);	

	protected abstract void findPath(Vector3 start, Vector3 end);

	protected abstract void buildPathFrom(Node finalNode);

	protected virtual void Start()
	{
		mesh = GameManager.getCurrentNavMesh();
	}

	protected abstract int calculateDistanceCost(Node nodeOne, Node nodeTwo);
	
}
