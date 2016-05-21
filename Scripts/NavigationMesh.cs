using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// The NavigationMesh holds a Dictionary of Nodes,
/// hashing on a 3D position.
/// </summary>


[System.Serializable]
public class NavigationMesh : ScriptableObject
{
	[HideInInspector]
	public Dictionary<SerializableVector3, Node> nodes = new Dictionary<SerializableVector3, Node>();

	public List<SerializableVector3> keys;
	public List<Node> values;

	[HideInInspector]
	public Dictionary<Node, List<Node>> neighboursDict = new Dictionary<Node, List<Node>>();

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
		Debug.LogFormat("Looking for a node near {0}", requestPos);
		Node theNode;

		if (nodes.ContainsKey(requestPos))
		{
			theNode = nodes[requestPos];
		}
		else
		{
//			Debug.Log("Need a full check");
			theNode = nodes.Values.Aggregate((d, n) => Vector3.Distance(d.position, requestPos) < 
				Vector3.Distance(n.position, requestPos) ? d:n);
			Debug.LogFormat("A full check gave a Node of {0}", theNode);
		}

		return theNode;
	}

	/// <summary>
	/// Sets the neighbours.
	/// </summary>
	/// <param name="newDict">New dict.</param>
	public void SetNeighbours(Dictionary<Node, List<Node>> newDict)
	{
		neighboursDict = newDict;
	}
	
	/// <summary>
	/// Gets the neighbours of node. Due to serialisation issues, the list of neighbours
	/// cannot be stored in the Node class, so this is a work around
	/// </summary>
	/// <returns>The neighbours of node.</returns>
	/// <param name="theNode">The node.</param>
	/// <param name="ignoreUnwalkableNodes">If set to <c>true</c>, unwalkable Nodes will be ignored.</param>
	public List<Node> GetNeighboursOfNode(Node theNode, bool ignoreUnwalkableNodes)
	{
		List<Node> neighbours = neighboursDict[theNode];
		
		if (ignoreUnwalkableNodes)
		{
			neighbours.RemoveAll(n=> !n.isWalkable);
		}
		
		return neighbours;
	}


	/// <summary>
	/// Adds a node to the mesh at a specified position.
	/// </summary>
	/// <param name="nodePosition">Position of the Node.</param>
	/// <param name="theNode">The node.</param>
	public void AddNode(SerializableVector3 nodePosition, Node theNode)
	{
		nodes.Add (nodePosition, theNode);
	}

	/// <summary>
	/// Stores the dictionary. Unity doesn't actually serialise them, so this is a workaround.
	/// </summary>
	public void StoreDictionary()
	{
		keys.Clear();
		keys.AddRange(nodes.Keys);
		values.Clear();
		values.AddRange(nodes.Values);
	}

	public void SetupDictionary()
	{
		if (nodes.Count == 0)
		{
			nodes = Enumerable.Range(0, keys.Count).ToDictionary(i => keys[i], i => values[i]);
		}
		if (neighboursDict.Count == 0)
		{
			for (int j = 0; j < values.Count; j++)
			{
				List<Node> neighbours = GetNeighbours(values[j], false);
				neighboursDict.Add(values[j], neighbours);
			}
		}

	}

	List<Node> GetNeighbours(Node theNode, bool removeWalkableNodes)
	{
		List<Node> neighbours = values.Where(d=> Vector3.Distance(d.position, theNode.position) < cellSize * 1.5f).ToList();
		if (removeWalkableNodes)
		{
			neighbours.RemoveAll(d=> !d.isWalkable);
		}
		return neighbours;
	}
}
