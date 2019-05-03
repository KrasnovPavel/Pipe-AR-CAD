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
		public GameObject ButtonBar;

		/// <summary> Объект, отображающий текстовые данные о участке трубе. </summary>
		[Tooltip("Объект, отображающий текстовые данные о участке трубе.")]
		public TextMesh TextLabel;

		/// <summary> Кнопка увеличения длины. </summary>
		[Tooltip("Кнопка увеличения длины.")]
		[CanBeNull] public Button3D IncreaseLengthButton;

		/// <summary> Кнопка уменьшения длины. </summary>
		[Tooltip("Кнопка уменьшения длины.")]
		[CanBeNull] public Button3D DecreaseLengthButton;
		
		/// <inheritdoc />
		protected override void CalculateBarPosition()
		{
			//TODO:  Calculate bar position for direct tube
		}

		/// <inheritdoc />
		protected override void CalculateLine()
		{
			//TODO: Calculate line for direct tube
		}

		/// <inheritdoc />
		protected override void SetText()
		{
			TextLabel.text = $"L:{_fragment.Length:0.000}м";
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
			
			
			if (IncreaseLengthButton != null)
			{
				IncreaseLengthButton.OnClick += delegate { _fragment.IncreaseLength(); };
			}
			if (DecreaseLengthButton != null)
			{
				DecreaseLengthButton.OnClick += delegate { _fragment.DecreaseLength(); };
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
