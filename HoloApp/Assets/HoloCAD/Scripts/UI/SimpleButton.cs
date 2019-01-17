using UnityEngine;

namespace HoloCAD.UI
{
	public class SimpleButton : Button3D {
		private static readonly int Fade = Shader.PropertyToID("_Fade");
		public override ButtonState State
		{
			get { return _state; }
			protected set
			{
				if (_state == value) return;

				_state = value;
				gameObject.GetComponent<MeshRenderer>().material.SetFloat(Fade, (float)State);
			}
		}
		
		private void Start()
		{
			State = ButtonState.Enabled;
			_label = transform.Find("Label").GetComponent<TextMesh>();
		}

		private void Update()
		{
			_label.text = Text;
		}
	}
}
