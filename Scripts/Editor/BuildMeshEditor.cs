using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(BuildNavMesh))]	
public class BuildMeshEditor : Editor {

	// a placeholder for a new walkable layer
	int walkable;

	// these determine the physical size of a Node in world space
	float cellSize = 1f;
	float maxCellHeight = 3f;
	float minCellHeight = 1f;

	bool toggleLightClamping;

	public GameObject[] objectsToIgnore;

	// the maximum light intensity
	float maxLightIntensity = 1f;

	string meshName = "NaveMesh Cube";

	public NavigationMesh navMesh;

	public override void OnInspectorGUI()
	{
		BuildNavMesh meshBuilder = target as BuildNavMesh;
		
		// displays the walkable layers, and allows them to be removed
		for (int i = 0; i < meshBuilder.walkableLayers.Count; i++)
		{
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LayerField(meshBuilder.walkableLayers[i]);
			if (GUILayout.Button("x") )
			{
				meshBuilder.walkableLayers.RemoveAt(i);
			}
			EditorGUILayout.EndHorizontal();
		}

		EditorGUILayout.Space();

		// allows the user to add more walkable layers
		GUILayout.Label("Select walkable layers from below");
		walkable = EditorGUILayout.LayerField(walkable);
		if (GUILayout.Button("Add walkable layer"))
		{
			meshBuilder.walkableLayers.Add (walkable);
		}
		if (GUILayout.Button("Clear Walkable Layers") )
		{
			meshBuilder.walkableLayers.Clear();
		}


		// set the cellSize
		EditorGUI.BeginChangeCheck();
		cellSize = EditorGUILayout.FloatField("Set cell size", cellSize);
		if (cellSize <= 0)
		{
			Debug.LogWarning("Cannot have a cell size of zero! Resetting to 0.5");
			cellSize = 0.5f;
		}

		minCellHeight = EditorGUILayout.FloatField("Set minimum cell height", minCellHeight);
		maxCellHeight = EditorGUILayout.FloatField("Set maximum cell height", maxCellHeight);

		if (maxCellHeight < minCellHeight)
		{
			Debug.LogWarning("The maximum cell height must be greater than the minimum!");
			maxCellHeight = minCellHeight + 0.1f;
		}
		EditorGUI.EndChangeCheck();
		EditorGUILayout.Space();


		// toggle light intensity clamping here
		EditorGUI.BeginChangeCheck();
		toggleLightClamping = EditorGUILayout.Toggle("Clamp illumination?", toggleLightClamping);
		if (toggleLightClamping)
		{
			maxLightIntensity = EditorGUILayout.FloatField("Maximum light intensity", maxLightIntensity);
			if (maxLightIntensity <= 0)
			{
				Debug.LogWarning("The maximum light intensity must be greater than zero!");
				maxLightIntensity = 0.1f;
			}
		}
		EditorGUI.EndChangeCheck();
		EditorGUILayout.Space();

		// get the name here
		GUILayout.BeginHorizontal();
		GUILayout.Label("Name of the mesh");
		meshName = GUILayout.TextField(meshName);
		GUILayout.EndHorizontal();

		// build the mesh
		if (GUILayout.Button("Build Mesh"))
		{
			BuildMesh(meshBuilder);
		}

		// load meshes from the disk, for testing.
		if (GUILayout.Button ("Load Meshes") )
		{
			LoadMeshes();
		}
	}

	void BuildMesh(BuildNavMesh builder)
	{
		
		navMesh = ScriptableObject.CreateInstance<NavigationMesh>();
		builder.BuildMesh(navMesh, cellSize, toggleLightClamping, maxLightIntensity, minCellHeight, maxCellHeight);
		Debug.LogFormat("Mesh has been built. It now has {0} nodes", navMesh.nodes.Count);
		AssetDatabase.CreateAsset(navMesh, "Assets/AceadePathfinding/NavMeshes/" + meshName + ".asset");
		AssetDatabase.SaveAssets();
		Selection.activeObject = navMesh;
	}

	void LoadMeshes()
	{
//		GameManager.LoadNavMeshes();
	}
	
}
