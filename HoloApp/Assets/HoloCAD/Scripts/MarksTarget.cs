// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using HoloCAD.UI;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.XR.WSA;

namespace HoloCAD
{
    /// <summary> Скрипт, выполняющий привязку модели к меткам. </summary>
    public sealed class MarksTarget : MonoBehaviour, INotifyPropertyChanged
    {
        /// <summary> Объект с мешем модели. </summary>
        public GameObject Model;

        /// <summary> Alpha текстуры модели в режиме прозрачности. </summary>
        public float TransparentAlpha = 0.5f;
        
        /// <summary> Список меток. </summary>
        [Tooltip("Список меток.")]
        public List<Mark> Marks = new List<Mark>();
        
        /// <summary> Список положений модели относительно меток. </summary>
        [Tooltip("Список положений модели относительно меток.")]
        public List<Vector3> PositionsOfMarks = new List<Vector3>();
        
        /// <summary> Список поворотов модели относительно меток. </summary>
        [Tooltip("Список поворотов модели относительно меток.")]
        public List<Vector3> RotationsOfMarks = new List<Vector3>();

        public event PropertyChangedEventHandler PropertyChanged;
        
        /// <summary> Изменение положения модели относительно метки. </summary>
        /// <param name="mark"> Метка. </param>
        /// <param name="delta"> Сдвиг. </param>
        public void ChangePosition(Mark mark, Vector3 delta)
        {
            int markId = Marks.FindIndex(obj => obj == mark);

            if (markId == -1) return;

            PositionsOfMarks[markId] += delta;
            OnPropertyChanged(nameof(PositionsOfMarks));
        }

        /// <summary> Переключает прозрачность модели, в том числе и прозрачность для курсора. </summary>
        /// <param name="transparent"></param>
        public void MakeTransparent(bool transparent)
        {
            Model.GetComponent<MeshCollider>().enabled = !transparent;
            Model.GetComponent<MeshRenderer>().sharedMaterial.SetFloat(Alpha, transparent ? TransparentAlpha : 1f);
        }
        
        /// <summary> Изменение поворота модели относительно метки. </summary>
        /// <param name="mark"> Метки. </param>
        /// <param name="delta"> Поворот. </param>
        public void ChangeRotation(Mark mark, Vector3 delta)
        {
            int markId = Marks.FindIndex(obj => obj == mark);

            if (markId == -1) return;

            RotationsOfMarks[markId] += delta;
            OnPropertyChanged(nameof(RotationsOfMarks));
        }

        /// <summary> Полуение положения по метке. </summary>
        /// <param name="mark"> Метка. </param>
        /// <returns> null если метка не соответствует этой модели. </returns>
        public Vector3? GetPosition(Mark mark)
        {
            int markId = Marks.FindIndex(obj => obj == mark);

            if (markId == -1) return null;

            return PositionsOfMarks[markId];
        }

        /// <summary> Полуение поворота по метке. </summary>
        /// <param name="mark"> Метка. </param>
        /// <returns> null если метка не соответствует этой модели. </returns>
        public Vector3? GetRotation(Mark mark)
        {
            int markId = Marks.FindIndex(obj => obj == mark);

            if (markId == -1) return null;

            return RotationsOfMarks[markId];
        }

        #region Unity event function

        private void Awake()
        {
            foreach (Mark mark in Marks)
            {
                if (mark == null) continue;
                GameObject markPanel = mark.transform.GetChild(0).gameObject;
                if (markPanel == null) continue;
                markPanel.GetComponent<MarkControlPanel>().Target = this;
            }
        }


        private void Update()
        {
            int markId = Marks.FindIndex(obj => obj != null && obj.enabled && obj.IsActive);

            if (markId == -1) return;
            
            Debug.Log($"mark{markId}");
            Transform currentMark = Marks[markId].transform;

            transform.SetParent(currentMark, false);
            transform.localScale = new Vector3(1 / currentMark.lossyScale.x, 
                                               1 / currentMark.lossyScale.z, 
                                               1 / currentMark.lossyScale.z);
            transform.localPosition = PositionsOfMarks[markId]*-6.25f;
            transform.localRotation = Quaternion.Euler(RotationsOfMarks[markId]);

            transform.SetParent(null, true);
            Debug.Log($"!!!!!!!{transform.localPosition.x},{transform.localPosition.y},{transform.localPosition.z}");
        }

        #endregion

        #region Private definitions
        
        private static readonly int Alpha = Shader.PropertyToID("_Alpha");
        private SpatialMappingCollider _spatialCollider;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
