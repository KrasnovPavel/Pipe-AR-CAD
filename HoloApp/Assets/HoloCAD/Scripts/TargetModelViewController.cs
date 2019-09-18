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
    public class TargetModelViewController : Singleton<TargetModelViewController>, INotifyPropertyChanged
    {
        /// <summary> Событие измененения свойств объекта </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>Привязанная модель </summary>
        public GameObject Target;

        /// <summary>Возможные состояния отображения модели </summary>
        public enum States : byte
        {
            Visible,
            VisibleWithoutWalls,
            SpatialMapping,
            Invisible
        };

        /// <summary>Состояние отображения модели на данный момент </summary>
        public States CurrentState
        {
            get => _currentState;
            set
            {
                value = NextState(value);
                if (Enum.IsDefined(typeof(States), value))
                {
                    _currentState = value;
                    OnPropertyChanged(nameof(CurrentState));
                }
            }
        }

        #region Private defenitions

        private States _currentState;

        /// <summary> Сохраняет состояние отображения на данный момент внутри диапазона возможных состояний отображения  </summary>
        /// <param name="value">Новое состояние отображения</param>
        /// <returns>Новое состояние отображения, лежащее внутри диапазона состояний отображения</returns>
        private States NextState(States value)
        {
            byte valueByte = (byte) value;
            valueByte %= (byte) Enum.GetValues(typeof(States)).Length;
            value = (States) valueByte;
            return value;
        }


        /// <summary> Меняет характеристики отображения модели в соотвествии с нынешним состоянием отображения </summary>
        private void ChangeModelToCurrentState()
        {
            switch (_currentState)
            {
                case States.Visible:
                    ChangeTargetToVisible();
                    break;
                case States.VisibleWithoutWalls:
                    ChangeTargetToVisibleWithoutWalls();
                    break;
                case States.SpatialMapping:
                    ChangeTargetToSpatialMappingMode();
                    break;
                case States.Invisible:
                    ChangeTargetToInvisible();
                    break;
            }
        }

        /// <summary> Делает модель видимой </summary>
        private void ChangeTargetToVisible()
        {
            UnityTubes.TubeUnityManager.ShowGrid(false);
            Target.gameObject.SetActive(true);
            MeshRenderer walls = Target.transform.Find("Low_Pole_re/Korpus_2").GetComponent<MeshRenderer>();
            walls.sharedMaterial.color = Color.grey;
        }

        /// <summary> Делает модель видимой, но стены прозрачными </summary>
        private void ChangeTargetToVisibleWithoutWalls()
        {
            UnityTubes.TubeUnityManager.ShowGrid(false);
            Target.gameObject.SetActive(true);
            MeshRenderer walls = Target.transform.Find("Low_Pole_re/Korpus_2").GetComponent<MeshRenderer>();
            walls.sharedMaterial.color = new Color(0.3f, 0.3f, 0.3f, 1f);
        }

        /// <summary> Делает модель невидимой </summary>
        private void ChangeTargetToInvisible()
        {
            UnityTubes.TubeUnityManager.ShowGrid(false);
            Target.gameObject.SetActive(false);
        }

        /// <summary> Включает сетку поверхности </summary>
        private void ChangeTargetToSpatialMappingMode()
        {
            Target.gameObject.SetActive(false);
            UnityTubes.TubeUnityManager.ShowGrid(true);
        }

        #endregion


        #region Unity events

        private void Start()
        {
            
            PropertyChanged += delegate(object sender, PropertyChangedEventArgs args)
            {
                if (args.PropertyName == nameof(CurrentState))
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