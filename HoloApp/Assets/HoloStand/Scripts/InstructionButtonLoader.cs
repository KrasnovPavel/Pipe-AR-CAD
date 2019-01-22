using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity.Receivers;
using UnityEngine;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using UnityEngine;

namespace  HoloStand
{
	
	public class InstructionButtonLoader : InteractionReceiver {
	
		[Serializable]
		[SuppressMessage("ReSharper", "InconsistentNaming")]
		private class InstructionsSet
		{
			public List<InstructionBut> instructionsBut;
		}
        
		[Serializable]
		[SuppressMessage("ReSharper", "InconsistentNaming")]
		public class InstructionBut
		{
			public string nameBt;

			public List<int> actionsBt;
		}

		public static List<InstructionBut> GetInstructionsForButton()
		{
            
			byte[] data = UnityEngine.Windows.File.ReadAllBytes(Path.Combine(Application.streamingAssetsPath, "./StandInstructions/Instruction.json"));
			string jsonTextFile = System.Text.Encoding.UTF8.GetString(data);
			return JsonUtility.FromJson<InstructionsSet>(jsonTextFile).instructionsBut;
		}
	}

	
}

