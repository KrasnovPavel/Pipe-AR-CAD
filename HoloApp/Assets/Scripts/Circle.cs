using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Circle : MonoBehaviour {
    private LineRenderer lineRenderer;
    private float radius;

    public Material material;

    [Range(0.001f, 0.01f)]
    public float width = 0.005f;
    
    [Range(0, 50)]
    public int segments = 50;

    public float diameter
    {
        get { return radius * 2f; }
        set
        {
            if (!radius.Equals(value / 2f))
            {
                radius = value / 2f;
                CreatePoints();
            }
        }
    }

    // Use this for initialization
    void Start ()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.transform.SetParent(gameObject.transform);
        lineRenderer.transform.localPosition = Vector3.zero;
        lineRenderer.transform.localRotation = Quaternion.identity;
        lineRenderer.useWorldSpace = false;
        lineRenderer.loop = true;
        lineRenderer.positionCount = segments;
        radius = 0;
        lineRenderer.material = material;
    }

    void Update()
    {
        lineRenderer.startWidth = width;
        lineRenderer.endWidth = width;
    }

    private void CreatePoints()
    {
        float x;
        float y;

        float angle = 360f / segments;

        for (int i = 0; i < (segments); i++)
        {
            x = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
            y = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;

            lineRenderer.SetPosition(i, new Vector3(x, y, 0));

            angle += (360f / segments);
        }
    }
}
