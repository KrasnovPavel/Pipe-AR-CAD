using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using HoloCore.UI;
using UnityEngine;

#if ENABLE_WINMD_SUPPORT
    using Windows.Storage;
    using Windows.Storage.Pickers;
    using HoloToolkit.Unity;
#endif
public class MarkSceneLoader : MonoBehaviour
{
    
    [Serializable]
    private struct SerializedMarks
    {
        public List<SerializedMark> AllMarks;

        public SerializedMarks(ReadOnlyCollection<SerializedMark> allMarks)
        {
            AllMarks = new List<SerializedMark>();
            foreach (var mark in allMarks)
                AllMarks.Add(mark);
        }
    }
    
    
    [Serializable]
    private struct SerializedMark
    {
        public int X;
        public int Y;
        public int Z;
        public int RotationX;
        public int RotationY;
        public int RotationZ;
        public string Name;
        public string DrawObjectName;

        public SerializedMark(int x, int y, int z, int rotationX, int rotationY, int rotationZ, string name, string drawObjectName)
        {
            X = x;
            Y = y;
            Z = z;
            RotationX = rotationX;
            RotationY = rotationY;
            RotationZ = rotationZ;
            Name = name;
            DrawObjectName = drawObjectName;
        }
    }
    void Start()
    {
        JsonText = "";
#if ENABLE_WINMD_SUPPORT
        UnityEngine.WSA.Application.InvokeOnUIThread(PickJson, true);

#endif
    }

    private void PickJson()
    {
#if ENABLE_WINMD_SUPPORT
        FileOpenPicker openPicker = new FileOpenPicker();
        openPicker.ViewMode = PickerViewMode.Thumbnail;
        openPicker.FileTypeFilter.Add(".json");
        StorageFile file = await openPicker.PickSingleFileAsync();
        if(file==null) return;
        JsonText = await FileIO.ReadTextAsync(file);
        if (JsonText.Length == 0)  return;
        UnityEngine.WSA.Application.InvokeOnUIThread(ReadJsonAndBuildMarkScene, true);
#endif

    }

    private void ReadJsonAndBuildMarkScene()
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
            currentMarksHandler.PositionsOfMarks.Add(new Vector3(mark.X,mark.Y,mark.Z));
            currentMarksHandler.PositionsOfMarks.Add(new Vector3(mark.RotationX,mark.RotationY, mark.RotationZ));    
        }
    }

    private string JsonText;
    private SerializedMarks SetializedMarksList;
}


