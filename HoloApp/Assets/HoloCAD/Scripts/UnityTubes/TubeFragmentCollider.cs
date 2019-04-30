using UnityEngine;

namespace HoloCAD.UnityTubes
{
	/// <summary>
	/// Класс коллайдера для участка трубы.
	/// Проверяет есть ли пересечения с другими трубами и сообщяет о них хозяину.
	/// </summary>
	public class TubeFragmentCollider : MonoBehaviour
	{
		public TubeFragment Owner;

		#region Unity event functions

		private void OnTriggerEnter(Collider other)
		{
			TubeFragmentCollider otherCollider = other.GetComponent<TubeFragmentCollider>();
			if (!otherCollider || otherCollider.Owner == Owner || IsNearFragment(otherCollider.Owner)) return;
			numberOfCollisions++;
			Owner.OnTubeCollisionEnter();
		}

		private void OnTriggerExit(Collider other)
		{
			TubeFragmentCollider otherCollider = other.GetComponent<TubeFragmentCollider>();
			if (!otherCollider || otherCollider.Owner == Owner || IsNearFragment(otherCollider.Owner)) return;
			numberOfCollisions--;
			if (numberOfCollisions == 0)
			{
				Owner.OnTubeCollisionExit();
			}
		}

		private bool IsNearFragment(TubeFragment other)
		{
			Tube tube = Owner.Owner;

			return tube.GetNextFragment(Owner) == other || tube.GetPreviousFragment(Owner) == other;
		}

		#endregion

		#region Private definitions

		private int numberOfCollisions;

		#endregion
	}
}
