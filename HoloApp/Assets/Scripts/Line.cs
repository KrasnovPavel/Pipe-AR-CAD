using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line : MonoBehaviour
{
    private LineRenderer lineRenderer;
    public Material material;
    public float length;
    public Vector3 startPosition;
    public float width;
    
    // Use this for initialization
    void Start()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.transform.SetParent(gameObject.transform);
        lineRenderer.transform.localPosition = Vector3.zero;
        lineRenderer.transform.localRotation = Quaternion.identity;
        lineRenderer.useWorldSpace = false;
        lineRenderer.material = material; 
    }

    void Update()
    {
        Vector3 endPos = startPosition;
        endPos.z = length;
        lineRenderer.SetPosition(0, startPosition);
        lineRenderer.SetPosition(1, endPos);
        lineRenderer.startWidth = width;
        lineRenderer.endWidth = width;
    }
}
