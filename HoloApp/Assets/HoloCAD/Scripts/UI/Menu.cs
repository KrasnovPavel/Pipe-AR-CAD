using UnityEngine;

public class Menu : MonoBehaviour
{
	private const float MovingSpeed = 0.0011f;
	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	private void Update ()
	{
		PlaceMenu();
	}

	private void PlaceMenu()
	{
		Camera camera = Camera.main;
		RaycastHit hit;
		if (Physics.Raycast(camera.transform.position, 
							 camera.transform.TransformDirection(Vector3.forward),
							 out hit, 4, 1 << 9)
		    && hit.collider.gameObject == gameObject)
		{
			return;
		}
		
		Vector3 labelPosition = camera.transform.position + camera.transform.forward * 3;
		Quaternion labelRotation = Quaternion.FromToRotation(Vector3.forward, camera.transform.forward);
		labelRotation = Quaternion.Euler(labelRotation.eulerAngles.x, labelRotation.eulerAngles.y, 0);

		transform.position = Vector3.Lerp(transform.position, labelPosition, Time.time * MovingSpeed);
		transform.rotation = Quaternion.Lerp(transform.rotation, labelRotation, Time.time * MovingSpeed);
	}
}
