﻿using UnityEngine;

#if ENABLE_WINMD_SUPPORT
    using Windows.Storage;
    using Windows.Storage.Pickers;
    using HoloToolkit.Unity;
#endif

namespace HoloCAD
{
    public class MarkSceneLoader : MonoBehaviour
    {
//    void Start()
//    {
//        JsonText = "";
//#if ENABLE_WINMD_SUPPORT
//       UnityEngine.WSA.Application.InvokeOnUIThread(PickJson, true);
//       // UnityEngine.WSA.Application.InvokeOnUIThread(PickObj, true);
//#endif
//    }
//
//    private async void PickJson()
//    {
//#if ENABLE_WINMD_SUPPORT
//        FileOpenPicker openPicker = new FileOpenPicker();
//        openPicker.ViewMode = PickerViewMode.Thumbnail;
//        openPicker.FileTypeFilter.Add(".json");
//        StorageFile file = await openPicker.PickSingleFileAsync();
//        if(file==null) return;
//        JsonText = await FileIO.ReadTextAsync(file);
//        if (JsonText.Length == 0)  return;
//        UnityEngine.WSA.Application.InvokeOnAppThread(ReadJsonAndBuildMarkScene, false);
//#endif
//
//    }
//
//    private async void ReadJsonAndBuildMarkScene()
//    {
//        try
//        {
//            SetializedMarksList = JsonUtility.FromJson<SerializedMarks>(JsonText);
//        }
//        catch
//        {
//            return;
//        }
//        foreach (SerializedMark mark in SetializedMarksList.Marks)
//        {
//            MarksTarget currentMarksHandler = GameObject.Find(mark.DrawObjectName).GetComponent<MarksTarget>();  
//            currentMarksHandler.Marks.Add(GameObject.Find(mark.Name));
//            Mark markTrackableEventHandler =
//                GameObject.Find(mark.Name).GetComponent<Mark>();
//            markTrackableEventHandler.Id =
//                currentMarksHandler.Marks.Count - 1;
//            Debug.Log($"{mark.X}, {mark.Y}, {mark.Z}");
//            markTrackableEventHandler.IsActive = false;
//            currentMarksHandler.PositionsOfMarks.Add(new Vector3(mark.X,mark.Y,mark.Z));
//            currentMarksHandler.RotationsOfMarks.Add(new Vector3(mark.RotationX,mark.RotationY, mark.RotationZ));    
//        }
//    }
//
//    private string JsonText;
//    private SerializedMarks SetializedMarksList;
    }
}


