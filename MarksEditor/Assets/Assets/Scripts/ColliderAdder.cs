using UnityEngine;

namespace MarksEditor
{
    public class ColliderAdder : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            foreach (MeshFilter meshFilterChild in gameObject.transform.GetComponentsInChildren<MeshFilter>())
            {
                GameObject newGameObject = new GameObject(meshFilterChild.mesh.name);
                newGameObject.layer = 30;
                Rigidbody rigidbody= newGameObject.AddComponent<Rigidbody>();
                rigidbody.isKinematic = true;
                MeshCollider meshCollider = newGameObject.AddComponent<MeshCollider>();
                meshCollider.sharedMesh = meshFilterChild.mesh;
                newGameObject.transform.SetParent(meshFilterChild.transform);
                newGameObject.transform.localPosition = Vector3.zero;
            
            }
        }

    }
}
