using System.Collections;
using System.Collections.Generic;
using HoloCAD.UI;
using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity.Receivers;
using UnityEngine;

namespace HoloStand
{
	public class InstructionsController : InteractionReceiver
	{
		private List<int> _actions;

		private int _stepNumber = -1;

		public Material Disabled;
		public Material AllowedEnabled;
		public Material DisallowedEnabled;
		public Material AllowedHovered;
		public Material DisallowedHovered;
		public Material Pressed;
		public LinePointer Pointer;
		public AudioSource Headphones;
		public AudioClip ErrorSound;
		public AudioClip FinishSound;
		
		// Use this for initialization
		void Start () {
			SetAllowed(-1);
		}
	
		// Update is called once per frame
		void Update () {
		
		}

		public void StartInstruction(List<int> instruction = null)
		{
			if (instruction != null)
			{
				_actions = instruction;	
			}
			_stepNumber = 0;
			SetAllowed(_actions[_stepNumber]);
		}

		protected override void InputDown(GameObject obj, InputEventData eventData)
		{
			base.InputDown(obj, eventData);

			if (_stepNumber < 0 || _stepNumber >= _actions.Count) return;

			if (_actions[_stepNumber] == GetControlNumber(obj))
			{
				if (++_stepNumber == _actions.Count)
				{
					OnFinish();
				}
				else
				{
					SetAllowed(_actions[_stepNumber]);	
				}
			}
			else
			{
				OnError();
			}
		}

		protected virtual void OnError()
		{
			_stepNumber = 0;
			SetAllowed(_actions[_stepNumber]);
			Headphones.PlayOneShot(ErrorSound);
		}

		protected virtual void OnFinish()
		{
			SetAllowed(-1);
			Headphones.PlayOneShot(FinishSound);
		}

		private int GetControlNumber(GameObject obj)
		{
			for (int i = 0; i < interactables.Count; i++)
			{
				if (obj == interactables[i]) return i;
			}

			return -1;
		}

		private void SetAllowed(int controllerNumber)
		{
			Pointer.Target = null;
			for (int i = 0; i < interactables.Count; i++)
			{
				ColorButton button = interactables[i].GetComponent<ColorButton>();
				button.DisabledMaterial = Disabled;
				button.PressedMaterial = Pressed;
				if (i == controllerNumber)
				{
					Pointer.Target = interactables[i];
					button.EnabledMaterial = AllowedEnabled;
					button.HoveredMaterial = AllowedHovered;
				}
				else
				{
					button.EnabledMaterial = DisallowedEnabled;
					button.HoveredMaterial = DisallowedHovered;
				}
			}
		}
	}
}
