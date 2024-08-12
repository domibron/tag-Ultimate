using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;

public class RoomListItem : MonoBehaviour
{
	[SerializeField] TMP_Text text;
	public RoomInfo Info;
	public Image image;

	public Color OpenRoomColour = Color.green;
	public Color ClosedRoomColour = Color.red;

	public void SetUp(RoomInfo info)
	{
		this.Info = info;

		text.text = $"{info.Name}\n{info.PlayerCount} / {info.MaxPlayers}";
	}

	void FixedUpdate()
	{
		if (Info.IsOpen)
			image.color = OpenRoomColour;
		else
			image.color = ClosedRoomColour;
	}

	public void OnClick()
	{
		MainMenu.current.JoinRoom(Info);
	}
}
