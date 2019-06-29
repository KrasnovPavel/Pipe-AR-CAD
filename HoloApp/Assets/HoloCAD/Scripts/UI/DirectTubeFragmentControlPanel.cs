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
	[RequireComponent(typeof(DirectTubeFragment))]
	public class DirectTubeFragmentControlPanel : TubeFragmentControlPanel {
		/// <summary> Панель с кнопками и информацией о трубе. </summary>
		[Tooltip("Панель с кнопками и информацией о трубе.")]
		[CanBeNull] public GameObject ButtonBar;

		/// <summary> Объект, отображающий текстовые данные о участке трубы. </summary>
		[Tooltip("Объект, отображающий текстовые данные о участке трубы.")]
		[CanBeNull] public TextMesh TextLabel;

		/// <summary> Кнопка увеличения длины. </summary>
		[Tooltip("Кнопка увеличения длины.")]
		[CanBeNull] public Button3D IncreaseLengthButton;

		/// <summary> Кнопка уменьшения длины. </summary>
		[Tooltip("Кнопка уменьшения длины.")]
		[CanBeNull] public Button3D DecreaseLengthButton;
		
		/// <summary> Кнопка добавления отростка. </summary>
		[Tooltip("Кнопка добавления отростка.")]
		[CanBeNull] public Button3D AddOutgrowthButton;

		/// <summary> Шаг изменения длины при нажатии на кнопку. </summary>
		[Tooltip("Шаг изменения длины при нажатии на кнопку.")]
		public float LengthStep = 0.05f;
		
		/// <inheritdoc />
		protected override void CalculateBarPosition()
		{
			if (_camera == null || ButtonBar == null) return;

			Vector3 endPointPosition = ButtonBar.transform.parent.position;
			
			Vector3 direction = _fragment.Diameter * 1.1f * (_camera.transform.position - endPointPosition).normalized;
			
			Quaternion rotation = Quaternion.FromToRotation(Vector3.back, direction);
			ButtonBar.transform.rotation = Quaternion.Euler(0, rotation.eulerAngles.y, 0);
			ButtonBar.transform.position = new Vector3(direction.x, 0, direction.z) + endPointPosition;
		}

		/// <inheritdoc />
		protected override void CalculateLine()
		{
		}

		/// <inheritdoc />
		protected override void SetText()
		{
			if (TextLabel != null) TextLabel.text = $"L:{_fragment.Length:0.000}м";
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
			
			
			if (IncreaseLengthButton != null)
			{
				IncreaseLengthButton.OnClick += delegate { _fragment.ChangeLength(LengthStep); };
			}
			if (DecreaseLengthButton != null)
			{
				DecreaseLengthButton.OnClick += delegate { _fragment.ChangeLength(-LengthStep); };
			}
			if (AddOutgrowthButton != null)
			{
				AddOutgrowthButton.OnClick += delegate { _fragment.AddOutgrowth(); };				
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
			if (NextFragmentButton != null) NextFragmentButton.SetEnabled(_fragment.HasChild);
		}

		#region Unity event functions

		/// <inheritdoc />
		protected override void Start()
		{
			base.Start();
			_camera = Camera.main;
			_fragment = GetComponent<DirectTubeFragment>();
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
		private DirectTubeFragment _fragment;

		#endregion
	}
}
