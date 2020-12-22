// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

// using System;
// using System.Collections.Generic;
// using System.Diagnostics.CodeAnalysis;
// using System.Linq;
// using System.Text;
// using UnityEngine;

// namespace HoloCAD.Tubes.Model
// {
//     public static class TubeSerializer
//     {
//         
        // /// <summary> Импорт трубы из json. </summary>
        // /// <remarks> Для выбора файла будет вызван диалог сохранения файла. </remarks>
        // /// <param name="data"> Данные о трубах. </param>
        // /// <param name="marks"> Коллекция меток для привязки труб. </param>
        // /// <return> Массив всех труб на сцене. </return>
        // public static void DeserializeScheme(byte[] data, List<Transform> marks = null)
        // {
        //     ExpTubesArray array = JsonUtility.FromJson<ExpTubesArray>(Encoding.UTF8.GetString(data));
        //     marks = marks ?? new List<Transform>();
        //
        //     for (int i = 0; i < array.tubes.Count; i++)
        //     {
        //         ExpTube expTube = array.tubes[i];
        //         Transform parent = i < marks.Count ? marks[i] : null;
        //         StartTubeFragment start = parent != null ? parent.Find("TubeStart(Clone)").GetComponent<StartTubeFragment>() 
        //                                                  : null;
        //
        //         TubeFragment lastFragment;
        //         if (start == null) return;
        //
        //         if (parent == null || start.HasChild)
        //         {
        //             TubeLoader.TubeData currentTubeData =
        //                 TubeLoader.FindTubeData((float) expTube.diameter / 1000, expTube.standard_name);
        //             Tube tube = new Tube(expTube.standard_name, currentTubeData);
        //             lastFragment = tube.StartFragment;  
        //         }
        //         else
        //         {
        //             start.Owner.Data = TubeLoader.FindTubeData((float) expTube.diameter / 1000, 
        //                                                         expTube.standard_name);
        //             lastFragment = start;
        //         }
        //         
        //         foreach (ExpFragment expFragment in expTube.fragments)
        //         {
        //             switch (expFragment.type)
        //             {
        //                 case "Direct":
        //                     lastFragment.AddDirectFragment();
        //                     break;
        //                 case "Bended":
        //                     lastFragment.AddBendFragment();
        //                     break;
        //             }
        //
        //             lastFragment = lastFragment.Child;
        //             ConvertFromExportFormat(lastFragment, expFragment);
        //         }
        //     }
        // }
        
//     }
// }
