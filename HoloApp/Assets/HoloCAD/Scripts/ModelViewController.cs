using System;
using HoloCAD;
using HoloCore;
using UnityEngine;

namespace HoloCAD
{
    /// <summary>Класс для переключения режимов отображения модели, привязанной к метке </summary>
    public class ModelViewController : Singleton<ModelViewController>
    {
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
                byte valueByte = (byte) value;
                valueByte %= (byte) Enum.GetValues(typeof(States)).Length;
                Debug.Log($"Current State: {valueByte}");
                value = (States) valueByte;
                if (Enum.IsDefined(typeof(States), value)){                
                    _currentState = value;
                    ChangeModelToCurrentState();
                }

            }
        }

        #region Private defenitions
        
        private States _currentState;

        /// <summary> Меняет характеристики отображения модели в соотвествии с сейчашним состоянием отображения </summary>
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
        private  void ChangeTargetToVisible()
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
        private  void ChangeTargetToInvisible()
        {
            UnityTubes.TubeUnityManager.ShowGrid(false);
            Target.gameObject.SetActive(false);
        }
        
        /// <summary> Включает сетку поверхности </summary>
        private  void ChangeTargetToSpatialMappingMode()
        {
            Target.gameObject.SetActive(false);
            UnityTubes.TubeUnityManager.ShowGrid(true);
        }
    
        #endregion
    }
}
