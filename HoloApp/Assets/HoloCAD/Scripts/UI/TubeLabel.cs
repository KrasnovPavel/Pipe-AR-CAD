using UnityEngine;
using HoloCAD.UnityTubes;

namespace HoloCAD.UI
{
	public class TubeLabel : MonoBehaviour
	{
		public GameObject Label;
		private BaseTube _baseTube;
		private BendedTube _bendedTube;
		private DirectTube _directTube;

		private void Start()
		{
			_directTube = GetComponent<DirectTube>();
			_bendedTube = gameObject.GetComponent<BendedTube>();
			_baseTube = GetComponent<BaseTube>();
		}

		private void Update()
		{
			Label.SetActive(_baseTube.IsSelected);

			Vector3 direction = (Camera.main.transform.position - transform.parent.position).normalized;
			Quaternion labelRotation = Quaternion.FromToRotation(Vector3.back, direction);
			Label.transform.rotation = Quaternion.Euler(0, labelRotation.eulerAngles.y, 0);
            
			if (!_directTube)
			{
				_directTube.LabelText.text = "Длина трубы: " + _directTube.Length + " м.";
			}
			else
			{
				float radius = _bendedTube.UseSecondRadius ? _bendedTube.Data.first_radius : _bendedTube.Data.second_radius;
				float rotation = gameObject.transform.rotation.eulerAngles.y;
				_bendedTube.LabelText.text = "Угол погиба: " + _bendedTube.Angle +
											 "°\r\n Угол поворота " + rotation.ToString("0.00") +
											 "°\r\n Радиус: " + radius.ToString("0.000");
			}
		}
	}
}
