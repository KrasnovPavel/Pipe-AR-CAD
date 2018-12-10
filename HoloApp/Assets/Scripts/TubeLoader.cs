using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

/// <summary>
/// Класс, реализующий загрузку данных стандартов труб.   
/// </summary>
[Serializable]
public static class TubeLoader
{
    [Serializable]
    private struct TubeStandard
    {
        public string name;
        public List<TubeData> available_tubes;
    }

    /// <summary>
    /// Класс, хранящий данные трубы.
    /// </summary>
    [Serializable]
    public class TubeData
    {
        public string name;
        public float diameter;
        public float first_radius;
        public float second_radius;
    }

    private static readonly List<TubeStandard> TubeStandards = new List<TubeStandard>();

    static TubeLoader()
    {
        string jsonTextFile = Resources.Load<TextAsset>("TubesConfig/OST").text;

        TubeStandards.Add(JsonUtility.FromJson<TubeStandard>(jsonTextFile));
    }

    /// <summary>
    /// Функция поиска данных о трубе по диаметру и наименованию стандарта.  
    /// </summary>
    /// <param name="diameter">Диаметр искомой трубы. </param>
    /// <param name="standardName">Стандарт искомой трубы.</param>
    /// <returns>Данные о трубе. </returns>
    [CanBeNull]
    public static TubeData FindTubeData(float diameter, string standardName)
    {
        IEnumerable<TubeData> tubes = GetAvailableTubes(standardName);

        foreach (var tube in tubes)
        {
            if (Math.Abs(tube.diameter - diameter) <= float.Epsilon)
            {
                return tube;
            }
        }

        return null;
    }

    /// <summary>
    /// Функция получения данных о всех трубах из запрошенного стандарта. 
    /// </summary>
    /// <param name="standardName">Запрошенный стандарт. </param>
    /// <returns>Список труб из запрошенного стндарта. </returns>
    [NotNull]
    public static IEnumerable<TubeData> GetAvailableTubes(string standardName)
    {
        if (standardName.Length == 0 && TubeStandards.Count > 0)
        {
            return TubeStandards[0].available_tubes;
        }

        foreach (var standard in TubeStandards)
        {
            if (standard.name == standardName)
            {
                return standard.available_tubes;
            }
        }

        return new List<TubeData>();
    }

    /// <summary>
    /// Функция получения всех стандартов.
    /// </summary>
    /// <returns>Список наименований стандартов. </returns>
    [NotNull]
    public static List<string> GetStandardNames()
    {
        List<string> standardNames = new List<string>();
        foreach (var tubeStandard in TubeStandards)
        {
            standardNames.Add(tubeStandard.name);
        }

        return standardNames;
    }
    
    /// <summary>
    /// Функция получения всех диаметров труб из запрашиваемого стандарта. 
    /// </summary>
    /// <param name="standardName">Наименование стандарта. </param>
    /// <returns>Список диаметров.</returns>
    [NotNull]
    public static List<float> GetDiameters(string standardName)
    {
        List<float> diameters = new List<float>();
        foreach (var tubeStandard in TubeStandards)
        {
            if (tubeStandard.name != standardName) continue;

            foreach (var tube in tubeStandard.available_tubes)
            {
                diameters.Add(tube.diameter);
            }
        }

        return diameters;
    }
    
    /// <summary>
    /// Функция получения данных о последующей по диаметру трубе.
    /// </summary>
    /// <param name="tube">Данная труба. </param>
    /// <param name="standardName">Наименование стандарта.</param>
    /// <returns>Данные о трубе.</returns>
    [CanBeNull]
    public static TubeData GetBigger(TubeData tube, string standardName)
    {
        foreach (var tubeStandard in TubeStandards)
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

        return null;
    }

    /// <summary>
    /// Функция получения данных о предыдущей по диаметру трубе.
    /// </summary>
    /// <param name="tube">Данная труба. </param>
    /// <param name="standardName">Наименование стандарта.</param>
    /// <returns>Данные о трубе.</returns>
    [CanBeNull]
    public static TubeData GetSmaller(TubeData tube, string standardName)
    {
        foreach (var tubeStandard in TubeStandards)
        {
            if (tubeStandard.name != standardName) continue;

            for (int i = tubeStandard.available_tubes.Count - 1; i > 0; i--)
            {
                if (Math.Abs(tubeStandard.available_tubes[i].diameter - tube.diameter) < float.Epsilon)
                {
                    return tubeStandard.available_tubes[i + 1];
                }
            }
        }

        return null;
    }
}