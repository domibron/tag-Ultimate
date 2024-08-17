using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class MapManager : MonoBehaviour
{
	public static MapManager Current;

	public Map[] maps;

	private Scene currentPrepedScene;

	public Map SelectedMap;

	public int currentMapNumber;


	void Awake()
	{
		Current = this;
	}

	void Start()
	{
		//currentMapNumber = 2;
		SelectMap(currentMapNumber);
	}

	void Update()
	{
		// if (!MenuManager.Instance.ReturnIsOpenMenuName("team room"))
		//     return;

	}

	//public int ReturnSelectedMapInt()
	//{
	//    foreach (Map _map in maps)
	//    {
	//        if (_map.mapSelected && _map.selectionMarker.activeSelf)
	//        {
	//            return _map.mapIndexNumber;
	//        }
	//    }
	//    return 1;
	//}

	public int GetSelectedMap()
	{
		return currentMapNumber;
	}

	public Sprite GetMapImage()
	{
		return SelectedMap.MapImage.sprite;
	}

	public Color GetMapColour()
	{
		return SelectedMap.MapImage.color;
	}

	public void SelectMap(Map map)
	{
		SelectMap(map.mapIndexNumber);
	}

	public bool ReturnSelectedMap(int mapBuildIndexNumber)
	{
		foreach (Map _map in maps)
		{
			if (_map.mapIndexNumber == mapBuildIndexNumber && _map.selectionMarker.activeSelf)
			{
				return true;
			}
		}
		return false;
	}

	public void SelectMap(int mapBuildIndexNumber)
	{
		for (int i = 0; i < maps.Length; i++)
		{
			if (maps[i].mapIndexNumber == mapBuildIndexNumber)
			{
				maps[i].Select();
				currentMapNumber = maps[i].mapIndexNumber;
				SelectedMap = maps[i];
			}
			else if (maps[i].mapSelected)
			{
				DeSelectMap(maps[i]);
			}
		}

		if (PhotonNetwork.InRoom) PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable() { { "SceneIndex", mapBuildIndexNumber } });

	}

	public void DeSelectMap(Map map)
	{
		map.DeSelect();
	}
}
