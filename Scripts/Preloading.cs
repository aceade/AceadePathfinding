using UnityEngine;
using System.Collections;

/// <summary>
/// This class is used to preload everything else.
/// </summary>

public class Preloading : MonoBehaviour {

	// Use this for initialization
	void Start () 
	{
		GameManager.LoadNavMeshes();
		GameManager.ChangeLevel("Test Scene");
	}

}
