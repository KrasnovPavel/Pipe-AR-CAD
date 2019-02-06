using UnityEngine;

namespace HoloCAD.UnityTubes
{
	public class TubeFragmentCollider : MonoBehaviour
	{
		private int numberOfCollisions;
		public TubeFragment Owner;

		private void OnTriggerEnter(Collider other)
		{
			if (!other.GetComponent<TubeFragmentCollider>()) return;
			numberOfCollisions++;
			Owner.OnTubeCollisionEnter();
		}

		private void OnTriggerExit(Collider other)
		{
			if (!other.GetComponent<TubeFragmentCollider>()) return;
			numberOfCollisions--;
			if (numberOfCollisions == 0)
			{
				Owner.OnTubeCollisionExit();
			}
		}
	}
}
