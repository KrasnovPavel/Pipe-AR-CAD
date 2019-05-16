// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System.Linq;
using HoloCAD.UnityTubes;
using JetBrains.Annotations;
using UnityEngine;

namespace HoloCAD
{
	/// <summary> Класс, рассчитывающий и отображающий расстояние между соединяемыми трубами. </summary>
	[RequireComponent(typeof(LineRenderer))]
	public class TubesConnector : MonoBehaviour
	{
		// ReSharper disable once NotNullMemberIsNotInitialized
		/// <summary> Первая труба </summary>
		[NotNull] public Tube FirstTube;
		
		/// <summary> Вторая труба. </summary>
		[CanBeNull] public Tube SecondTube;
		
		// ReSharper disable once NotNullMemberIsNotInitialized
		/// <summary> Местоположение курсора. </summary>
		[NotNull] public Transform Cursor;
		
		/// <summary> Допустимая ошибка по углу стыка. </summary>
		[Tooltip("Допустимая ошибка по углу стыка.")]
		public float AngleThreshold = 3f;
		
		/// <summary> Допустимая ошибка по расстоянию. </summary>
		[Tooltip("Допустимая ошибка по расстоянию.")]
		public float DistanceThreshold = 0.005f;
		
		public void RemoveThis()
		{
			Destroy(gameObject);
			FirstTube.RemoveTransformError();
			SecondTube?.RemoveTransformError();

			if (TubeUnityManager.ActiveTubesConnector == this)
			{
				TubeUnityManager.RemoveActiveTubesConnector();
			}
		}

		#region Unity event functions

		/// <summary> Функция, инициализирующая объект в Unity. </summary>
		/// <remarks>
		/// При переопределении в потомке обязательно должна вызываться с помощью <c> base.Start()</c>.
		/// </remarks>
		protected void Start ()
		{
			_renderer = GetComponent<LineRenderer>();
		}

		protected void Update()
		{
			CheckError();
		}

		#endregion

		#region Private definitions

		private LineRenderer _renderer;
		
		private void CheckError()
		{
			Transform first = FirstTube.Fragments.Last().EndPoint.transform;
			Transform second = (SecondTube == null) ? Cursor : SecondTube.Fragments.Last().EndPoint.transform;

			Vector3 distanceVector = first.position - second.position; 
			if (distanceVector.magnitude > DistanceThreshold || SecondTube == null)
			{
				ShowError(first, second);
				return;
			}
			
			Quaternion rotation1 = Quaternion.Inverse(first.rotation);
			Quaternion rotation2 = second.rotation;

			if (Quaternion.Angle(rotation1, rotation2) > AngleThreshold)
			{
				ShowError(first, second);
			}
		}
		
		private void ShowError(Transform first, Transform second)
		{
			_renderer.SetPositions(new [] { first.position, second.position });
		}

		#endregion
	}
}
