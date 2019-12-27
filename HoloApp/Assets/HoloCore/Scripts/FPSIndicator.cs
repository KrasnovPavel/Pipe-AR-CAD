// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

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
			_textMesh = transform.Find("Label").GetComponent<TextMesh>();
            // ReSharper disable once PossibleNullReferenceException
            _cameraTransform = Camera.main.transform;
		}
	
		protected void Update ()
		{
			_fpsValues[_pointer] = 1 / Time.deltaTime;
			_pointer++;

			if (_pointer == 5)
			{
				_pointer = 0;
				_textMesh.text = $"FPS: {_fpsValues.Average():0}";
			}

			transform.LookAt(_cameraTransform);

			Vector3 newPos = _cameraTransform.position + _cameraTransform.forward * DistanceFromCamera;
			
			transform.position = Vector3.Lerp(transform.position, newPos, MoveSpeed);
		}
		#endregion

		#region Private definitions

		/// <summary> Меш на который выводится информация. </summary>
		private TextMesh _textMesh;
		
		/// <summary> Список значений FPS за последние 5 кадров. </summary>
		private readonly float[] _fpsValues = {0f,0f,0f,0f,0f};

		private int _pointer;

		private Transform _cameraTransform;

		#endregion
	}
}
