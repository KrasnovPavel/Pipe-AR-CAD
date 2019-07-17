using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;

public static class OBJExporter
{
    public static string MeshToString(MeshFilter mf)
    {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        var m = mf.mesh;
        var mats = mf.GetComponent<MeshRenderer>().materials;

        var sb = new StringBuilder();

        sb.Append("g ").Append(mf.name).Append("\n");
        foreach (var v in m.vertices) sb.Append(string.Format("v {0} {1} {2}\n", v.x, v.y, v.z));
        sb.Append("\n");
        foreach (var v in m.normals) sb.Append(string.Format("vn {0} {1} {2}\n", v.x, v.y, v.z));
        sb.Append("\n");
        foreach (Vector3 v in m.uv) sb.Append(string.Format("vt {0} {1}\n", v.x, v.y));
        for (var material = 0; material < m.subMeshCount; material++)
        {
            sb.Append("\n");
            sb.Append("usemtl ").Append(mats[material].name).Append("\n");
            sb.Append("usemap ").Append(mats[material].name).Append("\n");

            var triangles = m.GetTriangles(material);
            for (var i = 0; i < triangles.Length; i += 3)
                sb.Append(string.Format("f {0}/{0}/{0} {1}/{1}/{1} {2}/{2}/{2}\n",
                    triangles[i] + 1, triangles[i + 1] + 1, triangles[i + 2] + 1));
        }

        return sb.ToString();
        
#endif
        return "";
    }

    public static void MeshToFile(MeshFilter mf, string filename)
    {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        using (var sw = new StreamWriter(filename))
        {
            sw.Write(MeshToString(mf));
        }
#endif
    }

    public static void FileToFile(string inputPath, string outputPath)
    {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        StreamWriter outputWriter = new StreamWriter(outputPath);
        StreamReader inputReader = new StreamReader(inputPath);
        outputWriter.Write(inputReader.ReadToEnd());
        outputWriter.Close();
        inputReader.Close();
#endif
    }
}
