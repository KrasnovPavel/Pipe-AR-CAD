using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace HoloStand
{
	public class LinePointer : MonoBehaviour
	{
		private LineRenderer _renderer;
		
		public GameObject Source;
		public GameObject Target;
		// Use this for initialization
		void Start ()
		{
			_renderer = GetComponent<LineRenderer>();
		}
	
		// Update is called once per frame
		void Update () {
			if (_renderer)
			{
				if (Source && Target)
				{
					_renderer.SetPosition(0, Source.transform.position);
					_renderer.SetPosition(1, Target.transform.position);
				}
				else
				{
					_renderer.SetPositions(new []{Vector3.zero, Vector3.zero});
				}
			}
		}
	}
}
