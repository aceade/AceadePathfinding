using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

/// <summary>
/// The GameManager manages general aspects of the game, 
/// including things like changing navmesh,
/// saving/loading, and options.
/// </summary>

public class GameManager : MonoBehaviour {

	public static GameManager manager;
	
	// the current navigation mesh
	public static NavigationMesh currentNavMesh;

	// a dictionary of meshes, with specific level names as keys
	static Dictionary<string, NavigationMesh> navMeshes = new Dictionary<string, NavigationMesh>();

	private static string savePath = Application.dataPath;

	private static string navSavePath = savePath +  "/Pathfinding/Meshes";
	

	/// <summary>
	/// Changes the level, and loads the relevant navmesh.
	/// </summary>
	/// <param name="newLevel">New level.</param>
	public static void ChangeLevel(string newLevel)
	{
		currentNavMesh = navMeshes[newLevel];
		Application.LoadLevel(newLevel);
	}

	/// <summary>
	/// Adds the specified NavigationMesh to the list,
	/// and then saves the list to the hard drive.
	/// </summary>
	/// <param name="mesh">Mesh.</param>
	public static void AddNavMesh(NavigationMesh mesh)
	{
		string name = mesh.meshName;
		Debug.Log("Adding " + mesh + " for level " + name);
		if (navMeshes.ContainsKey(name) == false)
			navMeshes.Add(name, mesh);
		else
			navMeshes[name] = mesh;

		SaveNavMeshes();
	}

	/// <summary>
	/// Loads the navmeshes from the hard disk.
	/// </summary>
	public static void LoadNavMeshes()
	{

		if (File.Exists(navSavePath + "/NavMeshes.nav") )
		{
			BinaryFormatter bf = new BinaryFormatter();
			var file = File.Open(navSavePath + "/NavMeshes.nav", FileMode.Open);
			navMeshes = (Dictionary<string, NavigationMesh>) bf.Deserialize(file);
			file.Close();
			Debug.Log("Loaded the navmeshes from the disk");
		}
		else
		{
			Debug.LogError("Problem loading meshes from " + navSavePath + "; file does not exist");
		}

	}

	/// <summary>
	/// Saves the navmeshes to the hard disk.
	/// </summary>
	public static void SaveNavMeshes()
	{
		BinaryFormatter bf = new BinaryFormatter();
		var navMeshFile = File.Create(navSavePath + "/NavMeshes.nav");

		bf.Serialize(navMeshFile, navMeshes);
		navMeshFile.Close();

		Debug.Log("Saved the nav meshes to the disk");
	}

}
