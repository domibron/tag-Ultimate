using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleDoor : MonoBehaviour
{
	public Transform DoorToRotateTransform;

	public Vector3 OpenRotation = new Vector3(0, 45, 0);
	public Vector3 ClosedRotation = new Vector3(0, -45, 0);

	public bool IsOpen = true;

	public float Speed = 2f;

	private float DoorTime = 0;

	[Header("")]
	public bool DelayDoor = false;

	public float DelayAmmount = 3f;

	private float DelayCount = 0;

	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		if (DelayDoor && DelayCount >= 0)
		{
			DelayCount -= Time.deltaTime;
		}

		if (IsOpen && DoorTime > 0 && DelayCount <= 0)
		{
			DoorTime -= Time.deltaTime * Speed;
		}
		else if (!IsOpen && DoorTime < 1 && DelayCount <= 0)
		{
			DoorTime += Time.deltaTime * Speed;
		}

		DoorToRotateTransform.localRotation = Quaternion.Euler(Vector3.Lerp(OpenRotation, ClosedRotation, DoorTime));


	}

	public void TriggerToggleDoor()
	{
		if (DelayDoor) DelayCount = DelayAmmount;
		IsOpen = !IsOpen;
	}

	public void SetDoorStart(bool b)
	{
		if (DelayDoor) DelayCount = DelayAmmount;
		IsOpen = b;
	}
}
