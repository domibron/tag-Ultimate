using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class RemoveLocalClutter : MonoBehaviour
{
	public Object[] ThingsToRemove;

	// Start is called before the first frame update
	void Start()
	{
		if (!GetComponent<PhotonView>().IsMine)
		{
			for (int i = 0; i < ThingsToRemove.Length; i++)
			{
				Destroy(ThingsToRemove[i]);
			}
		}
	}
}
