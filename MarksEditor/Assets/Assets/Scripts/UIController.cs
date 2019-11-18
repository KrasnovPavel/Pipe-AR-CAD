
using UnityEngine;

namespace MarksEditor
{
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

        public void AddMarkOnScene()
        {
            MarksController.Instance.AddMark();
        }
        public void LoadJsonFile()
        {
            MarksController.Instance.ReadJsonString(FileSaverLoader.LoadJsonFile());
        }

        public void SaveJsonFile()
        {
            FileSaverLoader.SaveSceneJsonFile(MarksController.Instance.CreateJsonString());
        }

        public void SelectMark()
        {
        
        }

        public void ImportModel()
        {
            ModelImporter.Instance.ImportModel();
        }
    }
}
