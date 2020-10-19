// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace HoloCAD.Tubes.View
{
    /// <summary> Класс, создающий меши погиба для всех возможных углов погиба. </summary>
    public static class MeshFactory
    {
        /// <summary> Получение мешей погиба трубы.</summary>
        /// <param name="tubeData"> Данные о трубе трубы. </param>
        /// <returns> Список из трех мешей: погиб первого радиуса, погиб второго радиуса, плоское кольцо. </returns>
        [NotNull] public static List<Mesh> GetMeshes(TubeLoader.TubeData tubeData)
        {
            if (GeneratedMeshes.ContainsKey(tubeData))
            {
                return GeneratedMeshes[tubeData];
            }

            List<Mesh> meshes = new List<Mesh>();

            float[] radiuses = {tubeData.first_radius, tubeData.second_radius};
            foreach (float r in radiuses)
            {
                meshes.Add(GenerateTube(tubeData.diameter, r));
            }

            meshes.Add(GenerateRing(tubeData.diameter));

            GeneratedMeshes[tubeData] = meshes;
            return meshes;
        }

        #region Private definitions

        /// <summary> Количество полигонов. </summary>
        private const int SegmentsCount = 20;

        /// <summary> Список уже сгенерированных мешей для каждой трубы. </summary>
        private static readonly Dictionary<TubeLoader.TubeData, List<Mesh>> GeneratedMeshes
            = new Dictionary<TubeLoader.TubeData, List<Mesh>>();

        /// <summary> Шаг изменения угла погиба. </summary>
        private const int DeltaAngle = 5;

        /// <summary> Генерирует трубу для погиба. </summary>
        /// <param name="tubeDiameter"> Диаметр трубы. </param>
        /// <param name="bendRadius"> Радиус погиба. </param>
        /// <returns> Меш трубы. </returns>
        [NotNull] private static Mesh GenerateTube(float tubeDiameter, float bendRadius)
        {
            List<Vector3> allVertices = GenerateVertices(tubeDiameter, bendRadius);
            List<Vector3> vertices    = allVertices.GetRange(0, (180 / DeltaAngle + 1) * SegmentsCount * 2);
            Mesh mesh = new Mesh
            {
                vertices  = vertices.ToArray(),
                triangles = GenerateTriangles(vertices.Count, 180 / DeltaAngle)
            };

            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            mesh.RecalculateTangents();

            return mesh;
        }


        /// <summary> Генерирует кольцо заданного диаметра. </summary>
        /// <param name="diameter"> Диаметр кольца. </param>
        /// <returns> Меш кольца. </returns>
        [NotNull] private static Mesh GenerateRing(float diameter)
        {
            Mesh mesh = new Mesh
            {
                vertices  = GenerateCircle(diameter),
                triangles = GenerateEdgeTriangles(0, true).ToArray()
            };

            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            mesh.RecalculateTangents();

            return mesh;
        }

        /// <summary> Расчитывает все точки меша погиба. </summary>
        /// <param name="tubeDiameter"> Диаметр трубы. </param>
        /// <param name="bendRadius"> Угол погиба. </param>
        /// <returns> Список вершин. </returns>
        [NotNull] private static List<Vector3> GenerateVertices(float tubeDiameter, float bendRadius)
        {
            List<Vector3> vertices = new List<Vector3>();

            Vector3   center = new Vector3(bendRadius, 0, 0);
            Vector3[] circle = GenerateCircle(tubeDiameter);

            for (int j = 0; j <= (180 / DeltaAngle); ++j)
            {
                Quaternion rot = Quaternion.Euler(0, -DeltaAngle * j, 0);
                foreach (Vector3 vertex in circle)
                {
                    vertices.Add(rot * (vertex + center) - center);
                }
            }

            return vertices;
        }

        /// <summary> Расчитывает точки окружности заданного диаметра. </summary>
        /// <param name="tubeDiameter"> Диаметр окружности. </param>
        /// <returns> Список вершин окружности. </returns>
        [NotNull] private static Vector3[] GenerateCircle(float tubeDiameter)
        {
            Vector3[] vertices = new Vector3[SegmentsCount * 2];

            for (int i = 0; i < SegmentsCount; ++i)
            {
                vertices[i * 2] = new Vector3(tubeDiameter * 0.5f * Mathf.Cos(2 * Mathf.PI / SegmentsCount * i),
                                              tubeDiameter * 0.5f * Mathf.Sin(2 * Mathf.PI / SegmentsCount * i),
                                              0);
                vertices[i * 2 + 1] = vertices[i * 2] * 0.95f;
            }

            return vertices;
        }

        /// <summary> Расчитывает полигоны одного участка погиба. </summary>
        /// <param name="verticesAmount"> Количество вершин меша. </param>
        /// <param name="level"> Местоположение границы погиба. </param>
        /// <returns> Список индексов вершин в порядке их следования в полигонах. </returns>
        [NotNull] private static int[] GenerateTriangles(int verticesAmount, int level)
        {
            List<int> triangles = new List<int>();

            triangles.AddRange(GenerateEdgeTriangles(0));
            triangles.AddRange(GenerateEdgeTriangles(level, true));
            triangles.AddRange(GenerateTubeTriangles(verticesAmount));

            return triangles.ToArray();
        }

        /// <summary> Создает полигоны на границе трубы. </summary>
        /// <param name="level"> Местоположение границы. </param>
        /// <param name="invert"> Надо ли инвертировать полигоны. </param>
        /// <returns> Список индексов вершин в порядке их следования в полигонах. </returns>
        [NotNull] private static List<int> GenerateEdgeTriangles(int level, bool invert = false)
        {
            List<int> triangles        = new List<int>();
            const int verticesInCircle = SegmentsCount * 2;

            for (int i = 0; i < SegmentsCount; ++i)
            {
                AddQuad(ref triangles,
                        level * verticesInCircle + i           * 2,
                        level * verticesInCircle + (i * 2 + 1) % verticesInCircle,
                        level * verticesInCircle + (i * 2 + 2) % verticesInCircle,
                        level * verticesInCircle + (i * 2 + 3) % verticesInCircle,
                        invert);
            }

            return triangles;
        }

        /// <summary> Создает полигоны трубы. </summary>
        /// <param name="verticesAmount"> Количество вершин </param>
        /// <returns> Список индексов вершин в порядке их следования в полигонах. </returns>
        [NotNull] private static List<int> GenerateTubeTriangles(int verticesAmount)
        {
            List<int> triangles        = new List<int>();
            const int verticesInCircle = SegmentsCount * 2;
            for (int j = 0; j < verticesAmount / verticesInCircle - 1; ++j)
            {
                for (int i = 0; i < verticesInCircle; ++i)
                {
                    AddQuad(ref triangles,
                            verticesInCircle * j + i,
                            verticesInCircle * j + (i + 2) % verticesInCircle,
                            verticesInCircle * j + i                          + verticesInCircle,
                            verticesInCircle * j + (i + 2) % verticesInCircle + verticesInCircle,
                            i                % 2 == 1);
                }
            }

            return triangles;
        }

        /// <summary> Создает два полигона из четырех индексов вершин. </summary>
        /// <param name="array"> Список в который добавляется полигоны. </param>
        /// <param name="point1"> Индекс первой вершины. </param>
        /// <param name="point2"> Индекс второй вершины. </param>
        /// <param name="point3"> Индекс третьей вершины. </param>
        /// <param name="point4"> Индекс четвертой вершины. </param>
        /// <param name="invert"> Надо ли перевернуть полигон. </param>
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

        #endregion
    }
}
