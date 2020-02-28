// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System.Collections.Generic;
using HoloCAD.UnityTubes;
using UnityEngine;

namespace HoloCAD.IO
{
    /// <summary> Класс, экспортирующий сцену в файл. </summary>
    public static class SceneExporter
    {
        /// <summary> Экспорт сцену. </summary>
        /// <remarks> Для выбора файла будет вызван диалог сохранения файла. </remarks>
        /// <param name="tubes"> Массив всех труб на сцене. </param>
        /// <param name="isExportAs"> Надо ли экспортировать как отдельный файл. </param>
        public static void Export(IEnumerable<Tube> tubes, bool isExportAs)
        {
            string data = SerializeScheme(tubes);
            UnityFileManager.PickAndWriteTextFileAsync(data, new[] {"json"});
        }

        #region Private definitioins

        /// <summary> Сериализует все трубы в json формат. </summary>
        /// <param name="tubes"> Все трубы на сцене. </param>
        /// <returns> Объект в формате json. </returns>
        private static string SerializeScheme(IEnumerable<Tube> tubes)
        {
            ExpTubesArray array = new ExpTubesArray();
            foreach (Tube tube in tubes)
            {
                ExpTube expTube = new ExpTube();
                array.tubes.Add(expTube);
                expTube.diameter = tube.Data.diameter * 1000;
                expTube.width = expTube.diameter * 0.1;
                expTube.standard_name = tube.StandardName;
                tube.MapFragmentsWithOutgrowth(fragment => expTube.fragments.Add(ConvertToExportFormat(fragment)));
                expTube.fragments.RemoveAll(element => element == null);
            }

            return JsonUtility.ToJson(array);
        }

        private static ExpFragment ConvertToExportFormat(TubeFragment fragment)
        {
            ExpFragment expFragment = new ExpFragment();

            switch (fragment)
            {
                case DirectTubeFragment dtf:
                    expFragment.type = "Direct";
                    expFragment.length = dtf.Length * 1000;
                    break;
                case BendedTubeFragment btf:
                    expFragment.type = "Bended";
                    expFragment.bendAngle = btf.Angle;
                    expFragment.rotationAngle = btf.RotationAngle;
                    expFragment.radius = btf.Radius * 1000;
                    break;
                default:
                    // Фланец не добавляем, он не является частью трубы
                    return null;
            }

            expFragment.transform = SerializeTransform(fragment, fragment.Owner.StartFragment.EndPoint.transform.position);
            return expFragment;
        }

        /// <summary>
        /// Превращает положение отрезка трубы в матрицу переноса 4х4 для левосторонней системы координат.
        /// </summary>
        /// <param name="fragment"> Отрезок трубы. </param>
        /// <param name="tubeOrigin"> Координаты начала трубы. </param>
        /// <returns> Матрица переноса 4х4. </returns>
        private static double[] SerializeTransform(TubeFragment fragment, Vector3 tubeOrigin)
        {
            double[] result = new double[16];
            Matrix4x4 tr = fragment.transform.localToWorldMatrix;
            Matrix4x4 toROS = new Matrix4x4(new Vector4(1,  0,  0, 0), 
                                            new Vector4(0,  0, -1, 0),
                                            new Vector4(0,  1,  0, 0),
                                            new Vector4(-tubeOrigin.x,  -tubeOrigin.z,  tubeOrigin.y, 1));

            tr = toROS * tr;
            
            result[0] = -tr[0];
            result[1] = -tr[1];
            result[2] = -tr[2];
            result[3] = -tr[3];
            result[4] = -tr[4];
            result[5] = -tr[5];
            result[6] = -tr[6];
            result[7] = -tr[7];
            result[8] = -tr[8];
            result[9] = -tr[9];
            result[10] = -tr[10];
            result[11] = -tr[11];
            result[12] = -tr[12] * 1000;
            result[13] = -tr[13] * 1000;
            result[14] = -tr[14] * 1000;
            result[15] = -tr[15];
            
            return result;
        }
        
        #endregion
    }
}