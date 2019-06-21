// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System.Collections.Generic;
using UnityEngine;

namespace HoloCAD.UnityTubes
{
	/// <summary>
	/// Класс коллайдера для участка трубы.
	/// Проверяет есть ли пересечения с другими трубами и сообщяет о них хозяину.
	/// </summary>
	public class TubeFragmentCollider : MonoBehaviour
	{
		/// <summary> Участок трубы к которому присоединён коллайдер. </summary>
		public TubeFragment Owner;

		/// <summary> Вызывается уничтожаемым коллайдером, у всех коллайдеров с которыми он пересекался. </summary>
		/// <remarks>
		/// Функция написана для обхода бага unity, когда при уничтожении объекта не вызывается OnTriggerExit.
		/// </remarks>
		/// <param name="otherCollider"> Коллайдер, который будет уничтожен. </param>
		public void OnOtherColliderDestroyed(TubeFragmentCollider otherCollider)
		{
			_otherColliders.Remove(otherCollider);
			
			if (_otherColliders.Count == 0)
			{
				IsInTrigger = false;
				Owner.OnTubeCollisionExit();
			}
		}

		/// <summary> Ещё раз проверяет, есть ли колизия с переданным коллайдером. </summary>
		/// <remarks>
		/// Функция написана для обхода бага unity,
		/// когда OnTriggerEnter вызывается до OnStart и объект может быть не инициализирован.
		/// </remarks>
		/// <param name="otherCollider"></param>
		public void CheckAgain(TubeFragmentCollider otherCollider)
		{
			if (otherCollider.Owner == Owner || IsNearFragment(otherCollider.Owner)) return;
			
			if (_otherColliders.Count == 0)
			{
				IsInTrigger = true;
				Owner.OnTubeCollisionEnter();
			}
		
			_otherColliders.Add(otherCollider);
		}
		
		public bool IsInTrigger { get; private set; }
		
		#region Unity event functions

		private void Start()
		{
			foreach (Collider otherCollider in _otherCollidersBeforeStart)
			{
				otherCollider.GetComponent<TubeFragmentCollider>()?.CheckAgain(this);
				OnTriggerEnter(otherCollider);
			}
		}

		private void OnTriggerEnter(Collider other)
		{
			// Если Owner равен null, значит объект не успел инициализироваться. 
			if (Owner == null)
			{
				_otherCollidersBeforeStart.Add(other);
				return;
			}
			
			TubeFragmentCollider otherCollider = other.GetComponent<TubeFragmentCollider>();

			// Коллизия не с трубой, игнорируем.
			if (otherCollider == null) return;
			
			// Второй коллайдер не инициализирован, ждем вызова CheckAgain.
			if (otherCollider.Owner == null) return;

			// Коллизия с соседним участком или самим собой(для погиба), игнорируем.
			if (otherCollider.Owner == Owner || IsNearFragment(otherCollider.Owner)) return;

			if (_otherColliders.Count == 0)
			{
				IsInTrigger = true;
				Owner.OnTubeCollisionEnter();
			}
			
			_otherColliders.Add(otherCollider);
		}

		private void OnTriggerExit(Collider other)
		{
			TubeFragmentCollider otherCollider = other.GetComponent<TubeFragmentCollider>();
			
			bool isOtherTubeInvalid = otherCollider == null || otherCollider.Owner == Owner 
			                                                || IsNearFragment(otherCollider.Owner);
			if (isOtherTubeInvalid) return;

			_otherColliders.Remove(otherCollider);
			
			if (_otherColliders.Count == 0)
			{
				IsInTrigger = false;
				Owner.OnTubeCollisionExit();
			}
		}

		private void OnDestroy()
		{
			foreach (TubeFragmentCollider otherCollider in _otherColliders)
			{
				otherCollider.OnOtherColliderDestroyed(this);
			}
		}

		private void OnDisable()
		{
			IsInTrigger = false;
			Owner.OnTubeCollisionExit();
			foreach (TubeFragmentCollider otherCollider in _otherColliders)
			{
				otherCollider.OnOtherColliderDestroyed(this);
			}
		}

		#endregion

		#region Private definitions

		private readonly List<TubeFragmentCollider> _otherColliders = new List<TubeFragmentCollider>();
		private readonly List<Collider> _otherCollidersBeforeStart = new List<Collider>();
		
		/// <summary> Является ли переданный фрагмент соседним для хозяина этого коллайдера? </summary>
		/// <param name="other"> Фрагмент трубы, который надо проверить. </param>
		/// <returns></returns>
		private bool IsNearFragment(TubeFragment other)
		{
			Tube tube = Owner.Owner;
			return tube.GetNextFragment(Owner) == other || tube.GetPreviousFragment(Owner) == other;
		}

		#endregion
	}
}
