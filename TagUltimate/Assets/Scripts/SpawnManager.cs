using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
	public static SpawnManager Instance;

	public Transform[] SeekerSpawnPoints;
	public Transform[] HiderSpawnPoints;

	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{

	}

	public Transform GetSeekerSpawnPoint()
	{
		if (SeekerSpawnPoints.Length > 0)
			return SeekerSpawnPoints[Random.Range(0, SeekerSpawnPoints.Length)];
		else
			return transform;
	}

	public Transform GetHiderSpawnPoint()
	{
		if (HiderSpawnPoints.Length > 0)
			return HiderSpawnPoints[Random.Range(0, HiderSpawnPoints.Length)];
		else
			return transform;
	}
}


