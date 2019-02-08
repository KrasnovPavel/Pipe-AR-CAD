using UnityEngine;

namespace HoloCAD.UnityTubes
{
	public class TubeFragmentCollider : MonoBehaviour
	{
		private int numberOfCollisions;
		public TubeFragment Owner;

		private void OnTriggerEnter(Collider other)
		{
			TubeFragmentCollider otherCollider = other.GetComponent<TubeFragmentCollider>();
			if (!otherCollider || otherCollider.Owner == Owner) return;
			numberOfCollisions++;
			Owner.OnTubeCollisionEnter();
		}

		private void OnTriggerExit(Collider other)
		{
			TubeFragmentCollider otherCollider = other.GetComponent<TubeFragmentCollider>();
			if (!otherCollider || otherCollider.Owner == Owner) return;
			numberOfCollisions--;
			if (numberOfCollisions == 0)
			{
				Owner.OnTubeCollisionExit();
			}
		}
	}
}
