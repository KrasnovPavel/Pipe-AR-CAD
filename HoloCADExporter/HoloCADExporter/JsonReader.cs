using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WpfApplication1
{
    public static class JsonReader
    {
        public static List<Tube> ReadFile(string filename)
        {
            using (StreamReader reader = new StreamReader(filename, Encoding.UTF8))
            {
                 return JsonConvert.DeserializeObject<ImpTubesArray>(reader.ReadToEnd()).tubes;
            }
        }

        #region Private definitions

        /// <summary> Коренной объект Json. </summary>
        [Serializable]
        private class ImpTubesArray
        {
            // ReSharper disable once InconsistentNaming
            /// <summary> Массив экспортируемых труб. </summary>
            public readonly List<Tube> tubes = new List<Tube>();
        }

        #endregion
    }
}