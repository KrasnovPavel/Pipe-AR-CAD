using UnityEngine;

/// <inheritdoc />
/// <summary>
/// Панель кнопок.
/// </summary>
public class ButtonBar : MonoBehaviour {
    /// <value> Смещение панели кнопок по направлению к камере. </value>
    public float Offset;
	
	// Update is called once per frame
	void Update ()
    {
        Vector3 direction = (Camera.main.transform.position - transform.parent.position).normalized * Offset;
        Quaternion rotation = Quaternion.FromToRotation(Vector3.back, direction);
        transform.rotation = Quaternion.Euler(0, rotation.eulerAngles.y, 0);
        transform.position = new Vector3(direction.x, 0, direction.z) + transform.parent.position;
    }
}
