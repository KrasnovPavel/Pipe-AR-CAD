using HoloCAD.UnityTubes;
using HoloCore;
using UnityEngine;

namespace HoloCAD.UI
{
	/// <inheritdoc />
	/// <summary> Класс, отображающий кнопки и информацию о фланце. </summary>
	[RequireComponent(typeof(LineRenderer)), RequireComponent(typeof(StartTubeFragment))]
	public class StartTubeFragmentControlPanel : TubeFragmentControlPanel {
		/// <summary> Панель с кнопками и информацией о трубе. </summary>
		[Tooltip("Панель с кнопками и информацией о трубе.")]
		public GameObject ButtonBar;

		/// <summary> Объект, отображающий текстовые данные о участке трубе. </summary>
		[Tooltip("Объект, отображающий текстовые данные о участке трубе.")]
		public TextMesh TextLabel;

		/// <inheritdoc />
		protected override void CalculateBarPosition()
		{
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
			TextLabel.text = $"Ø:{_fragment.Diameter:0.000}м";
		}

		#region Unity event functions

		/// <inheritdoc />
		protected override void Start()
		{
			_camera = Camera.main;
			_lineRenderer = GetComponent<LineRenderer>();
			_fragment = GetComponent<StartTubeFragment>();
		}

		private void OnDisable()
		{
			if (_lineRenderer != null) _lineRenderer.enabled = false;
		}

		private void OnEnable()
		{
			if (_lineRenderer != null) _lineRenderer.enabled = true;
		}

		#endregion

		#region Private definitions

		private Camera _camera;
		private LineRenderer _lineRenderer;
		private StartTubeFragment _fragment;

		#endregion
	}
}
