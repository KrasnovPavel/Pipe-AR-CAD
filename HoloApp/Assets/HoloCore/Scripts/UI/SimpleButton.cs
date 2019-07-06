// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

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
			get => base.State;
			protected set
			{
				base.State = value;
				if (Application.isPlaying && ButtonRenderer != null)
				{
					ButtonRenderer.material.SetFloat(Fade, (float) State);
				}
			}
		}
	}
}
