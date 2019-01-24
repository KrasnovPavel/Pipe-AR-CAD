using UnityEngine;

namespace HoloCAD.UI
{
	public class SimpleButton : Button3D {
		private static readonly int Fade = Shader.PropertyToID("_Fade");
		
		public override ButtonState State
		{
			get { return _state; }
			protected set
			{
				if (_state == value) return;

				_state = value;
				if (Application.isPlaying)
				{
					ButtonRenderer.material.SetFloat(Fade, (float)State);
				}
			}
		}

		protected override void Start()
		{
			base.Start();
		}

		protected override void Update()
		{
			base.Update();
		}
	}
}
