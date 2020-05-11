// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UnityEngine;

namespace HoloCore.Marks
{
    /// <summary> Класс для переключения режимов отображения модели, привязанной к метке </summary>
    public class ViewModes : MonoBehaviour, INotifyPropertyChanged
    {
        /// <summary> Событие измененения свойств объекта </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        
        /// <summary> Объект стен </summary>
        public GameObject Walls;

        /// <summary> Возможные состояния отображения модели </summary>
        public enum Modes : byte
        {
            Visible,
            VisibleWithoutWalls,
            SpatialMapping,
            Invisible
        };

        /// <summary> Состояние отображения модели на данный момент </summary>
        public Modes Current
        {
            get => _current;
            set
            {
                if (Enum.IsDefined(typeof(Modes), value) && value != _current)
                {
                    _current = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary> Переключает состояние отображения на следующее.  </summary>
        public void Next()
        {
            _current++;
            byte valueByte = (byte) _current;
            valueByte %= (byte) Enum.GetValues(typeof(Modes)).Length;
            _current = (Modes) valueByte;
            OnPropertyChanged(nameof(Current));
        }
        
        #region Private defenitions

        private Modes _current;

        /// <summary> Отображает модель в нужном виде </summary>
        private void ChangeModelView()
        {
            switch (_current)
            {
                case Modes.Visible:
                    MakeTargetVisible();
                    break;
                case Modes.VisibleWithoutWalls:
                    MakeTargetVisibleWithoutWalls();
                    break;
                case Modes.SpatialMapping:
                    DrawSpatialMapping();
                    break;
                case Modes.Invisible:
                    MakeTargetInvisible();
                    break;
            }
        }

        /// <summary> Делает модель видимой </summary>
        private void MakeTargetVisible()
        {
            SpatialGrid.Instance.enabled = false;
            gameObject.SetActive(true);
            Walls.SetActive(true);
        }

        /// <summary> Делает модель видимой, но стены прозрачными </summary>
        private void MakeTargetVisibleWithoutWalls()
        {
            SpatialGrid.Instance.enabled = false;
            gameObject.SetActive(true);
            Walls.SetActive(false);
        }

        /// <summary> Делает модель невидимой </summary>
        private void MakeTargetInvisible()
        {
            SpatialGrid.Instance.enabled = false;
            gameObject.SetActive(false);
        }

        /// <summary> Включает сетку поверхности </summary>
        private void DrawSpatialMapping()
        {
            gameObject.SetActive(false);
            SpatialGrid.Instance.enabled = true;
        }

        #endregion

        #region Unity events

        private void Start()
        {
            PropertyChanged += delegate(object sender, PropertyChangedEventArgs args)
            {
                if (args.PropertyName == nameof(Current))
                {
                    ChangeModelView();
                }
            };
        }
        
        #endregion

        #region Protected defenitions
        
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
        #endregion
    }
}