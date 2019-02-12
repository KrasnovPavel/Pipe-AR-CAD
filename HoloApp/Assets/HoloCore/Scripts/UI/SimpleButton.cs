using UnityEngine;

namespace HoloCore.UI
{
	/// <inheritdoc />
	/// <summary> Простая трехмерная кнопка. </summary>
	public class SimpleButton : Button3D {
		private static readonly int Fade = Shader.PropertyToID("_Fade");
		
		/// <inheritdoc />
		public override ButtonState State
		{
			get { return _state; }
			protected set
			{
				_state = value;
				if (Application.isPlaying && ButtonRenderer != null)
				{
					ButtonRenderer.material.SetFloat(Fade, (float) State);
				}
			}
		}
	}
}
