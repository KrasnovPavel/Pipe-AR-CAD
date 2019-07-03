using System;
using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity;
using UnityEngine;
using Vuforia;
public class MarksHandler : MonoBehaviour
{
    public GameObject[] AllMarks;
    public List<GameObject> ActiveMarks = new List<GameObject>();
    public Vector3[] PositionsOfMarks;
    public Vector3[] RotationsOfMarks;
    public Vector3 PrevRotation;
    public Vector3 PrevCenterVector;
    public Vector3 PrevPos;
    private void Update()
    {
        if (ActiveMarks.Count == 0)
        {
            if (PrevPos != null)
            {
               // Quaternion RotationOfVector = Quaternion.Euler(PrevRotation.x,PrevRotation.y,PrevRotation.z);
               // Vector3 VectorBetweenCenterAndMark = RotationOfVector * new Vector3(PrevCenterVector.x,PrevCenterVector.y,PrevCenterVector.z);
                transform.position = new Vector3(PrevPos.x-PrevCenterVector.x,PrevPos.y-PrevCenterVector.y,PrevPos.z-PrevCenterVector.z);
                transform.rotation = Quaternion.Euler(PrevRotation.x, PrevRotation.y,PrevRotation.z);
            }
           
        }
        else
        {
            var rendererComponents = GetComponentsInChildren<Renderer>(true);
            foreach (var component in rendererComponents)
                component.enabled = true;
            //TODO: научиться считать средний угол поворота меток через задание каждой метке вектора наклона
            PrevRotation= new Vector3(ActiveMarks[0].transform.rotation.x - RotationsOfMarks[0].x, 
                ActiveMarks[0].transform.rotation.y- RotationsOfMarks[0].y,
                ActiveMarks[0].transform.rotation.z- RotationsOfMarks[0].z);
            PrevPos = new Vector3(0,0,0);
            PrevCenterVector = new Vector3(0,0,0);
            foreach (GameObject activeMark in ActiveMarks)
            {
                
                PrevCenterVector+=PositionsOfMarks[activeMark.GetComponent<MarkTrackableEventHandler>().Id] / ActiveMarks.Count;
                PrevPos += activeMark.transform.position / ActiveMarks.Count;
                Quaternion RotationOfVector = Quaternion.Euler(activeMark.transform.eulerAngles.x,activeMark.transform.eulerAngles.y,activeMark.transform.eulerAngles.z);
                Vector3 VectorBetweenCenterAndMark = RotationOfVector * PositionsOfMarks[activeMark.GetComponent<MarkTrackableEventHandler>().Id];
                PrevCenterVector += VectorBetweenCenterAndMark/ ActiveMarks.Count;
            }
            transform.position = new Vector3(PrevPos.x-PrevCenterVector.x,PrevPos.y-PrevCenterVector.y,PrevPos.z-PrevCenterVector.z);
            transform.rotation = Quaternion.Euler(PrevRotation.x, PrevRotation.y,PrevRotation.z);
            Debug.Log(String.Format("{0},{1},{2}",PrevRotation.x,PrevPos.x,PrevCenterVector.x));
        }
        
        
    }
}
