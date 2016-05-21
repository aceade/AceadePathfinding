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


	private Vector3 bottomLeftCorner;

	Bounds cubeBounds;

	public float cellSize = 1f;

	float width, length;

	public float minCellHeight = 1f;

	public float maxCellHeight = 3f;
	
	RaycastHit hit;
	
	public List<int> walkableLayers;

	public bool clampIllumination = false;

	public GameObject[] ignoredObjects;

	Dictionary<Node, List<Node>> neighboursOfNodes = new Dictionary<Node, List<Node>>();
	NavigationMesh theMesh;

	/// <summary>
	/// Builds the mesh, using the full specified values.
	/// </summary>
	/// <param name = "mesh"></param>
	/// <param name="newCellSize">New cell size.</param>
	/// <param name="willClampIllumination">If set to <c>true</c> will clamp illumination.</param>
	/// <param name="maxLightIntensity">Max light intensity.</param>
	public void BuildMesh (NavigationMesh mesh, float newCellSize, bool willClampIllumination, float maxLightIntensity, float newMinCellHeight, float newMaxCellHeight)
	{
		ignoredObjects = GameObject.FindGameObjectsWithTag("NavMeshIgnore").ToArray();
		EnableUnits (false);

		theMesh = mesh;
		theMesh.nodes = new Dictionary<SerializableVector3, Node>();
		theMesh.neighboursDict = new Dictionary<Node, List<Node>>();
		theMesh.keys = new List<SerializableVector3>();
		theMesh.values = new List<Node>();

		neighboursOfNodes.Clear();
		cellSize = newCellSize;
		clampIllumination = willClampIllumination;
		minCellHeight = newMinCellHeight;
		maxCellHeight = newMaxCellHeight;
		theMesh.minCellHeight = minCellHeight;
		theMesh.maxCellHeight = maxCellHeight;
		theMesh.cellSize = cellSize;

		cubeBounds = GetComponent<Collider> ().bounds;
		bottomLeftCorner = cubeBounds.center - cubeBounds.extents
			+ (Vector3.up * cubeBounds.size.y);

		Debug.Log ("The bottom left corner is " + bottomLeftCorner);

		width = cubeBounds.size.x / cellSize;
		length = cubeBounds.size.z / cellSize;

		Vector3 tempPos = bottomLeftCorner;

		for (float x = bottomLeftCorner.x; x <= width; x += cellSize) {
			for (float z = bottomLeftCorner.z; z <= length; z += cellSize) {
				tempPos.x = x + cellSize / 2;
				tempPos.z = z + cellSize / 2;

				if (Physics.Raycast (tempPos, Vector3.down, out hit, cubeBounds.size.y)) {
					Vector3 hitPoint = hit.point;
					var newPos = new SerializableVector3 (hitPoint.x, hitPoint.y, hitPoint.z);
					var newNode = new Node ();
					newNode.position = newPos;
					theMesh.AddNode (newPos, newNode);

					Debug.LogFormat("New node at {0}", newPos);

					if (walkableLayers.Contains (hit.collider.gameObject.layer))
					{
						newNode.isWalkable = true;
					}
					else
					{
						newNode.isWalkable = false;
					}

					newNode.height = Mathf.Clamp (bottomLeftCorner.y - hitPoint.y, minCellHeight, maxCellHeight);

					Bounds hitBounds = hit.collider.bounds;
//					Debug.Log(hitBounds.size.y);
					if (bottomLeftCorner.y - (hitBounds.extents.y + hitPoint.y) >= minCellHeight) {

						Vector3 underPos = hitPoint + (Vector3.down * hitBounds.size.y);

						if (Physics.Raycast (underPos, Vector3.down, out hit)) {
//							Debug.Log("New node under an obstacle at " + hit.point);
							var underNode = new Node ();
							newPos = new SerializableVector3 (hit.point.x, hit.point.y, hit.point.z);
							underNode.position = newPos;
							theMesh.AddNode (newPos, underNode);

							underNode.height = Mathf.Clamp (hitPoint.y - hit.point.y, minCellHeight, maxCellHeight);

							if (walkableLayers.Contains (hit.collider.gameObject.layer)) 
							{
								underNode.isWalkable = true;
							} 
							else 
							{
								underNode.isWalkable = false;
							}
						}

					}
				}
			}
		}

		// post processing steps
		RemoveUselessNodes();
		GatherNeighbours();
		CalculateIllumination(maxLightIntensity);
		FixEdges();

		EnableUnits(true);
		theMesh.StoreDictionary();

	}

	/// <summary>
	/// Gathers the neighbours of each node.
	/// </summary
	void GatherNeighbours()
	{
		foreach(Node node in theMesh.nodes.Values)
		{
			List<Node> neighbours = FindNeighbours(node, cellSize * 1.5f);
			neighboursOfNodes.Add (node, neighbours);
		}
		theMesh.SetNeighbours(neighboursOfNodes);
	}
	
	/// <summary>
	/// Finds the neighbours of the specified node.
	/// </summary>
	/// <param name="theNode">The node.</param>
	/// <param name="distance">The maximum search distance.</param>
	List<Node> FindNeighbours(Node theNode, float distance)
	{
		List<Node> neighbours = theMesh.nodes.Values.Where(d=> Vector3.Distance(theNode.position, d.position) <= distance ).ToList();
		neighbours.Remove(theNode);
		return neighbours;
	}
	
	/// <summary>
	/// Fixs the edges.
	/// </summary>
	void FixEdges()
	{
		Debug.Log("Fixing edges around obstacles");
		List<Node> affectedNodes = neighboursOfNodes.Keys.Where(d=> neighboursOfNodes[d].Count() < 8).ToList();
		
		Debug.Log("There are " + affectedNodes.Count + " nodes beside an obstacle. Total nodes in map: "+ theMesh.nodes.Count);
		for (int i = 0; i < affectedNodes.Count; i++)
		{
			affectedNodes[i].isWalkable = false;
		}
	}
	
	/// <summary>
	/// Removes useless and inaccessible nodes.
	/// </summary>
	void RemoveUselessNodes()
	{
		Debug.Log("Removing useless nodes from the map");
		var uselessNodes = new List<Node>();
		Node theNode;
		foreach (KeyValuePair<SerializableVector3, Node> pair in theMesh.nodes)
		{
			theNode = pair.Value;
			List<Node> longNeighbours = FindNeighbours(theNode, cellSize * 3f);
			
			if (longNeighbours.Count(d=> d.isWalkable) == 0)
			{
				uselessNodes.Add(theNode);
			}
			
		}
		
		Debug.Log(uselessNodes.Count + " useless nodes in the map");
		
		for(int i = 0; i < uselessNodes.Count; i++)
		{
			neighboursOfNodes.Remove(uselessNodes[i]);
			theMesh.nodes.Remove(uselessNodes[i].position);
		}
	}

	/// <summary>
	/// Calculates the illumination at each node.
	/// 
	/// For the purposes of this, it is assumed that
	/// only Point Lights are used.
	/// </summary>
	void CalculateIllumination(float upperLightIntensity)
	{
//		Debug.Log("Is illumination clamped? " + clampIllumination);
		List<Light> lights = GameObject.FindObjectsOfType<Light>().Where(l=> (l.type == LightType.Point)).ToList();

		for (int i = 0; i < lights.Count; i++)
		{
			Light theLight = lights[i];
			Node theLightNode = theMesh.getNodeFromPosition(theLight.transform.position);
			List<Node> neighbours = neighboursOfNodes[theLightNode];
			var affectedNodes = new List<Node>();

			// loop through nodes affected by this light
			for (int j = 0; j < neighbours.Count; j++)
			{

				// calculate the direction of the light, then find all nodes along it
				Vector3 dir = (Vector3)neighbours[i].position - (Vector3)theLightNode.position;
				Physics.Raycast(theLight.transform.position, dir, out hit, theLight.range);
				Node endNode = theMesh.getNodeFromPosition(hit.point);

				List<Node> tempNodes = theMesh.nodes.Values.Where(d=> ((Vector3)d.position - (Vector3)theLightNode.position).normalized == dir.normalized).ToList();
				affectedNodes.AddRange (tempNodes);
			}

			Debug.Log("The light at " + theLight.transform.position + " affects " + affectedNodes.Count + " nodes");

			// calculate the illumination at those nodes
			for (int k = 0; k < affectedNodes.Count; k++)
			{

				// calculate the intensity from this light, and add it to the illumination of the node.
				float illumination = theLight.intensity / 
					Vector3.Distance(affectedNodes[k].position, theLightNode.position);
				affectedNodes[k].illumination += illumination;

				// Optionally, clamp it to the range [0,1]
				if (clampIllumination)
				{
					affectedNodes[k].illumination = Mathf.Clamp(affectedNodes[k].illumination, 0f, upperLightIntensity);
				}

			}
		}

	}

	/// <summary>
	/// Toggles the units before and after building.
	/// </summary>
	/// <param name="toggle">If set to <c>true</c> toggle.</param>
	void EnableUnits(bool toggle)
	{
		
		for (int i = 0; i < ignoredObjects.Length; i++)
		{
			ignoredObjects[i].SetActive(toggle);
		}
	}


	public void ToggleClamping()
	{
		clampIllumination = !clampIllumination;
		Debug.Log("Clamping Illumination: " + clampIllumination);
	}

	void OnGizmosDrawSelected()
	{
		if (theMesh.nodes.Count == 0)
		{
			Debug.LogError("The mesh is empty!");
		}

		foreach (KeyValuePair<SerializableVector3, Node> pair in theMesh.nodes)
		{
			Gizmos.color = pair.Value.isWalkable ? Color.green:Color.red;
			Gizmos.DrawCube(pair.Key, theMesh.cellSize * Vector3.one * 0.75f);
		}
	}

}
