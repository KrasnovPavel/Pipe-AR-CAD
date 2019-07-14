using System;
using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity;
using UnityEngine;
using Vuforia;

public class MarksHandler : MonoBehaviour
{
    public List<GameObject> AllMarks = new List<GameObject>();
    public List<Vector3> PositionsOfMarks = new List<Vector3>();
    public List<Vector3> RotationsOfMarks = new List<Vector3>();
    public Vector3 PrevRotation;
    public Vector3 PrevPos;
    private void Update()
    {
        foreach (GameObject currentMark in AllMarks)
        {
            if (!currentMark.GetComponent<MarkTrackableEventHandler>().IsActive) continue;
            var rendererComponents = GetComponentsInChildren<Renderer>(true);
            Debug.Log($"!!!!!!!{currentMark.GetComponent<MarkTrackableEventHandler>().Id}, {AllMarks.Count}");
            foreach (var component in rendererComponents)
                component.enabled = true;
            PrevRotation = new Vector3(
                currentMark.transform.eulerAngles.x -
                RotationsOfMarks[currentMark.GetComponent<MarkTrackableEventHandler>().Id].x,
                currentMark.transform.eulerAngles.y -
                RotationsOfMarks[currentMark.GetComponent<MarkTrackableEventHandler>().Id].y,
                currentMark.transform.eulerAngles.z -
                RotationsOfMarks[currentMark.GetComponent<MarkTrackableEventHandler>().Id].z);
            Debug.Log($"{currentMark.transform.eulerAngles.x}, {currentMark.transform.eulerAngles.y}, {currentMark.transform.eulerAngles.z}");
            Debug.Log($"{currentMark.transform.eulerAngles.x}, {currentMark.transform.eulerAngles.y}, {currentMark.transform.eulerAngles.z}");
            /*Vector3 RotationOfMark = new Vector3(
                currentMark.transform.eulerAngles.x -
                RotationsOfMarks[currentMark.GetComponent<MarkTrackableEventHandler>().Id].x,
                currentMark.transform.eulerAngles.y -
                RotationsOfMarks[currentMark.GetComponent<MarkTrackableEventHandler>().Id].y,
                currentMark.transform.eulerAngles.z -
                RotationsOfMarks[currentMark.GetComponent<MarkTrackableEventHandler>().Id].z);*/
            Vector3 VectorOfRotationCenter = Quaternion.Euler(PrevRotation) *
                                         PositionsOfMarks[currentMark.GetComponent<MarkTrackableEventHandler>().Id];
            PrevPos = (currentMark.transform.position - VectorOfRotationCenter);
            break;
        }

        if (PrevPos != null)
        {
            transform.position = PrevPos;
            transform.rotation = Quaternion.Euler(PrevRotation.x, PrevRotation.y, PrevRotation.z);
        }


    }

}
