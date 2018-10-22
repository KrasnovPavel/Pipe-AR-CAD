using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tube : MonoBehaviour {
    protected List<Line> lines;
    protected List<GameObject> circles;
    // values in meters
    [Range(0.01f, 1f)]
    public float outerDiameter;
    [Range(0f, 10f)]
    public float length;
    [Range(0.001f, 0.01f)]
    public float lineWidth = 0.001f;
    public Material redMaterial;
    public Material yellowMaterial;

    public bool isFirstPivotUsed = false;
    public bool isSecondPivotUsed = false;
    protected GameObject startLight;
    protected GameObject endLight;

    public const int numberOfLines = 8;

	// Use this for initialization
	protected void Start ()
    {
        startLight = transform.Find("StartPoint").gameObject;
        endLight = transform.Find("EndPoint").gameObject;
        startLight.GetComponent<Circle>().material = yellowMaterial;
        endLight.GetComponent<Circle>().material = yellowMaterial;

        lines = new List<Line>();
        circles = new List<GameObject>();
        MakeLines(1, redMaterial);
        CalculateLine(0, 0, 0);
        MakeLines(numberOfLines, yellowMaterial);
        CalculateLine(outerDiameter, 1, numberOfLines + 1);
    }
    
    void Update()
    {
        CalculateLine(0, 0, 0);
        CalculateLine(outerDiameter, 1, numberOfLines + 1);

        startLight.SetActive(!isFirstPivotUsed);
        endLight.SetActive(!isSecondPivotUsed);
        startLight.GetComponent<Light>().range = outerDiameter / 2;
        endLight.GetComponent<Light>().range = outerDiameter / 2;
        endLight.transform.localPosition = new Vector3(0, 0, length);

        CalculateCircles();
    }

    protected void MakeLines(int numberOfLines, Material material)
    {
        for (int i = 0; i < numberOfLines; ++i)
        {
            var go = new GameObject("line");
            go.transform.SetParent(gameObject.transform);
            go.transform.localPosition = Vector3.zero;
            lines.Add(go.AddComponent<Line>());
            lines[lines.Count-1].material = material;
        }
    }

    protected void CalculateLine(float diameter, int startLine, int endLine)
    {
        Quaternion quat = Quaternion.Euler(0, 0, 360f / numberOfLines);
        Vector3 startPoint = new Vector3(0, diameter / 2, 0);
        for (int i = startLine; i <= endLine && i < lines.Count; ++i)
        {
            lines[i].length = length;
            startPoint = quat * startPoint;
            lines[i].startPosition = startPoint;
            lines[i].width = lineWidth;
        }
    }

    protected void CalculateCircles()
    {
        startLight.GetComponent<Circle>().diameter = outerDiameter;
        startLight.GetComponent<Circle>().width = lineWidth;
        endLight.GetComponent<Circle>().diameter = outerDiameter;
        endLight.GetComponent<Circle>().width = lineWidth;

        int numberOfCircles = 0;
        float circlesInterval = outerDiameter * 2;
        if (outerDiameter > 0)
        {
            numberOfCircles = Mathf.CeilToInt(length / circlesInterval);
        }

        for (int i = circles.Count; i < numberOfCircles; ++i)
        {
            GameObject circle = new GameObject("circle");
            circle.transform.SetParent(gameObject.transform);
            circle.AddComponent<Circle>().material = yellowMaterial;
            circles.Add(circle);
        }
        for (int i = circles.Count - 1; i > numberOfCircles - 1; --i)
        {
            Destroy(circles[i]);
            circles.RemoveAt(i);
        }
        for (int i = 0; i < circles.Count; ++i)
        {
            circles[i].transform.localPosition = new Vector3(0, 0, i * circlesInterval);
            circles[i].transform.localRotation = Quaternion.identity;
            circles[i].GetComponent<Circle>().diameter = outerDiameter;
            circles[i].GetComponent<Circle>().width = lineWidth;
        }
    }
}
