// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using HoloCAD.UnityTubes;
using HoloCore;
using HoloCore.UI;
using JetBrains.Annotations;
using UnityEngine;

namespace HoloCAD.UI
{
	/// <inheritdoc />
	/// <summary> Класс, отображающий кнопки и информацию о фланце. </summary>
	[RequireComponent(typeof(LineRenderer)), RequireComponent(typeof(StartTubeFragment))]
	public class StartTubeFragmentControlPanel : TubeFragmentControlPanel {
		/// <summary> Панель с кнопками и информацией о трубе. </summary>
		[Tooltip("Панель с кнопками и информацией о трубе.")]
		[CanBeNull] public GameObject ButtonBar;

		/// <summary> Объект, отображающий текстовые данные о участке трубе. </summary>
		[Tooltip("Объект, отображающий текстовые данные о участке трубе.")]
		[CanBeNull] public TextMesh TextLabel;

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
			if (ButtonBar == null) return;
			
			Vector3 barPosition = Vector3.zero;
			Vector3 cameraPosition = _camera.transform.position - transform.position; 

			barPosition.x = (cameraPosition.ProjectOn(transform.right) > 0) ? ( _fragment.Diameter/2 + 0.3f) 
																			: (-_fragment.Diameter/2 - 0.3f);
			
			barPosition.y = (cameraPosition.ProjectOn(transform.up) > 0) ? ( _fragment.Diameter/2 + 0.2f) 
																	     : (-_fragment.Diameter/2 - 0.2f);

			ButtonBar.transform.localPosition = barPosition;
		}

		/// <inheritdoc />
		protected override void CalculateLine()
		{
			if (ButtonBar == null) return;
			
			Vector3 cameraPosition = _camera.transform.position - transform.position;
			Vector3 barPosition = ButtonBar.transform.localPosition;
			Vector3[] linePositions = { Vector3.zero, Vector3.zero, Vector3.zero };

			barPosition.x += (cameraPosition.ProjectOn(transform.right) > 0) ? -0.25f : +0.25f;
			barPosition.y -= 0.07f;
			float angle = Vector3.Angle(Vector3.left, barPosition);
			if (cameraPosition.ProjectOn(transform.up) > 0)
			{
				angle = 360f - angle;
			}
			linePositions[0] = Quaternion.Euler(0f, 0f, angle) * Vector3.right * _fragment.Diameter / 2;
			linePositions[1] = barPosition;
			linePositions[2] = barPosition 
			                   + ((cameraPosition.ProjectOn(transform.right) > 0) ? new Vector3(0.5f, 0f, 0f)
																				 : new Vector3(-0.5f, 0f, 0f));
			_lineRenderer.SetPositions(linePositions);
			_lineRenderer.startWidth = _lineRenderer.endWidth = 0.1f * _fragment.Diameter/2;
		}

		/// <inheritdoc />
		protected override void SetText()
		{
			if (TextLabel != null) TextLabel.text = $"Ø:{_fragment.Diameter:0.000}м";
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
			_lineRenderer = GetComponent<LineRenderer>();
			_fragment = GetComponent<StartTubeFragment>();
		}

		private void OnDisable()
		{
			if (_lineRenderer != null) _lineRenderer.enabled = false;
			if (ButtonBar != null) ButtonBar.SetActive(false);
		}

		private void OnEnable()
		{
			if (_lineRenderer != null) _lineRenderer.enabled = true;
			if (ButtonBar != null) ButtonBar.SetActive(true);
		}

		#endregion

		#region Private definitions

		private Camera _camera;
		private LineRenderer _lineRenderer;
		private StartTubeFragment _fragment;

		#endregion
	}
}
