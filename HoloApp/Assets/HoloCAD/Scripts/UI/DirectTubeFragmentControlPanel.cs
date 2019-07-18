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
	[RequireComponent(typeof(DirectTubeFragment))]
	public class DirectTubeFragmentControlPanel : TubeFragmentControlPanel 
	{
		/// <summary> Кнопка увеличения длины. </summary>
		[Tooltip("Кнопка увеличения длины.")]
		[CanBeNull] public Button3D IncreaseLengthButton;

		/// <summary> Кнопка уменьшения длины. </summary>
		[Tooltip("Кнопка уменьшения длины.")]
		[CanBeNull] public Button3D DecreaseLengthButton;
		
		/// <summary> Кнопка добавления отростка. </summary>
		[Tooltip("Кнопка добавления отростка.")]
		[CanBeNull] public Button3D AddOutgrowthButton;
		
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
			
			
			if (IncreaseLengthButton != null)
			{
				IncreaseLengthButton.OnClick += delegate { _fragment.ChangeLength(Steps.Linear); };
			}
			if (DecreaseLengthButton != null)
			{
				DecreaseLengthButton.OnClick += delegate { _fragment.ChangeLength(-Steps.Linear); };
			}
			if (AddOutgrowthButton != null)
			{
				AddOutgrowthButton.OnClick += delegate { _fragment.AddOutgrowth(); };				
			}
		}

		#region Unity event functions

		/// <inheritdoc />
		protected override void Start()
		{
			base.Start();
			_camera = Camera.main;
			_fragment = GetComponent<DirectTubeFragment>();
			_fragment.PropertyChanged += delegate(object sender, PropertyChangedEventArgs args)
			{
				if (args.PropertyName == nameof(DirectTubeFragment.Length))
				{
					SetText();
				}
			};
			
			CheckIsButtonsEnabled(_fragment);
			SetText();
		}

		#endregion

		#region Private definitions

		private Camera _camera;
		private DirectTubeFragment _fragment;

		private void SetText()
		{
			if (TextLabel != null) TextLabel.text = $"L: {_fragment.Length:0.000}";
		}

		#endregion
	}
}
