using System.Collections.Generic;
using HoloToolkit.Unity;
using UnityEngine;

namespace HoloCAD
{
    public class MarksHandler : MonoBehaviour
    {
        public List<Mark> AllMarks = new List<Mark>();
        public List<Vector3> PositionsOfMarks = new List<Vector3>();
        public List<Vector3> RotationsOfMarks = new List<Vector3>();
    
        private void Update()
        {
            int markId = AllMarks.FindIndex(obj => obj.IsActive);

            if (markId == -1) return;
        
            Transform currentMark = AllMarks[markId].transform;

            transform.SetParent(currentMark, false);
            transform.localScale = new Vector3(1 / currentMark.lossyScale.x, 
                1 / currentMark.lossyScale.z, 
                1 / currentMark.lossyScale.z);
            transform.localPosition = PositionsOfMarks[markId];
            transform.localRotation = Quaternion.Euler(RotationsOfMarks[markId]);

            transform.SetParent(null, true);
        }
    }
}
