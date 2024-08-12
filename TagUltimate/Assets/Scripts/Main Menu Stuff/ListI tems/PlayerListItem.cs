using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;

public class PlayerListItem : MonoBehaviourPunCallbacks
{
	[SerializeField] TMP_Text text;
	public Image BackGround;
	public Image PlayerHostCrown;

	public Player player;

	public Color NoneLocalPlayerColour = Color.white;
	public Color PlayerLocalPlayerColour = Color.blue;

	public void SetUp(Player _player)
	{
		player = _player;
		text.text = _player.NickName;

		if (_player.IsLocal)
			BackGround.color = PlayerLocalPlayerColour;
		else
			BackGround.color = NoneLocalPlayerColour;

	}

	void FixedUpdate()
	{
		if (player == PhotonNetwork.MasterClient)
		{
			PlayerHostCrown.gameObject.SetActive(true);
		}
		else
		{
			PlayerHostCrown.gameObject.SetActive(false);
		}
	}

	public override void OnPlayerLeftRoom(Player otherPlayer)
	{
		if (player == otherPlayer)
		{
			Destroy(gameObject);
		}
	}
	public override void OnLeftRoom()
	{
		Destroy(gameObject);
	}
}
