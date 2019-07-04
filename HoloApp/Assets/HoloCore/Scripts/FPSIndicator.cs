using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HoloCore
{
	/// <summary> Скрипт для отображения частоты кадров. </summary>
	public class FPSIndicator : MonoBehaviour
	{
		/// <summary> Расстояние от камеры в метрах. </summary>
		public float DistanceFromCamera = 2f;

		public float MoveSpeed = 1;
		
		#region Unity events

		protected void Start()
		{
			_camera = Camera.main;
			_textMesh = transform.Find("Label").GetComponent<TextMesh>();
		}
	
		protected void Update ()
		{
			_fpsValues.RemoveAt(0);
			_fpsValues.Add(1 / Time.deltaTime);

			_textMesh.text = $"FPS: {_fpsValues.Average():0}";
			
			transform.LookAt(_camera.transform);

			Vector3 newPos = _camera.transform.position + _camera.transform.forward * DistanceFromCamera;
			
			transform.position = Vector3.Lerp(transform.position, newPos, MoveSpeed);
		}
		#endregion

		#region Private definitions

		/// <summary> Меш на который выводится информация. </summary>
		private TextMesh _textMesh;
		
		/// <summary> Список значений FPS за последние 5 кадров. </summary>
		private readonly List<float> _fpsValues = new List<float>(new[]{0f,0f,0f,0f,0f});

		private Camera _camera;

		#endregion
	}
}
