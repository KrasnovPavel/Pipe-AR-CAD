﻿using System;
using UnityEngine;

namespace HoloCAD.UI
{
	/// <inheritdoc />
	/// <summary>
	/// Кнопка, которая при наведении и нажатии меняет цвет.
	/// </summary>
	public class ColorButton : Button3D
	{
		private Material _disabledMaterial;
		private Material _enabledMaterial;
		private Material _hoveredMaterial;
		private Material _pressedMaterial;

		public Material DefaultDisabled;
		public Material DefaultEnabled;
		public Material DefaultHovered;
		public Material DefaultPressed;

		/// <inheritdoc />
		public override ButtonState State
		{
			get { return _state; }
			protected set
			{
				if (value == _state) return;

				_state = value;
				SetCurrentMaterial();
			}
		}

		/// <summary> Материал выключенной кнопки. </summary>
		public Material DisabledMaterial
		{
			get { return _disabledMaterial; }
			set 
			{ 
				_disabledMaterial = value; 
				SetCurrentMaterial();
				
			}
		}

		/// <summary> Материал включенной кнопки. </summary>
		public Material EnabledMaterial
		{
			get { return _enabledMaterial; }
			set
			{
				_enabledMaterial = value;
				SetCurrentMaterial();
			}
		}

		/// <summary> Материал кнопки, на которую наведен курсор. </summary>
		public Material HoveredMaterial
		{
			get { return _hoveredMaterial; }
			set
			{
				_hoveredMaterial = value;
				SetCurrentMaterial();
			}
		}

		/// <summary> Материал нажатой кнопки. </summary>
		public Material PressedMaterial
		{
			get { return _pressedMaterial; }
			set
			{
				_pressedMaterial = value;
				SetCurrentMaterial();
			}
		}

		private void SetCurrentMaterial()
		{
			switch (State)
			{
				case ButtonState.Disabled:
					ButtonRenderer.sharedMaterial = DisabledMaterial;
					break;
				case ButtonState.Enabled:
					ButtonRenderer.sharedMaterial = EnabledMaterial;
					break;
				case ButtonState.Hovered:
					ButtonRenderer.sharedMaterial = HoveredMaterial;
					break;
				case ButtonState.Pressed:
					ButtonRenderer.sharedMaterial = PressedMaterial;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		/// <inheritdoc />
		protected override void Start ()
		{
			_enabledMaterial  = DefaultEnabled;
			_disabledMaterial = DefaultDisabled;
			_hoveredMaterial  = DefaultHovered;
			_pressedMaterial  = DefaultPressed;
			base.Start();
		}

		/// <inheritdoc />
		protected override void Update()
		{
			base.Update();
			transform.LookAt(Camera.main.transform.position, Vector3.up);
		}
	}
}
