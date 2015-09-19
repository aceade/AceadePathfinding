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

	private static string navSavePath = Application.streamingAssetsPath +  "/AceadePathfinding/navMeshes.nav";
	

	/// <summary>
	/// Changes the level, and loads the relevant navmesh.
	/// </summary>
	/// <param name="newLevel">New level.</param>
	public static void ChangeLevel(string newLevel)
	{
		if (navMeshes.ContainsKey(newLevel) )
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
		Debug.Log(string.Format("Adding {0} for level {1}", mesh, name));
		if (navMeshes.ContainsKey(name) == false)
		{
			navMeshes.Add(name, mesh);
		}
		else
		{
			navMeshes[name] = mesh;
		}

		SaveNavMeshes();
	}

	/// <summary>
	/// Loads the navmeshes from the hard disk.
	/// </summary>
	public static void LoadNavMeshes()
	{

		if (File.Exists(navSavePath) )
		{
			BinaryFormatter bf = new BinaryFormatter();
			var file = File.Open(navSavePath, FileMode.Open);
			navMeshes = (Dictionary<string, NavigationMesh>) bf.Deserialize(file);
			file.Close();
			Debug.Log(string.Format("Loaded {0} navmeshes from the disk", navMeshes.Count));
		}
		else
		{
			Debug.LogError(string.Format("Problem loading meshes from {0}; file does not exist", navSavePath));
		}

	}

	/// <summary>
	/// Saves the navmeshes to the hard disk.
	/// </summary>
	public static void SaveNavMeshes()
	{
		BinaryFormatter bf = new BinaryFormatter();
		var navMeshFile = File.Create(navSavePath);

		bf.Serialize(navMeshFile, navMeshes);
		navMeshFile.Close();

		Debug.Log(string.Format("Saved {0} nav meshes to the disk", navMeshes.Count));
	}

	/// <summary>
	/// Gets the current nav mesh.
	/// </summary>
	/// <returns>The current nav mesh.</returns>
	public static NavigationMesh getCurrentNavMesh()
	{
		if (currentNavMesh == null)
		{
			LoadNavMeshes();
			currentNavMesh = navMeshes[Application.loadedLevelName];
		}

		return currentNavMesh;
	}

}
