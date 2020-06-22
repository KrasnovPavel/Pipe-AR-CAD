// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using HoloCore;
using UnityEngine;
#if ENABLE_WINMD_SUPPORT
using UnityEngine.XR.WSA.Input;
#endif

namespace HoloCAD.Bluetooth.Examples
{   
    /// <summary> Класс виджета измерителя расстояний между точками. </summary>
    [RequireComponent(typeof(LineRenderer))]
    public class LengthMeter : MonoBehaviour
    {
        /// <summary> Префаб точки. </summary>
        public GameObject PointPrefab;
        
        /// <summary> Контейнер для текстовых панелей. </summary>
        public Transform Panel;
        
        /// <summary> Текстовая панель с расстоянием измеренным по модели. </summary>
        public TextMesh ModelLengthLabel;
        
        /// <summary> Текстовая панель с расстоянием измеренным в реальном мире. </summary>
        public TextMesh RealLengthLabel;
        
        /// <summary> Состояние измерителя. </summary>
        public LengthMeterState State { get; private set; }

        #region Unity event functions

        void Start()
        {
            State = LengthMeterState.ChooseFirst;
            SpatialGrid.Instance.enabled = true;
            _firstPoint = Instantiate(PointPrefab, transform).transform;
            _lineRenderer = GetComponent<LineRenderer>();
            // ReSharper disable once PossibleNullReferenceException
            _camera = Camera.main.transform;

#if ENABLE_WINMD_SUPPORT
            _recognizer = new GestureRecognizer();
            _recognizer.Tapped += delegate { OnTap(); };
            _recognizer.StartCapturingGestures();
#endif
        }

        void Update()
        {
            switch (State)
            {
                case LengthMeterState.ChooseFirst:
                    Place(_firstPoint);
                    break;
                case LengthMeterState.ChooseSecond:
                    Place(_secondPoint);
                    _lineRenderer.SetPosition(1, _secondPoint.position);
                    break;
                case LengthMeterState.GetRealLength:
                    break;
                case LengthMeterState.Finished:
                    break;
            }
        }

        #endregion

        #region Private definitions

        private Transform _firstPoint;
        private Transform _secondPoint;
        private LineRenderer _lineRenderer;
        private Transform _camera;

        /// <summary> Обработчик получения данных с дальномера. </summary>
        /// <param name="length"> Расстояние. </param>
        private void OnDataReceived(float length)
        {
            if (State != LengthMeterState.GetRealLength) return;
            
            State = LengthMeterState.Finished;
            RealLengthLabel.gameObject.SetActive(true);
            RealLengthLabel.text = $"Измеренное расстояние: {length:F3}м";
            BluetoothDataProvider.NewDataReceived -= OnDataReceived;
        }
        
        /// <summary> Обработчик жеста "Тап". </summary>
        private void OnTap()
        {
            switch (State)
            {
                case LengthMeterState.ChooseFirst:
                    State = LengthMeterState.ChooseSecond;
                    _secondPoint = Instantiate(PointPrefab, transform).transform;
                    _lineRenderer.enabled = true;
                    _lineRenderer.positionCount = 2;
                    _lineRenderer.SetPosition(0, _firstPoint.position);
                    break;
                case LengthMeterState.ChooseSecond:
                    State = LengthMeterState.GetRealLength;
                    float length = (_firstPoint.position - _secondPoint.position).magnitude;
                    ModelLengthLabel.gameObject.SetActive(true);
                    ModelLengthLabel.text = $"Расстояние по модели: {length:F3}м.";
                    BluetoothDataProvider.NewDataReceived += OnDataReceived;
                    Panel.position = (_secondPoint.position - _firstPoint.position) / 2 + _firstPoint.position;
                    Panel.LookAt(_secondPoint);
                    SpatialGrid.Instance.enabled = false;
#if ENABLE_WINMD_SUPPORT
                    _recognizer.Dispose();
#endif
                    break;
                case LengthMeterState.GetRealLength:
                    break;
                case LengthMeterState.Finished:
                    break;
            }
        }

        /// <summary> Размещает переданный объект на поверхности куда смотрит пользователь. </summary>
        /// <param name="point"></param>
        private void Place(Transform point)
        {
            Vector3 headPosition = _camera.transform.position;
            Vector3 gazeDirection = _camera.transform.forward;
    
            RaycastHit hitInfo;
            if (!Physics.Raycast(headPosition, gazeDirection, out hitInfo, 30.0f)) return;

            point.position = hitInfo.point;
        }
        
#if ENABLE_WINMD_SUPPORT
        private GestureRecognizer _recognizer;
#endif
        
        #endregion
    }
    
    /// <summary> Возможные состояния виджета измерителя расстояния. </summary>
    public enum LengthMeterState 
    {
        /// <summary> Выбор первой точки. </summary>
        ChooseFirst,
        /// <summary> Выбор второй точки. </summary>
        ChooseSecond,
        /// <summary> Ожидание данных с дальномера. </summary>
        GetRealLength,
        /// <summary> Работа завершена. </summary>
        Finished
    }
}
