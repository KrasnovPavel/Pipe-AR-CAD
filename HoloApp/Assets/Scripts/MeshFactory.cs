using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshFactory {
    public static int deltaAngle = 15;
    public static int segments = 20;

    public static List<Mesh> CreateMeshes(float tubeDiameter = 1f, float firstBendRadius = 1.2f, float secondBendRadius = 0.87f)
    {
        List<Mesh> meshes = new List<Mesh>();
        float[] radiuses = { firstBendRadius, secondBendRadius };

        for (int j = 0; j < 2; ++j)
        {
            List<Vector3> allVertices = GenerateVertices(tubeDiameter, radiuses[j]);
            for (int i = 1; i <= 180 / deltaAngle; ++i)
            {
                List<Vector3> vertices = allVertices.GetRange(0, (i + 1) * segments * 2);
                meshes.Add(new Mesh());
                meshes[meshes.Count - 1].vertices = vertices.ToArray();
                meshes[meshes.Count - 1].triangles = GenerateTriangles(ref vertices, i);

                meshes[meshes.Count - 1].RecalculateBounds();
                meshes[meshes.Count - 1].RecalculateNormals();
                meshes[meshes.Count - 1].RecalculateTangents();
            }
        }
        return meshes;
    }

    private static List<Vector3> GenerateVertices(float tubeDiameter, float bendRadius)
    {
        List<Vector3> vertices = new List<Vector3>();

        Vector3 center = new Vector3(bendRadius, 0, 0);
        Vector3[] circle = GenerateCircle(tubeDiameter);

        for (int j = 0; j <= (180 / deltaAngle); ++j)
        {
            Quaternion rot = Quaternion.Euler(0, -deltaAngle * j, 0);
            foreach(Vector3 vertex in circle)
            {
                vertices.Add(rot * (vertex + center));
            }
        }

        return vertices;
    }

    private static Vector3[] GenerateCircle(float tubeDiameter)
    {
        Vector3[] vertices = new Vector3[segments * 2];

        for (int i = 0; i < segments; ++i)
        {
            vertices[i * 2] = new Vector3(tubeDiameter * 0.5f * Mathf.Cos(2 * Mathf.PI / segments * i),
                                          tubeDiameter * 0.5f * Mathf.Sin(2 * Mathf.PI / segments * i),
                                          0);
            vertices[i * 2 + 1] = vertices[i * 2] * 0.95f;
        }

        return vertices;
    }

    private static int[] GenerateTriangles(ref List<Vector3> vertices, int level)
    {
        List<int> triangles = new List<int>();

        triangles.AddRange(GenerateEdgeTriangles(ref vertices, 0));
        triangles.AddRange(GenerateEdgeTriangles(ref vertices, level, true));
        triangles.AddRange(GenerateTubeTriangles(ref vertices));

        return triangles.ToArray();
    }

    private static List<int> GenerateEdgeTriangles(ref List<Vector3> vertices, int level, bool invert = false)
    {
        List<int> triangles = new List<int>();
        int verticesInCircle = segments * 2;

        for (int i = 0; i < segments; ++i)
        {
            AddQuad(ref triangles,
                    level * verticesInCircle + i * 2,
                    level * verticesInCircle + (i * 2 + 1) % (verticesInCircle),
                    level * verticesInCircle + (i * 2 + 2) % (verticesInCircle),
                    level * verticesInCircle + (i * 2 + 3) % (verticesInCircle),
                    invert);
        }

        return triangles;
    }

    private static List<int> GenerateTubeTriangles(ref List<Vector3> vertices)
    {
        List<int> triangles = new List<int>();
        int verticesInCircle = segments * 2;
        for (int j = 0; j < vertices.Count / verticesInCircle - 1; ++j)
        {
            for (int i = 0; i < verticesInCircle; ++i)
            {
                AddQuad(ref triangles,
                        verticesInCircle * j + i,
                        verticesInCircle * j + (i + 2) % verticesInCircle,
                        verticesInCircle * j + i + verticesInCircle,
                        verticesInCircle * j + (i + 2) % verticesInCircle + verticesInCircle,
                        i % 2 == 1);
            }
        }
        return triangles;
    }

    private static void AddQuad(ref List<int> array, int point1, int point2, int point3, int point4, bool invert)
    {
        if (invert)
        {
            array.Add(point3);
            array.Add(point2);
            array.Add(point1);
            array.Add(point2);
            array.Add(point3);
            array.Add(point4);
        }
        else
        {
            array.Add(point1);
            array.Add(point2);
            array.Add(point3);
            array.Add(point2);
            array.Add(point4);
            array.Add(point3);
        }
    }
}
