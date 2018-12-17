using UnityEngine;

namespace HoloCAD
{
	public class TubeLabel : MonoBehaviour
	{
		public GameObject Label;

		private void Update()
		{
			Label.SetActive(GetComponent<BaseTube>().IsSelected);

			Vector3 direction = (Camera.main.transform.position - transform.parent.position).normalized;
			Quaternion labelRotation = Quaternion.FromToRotation(Vector3.back, direction);
			Label.transform.rotation = Quaternion.Euler(0, labelRotation.eulerAngles.y, 0);
            
			if (gameObject.GetComponent<BendedTube>() == null)
			{
				Label.GetComponent<TextMesh>().text = "Длина трубы: " + GetComponent<DirectTube>().Length + " м.";
			}
			else
			{
				BendedTube tube = GetComponent<BendedTube>();
				float radius = tube.UseSecondRadius ? tube.Data.first_radius : tube.Data.second_radius;
				float rotation = gameObject.transform.rotation.eulerAngles.y;
				Label.GetComponent<TextMesh>().text =
					"Угол погиба: " + tube.Angle +
					"°\r\n Угол поворота " + rotation.ToString("0.00") +
					"°\r\n Радиус: " + radius.ToString("0.000");
			}
		}
	}
}
