using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
	public float MouseSensitivity = 1f;

	public Transform Orientation;
	public Transform CamHolder;

	private float verticalRotation;
	private float horizontalRotation;

	void LateUpdate()
	{


		float mouseX = Input.GetAxis("Mouse X");
		float mouseY = Input.GetAxis("Mouse Y");

		verticalRotation -= mouseY * MouseSensitivity;
		verticalRotation = Mathf.Clamp(verticalRotation, -70f, 70f);

		horizontalRotation += mouseX * MouseSensitivity;

		Orientation.Rotate(0, mouseX * MouseSensitivity, 0);

		CamHolder.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
	}
}