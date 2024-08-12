using System.Collections;
using System.Collections.Generic;
using System.IO;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomManager : MonoBehaviourPunCallbacks
{
	public static RoomManager Current;

	bool active;

	// extarnlly set vars
	#region Awake
	void Awake()
	{
		if (Current != null && Current != this)
		{
			Destroy(this.gameObject);
		}
		else
		{
			Current = this;
			DontDestroyOnLoad(this);
		}
	}
	#endregion

	#region OnEnable
	public override void OnEnable()
	{
		base.OnEnable();
		SceneManager.sceneLoaded += OnSceneLoaded;
	}
	#endregion

	#region OnDisable
	public override void OnDisable()
	{
		SceneManager.sceneLoaded -= OnSceneLoaded;
	}
	#endregion

	#region OnSceneLoaded
	void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
	{
		// make a swich statement.


		// make into fuction to  instanciate. please make a list of scenes this is just bad.
		if (scene.buildIndex != 0) // game scene
		{
			InstaciatePlayerManager();
		}
	}
	#endregion

	#region InstaciatePlayerManager
	void InstaciatePlayerManager()
	{
		GameObject playerRe = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerManager"), Vector3.zero, Quaternion.identity); // create function
		PlayerManager playerManager = playerRe.GetComponent<PlayerManager>();
		//playerManager.gaw = 69; // set sensitivity and such
	}
	#endregion
}
