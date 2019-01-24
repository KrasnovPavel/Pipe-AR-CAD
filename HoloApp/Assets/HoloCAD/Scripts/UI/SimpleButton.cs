using UnityEngine;

namespace HoloCAD.UI
{
	/// <inheritdoc />
	/// <summary>
	/// Простая трехмерная кнопка.
	/// </summary>
	public class SimpleButton : Button3D {
		private static readonly int Fade = Shader.PropertyToID("_Fade");
		
		/// <inheritdoc />
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

		/// <inheritdoc />
		protected override void Start()
		{
			base.Start();
		}

		/// <inheritdoc />
		protected override void Update()
		{
			base.Update();
		}
	}
}
