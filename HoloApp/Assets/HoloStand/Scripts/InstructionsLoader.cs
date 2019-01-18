using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using UnityEngine;

namespace HoloStand
{
    public static class InstructionsLoader {

        [Serializable]
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        private class InstructionsSet
        {
            public List<Instruction> instructions;
        }
        
        [Serializable]
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public class Instruction
        {
            public string name;

            public List<int> actions;
        }

        public static List<Instruction> GetInstructionsForScene(string sceneName)
        {
            
            byte[] data = UnityEngine.Windows.File.ReadAllBytes(Path.Combine(Application.streamingAssetsPath, "./StandInstructions/" + sceneName + ".json"));
            string jsonTextFile = System.Text.Encoding.UTF8.GetString(data);
            return JsonUtility.FromJson<InstructionsSet>(jsonTextFile).instructions;
        }
    }
}
