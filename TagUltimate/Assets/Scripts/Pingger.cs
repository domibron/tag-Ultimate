using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pingger : MonoBehaviour
{
	public float Force = 10;

	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{

	}

	void OnCollisionEnter(Collision other)
	{
		if (other.gameObject.GetComponent<Rigidbody>() != null)
		{
			Vector3 vector = other.transform.position - transform.position;

			other.gameObject.GetComponent<Rigidbody>().AddForce(vector.normalized * Force);
		}
	}
}
