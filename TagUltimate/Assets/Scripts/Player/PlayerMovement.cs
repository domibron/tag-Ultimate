using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

public class PlayerMovement : MonoBehaviour
{
	public Rigidbody rb;

	public float PlayerSpeed = 2f;

	public Transform Orientation;



	// Start is called before the first frame update
	void Awake()
	{
		rb = GetComponent<Rigidbody>();
	}


	
		//if (HasStateAuthority == false)
		//{
		//	return;
		//}


		//if (GetInput(out NetworkInputData data))
		//{
		//	Vector3 move = (Orientation.right * data.direction.x + Orientation.forward * data.direction.z);

		//	rb.AddForce((move.normalized * PlayerSpeed) - rb.velocity, ForceMode.Force);
		//}

	
}


