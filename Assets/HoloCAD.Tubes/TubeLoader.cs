// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using JetBrains.Annotations;
using UnityEngine;

namespace HoloCAD.Tubes
{
    /// <summary> Класс, реализующий загрузку данных стандартов труб. </summary>
    [Serializable]
    public static class TubeLoader
    {
        /// <summary> Класс, хранящий данные трубы. </summary>
        [Serializable]
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public class TubeData
        {
            protected bool Equals(TubeData other)
            {
                return string.Equals(name, other.name)
                       && (Math.Abs(diameter      - other.diameter)      < float.Epsilon)
                       && (Math.Abs(first_radius  - other.first_radius)  < float.Epsilon)
                       && (Math.Abs(second_radius - other.second_radius) < float.Epsilon);
            }

            /// <inheritdoc />
            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((TubeData) obj);
            }

            /// <inheritdoc />
            [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
            public override int GetHashCode()
            {
                unchecked
                {
                    int hashCode = (name != null ? name.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ diameter.GetHashCode();
                    hashCode = (hashCode * 397) ^ first_radius.GetHashCode();
                    hashCode = (hashCode * 397) ^ second_radius.GetHashCode();
                    return hashCode;
                }
            }

            /// <inheritdoc />
            public override string ToString()
            {
                return $"TubeData(\"{name}\", {diameter}, {first_radius}, {second_radius})";
            }

            /// <summary> Имя трубы (может отсутствовать) </summary>
            public string name;

            /// <summary> Диаметр трубы в метрах </summary>
            public float diameter;

            /// <summary> Первый из двух допустимых радиусов погиба. </summary>
            public float first_radius;

            /// <summary> Второй из двух допустимых радиусов погиба. </summary>
            public float second_radius;
        }

        /// <summary> Функция поиска данных о трубе по диаметру и наименованию стандарта. </summary>
        /// <param name="diameter"> Диаметр искомой трубы. </param>
        /// <param name="standardName"> Стандарт искомой трубы. </param>
        /// <returns> Данные о трубе. </returns>
        [CanBeNull]
        public static TubeData FindTubeData(float diameter, string standardName)
        {
            List<TubeData> tubes = GetAvailableTubes(standardName);

            foreach (TubeData tube in tubes)
            {
                if (Math.Abs(tube.diameter - diameter) <= float.Epsilon)
                {
                    return tube;
                }
            }

            return null;
        }

        /// <summary> Функция получения данных о всех трубах из запрошенного стандарта. </summary>
        /// <param name="standardName"> Запрошенный стандарт. </param>
        /// <returns> Список труб из запрошенного стандарта. </returns>
        [NotNull]
        public static List<TubeData> GetAvailableTubes(string standardName)
        {
            if (standardName.Length == 0 && TubeStandards.Count > 0)
            {
                return TubeStandards[0].available_tubes;
            }

            foreach (TubeStandard standard in TubeStandards)
            {
                if (standard.name == standardName)
                {
                    return standard.available_tubes;
                }
            }

            return new List<TubeData>();
        }

        /// <summary> Функция получения всех стандартов. </summary>
        /// <returns> Список наименований стандартов. </returns>
        [NotNull]
        public static List<string> GetStandardNames()
        {
            List<string> standardNames = new List<string>();
            foreach (TubeStandard tubeStandard in TubeStandards)
            {
                standardNames.Add(tubeStandard.name);
            }

            return standardNames;
        }

        /// <summary> Функция получения всех диаметров труб из запрашиваемого стандарта. </summary>
        /// <param name="standardName"> Наименование стандарта. </param>
        /// <returns> Список диаметров. </returns>
        [NotNull]
        public static List<float> GetDiameters(string standardName)
        {
            List<float> diameters = new List<float>();
            foreach (TubeStandard tubeStandard in TubeStandards)
            {
                if (tubeStandard.name != standardName) continue;

                foreach (TubeData tube in tubeStandard.available_tubes)
                {
                    diameters.Add(tube.diameter);
                }
            }

            return diameters;
        }

        /// <summary> Функция получения данных о последующей по диаметру трубе. </summary>
        /// <param name="tube"> Данная труба. </param>
        /// <param name="standardName"> Наименование стандарта. </param>
        /// <returns> Данные о трубе. </returns>
        [CanBeNull]
        public static TubeData GetBigger(TubeData tube, string standardName)
        {
            foreach (TubeStandard tubeStandard in TubeStandards)
            {
                if (tubeStandard.name != standardName) continue;

                for (int i = 0; i < tubeStandard.available_tubes.Count - 1; i++)
                {
                    if (Math.Abs(tubeStandard.available_tubes[i].diameter - tube.diameter) < float.Epsilon)
                    {
                        return tubeStandard.available_tubes[i + 1];
                    }
                }
            }

            return tube;
        }

        /// <summary> Функция получения данных о предыдущей по диаметру трубе. </summary>
        /// <param name="tube"> Данная труба. </param>
        /// <param name="standardName"> Наименование стандарта. </param>
        /// <returns> Данные о трубе. </returns>
        [CanBeNull]
        public static TubeData GetSmaller(TubeData tube, string standardName)
        {
            foreach (TubeStandard tubeStandard in TubeStandards)
            {
                if (tubeStandard.name != standardName) continue;

                for (int i = tubeStandard.available_tubes.Count - 1; i >= 1; i--)
                {
                    if (Math.Abs(tubeStandard.available_tubes[i].diameter - tube.diameter) < float.Epsilon)
                    {
                        return tubeStandard.available_tubes[i - 1];
                    }
                }
            }

            return tube;
        }

        #region Priavate definitions

        [Serializable]
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        private struct TubeStandard
        {
            public string         name;
            public List<TubeData> available_tubes;
        }

        private static readonly List<TubeStandard> TubeStandards = new List<TubeStandard>();

        /// <summary> Статический конструктор. Читает данные стандартов из файлов. </summary>
        static TubeLoader()
        {
            // TODO: Чтение из всех файлов в директории ./StreamingAssets/TubesConfig
            byte[] data = UnityEngine.Windows.File.ReadAllBytes(Path.Combine(Application.streamingAssetsPath,
                                                                             "./TubesConfig/OST.json"));
            string jsonTextFile = System.Text.Encoding.UTF8.GetString(data);
            TubeStandards.Add(JsonUtility.FromJson<TubeStandard>(jsonTextFile));
        }

        #endregion
    }
}
