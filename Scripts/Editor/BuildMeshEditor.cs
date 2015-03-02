using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(BuildNavMesh))]	
public class BuildMeshEditor : Editor {



	public override void OnInspectorGUI()
	{
		BuildNavMesh meshBuilder = target as BuildNavMesh;

		// toggle light intensity clamping
		if(GUILayout.Toggle(meshBuilder.clampIllumination, "Clamp Light Intensity?") )
		{
			meshBuilder.ToggleClamping();
		}

		// build the mesh
		if (GUILayout.Button("Build Mesh"))
		{
			meshBuilder.BuildMesh();
		}

		// load meshes frmo the disk, for testing.
		if (GUILayout.Button ("Load Meshes") )
		{
			GameManager.LoadNavMeshes();
		}
	}
	
}
