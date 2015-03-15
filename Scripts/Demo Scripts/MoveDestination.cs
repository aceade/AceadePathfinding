using UnityEngine;
using System.Collections;

/// <summary>
/// Moves the destination and alerts the sample agent.
/// </summary>
public class MoveDestination : MonoBehaviour {

	private bool canMove = true;

	[Tooltip("The minimum time between position changes")]
	public float delay = 1f;

	[Tooltip("The main camera")]
	public Camera mainCamera;

	[Tooltip("The sample agent")]
	public GameObject agent;

	private Transform myTrans;

	private Ray ray;

	private RaycastHit hit;

	void Start()
	{
		myTrans = transform;

		GameManager.LoadNavMeshes();

		if (mainCamera == null)
			mainCamera = Camera.main;
	}

	// Update is called once per frame
	void Update () 
	{
	
		// if the left mouse button is clicked, cast a ray from the camera
		// and if a collider is hit, move the destination there.
		if (Input.GetMouseButton(0) && canMove )
		{
			canMove = false;

			ray = mainCamera.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out hit))
			{
				myTrans.position = hit.point;
				agent.SetActive(true);

			}

			Invoke ("resetMovement", delay);
		}

	}

	void resetMovement()
	{
		agent.GetComponent<AStar>().SetDestination(myTrans.position);
		canMove = true;
	}
}
