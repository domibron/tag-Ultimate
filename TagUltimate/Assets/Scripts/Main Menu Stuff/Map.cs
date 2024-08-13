using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Map : MonoBehaviour
{
	public GameObject selectionMarker;

	public string mapName;
	public int mapIndexNumber;
	public bool mapSelected;

	public Image MapImage;

	void Start()
	{
		MapImage = GetComponent<Image>();
	}

	public void Select()
	{
		mapSelected = true;
		selectionMarker.SetActive(true);
	}

	public void DeSelect()
	{
		mapSelected = false;
		selectionMarker.SetActive(false);
	}
}
