// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

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

		#region Unity event functions
		
		private void Update()
		{
			if (_isInTrigger)
			{
				_counter = 0;
				Owner.OnTubeCollisionEnter();
			}
			else if (_counter >= 5)
			{
				Owner.OnTubeCollisionExit();
			}

			_counter++;

			_isInTrigger = false;
		}

		private void OnTriggerStay(Collider other)
		{
			TubeFragmentCollider otherCollider = other.GetComponent<TubeFragmentCollider>();
			if (!otherCollider || otherCollider.Owner == Owner || IsNearFragment(otherCollider.Owner)) return;
			
			_isInTrigger = true;
		}


		#endregion

		#region Private definitions

		private int _numberOfCollisions;
		private bool _isInTrigger;
		private int _counter;
		
		private bool IsNearFragment(TubeFragment other)
		{
			Tube tube = Owner.Owner; // TODO: FIX NRE

			return tube.GetNextFragment(Owner) == other || tube.GetPreviousFragment(Owner) == other;
		}

		#endregion
	}
}
