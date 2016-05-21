using System.Collections.Generic;

/// <summary>
/// A pathfinding Node is a position with additional attributes.
/// </summary>

[System.Serializable]
public class Node 
{
	public SerializableVector3 position;

	public float height;

	public float illumination;

	public bool isWalkable;

	public string ToString()
	{
		return string.Format("Node at {0} has illumination {1} and {2} walkable", position, illumination, isWalkable? "is":"is not");
	}
}