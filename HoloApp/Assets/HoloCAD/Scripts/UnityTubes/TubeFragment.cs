using System;
using HoloCAD.UI;
using HoloCore.UI;
using JetBrains.Annotations;
using UnityEngine;

namespace HoloCAD.UnityTubes
{
    /// <inheritdoc />
    /// <summary> Базовый класс участка трубы. От него наследуются все остальные классы участков труб. </summary>
    public class TubeFragment : MonoBehaviour
    {
        /// <summary> Поле хранящее диаметр участка трубы. </summary>
        /// <remarks> ВНИМАНИЕ!!! НАПРЯМУЮ ОБРАЩАТЬСЯ К НЕМУ ЗАПРЕЩЕНО!!! Используйте <see cref="Diameter"/>. </remarks>
        protected float _diameter;

        /// <summary> Объект, содержащий меш участка трубы. </summary>
        protected GameObject Tube;

        /// <summary> Объект, указывающий конечную точку участка трубы. </summary>
        public GameObject EndPoint { get; protected set; }

        /// <summary> Панель с кнопками. </summary>
        protected GameObject ButtonBar;

        /// <summary> Надпись с информацией о участке трубы. </summary>
        [Tooltip("Надпись с информацией о участке трубы.")]
        [Obsolete("Теперь вместо прямого доступа к надписи используется TubeFragmentControlPanel")]
        public GameObject Label;
        
        // TODO: Перенести работу с кнопками в ControlPanel.
        /// <summary> Кнопка добавления погиба. </summary>
        [Tooltip("Кнопка добавления погиба.")]
        [CanBeNull] public Button3D AddBendFragmentButton;

        /// <summary> Кнопка добавления прямого участка трубы. </summary>
        [Tooltip("Кнопка добавления прямого участка трубы.")]
        [CanBeNull] public Button3D AddDirectFragmentButton;

        /// <summary> Кнопка создания новой трубы. </summary>
        [Tooltip("Кнопка создания новой трубы.")]
        [CanBeNull] public Button3D CreateTubeButton;

        /// <summary> Кнопка удаления этого участка трубы. </summary>
        [Tooltip("Кнопка удаления этого участка трубы.")]
        [CanBeNull] public Button3D RemoveThisFragmentButton;
        
        /// <summary> Кнопка добавления объекта отображения расстояния между трубами. </summary>
        [Tooltip("Кнопка добавления объекта отображения расстояния между трубами.")]
        [CanBeNull] public Button3D ConnectTubesButton;
        
        //TODO: Сделать по другому. Вешать на каждый фрагмент кнопку сохранения всей сцены - ущербно.
        /// <summary> Кнопка сохранения сцены. </summary>
        [Tooltip("Кнопка добавления объекта отображения расстояния между трубами.")]
        [CanBeNull] public Button3D SaveSceneButton;

        /// <summary> Флаг, находится ли участок трубы в режиме перемещения. </summary>
        public bool IsPlacing
        {
            get => _isPlacing;
            set
            {
                _isPlacing = value;
                Tube.GetComponent<MeshCollider>().enabled = !_isPlacing;
            }
        }

        /// <summary> Труба-хозяин этого участка. </summary>
        public Tube Owner;

        /// <summary> Состояние участка трубы. Выбрана он или нет. </summary>
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected == value) return;

                _isSelected = value;

                SetColor();
                if (ButtonBar != null) ButtonBar.SetActive(_isSelected);
                
