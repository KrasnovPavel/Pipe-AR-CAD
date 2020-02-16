using HoloCore;
using PiXYZ.Plugin.Unity;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
using static SFB.StandaloneFileBrowser;

#endif


namespace GLTFConverter
{
    /// <summary> Класс, отвечающий за работу с PiXYZ </summary>
    public class ModelImporter : Singleton<ModelImporter>
    {
        public ImportSettings ImportSettings;

        public void ImportModel()
        {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
            string[] paths;
            paths = OpenFilePanel("Open scene", "", "*", false);
            if (paths.Length == 0) return;

            var importer = new Importer(paths[0], ImportSettings);
            importer.progressed += onProgressChanged;
            importer.isAsynchronous = true;
            importer.completed += onImportEnded;
            importer.run();
#endif
        }

        void onProgressChanged(float progress, string message)
        {
            Debug.Log("Progress : " + 100f * progress + "%");
        }


        void onImportEnded(GameObject importedGameObject)
        {
            GameObject targetGameObject = GameObject.Find("Target");
            // GLTFExporter.Instance.Target = importedGameObject;
            foreach (MeshFilter meshFilterChild in importedGameObject.transform.GetComponentsInChildren<MeshFilter>())
            {
                GameObject newGameObject = new GameObject(meshFilterChild.mesh.name);
                Transform meshFilterChildTransform = meshFilterChild.transform;
                meshFilterChildTransform.SetParent(targetGameObject.transform);
                newGameObject.transform.SetParent(meshFilterChildTransform);
                newGameObject.transform.localScale = Vector3.one;
                newGameObject.transform.localPosition = Vector3.zero;
                newGameObject.transform.localRotation = Quaternion.Euler(Vector3.zero);

                newGameObject.layer = 30;
                Rigidbody newGameObjectRigidbody = newGameObject.AddComponent<Rigidbody>();
                newGameObjectRigidbody.isKinematic = true;
                MeshCollider meshCollider = newGameObject.AddComponent<MeshCollider>();
                meshCollider.sharedMesh = meshFilterChild.mesh;
            }
        }
    }
}