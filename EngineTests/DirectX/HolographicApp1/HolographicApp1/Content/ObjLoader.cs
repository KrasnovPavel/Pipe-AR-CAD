using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using SharpDX;
using System.Globalization;
using System.Numerics;
using Windows.Storage;
using Windows.Storage.Pickers;
using System.Threading.Tasks;

namespace HolographicApp1.Content
{
    /// <summary>
    /// Load an obj model
    /// </summary>
    public class Objloader
    {
        private static CultureInfo infos = CultureInfo.InvariantCulture;

        /// <summary>
        /// Vertex data
        /// </summary>
        public List<VertexPositionColor> Vertices;

        public List<ushort> Faces;

        private Objloader()
        {
            this.Vertices = new List<VertexPositionColor>();
            this.Faces = new List<ushort>();
        }

        /// <summary>
        /// Create obj from file
        /// </summary>
        /// <param name="filename">Filename</param>
        /// <returns>Models</returns>
        public static async Task<Objloader> CreateFromObjAsync()
        {
            Objloader geom = new Objloader();
            var openPicker = new FileOpenPicker();

            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.FileTypeFilter.Add(".txt");

            StorageFile file = await openPicker.PickSingleFileAsync();

            IList<string> lines = await FileIO.ReadLinesAsync(file);

            foreach (string line in lines)
            {
                if (line.Contains("v "))
                {
                    geom.Vertices.Add(GetPositionColor(line));
                } else if (line.Contains("f "))
                {
                    geom.Faces.AddRange(GetFace(line));
                }
            }
            return geom;
        }

        private static string[] Parts(string name, string line)
        {
            return line.Replace(name, "").Trim().Split(' ');
        }

        private static VertexPositionColor GetPositionColor(string line)
        {
            string[] parts = Parts("v", line);
            return new VertexPositionColor(new Vector3(
                                                float.Parse(parts[0], infos),
                                                float.Parse(parts[1], infos),
                                                float.Parse(parts[2], infos)),
                                           new Vector3(0.9f, 0.9f, 0.9f));
        }        

        private static ushort[] GetFace(string line)
        {
            string[] parts = Parts("f", line);
            List<ushort> lists = new List<ushort>();
            foreach (String s in parts)
            {
                lists.AddRange((from ss in s.Split('/') select ushort.Parse(ss)).ToArray());
            }
            return lists.ToArray();
        }
    }
}
