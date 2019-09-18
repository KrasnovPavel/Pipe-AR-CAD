using System;
using System.Configuration;
using HoloCAD;
using HoloCore;
using UnityEngine;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace HoloCAD
{
    /// <summary>Класс для переключения режимов отображения модели, привязанной к метке </summary>
    public class ViewModes : MonoBehaviour, INotifyPropertyChanged
    {
        /// <summary> Событие измененения свойств объекта </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        
        /// <summary> Объект стен </summary>
        public GameObject Walls;

        /// <summary>Возможные состояния отображения модели </summary>
        public enum Modes : byte
        {
            Visible,
            VisibleWithoutWalls,
            SpatialMapping,
            Invisible
        };

        /// <summary>Состояние отображения модели на данный момент </summary>
        public Modes Current
        {
            get => _current;
            set
            {
                if (Enum.IsDefined(typeof(Modes), value) && value!= _current)
                {
                    _current = value;
                    OnPropertyChanged(nameof(Current));
                }
            }
        }

        /// <summary> Сохраняет состояние отображения на данный момент внутри диапазона возможных состояний отображения  </summary>
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



        /// <summary> Меняет характеристики отображения модели в соотвествии с нынешним состоянием отображения </summary>
        private void ChangeModelToCurrentState()
        {
            switch (_current)
            {
                case Modes.Visible:
                    ChangeTargetToVisible();
                    break;
                case Modes.VisibleWithoutWalls:
                    ChangeTargetToVisibleWithoutWalls();
                    break;
                case Modes.SpatialMapping:
                    ChangeTargetToSpatialMappingMode();
                    break;
                case Modes.Invisible:
                    ChangeTargetToInvisible();
                    break;
            }
        }

        /// <summary> Делает модель видимой </summary>
        private void ChangeTargetToVisible()
        {
            UnityTubes.TubeUnityManager.ShowGrid(false);
            gameObject.SetActive(true);
            MeshRenderer walls = Walls.GetComponent<MeshRenderer>();
            walls.sharedMaterial.color = Color.grey;
        }

        /// <summary> Делает модель видимой, но стены прозрачными </summary>
        private void ChangeTargetToVisibleWithoutWalls()
        {
            UnityTubes.TubeUnityManager.ShowGrid(false);
            gameObject.SetActive(true);
            MeshRenderer walls = Walls.GetComponent<MeshRenderer>();
            walls.sharedMaterial.color = new Color(0.3f, 0.3f, 0.3f, 1f);
        }

        /// <summary> Делает модель невидимой </summary>
        private void ChangeTargetToInvisible()
        {
            UnityTubes.TubeUnityManager.ShowGrid(false);
            gameObject.SetActive(false);
        }

        /// <summary> Включает сетку поверхности </summary>
        private void ChangeTargetToSpatialMappingMode()
        {
            gameObject.SetActive(false);
            UnityTubes.TubeUnityManager.ShowGrid(true);
        }

        #endregion


        #region Unity events

        private void Start()
        {
            
            PropertyChanged += delegate(object sender, PropertyChangedEventArgs args)
            {
                if (args.PropertyName == nameof(Current))
                {
                    ChangeModelToCurrentState();
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