using System;
using UnityEngine;

namespace HoloCAD.UI
{
	public class ColorButton : Button3D
	{
		private MeshRenderer _renderer;
		
		public Material DisabledMaterial;
		public Material EnabledMaterial;
		public Material HoveredMaterial;
		public Material PressedMaterial;

		public override ButtonState State
		{
			get { return _state; }
			protected set
			{
				if (value == _state) return;

				_state = value;
				switch (State)
				{
					case ButtonState.Disabled:
						_renderer.material = DisabledMaterial;
						break;
					case ButtonState.Enabled:
						_renderer.material = EnabledMaterial;
						break;
					case ButtonState.Hovered:
						_renderer.material = HoveredMaterial;
						break;
					case ButtonState.Pressed:
						_renderer.material = PressedMaterial;
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
		}

		protected override void Start ()
		{
			_renderer = GetComponent<MeshRenderer>();
			base.Start();
		}

		private void Update()
		{
			base.Update();
			transform.LookAt(Camera.main.transform.position, Vector3.up);
		}
	}
}
