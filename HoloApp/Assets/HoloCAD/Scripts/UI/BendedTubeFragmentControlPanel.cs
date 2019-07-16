// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System.ComponentModel;
using HoloCAD.UnityTubes;
using HoloCore.UI;
using JetBrains.Annotations;
using UnityEngine;

namespace HoloCAD.UI
{
	/// <inheritdoc />
	/// <summary> Класс, отображающий кнопки и информацию о фланце. </summary>
	[RequireComponent(typeof(BendedTubeFragment))]
	public class BendedTubeFragmentControlPanel : TubeFragmentControlPanel 
	{
		/// <summary> Кнопка увеличения угла погиба. </summary>
		[Tooltip("Кнопка увеличения угла погиба.")]
		[CanBeNull] public Button3D IncreaseAngleButton;
        
		/// <summary> Кнопка уменьшения угла погиба. </summary>
		[Tooltip("Кнопка уменьшения угла погиба.")]
		[CanBeNull] public Button3D DecreaseAngleButton;
        
		/// <summary> Кнопка поворота погиба по часовой стрелке. </summary>
		[Tooltip("Кнопка поворота погиба по часовой стрелке.")]
		[CanBeNull] public Button3D TurnClockwiseButton;
        
		/// <summary> Кнопка поворота погиба против часовой стрелки. </summary>
		[Tooltip("Кнопка поворота погиба против часовой стрелки.")]
		[CanBeNull] public Button3D TurnAnticlockwiseButton;
        
		/// <summary> Кнопка смены радиуса погиба. </summary>
		[Tooltip("Кнопка смены радиуса погиба.")]
		[CanBeNull] public Button3D ChangeRadiusButton;
		
		/// <summary> Шаг изменения угла при нажатии на кнопку. </summary>
		[Tooltip("Шаг изменения угла при нажатии на кнопку.")]
		public float AngleStep = 5f;
		
		/// <inheritdoc />
		protected override void CalculateBarPosition()
		{
			Vector3 endPointPosition = ButtonBar.parent.position;
			
			Vector3 direction = _fragment.Diameter * 1.1f * (_camera.transform.position - endPointPosition).normalized;
			
			Quaternion rotation = Quaternion.FromToRotation(Vector3.back, direction);
			ButtonBar.rotation = Quaternion.Euler(0, rotation.eulerAngles.y, 0);
			ButtonBar.position = new Vector3(direction.x, 0, direction.z) + endPointPosition;
		}

		protected override void InitButtons()
		{
			if (AddBendFragmentButton != null)
			{
				AddBendFragmentButton.OnClick += delegate { _fragment.AddBendFragment(); };
			}
			if (AddDirectFragmentButton != null)
			{
				AddDirectFragmentButton.OnClick += delegate { _fragment.AddDirectFragment(); };
			}
			if (RemoveThisFragmentButton != null)
			{
				RemoveThisFragmentButton.OnClick += delegate { _fragment.RemoveThisFragment(); };
			}
			if (ConnectTubesButton != null)
			{
				ConnectTubesButton.OnClick += delegate { _fragment.Owner.CreateTubesConnector(); };
			}
			if (NextFragmentButton != null)
			{
				NextFragmentButton.OnClick += delegate { TubeManager.SelectNext(); };
			}
			if (PreviousFragmentButton != null)
			{
				PreviousFragmentButton.OnClick += delegate { TubeManager.SelectPrevious(); };
			}
			

			if (IncreaseAngleButton != null)
			{
				IncreaseAngleButton.OnClick = delegate { _fragment.ChangeAngle(AngleStep); };
			}
			if (DecreaseAngleButton != null)
			{
				DecreaseAngleButton.OnClick = delegate { _fragment.ChangeAngle(-AngleStep); };
			}
			if (TurnClockwiseButton != null)
			{
				TurnClockwiseButton.OnClick = delegate { _fragment.TurnAround(AngleStep); };
			}
			if (TurnAnticlockwiseButton != null)
			{
				TurnAnticlockwiseButton.OnClick = delegate { _fragment.TurnAround(-AngleStep); };
			}
			if (ChangeRadiusButton != null)
			{
				ChangeRadiusButton.OnClick = delegate { _fragment.ChangeRadius(); };
			}
		}

		#region Unity event functions

		/// <inheritdoc />
		protected override void Start()
		{
			base.Start();
			_camera = Camera.main;
			_fragment = GetComponent<BendedTubeFragment>();
			_fragment.PropertyChanged += delegate(object sender, PropertyChangedEventArgs args)
			{
				switch (args.PropertyName)
				{
					case nameof(BendedTubeFragment.Angle):
					case nameof(BendedTubeFragment.RotationAngle):
						SetText();
						break;
				}
			};
			CheckIsButtonsEnabled(_fragment);
			SetText();
		}

		#endregion

		#region Private definitions

		private Camera _camera;
		private BendedTubeFragment _fragment;

		private void SetText()
		{
			if (TextLabel != null) TextLabel.text = $"A:{_fragment.Angle:0.00} B:{_fragment.RotationAngle:0.00}";
		}

		#endregion
	}
}
