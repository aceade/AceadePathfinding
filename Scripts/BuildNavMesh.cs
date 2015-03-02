using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Build nav mesh.
/// 
/// To use this, attach to a cube in the scene with the following format:
/// 
/// LevelName NavMesh
/// </summary>

public class BuildNavMesh : MonoBehaviour {


	private Vector3 topLeftCorner;

	private Bounds cubeBounds;

	public float cellSize = 1f;

	private float width, length;

	public float minCellHeight = 1f;

	public float maxCellHeight = 3f;
	
	private RaycastHit hit;
	
	public List<int> walkableLayers;

	public bool clampIllumination = true;

	NavigationMesh theMesh;

	/// <summary>
	/// Builds the mesh.
	/// </summary>
	public void BuildMesh()
	{
		cubeBounds = GetComponent<Collider>().bounds;
		topLeftCorner = cubeBounds.center - cubeBounds.extents
			+ (Vector3.up * cubeBounds.size.y);

		theMesh = new NavigationMesh();
		theMesh.SetName(gameObject.name);

		width = cubeBounds.size.x / cellSize;
		length = cubeBounds.size.z / cellSize;

		Vector3 tempPos = topLeftCorner;

		for (float x = topLeftCorner.x; x <= width; x += cellSize)
		{
			for (float z = topLeftCorner.z; z <= length; z += cellSize)
			{
				tempPos.x = x + cellSize/2;
				tempPos.z = z + cellSize/2;

				if (Physics.Raycast(tempPos, Vector3.down, out hit, cubeBounds.size.y) )
				{
					Vector3 hitPoint = hit.point;
					SerializableVector3 newPos = new SerializableVector3(hitPoint.x, hitPoint.y, hitPoint.z);
					Node newNode = new Node();
					newNode.position = newPos;
					theMesh.AddNode(newPos, newNode);

//					Debug.Log("New node at " +hitPoint);

					if (walkableLayers.Contains(hit.collider.gameObject.layer) )
						newNode.isWalkable = true;
					else
						newNode.isWalkable = false;

					newNode.height = Mathf.Clamp(topLeftCorner.y - hitPoint.y, minCellHeight, maxCellHeight);

					Bounds hitBounds = hit.collider.bounds;
					if ( topLeftCorner.y - (hitBounds.size.y + hitPoint.y) > minCellHeight )
					{
						hitPoint.y -= hitBounds.size.z;
						if (Physics.Raycast(hitPoint, Vector3.down, out hit))
						{
							Debug.Log("New node under an obstacle at " + hit.point);
							Node underNode = new Node();
							newPos = new SerializableVector3(hitPoint.x, hitPoint.y, hitPoint.z);
							underNode.position = newPos;
							theMesh.AddNode (newPos, underNode);

							underNode.height = Mathf.Clamp(hitPoint.y - hit.point.y, minCellHeight, maxCellHeight);

							if (walkableLayers.Contains( hit.collider.gameObject.layer) )
							{
								underNode.isWalkable = true;
							}
							else
								underNode.isWalkable = false;
						}

					}
				}
			}
		}

		CalculateIllumination();

		foreach (Node node in theMesh.nodes.Values)
		{
			FindNeighbours(node);
		}
		// add this to the list of meshes
		GameManager.AddNavMesh(theMesh);

	}

	/// <summary>
	/// Finds the neighbours of the specified node.
	/// </summary>
	/// <param name="theNode">The node.</param>
	void FindNeighbours(Node theNode)
	{
		List<Node> neighbours = theMesh.nodes.Values.Where(d=> Mathf.Abs(d.position.x - theNode.position.x) <= cellSize
		                                                   &&  Mathf.Abs(d.position.y - theNode.position.y) <= maxCellHeight
		                                                   &&  Mathf.Abs(d.position.z - theNode.position.z) <= cellSize ).ToList();

		neighbours.Remove(theNode);
		theNode.neighbours = neighbours;
	}

	/// <summary>
	/// Calculates the illumination at each node.
	/// 
	/// For the purposes of this, it is assumed that
	/// only Point Lights are used.
	/// </summary>
	void CalculateIllumination()
	{
		Debug.Log("Is illumination clamped? " + clampIllumination);
		List<Light> lights = GameObject.FindObjectsOfType<Light>().Where(l=> (l.type == LightType.Point)).ToList();

		for (int i = 0; i < lights.Count; i++)
		{
			Light theLight = lights[i];
			Node theLightNode = theMesh.getNodeFromPosition(theLight.transform.position);
			List<Node> affectedNodes = theMesh.nodes.Values.Where(d=> Mathf.Abs(d.position.x - theLightNode.position.x) < theLight.range
			                                               && Mathf.Abs(d.position.z - theLightNode.position.z) < theLight.range ).ToList();

			// loop through nodes affected by this light
			for (int j = 0; j < affectedNodes.Count; j++)
			{

				// calculate the intensity from this light, and add it to the illumination of the node.
				float illumination = theLight.intensity / 
					Vector3.Distance(affectedNodes[j].position, theLightNode.position);
				affectedNodes[j].illumination += illumination;

				// Optionally, clamp it to the range [0,1]
//				if (clampIllumination == true)
					affectedNodes[j].illumination = Mathf.Clamp(affectedNodes[j].illumination, 0f, 1f);

//				Debug.Log("The illumination at " + affectedNodes[j].position.ToString() + " is " 
//				          + affectedNodes[j].illumination);
			}
		}
	}

	void OnDrawGizmosSelected()
	{
		if (theMesh == null ) 
			return;

		// each node on the map
		foreach (SerializableVector3 position in theMesh.nodes.Keys)
		{
			Node theNode = theMesh.getNodeFromPosition(position);
			Gizmos.DrawCube(position, Vector3.one * 0.75f);
			if (theNode.isWalkable == true)
				Gizmos.color = Color.green;
			else
				Gizmos.color = Color.red;
		}
	}


	public void ToggleClamping()
	{
		clampIllumination = !clampIllumination;
		Debug.Log("Clamping Illumination: " + clampIllumination);
	}
}
