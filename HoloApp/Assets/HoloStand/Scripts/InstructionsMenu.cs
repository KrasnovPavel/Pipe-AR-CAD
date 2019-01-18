using System.Collections;
using System.Collections.Generic;
using HoloCAD.UI;
using HoloStand;
using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity.Receivers;
using UnityEngine;

namespace HoloStand
{
	public class InstructionsMenu : InteractionReceiver
	{
		private List<InstructionsLoader.Instruction> _instructions;
		public InstructionsController Controller;
	
		// Use this for initialization
		void Start ()
		{
			_instructions = InstructionsLoader.GetInstructionsForScene("Demo");
			Debug.Log(interactables.Count);
			interactables[0].GetComponent<Button3D>().Text = _instructions[0].name;
			interactables[1].GetComponent<Button3D>().Text = _instructions[1].name;
		}

		protected override void InputClicked(GameObject obj, InputClickedEventData eventData)
		{
			base.InputClicked(obj, eventData);

			switch (obj.name)
			{
				case "FirstButton":
					Controller.StartInstruction(_instructions[0].actions);
					break;
				case "SecondButton":
					Controller.StartInstruction(_instructions[1].actions);
					break;
			}
		}
	}
}
