using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// The NavigationMesh holds a Dictionary of Nodes,
/// hashing on a 3D position.
/// </summary>

[System.Serializable]
public class NavigationMesh
{
	[HideInInspector]
	public Dictionary<SerializableVector3, Node> nodes = new Dictionary<SerializableVector3, Node>();

	/// <summary>
	/// The name of the mesh corresponds to the level name
	/// </summary>
	public string meshName;

	public float maxCellHeight;

	public float minCellHeight;

	public SerializableVector3 origin;

	public float cellSize;


	/// <summary>
	/// Finds the node that is closest to a position.
	/// </summary>
	/// <returns>The node.</returns>
	/// <param name="requestPos">Requested position.</param>
	public Node getNodeFromPosition(SerializableVector3 requestPos)
	{
		Node theNode;

		if (nodes.ContainsKey(requestPos))
			theNode = nodes[requestPos];
		else
		{
//			Debug.Log("Need a full check");
			theNode = nodes.Values.Where(d=> 
			                      (Mathf.Abs(d.position.x - requestPos.x) < cellSize)
			                      && (Mathf.Abs(d.position.y - requestPos.y) <= maxCellHeight)
			                      && (Mathf.Abs(d.position.z - requestPos.z) < cellSize) ) .FirstOrDefault();
		}

		return theNode;
	}

	/// <summary>
	/// Gets the neighbours of node. Due to serialisation issues, the list of neighbours
	/// cannot be stored in the Node class, so this is a work around
	/// </summary>
	/// <returns>The neighbours of node.</returns>
	/// <param name="theNode">The node.</param>
	public List<Node> GetNeighboursOfNode(Node theNode)
	{
		List<Node> neighbours = nodes.Values.Where(d=> Mathf.Abs(d.position.x - theNode.position.x) <= cellSize
		                                                   &&  Mathf.Abs(d.position.y - theNode.position.y) <= maxCellHeight
		                                                   &&  Mathf.Abs(d.position.z - theNode.position.z) <= cellSize ).ToList();
		
		neighbours.Remove(theNode);

		return neighbours;
	}


	/// <summary>
	/// Adds a node to the mesh at a specified position.
	/// </summary>
	/// <param name="position">Position.</param>
	/// <param name="theNode">The node.</param>
	public void AddNode(SerializableVector3 nodePosition, Node theNode)
	{
		nodes.Add (nodePosition, theNode);
	}

	/// <summary>
	/// Sets the name of the mesh.
	/// </summary>
	/// <param name="name">Name. Must match the name of the level, and end in " NavMesh" 
	/// (note the space)</param>
	public void SetName(string name)
	{
		int index = name.IndexOf(" NavMesh");
		meshName = name.Remove(index);
		Debug.Log("The name of the mesh is (" + meshName + ")");
	}

}
