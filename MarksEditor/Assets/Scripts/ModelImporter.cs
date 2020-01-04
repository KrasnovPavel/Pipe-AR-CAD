using HoloCore;
//using PiXYZ.Plugin.Unity;
using UnityEngine;
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
    using static SFB.StandaloneFileBrowser;
#endif
namespace MarksEditor
{
    /// <summary> Класс, отвеччающий за работу с PiXYZ </summary>
    public class ModelImporter : Singleton<ModelImporter>
    {


/*        public ImportSettings ImportSettings;
    
        public void ImportModel()
        {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
            string[] paths;
            paths = OpenFilePanel("Open scene","","*",false);
            if (paths.Length == 0) return;
            var importer = new Importer(paths[0], ImportSettings);
            importer.progressed += onProgressChanged;
            importer.isAsynchronous = true;
            importer.completed += onImportEnded;
            importer.run();
#endif


        }
    
        void onProgressChanged(float progress, string message) {

            Debug.Log("Progress : " + 100f * progress + "%");
        }


        void onImportEnded(GameObject importedGameObject)
        {
            foreach (MeshFilter meshFilterChild in importedGameObject.transform.GetComponentsInChildren<MeshFilter>())
            {
                GameObject newGameObject = new GameObject(meshFilterChild.mesh.name);
                Rigidbody rigidbody= newGameObject.AddComponent<Rigidbody>();
                rigidbody.isKinematic = true;
                MeshCollider meshCollider = newGameObject.AddComponent<MeshCollider>();
                meshCollider.sharedMesh = meshFilterChild.mesh;
                newGameObject.transform.SetParent(meshFilterChild.transform);
            }
            
        }
    }*/
    }
}
