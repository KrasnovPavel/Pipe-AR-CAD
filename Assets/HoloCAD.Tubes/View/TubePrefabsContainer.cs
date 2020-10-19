// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using HoloCore;
using UnityEngine;

namespace HoloCAD.Tubes.View
{
    public class TubePrefabsContainer : Singleton<TubePrefabsContainer>
    {
        /// <summary> Префаб виджета точки. </summary>
        public GameObject PointPrefab;

        /// <summary> Префаб виджета отрезка. </summary>
        public GameObject SegmentPrefab;

        /// <summary> Префаб виджета фланца. </summary>
        public GameObject FlangeView;
    }
}
