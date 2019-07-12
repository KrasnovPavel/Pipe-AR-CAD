using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public void LoadObjModel()
    {
        FileSaverLoader.LoadObjModel();
    }

    public void SaveSceneFile()
    {
        FileSaverLoader.SaveSceneFile();
    }
    public void LoadSceneFile()
    {
        FileSaverLoader.LoadSceneFile();
    }
}
