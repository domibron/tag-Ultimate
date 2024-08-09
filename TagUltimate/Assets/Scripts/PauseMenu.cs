using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviourPunCallbacks
{

	public GameObject pauseMenu;

	bool isPauseMenuActive;

	private void Awake()
	{
		pauseMenu.SetActive(false);

	}

	void Start()
	{
		// pauseMenuSensitivity = playerController2.mouseSensitivity;
		// sensitivitySlider.value = playerController2.mouseSensitivity * 10;
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			isPauseMenuActive = !isPauseMenuActive;
		}

		pauseMenu.SetActive(isPauseMenuActive);

		Cursor.lockState = (isPauseMenuActive ? CursorLockMode.Confined : CursorLockMode.Locked);
		Cursor.visible = isPauseMenuActive;

		// pauseMenuSensitivity = sensitivitySlider.value / 10;

		// sensitivitySliderText.text = pauseMenuSensitivity.ToString();
		// playerController2.mouseSensitivity = pauseMenuSensitivity;

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

	public override void OnLeftRoom()
	{
		Debug.Log("left room");
	}
}
