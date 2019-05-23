// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using HoloCAD.UnityTubes;
using HoloCore.UI;
using JetBrains.Annotations;
using UnityEngine;

namespace HoloCAD.UI
{
	/// <inheritdoc />
	/// <summary> Класс, отображающий кнопки и информацию о фланце. </summary>
	[RequireComponent(typeof(BendedTubeFragment))]
	public class BendedTubeFragmentControlPanel : TubeFragmentControlPanel {
		/// <summary> Панель с кнопками и информацией о трубе. </summary>
		[Tooltip("Панель с кнопками и информацией о трубе.")]
		public GameObject ButtonBar;

		/// <summary> Объект, отображающий текстовые данные о участке трубе. </summary>
		[Tooltip("Объект, отображающий текстовые данные о участке трубе.")]
		public TextMesh TextLabel;

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
		
		/// <inheritdoc />
		protected override void CalculateBarPosition()
		{
			if (_camera == null) return;

			Vector3 endPointPosition = ButtonBar.transform.parent.position;
			
			Vector3 direction = (_camera.transform.position - endPointPosition).normalized 
			                    * _fragment.Diameter * 1.1f;
			
			Quaternion rotation = Quaternion.FromToRotation(Vector3.back, direction);
			ButtonBar.transform.rotation = Quaternion.Euler(0, rotation.eulerAngles.y, 0);
			ButtonBar.transform.position = new Vector3(direction.x, 0, direction.z) + endPointPosition;
		}

		/// <inheritdoc />
		protected override void CalculateLine()
		{
			// TODO: Calculate lines for bended tube 
		}

		/// <inheritdoc />
		protected override void SetText()
		{
			TextLabel.text = $"A:{_fragment.Angle} B:{_fragment.RotationAngle}";
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

			if (IncreaseAngleButton != null)
			{
				IncreaseAngleButton.OnClick = delegate { _fragment.IncreaseAngle(); };
			}
			if (DecreaseAngleButton != null)
			{
				DecreaseAngleButton.OnClick = delegate { _fragment.DecreaseAngle(); };
			}
			if (TurnClockwiseButton != null)
			{
				TurnClockwiseButton.OnClick = delegate { _fragment.TurnClockwise(); };
			}
			if (TurnAnticlockwiseButton != null)
			{
				TurnAnticlockwiseButton.OnClick = delegate { _fragment.TurnAnticlockwise(); };
			}
			if (ChangeRadiusButton != null)
			{
				ChangeRadiusButton.OnClick = delegate { _fragment.ChangeRadius(); };
			}
		}

		protected override void CheckIsButtonsEnabled()
		{
			if (ConnectTubesButton != null)
			{
				ConnectTubesButton.SetEnabled(!_fragment.Owner.HasTubesConnector 
				                              && !TubeUnityManager.HasActiveTubesConnector);
			}
			
			if (AddBendFragmentButton != null) AddBendFragmentButton.SetEnabled(!_fragment.HasChild);
			if (AddDirectFragmentButton != null) AddDirectFragmentButton.SetEnabled(!_fragment.HasChild);
		}

		#region Unity event functions

		/// <inheritdoc />
		protected override void Start()
		{
			base.Start();
			_camera = Camera.main;
			_fragment = GetComponent<BendedTubeFragment>();
		}

		private void OnDisable()
		{
			if (ButtonBar != null) ButtonBar.SetActive(false);
		}

		private void OnEnable()
		{
			if (ButtonBar != null) ButtonBar.SetActive(true);
		}

		#endregion

		#region Private definitions

		private Camera _camera;
		private BendedTubeFragment _fragment;

		#endregion
	}
}
