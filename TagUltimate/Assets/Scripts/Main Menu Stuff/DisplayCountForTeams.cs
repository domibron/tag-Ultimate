using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class DisplayCountForTeams : MonoBehaviourPunCallbacks
{
	public TMP_Text RedTeam;
	public TMP_Text GreenTeam;

	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{

	}

	public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
	{
		if (propertiesThatChanged.ContainsKey("Seekers"))
		{
			RedTeam.text = propertiesThatChanged["Seekers"].ToString();
		}

		if (propertiesThatChanged.ContainsKey("Hiders"))
		{
			GreenTeam.text = propertiesThatChanged["Hiders"].ToString();

		}


	}
}
