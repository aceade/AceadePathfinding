using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// A pathfinding Node is a position with additional attributes.
/// </summary>

[System.Serializable]
public class Node 
{
	public SerializableVector3 position;

	// apparently, this exceeds the serialisation limit
	[System.NonSerialized]
	public List<Node> neighbours;

	public float height;

	public float illumination;

	public bool isWalkable;

}