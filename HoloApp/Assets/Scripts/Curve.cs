using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Curve : MonoBehaviour {
    private LineRenderer lineRenderer;
    private float _angle;

    public Material material;
    private Vector3 _startPosition;
    public float width = 0.001f;
    private float _radius;
    protected float interval = 15;

    public Vector3 startPosition
    {
        get { return _startPosition; }
        set
        {
            if (!_startPosition.Equals(value))
            {
                _startPosition = value;
                CalculatePositions();
            }
        }
    }

    public float radius
    {
        get { return _radius; }
        set
        {
            if (!_radius.Equals(value))
            {
                _radius = value;
                CalculatePositions();
            }
        }
    }

    public float angle
    {
        get { return _angle; }
        set
        {
            if (!_angle.Equals(value))
            {
                _angle = value;
                CalculatePositions();
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
        lineRenderer.material = material;
        _angle = 0;
        lineRenderer.startWidth = width;
        lineRenderer.endWidth = width;
        lineRenderer.loop = false;
    }

    void CalculatePositions()
    {
        if (lineRenderer == null)
        {
            return;
        }

        int i;
        lineRenderer.positionCount = Mathf.CeilToInt(_angle / interval + 1);
        Vector3 radiusVector = new Vector3(radius + startPosition.x, 0, 0);
        for (i = 0; i < _angle / interval; ++i)
        {
            lineRenderer.SetPosition(i, Quaternion.Euler(0, -i * interval, 0) * radiusVector - radiusVector + startPosition);
        }
        lineRenderer.SetPosition(i, Quaternion.Euler(0, -angle, 0) * radiusVector - radiusVector + startPosition);
    }
}
