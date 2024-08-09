using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerManager : MonoBehaviourPunCallbacks
{
	// [SerializeField] GameObject panel;
	// [SerializeField] TMP_Text text;
	// [SerializeField] Canvas canvas;

	[SerializeField] GameObject PlayerCharacter;
	[SerializeField] TMP_Text TopMidInfo;

	// [SerializeField] GameObject winningPanel;
	// [SerializeField] TMP_Text winnerText;

	public float matchDuration;
	// public int maxKills;

	public float matchTime;

	float minutes;
	float seconds;
	string textHolder;

	bool isGameOver;
	bool ranPrimary = false;

	PhotonView PV;
	Room currentRoom;
	Player masterPlayer;

	GameObject controller;

	int kills;
	int deaths;

	float timeLeft;
	float respawnTime = 3f;

	private int SeekerCount = 0;
	private int HiderCount = 0;

	void Awake()
	{
		PV = GetComponent<PhotonView>();
	}

	IEnumerator Start()
	{
		// panel.SetActive(false);
		currentRoom = PhotonNetwork.CurrentRoom;

		foreach (Player _player in PhotonNetwork.PlayerList)
		{
			if (_player.IsMasterClient)
			{
				masterPlayer = _player;
			}

			if ((int)_player.CustomProperties["team"] == 0)
			{
				SeekerCount++;
			}
			else if ((int)_player.CustomProperties["team"] == 1)
			{
				HiderCount++;
			}
		}

		if (PV.IsMine)
		{
			//if (PhotonNetwork.IsMasterClient)
			//{
			//    matchTime = durationOfMatch * 60;
			//    Hashtable hash = new Hashtable();
			//    hash.Add("MasterTime", matchTime);
			//    hash.Add("MasterKills", maxKills);
			//    PhotonNetwork.NetworkingClient.OpSetCustomPropertiesOfRoom(hash);
			//}

			CreateController();
			SetVaribles();

			yield return new WaitForSeconds(0.5f);

			// after the varibles are set it will set match duration - host will manage the time.
			// this is to pervent d-sync and anyone trying to trigger a endgame.
			matchTime = matchDuration; // converts minuets to seconds.

			while (!PhotonNetwork.IsConnectedAndReady)
			{
				yield return new WaitForSeconds(0.1f);
			}
			SetVaribles();
			matchTime = matchDuration;
		}
		else
		{
			Destroy(PlayerCharacter);
			//Destroy(panel);
		}
	}

	public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
	{
		if (!ranPrimary)
			SetVaribles();
		else if (!PhotonNetwork.IsMasterClient) // add master client check
		{
			matchTime = (float)PhotonNetwork.CurrentRoom.CustomProperties["MasterCT"];

			minutes = Mathf.Floor(matchTime / 60);
			seconds = matchTime % 60;
			textHolder = $"{minutes}:{Mathf.RoundToInt(seconds)}"; // time left display - currently for mins and secs.
		}
	}

	void SetVaribles()
	{
		ranPrimary = true;

		matchDuration = (float)PhotonNetwork.CurrentRoom.CustomProperties["MasterTime"] * 60f;
		//maxKills = (int)PhotonNetwork.CurrentRoom.CustomProperties["MasterKills"];
		matchTime = (float)PhotonNetwork.CurrentRoom.CustomProperties["MasterCT"]; // DO NOT REMOVE THIS - the time does not set to matchDuration.

		SeekerCount = (int)PhotonNetwork.CurrentRoom.CustomProperties["Seekers"];
		HiderCount = (int)PhotonNetwork.CurrentRoom.CustomProperties["Hiders"];

		matchTime = matchDuration;

		//print(matchTime + " match time");
		//print(maxKills);
		//print(matchDuration);
	}

	void Update()
	{

		if (PV.IsMine)
		{

			if (!isGameOver && PhotonNetwork.IsMasterClient) // change the time - yes i need to point this out.
			{
				matchTime -= Time.deltaTime;
				PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable() { { "MasterCT", matchTime } }); // set the global value

				minutes = Mathf.Floor(matchTime / 60);
				seconds = matchTime % 60;
				textHolder = $"{minutes}:{Mathf.RoundToInt(seconds)}";
			}
			else if (!isGameOver)
			{
				matchTime = (float)PhotonNetwork.CurrentRoom.CustomProperties["MasterCT"]; // syncing off hosts.
			}


			//float timeHolder = matchTime / 60f; // match time is getting set faster and before this update is called so temp is here to help with that.

			//string textHolder = $"{minutes}:{Mathf.RoundToInt(seconds)}"; // time left display - currently for mins and secs.



			TopMidInfo.text = $"Time remaining: <mspace=30>{textHolder}</mspace>\nSeekers Remaining: {SeekerCount} | Hiders Remaining {HiderCount}";

			// ==== end match logic ====
			if (PhotonNetwork.IsMasterClient && matchTime <= 0 && PhotonNetwork.CurrentRoom.PlayerCount > 1)
			{
				// GameOver send RPC event
				//Scoreboard.Instance.GetPlayerKills(Scoreboard.Instance.GetPlayerWithMostKills());

				//PV.RPC(nameof(RPC_SendWinner), RpcTarget.All, Scoreboard.Instance.GetPlayerWithMostKills(), Scoreboard.Instance.GetMostKillsInGame());

				// handle kill count

				//TODO can replace with win screen.
				ForceEveryoneToLeave();
			}

			if (HiderCount <= 0 && PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount > 1)
			{
				//TODO can replace with win screen.
				ForceEveryoneToLeave();
			}

			// if (kills >= maxKills)
			// {
			// 	// game over
			// 	// this player wins
			// 	PV.RPC(nameof(RPC_SendWinner), RpcTarget.All, PV.Owner, kills);
			// }
		}

		// if (panel.activeSelf)
		// {
		// 	timeLeft -= Time.deltaTime;
		// 	text.text = $"<b>You Died lol!</b><br>Respawning in:<br><mspace=0.75em>{(Mathf.Round(timeLeft * 100f) / 100f).ToString("N2")}</mspace>";
		// }
	}

	void CreateController()
	{
		Transform spawnpoint = null;

		if ((int)PV.Owner.CustomProperties["team"] == 0)
		{
			spawnpoint = SpawnManager.Current.GetSeekerSpawnPoint();
			controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerSeeker"), spawnpoint.position, spawnpoint.rotation, 0, new object[] { PV.ViewID });

		}
		else if ((int)PV.Owner.CustomProperties["team"] == 1)
		{
			spawnpoint = SpawnManager.Current.GetHiderSpawnPoint();
			controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerHider"), spawnpoint.position, spawnpoint.rotation, 0, new object[] { PV.ViewID });
		}


		// respawn point change to respected team.

	}

	IEnumerator Respawn()
	{
		//panel.SetActive(true);
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;

		timeLeft = respawnTime;
		yield return new WaitForSeconds(respawnTime);

		//panel.SetActive(false);
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;

		if (!isGameOver)
			CreateController();
		else
		{
			//panel.SetActive(false);
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.Confined;
		}
	}

	public void Die()
	{
		PhotonNetwork.Destroy(controller);


		Hashtable hash = new Hashtable();
		hash.Add("team", 0);
		PhotonNetwork.LocalPlayer.SetCustomProperties(hash);

		if ((int)PV.Owner.CustomProperties["team"] == 1) PV.RPC(nameof(ConverToSeeker), RpcTarget.All);

		StartCoroutine(Respawn());
	}

	[PunRPC]
	public void ConverToSeeker()
	{
		HiderCount--;
		SeekerCount++;


	}

	public void GetKill()
	{
		PV.RPC(nameof(RPC_GetKill), PV.Owner);
	}

	[PunRPC]
	void ChangeHiders(int ammount)
	{
		Hashtable hashtable = new Hashtable();

		hashtable.Add("Hiders", (int)PhotonNetwork.CurrentRoom.CustomProperties["Hiders"] + ammount);


		PhotonNetwork.CurrentRoom.SetCustomProperties(hashtable);
	}

	[PunRPC]
	void ChangeSeekers(int ammount)
	{
		Hashtable hashtable = new Hashtable();

		hashtable.Add("Seekers", (int)PhotonNetwork.CurrentRoom.CustomProperties["Seekers"] + ammount);

		PhotonNetwork.CurrentRoom.SetCustomProperties(hashtable);
	}


	[PunRPC]
	void RPC_GetKill()
	{
		//kills++;

		//Hashtable hash = new Hashtable();
		//hash.Add("kills", kills);
		//PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
	}

	public static PlayerManager Find(Player player)
	{
		return FindObjectsByType<PlayerManager>(FindObjectsSortMode.None).SingleOrDefault(x => x.PV.Owner == player);
	}

	// ====================== eeee

	[PunRPC]
	void RPC_SendWinner(Player winnerPlayer, int kills)
	{
		isGameOver = true;

		// winnerText.text = $"Player [{winnerPlayer.NickName}] Won the game!<br>Kills: {Scoreboard.Instance.GetMostKillsInGame()}";

		if (controller != null)
			PhotonNetwork.Destroy(controller);

		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.Confined;

		StopCoroutine(Respawn());

		if (controller != null)
			PhotonNetwork.Destroy(controller);

		// if (container != null)
		// 	container.SetActive(false);

		// winningPanel.SetActive(true);

		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.Confined;


		// need to do this
	}

	public void SpawnObject(string path, Vector3 pos, Quaternion rotaton)
	{
		PV.RPC(nameof(RPC_SpawnObject), RpcTarget.All, path, pos, rotaton);
	}


	[PunRPC]
	void RPC_SpawnObject(String path, Vector3 pos, Quaternion rotaton)
	{
		Instantiate(Resources.Load(path), pos, rotaton);
	}

	// ====================== leave room management ======================

	public void ForceEveryoneToLeave()
	{
		PV.RPC(nameof(RPC_DisconnectEveryoneAndHost), RpcTarget.All);
	}

	[PunRPC]
	void RPC_DisconnectEveryoneAndHost()
	{
		leaveRoom();
	}

	public void leaveRoom()
	{
		PhotonNetwork.AutomaticallySyncScene = false;

		Destroy(RoomManager.Current.gameObject);

		//Destroy(DiscordHanderler.Instance.gameObject);

		StartCoroutine(DisconnectAndLoad());
	}

	IEnumerator DisconnectAndLoad()
	{
		//PhotonNetwork.Disconnect();
		PhotonNetwork.LeaveRoom();
		//while(PhotonNetwork.InRoom)
		while (PhotonNetwork.InRoom)
			yield return null;
		SceneManager.LoadScene(0);
	}
}
