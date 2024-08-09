using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillZone : MonoBehaviour
{
	public static KillZone Current;

	void Awake()
	{
		if (Current != null && Current != this)
		{
			Destroy(this.gameObject);
		}
		else
		{
			Current = this;
		}
	}

	public float YLevel
	{
		get
		{
			return transform.position.y;
		}
	}

	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{

	}
}
