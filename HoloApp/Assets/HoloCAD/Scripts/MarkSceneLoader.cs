using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using HoloCore.UI;
using UnityEngine;
using HoloCore;
#if ENABLE_WINMD_SUPPORT
    using Windows.Storage;
    using Windows.Storage.Pickers;
    using HoloToolkit.Unity;
#endif

public class MarkSceneLoader : MonoBehaviour
{
    void Start()
    {
        JsonText = "";
#if ENABLE_WINMD_SUPPORT
       UnityEngine.WSA.Application.InvokeOnUIThread(PickJson, true);
       // UnityEngine.WSA.Application.InvokeOnUIThread(PickObj, true);
#endif
    }

    private async void PickJson()
    {
#if ENABLE_WINMD_SUPPORT
        FileOpenPicker openPicker = new FileOpenPicker();
        openPicker.ViewMode = PickerViewMode.Thumbnail;
        openPicker.FileTypeFilter.Add(".json");
        StorageFile file = await openPicker.PickSingleFileAsync();
        if(file==null) return;
        JsonText = await FileIO.ReadTextAsync(file);
        if (JsonText.Length == 0)  return;
        UnityEngine.WSA.Application.InvokeOnAppThread(ReadJsonAndBuildMarkScene, false);
#endif

    }

    private async void ReadJsonAndBuildMarkScene()
    {
        try
        {
            SetializedMarksList= JsonUtility.FromJson<SerializedMarks>(JsonText);
        }
        catch
        {
            return;
        }
        foreach (SerializedMark mark in SetializedMarksList.AllMarks)
        {
            
            MarksHandler currentMarksHandler = GameObject.Find(mark.DrawObjectName).GetComponent<MarksHandler>();  
            currentMarksHandler.AllMarks.Add(GameObject.Find(mark.Name));
            MarkTrackableEventHandler markTrackableEventHandler =
                GameObject.Find(mark.Name).GetComponent<MarkTrackableEventHandler>();
            markTrackableEventHandler.Id =
                currentMarksHandler.AllMarks.Count - 1;
            Debug.Log($"{mark.X}, {mark.Y}, {mark.Z}");
            markTrackableEventHandler.IsActive = false;
            currentMarksHandler.PositionsOfMarks.Add(new Vector3(mark.X,mark.Y,mark.Z));
            currentMarksHandler.RotationsOfMarks.Add(new Vector3(mark.RotationX,mark.RotationY, mark.RotationZ));    
        }
    }

    private string JsonText;
    private SerializedMarks SetializedMarksList;
}