                var controlPanel = GetComponent<TubeFragmentControlPanel>();
                if (controlPanel != null) controlPanel.enabled = _isSelected;
            }
        }

        /// <summary> Диаметр участка трубы. </summary>
        public virtual float Diameter
        {
            get => _diameter;
            set => _diameter = value;
        }

        /// <summary> Выходит ли из этого участка трубы другой? </summary>
        public virtual bool HasChild
        {
            get => _hasChild;
            set
            {
                if (_hasChild == value) return;

                _hasChild = value;
                if (AddBendFragmentButton != null) AddBendFragmentButton.SetEnabled(!_hasChild);
                if (AddDirectFragmentButton != null) AddDirectFragmentButton.SetEnabled(!_hasChild);
            }
        }

        /// <summary> Функция, которая вызывается когда этот участок трубы пересекается с другим </summary>
        /// <remarks>
        /// При переопределении в потомке обязательно должна вызываться с помощью <c> base.OnTubeCollisionEnter()</c>.
        /// </remarks>
        public virtual void OnTubeCollisionEnter()
        {
            _isColliding = true;
            SetColor();
        }

        /// <summary> Функция, которая вызывается когда этот участок трубы перестает пересекаться с другим </summary>
        /// <remarks>
        /// При переопределении в потомке обязательно должна вызываться с помощью <c> base.OnTubeCollisionExit()</c>.
        /// </remarks>
        public virtual void OnTubeCollisionExit()
        {
            _isColliding = false;
            SetColor();
        }

        /// <summary> Добавление нового погиба. </summary>
        /// <remarks>
        /// При переопределении в потомке обязательно должна вызываться с помощью <c> base.AddBendFragment()</c>.
        /// </remarks>
        public virtual void AddBendFragment()
        {
            Owner.CreateBendedTubeFragment(EndPoint.transform);
        }

        /// <summary> Добавление нового прямого участка трубы. </summary>
        /// <remarks>
        /// При переопределении в потомке обязательно должна вызываться с помощью <c> base.AddDirectFragment()</c>.
        /// </remarks>
        public virtual void AddDirectFragment()
        {
            Owner.CreateDirectTubeFragment(EndPoint.transform);
        }

        /// <summary> Создание новой трубы. </summary>
        /// <remarks>
        /// При переопределении в потомке обязательно должна вызываться с помощью <c> base.CreateFragment()</c>.
        /// </remarks>
        public virtual void CreateTube()
        {
            TubeManager.CreateTube(Owner);
        }

        /// <summary> Удаление этого участка трубы. </summary>
        /// <remarks>
        /// При переопределении в потомке обязательно должна вызываться с помощью <c> base.RemoveThisFragment()</c>.
        /// </remarks>
        public virtual void RemoveThisFragment()
        {
            Destroy(gameObject);
        }

        /// <summary> Создает объект соединения труб. </summary>
        /// <remarks>
        /// При переопределении в потомке обязательно должна вызываться с помощью <c> base.ConnectTubes()</c>.
        /// </remarks>
        public virtual void ConnectTubes()
        {
            Owner.CreateTubesConnector();
        }

        /// <summary> Включает или выключает кнопку соединения труб, в зависимости от внутренней логики. </summary>
        /// <remarks>
        /// При переопределении в потомке обязательно должна вызываться с помощью <c> base.CheckConnectButton()</c>.
        /// </remarks>
        public virtual void CheckConnectButton()
        {
            if (ConnectTubesButton == null) return;
            
            ConnectTubesButton.SetEnabled(!Owner.HasTubesConnector && !TubeUnityManager.HasActiveTubesConnector);
        }
        
        /// <summary> Функция, инициализирующая кнопки. </summary>
        /// <remarks>
        /// При переопределении в потомке обязательно должна вызываться с помощью <c> base.InitButtons()</c>.
        /// </remarks>
        protected virtual void InitButtons()
        {
            if (AddBendFragmentButton != null) AddBendFragmentButton.OnClick += delegate { AddBendFragment(); };
            if (AddDirectFragmentButton != null) AddDirectFragmentButton.OnClick += delegate { AddDirectFragment(); };
            if (CreateTubeButton != null) CreateTubeButton.OnClick += delegate { CreateTube(); };
            if (RemoveThisFragmentButton != null)RemoveThisFragmentButton.OnClick += delegate { RemoveThisFragment(); };
            if (ConnectTubesButton != null)
            {
                ConnectTubesButton.SetEnabled(!Owner.HasTubesConnector);
                ConnectTubesButton.OnClick += delegate { ConnectTubes(); };
            }
            if (SaveSceneButton != null)
            {
                SaveSceneButton.OnClick += delegate
                {
                    SchemeExporter.Export(TubeManager.AllTubes);
                };
            }
        }
        
        /// <summary> Функция, которая устанавлявает соответствующий цвет участку трубы. </summary>
        /// <remarks>
        /// При переопределении в потомке обязательно должна вызываться с помощью <c> base.SetColor()</c>.
        /// </remarks>
        protected virtual void SetColor()
        {
            if (IsSelected)
            {
                Tube.GetComponent<MeshRenderer>().material.SetColor(GridColor, SelectedTubeColor);
                return;
            }

            Tube.GetComponent<MeshRenderer>().material.SetColor(GridColor,
                _isColliding ? CollidingTubeColor : DefaultTubeColor);
        }

        #region Unity event functions

        /// <summary> Функция, инициализирующая участок трубы в Unity. </summary>
        /// <remarks>
        /// При переопределении в потомке обязательно должна вызываться с помощью <c> base.Start()</c>.
        /// </remarks>
        protected virtual void Start()
        {
            tag = "Tube";
            Tube = transform.Find("Tube").gameObject;
            TubeFragmentCollider tubeCollider = Tube.transform.Find("Collider")?.GetComponent<TubeFragmentCollider>();
            if (tubeCollider != null)
            {
                tubeCollider.Owner = this;
            }

            EndPoint = transform.Find("End Point").gameObject;
            Transform bb = transform.Find("Button Bar");
            if (bb == null)
            {
                bb = EndPoint.transform.Find("Button Bar");
            }

            ButtonBar = bb.gameObject;
            Diameter = Owner.Data.diameter;
            InitButtons();
        }

        /// <summary> Функция, выполняющаяся в Unity каждый кадр. </summary>
        /// <remarks>
        /// При переопределении в потомке обязательно должна вызываться с помощью <c> base.Update()</c>.
        /// </remarks>
        protected virtual void Update()
        {
        }
        
        /// <summary> Обработчик удаления этого участка трубы. </summary>
        /// <remarks>
        /// При переопределении в потомке обязательно должна вызываться с помощью <c> base.OnDestroy()</c>.
        /// </remarks>
        protected void OnDestroy()
        {
            Owner.OnFragmentRemoved(this);
        }

        #endregion

        #region Private definitions

        private bool _isSelected;
        private bool _isPlacing;
        private bool _isColliding;
        private bool _hasChild;
        private bool _hasTransformError;
        private static readonly int GridColor = Shader.PropertyToID("_GridColor");

        /// <summary> Цвет участка трубы. </summary>
        private static readonly Color DefaultTubeColor = new Color(1f, 1f, 0f, 1f);

        /// <summary> Цвет участка трубы, когда она выбрана. </summary>
        private static readonly Color SelectedTubeColor = new Color(0f, 1f, 0f, 1f);

        /// <summary> Цвет участка трубы, когда она пересекается с другим участком трубы. </summary>
        private static readonly Color CollidingTubeColor = new Color(1f, 0f, 0f, 1f);

        #endregion
    }
}
