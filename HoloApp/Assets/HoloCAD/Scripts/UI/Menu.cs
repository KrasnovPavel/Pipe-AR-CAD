using UnityEngine;

public class Menu : MonoBehaviour
{
	private Camera _camera;

	private void Start()
	{
		_camera = Camera.main;
	}

	private const float MovingSpeed = 0.0011f;
	
	private void Update ()
	{
		PlaceMenu();
	}

	private void PlaceMenu()
	{
		RaycastHit hit;
		if (Physics.Raycast(_camera.transform.position, 
							 _camera.transform.TransformDirection(Vector3.forward),
							 out hit, 4, 1 << 9)
		    && hit.collider.gameObject == gameObject)
		{
			return;
		}
		
		Vector3 labelPosition = _camera.transform.position + _camera.transform.forward * 3;
		Quaternion labelRotation = Quaternion.FromToRotation(Vector3.forward, _camera.transform.forward);
		labelRotation = Quaternion.Euler(labelRotation.eulerAngles.x, labelRotation.eulerAngles.y, 0);

		transform.position = Vector3.Lerp(transform.position, labelPosition, Time.time * MovingSpeed);
		transform.rotation = Quaternion.Lerp(transform.rotation, labelRotation, Time.time * MovingSpeed);
	}
}
