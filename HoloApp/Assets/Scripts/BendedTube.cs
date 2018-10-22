using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BendedTube : MonoBehaviour
{
    List<Curve> curves;
    List<GameObject> circles;
    // valies in meters
    [Range(0.01f, 1f)]
    public float outerDiameter;
    public float firstBendRadius;
    public float secondBendRadius;
    public float maxAngle;
    public float angle = 0;
    public bool useSecondRadius = false;
    [Range(0.001f, 0.01f)]
    public float lineWidth = 0.001f;
    public Material redMaterial;
    public Material yellowMaterial;

    public bool isFirstPivotUsed = false;
    public bool isSecondPivotUsed = false;
    protected GameObject startLight;
    protected GameObject endLight;

    public const int numberOfLines = 8;

    public BendedTube(int outerDiameter, Tuple<int, int, int> standardData)
    {
        this.outerDiameter = outerDiameter * 1000f;
        this.firstBendRadius = standardData.Item1 * 1000f;
        this.secondBendRadius = standardData.Item2 * 1000f;
        this.maxAngle = standardData.Item3;
    }

    // Use this for initialization
    void Start()
    {
        startLight = transform.Find("StartPoint").gameObject;
        endLight = transform.Find("EndPoint").gameObject;
        startLight.GetComponent<Circle>().material = yellowMaterial;
        endLight.GetComponent<Circle>().material = yellowMaterial;

        curves = new List<Curve>();
        circles = new List<GameObject>();
        MakeLines(1, redMaterial);
        CalculateLine(0, 0, 0);
        MakeLines(numberOfLines, yellowMaterial);
        CalculateLine(outerDiameter, 1, numberOfLines + 1);
    }

    // Update is called once per frame
    void Update()
    {
        if (angle > maxAngle)
        {
            angle = maxAngle;
        }

        startLight.SetActive(!isFirstPivotUsed);
        endLight.SetActive(!isSecondPivotUsed);
        startLight.GetComponent<Light>().range = outerDiameter / 2;
        endLight.GetComponent<Light>().range = outerDiameter / 2;

        CalculateLine(0, 0, 0);
        CalculateLine(outerDiameter, 1, numberOfLines + 1);
        CalculateCircles();
    }

    protected void MakeLines(int numberOfLines, Material material)
    {
        for (int i = 0; i < numberOfLines; ++i)
        {
            var go = new GameObject("curve");
            go.transform.SetParent(gameObject.transform);
            go.transform.localPosition = Vector3.zero;
            curves.Add(go.AddComponent<Curve>());
            curves[curves.Count - 1].material = material;
        }
    }

    protected void CalculateLine(float diameter, int startLine, int endLine)
    {
        Quaternion quat = Quaternion.Euler(0, 0, 360f / numberOfLines);
        Vector3 startPoint = new Vector3(0, diameter / 2, 0);
        for (int i = startLine; i <= endLine && i < curves.Count; ++i)
        {
            curves[i].angle = angle;
            startPoint = quat * startPoint;
            curves[i].startPosition = startPoint;
            curves[i].width = lineWidth;
            curves[i].radius = useSecondRadius ? secondBendRadius : firstBendRadius;
        }
    }

    protected void CalculateCircles()
    {
        startLight.GetComponent<Circle>().diameter = outerDiameter;
        startLight.GetComponent<Circle>().width = lineWidth;
        endLight.GetComponent<Circle>().diameter = outerDiameter;
        endLight.GetComponent<Circle>().width = lineWidth;
        Vector3 maxRadiusVector = new Vector3(useSecondRadius ? secondBendRadius : firstBendRadius, 0, 0);
        endLight.transform.localPosition = Quaternion.Euler(0, -angle, 0) * maxRadiusVector - maxRadiusVector;
        endLight.transform.localRotation = Quaternion.Euler(0, -angle, 0);

        int numberOfCircles = 0;
        float circlesInterval = 15;
        if (outerDiameter > 0)
        {
            numberOfCircles = Mathf.CeilToInt(angle / circlesInterval);
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
            Vector3 radiusVector = new Vector3(useSecondRadius ? secondBendRadius : firstBendRadius, 0, 0);
            circles[i].transform.localPosition = Quaternion.Euler(0, -i * circlesInterval, 0) * radiusVector - radiusVector;
            circles[i].transform.localRotation = Quaternion.Euler(0, -i * circlesInterval, 0);
            circles[i].GetComponent<Circle>().diameter = outerDiameter;
            circles[i].GetComponent<Circle>().width = lineWidth;
        }
    }
}
