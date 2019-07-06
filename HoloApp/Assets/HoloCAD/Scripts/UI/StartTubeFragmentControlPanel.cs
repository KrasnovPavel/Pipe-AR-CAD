// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System.ComponentModel;
using HoloCAD.UnityTubes;
using HoloCore;
using HoloCore.UI;
using JetBrains.Annotations;
using UnityEngine;

namespace HoloCAD.UI
{
	/// <inheritdoc />
	/// <summary> Класс, отображающий кнопки и информацию о фланце. </summary>
	/// <remarks> </remarks>
	[RequireComponent(typeof(StartTubeFragment))]
	public class StartTubeFragmentControlPanel : TubeFragmentControlPanel 
	{
		/// <summary> Кнопка увеличения диаметра трубы. </summary>
		[Tooltip("Кнопка увеличения диаметра трубы.")]
		[CanBeNull] public Button3D IncreaseDiameterButton;
        
		/// <summary> Кнопка уменьшения диаметра трубы. </summary>
		[Tooltip("Кнопка уменьшения диаметра трубы.")]
		[CanBeNull] public Button3D DecreaseDiameterButton;
        
		/// <summary> Кнопка перехода в режим размещения трубы. </summary>
		[Tooltip("Кнопка перехода в режим размещения трубы.")]
		[CanBeNull] public Button3D StartPlacingButton;
		
		/// <summary> Кнопка сохранения сцены. </summary>
		[Tooltip("Кнопка добавления объекта отображения расстояния между трубами.")]
		[CanBeNull] public Button3D SaveSceneButton;
		
		/// <summary> Кнопка создания новой трубы. </summary>
		[Tooltip("Кнопка создания новой трубы.")]
		[CanBeNull] public Button3D CreateTubeButton;
		
		/// <inheritdoc />
		protected override void CalculateBarPosition()
		{
			Vector3 barPosition = Vector3.zero;
			Vector3 cameraPosition = _camera.position - transform.position; 

			barPosition.x = (cameraPosition.ProjectOn(transform.right) > 0) ? ( _fragment.Diameter/2 + 0.3f) 
																			: (-_fragment.Diameter/2 - 0.3f);
			
			barPosition.y = (cameraPosition.ProjectOn(transform.up) > 0) ? ( _fragment.Diameter/2 + 0.2f) 
																	     : (-_fragment.Diameter/2 - 0.2f);

			if (ButtonBar.localPosition.FloatEquals(barPosition)) return;
			
			ButtonBar.localPosition = barPosition;
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
			
			
			if (CreateTubeButton != null)
			{
				CreateTubeButton.OnClick += delegate { _fragment.CreateTube(); };
			}
			if (SaveSceneButton != null)
			{
				SaveSceneButton.OnClick += delegate { TubeManager.SaveScene(); };
			}
			if (IncreaseDiameterButton != null)
			{
				IncreaseDiameterButton.OnClick += delegate { _fragment.IncreaseDiameter(); };
			}
			if (DecreaseDiameterButton != null)
			{
				DecreaseDiameterButton.OnClick += delegate { _fragment.DecreaseDiameter(); };
			}
			if (StartPlacingButton != null)
			{
				StartPlacingButton.OnClick += delegate { _fragment.StartPlacing(); };
			}
		}

		#region Unity event functions

		/// <inheritdoc />
		protected override void Start()
		{
			base.Start();
			_camera = Camera.main.transform;
			_fragment = GetComponent<StartTubeFragment>();
			_fragment.PropertyChanged += delegate(object sender, PropertyChangedEventArgs args)
			{
				if (args.PropertyName == nameof(_fragment.Diameter))
				{
					SetText();
				}
			};
			
			CheckIsButtonsEnabled(_fragment);
			SetText();
		}

		#endregion

		#region Private definitions

		private Transform _camera;
		private StartTubeFragment _fragment;

		private void SetText()
		{
			if (TextLabel != null) TextLabel.text = $"Ø:{_fragment.Diameter:0.000}м";
		}

		#endregion
	}
}
