using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonBar : MonoBehaviour {
    public float offset;
	
	// Update is called once per frame
	void Update ()
    {
        Vector3 direction = (Camera.main.transform.position - transform.parent.position).normalized * offset;
        Quaternion rotation = Quaternion.FromToRotation(Vector3.back, direction);
        transform.rotation = Quaternion.Euler(0, rotation.eulerAngles.y, 0);
        transform.position = new Vector3(direction.x, 0, direction.z) + transform.parent.position;
    }
}
