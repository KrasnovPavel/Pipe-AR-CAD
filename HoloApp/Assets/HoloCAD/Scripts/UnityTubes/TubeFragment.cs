// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using HoloCAD.UI;
using HoloCAD.UI.TubeControls;
using JetBrains.Annotations;
using UnityEngine;

namespace HoloCAD.UnityTubes
{
    /// <inheritdoc cref="MonoBehaviour" />
    /// <summary> Базовый класс участка трубы. От него наследуются все остальные классы участков труб. </summary>
    public class TubeFragment : MonoBehaviour, INotifyPropertyChanged, ISelectable
    {
        /// <summary> Объект, содержащий меш участка трубы. </summary>
        protected GameObject Tube;

        /// <summary> Объект, указывающий конечную точку участка трубы. </summary>
        public GameObject EndPoint { get; protected set; }

        /// <summary> Флаг, находится ли участок трубы в режиме перемещения. </summary>
        public virtual bool IsPlacing { get; set; }

        /// <summary> Труба-хозяин этого участка. </summary>
        public Tube Owner;

        /// <summary> Состояние участка трубы. Выбрана он или нет. </summary>
        public bool IsSelected => SelectableObject.SelectedObject == gameObject;

        /// <inheritdoc/>
        public void ToggleSelection()
        {
            GetComponent<SelectableObject>().ToggleSelection();
        }

        /// <inheritdoc/>
        public void SelectThis()
        {
            GetComponent<SelectableObject>().SelectThis();
        }

        /// <inheritdoc/>
        public void DeselectThis()
        {
            GetComponent<SelectableObject>().DeselectThis();
        }

        /// <inheritdoc/>
        public void OnSelect()
        {
            SetColor();
            var controlPanel = GetComponent<TubeFragmentControlPanel>();
            if (controlPanel != null) controlPanel.enabled = true;
        }

        /// <inheritdoc/>
        public void OnDeselect()
        {
            SetColor();
            var controlPanel = GetComponent<TubeFragmentControlPanel>();
            if (controlPanel != null) controlPanel.enabled = false;
        }

        /// <summary> Выбирает следующий отрезок трубы. </summary>
        public void SelectChild()
        {
            if (HasChild) Child.SelectThis();
        }

        /// <summary> Выбирает предыдущий отрезок трубы. </summary>
        public void SelectParent()
        {
            if (Parent != null) Parent.SelectThis();
        }

        /// <summary> Диаметр участка трубы. </summary>
        public virtual float Diameter
        {
            get => _diameter;
            set
            {
                if (Math.Abs(_diameter - value) < float.Epsilon) return;

                _diameter = value;
                OnPropertyChanged();
            }
        }

        /// <summary> Выходит ли из этого участка трубы другой? </summary>
        public bool HasChild => Child != null;

        /// <summary> Следующий фрагмент трубы. </summary>
        public TubeFragment Child
        {
            get => _child;
            set
            {
                if (_child == value) return;

                _child = value;
                OnPropertyChanged();
            }
        }

        /// <summary> Предыдущий фрагмент трубы. </summary>
        public TubeFragment Parent
        {
            get => _parent;
            set
            {
                if (_parent == value) return;

                _parent = value;
                OnPropertyChanged();
            }
        }

        /// <summary> Пересекается ли этот участок трубы с другим? </summary>
        public bool IsColliding
        {
            get => _isColliding;
            private set
            {
                if (_isColliding == value) return;

                _isColliding = value;
                OnPropertyChanged();
            }
        }

        /// <summary> Функция, которая вызывается когда этот участок трубы пересекается с другим </summary>
        /// <remarks>
        /// При переопределении в потомке обязательно должна вызываться с помощью <c> base.OnTubeCollisionEnter()</c>.
        /// </remarks>
        public virtual void OnTubeCollisionEnter()
        {
            IsColliding = true;
            SetColor();
        }

        /// <summary> Функция, которая вызывается когда этот участок трубы перестает пересекаться с другим </summary>
        /// <remarks>
        /// При переопределении в потомке обязательно должна вызываться с помощью <c> base.OnTubeCollisionExit()</c>.
        /// </remarks>
        public virtual void OnTubeCollisionExit()
        {
            IsColliding = false;
            SetColor();
        }

        /// <summary> Добавление нового погиба. </summary>
        /// <remarks>
        /// При переопределении в потомке обязательно должна вызываться с помощью <c> base.AddBendFragment()</c>.
        /// </remarks>
        public virtual void AddBendFragment()
        {
            if (!HasChild) Child = TubeUnityManager.CreateBendedTubeFragment(Owner, EndPoint.transform, this);
        }
        
        /// <summary> Добавление нового погиба. </summary>
        /// <remarks>
        /// При переопределении в потомке обязательно должна вызываться с помощью <c> base.AddBendFragment()</c>.
        /// </remarks>
        /// <param name="startAngle">Стартовый угол наклона</param>
        /// <param name="startUseSecondRadius">Стартовый угол погиба</param>
        public virtual void AddBendFragment(float startAngle, bool startUseSecondRadius)
        {
            if (!HasChild) Child = TubeUnityManager.CreateBendedTubeFragment(Owner, EndPoint.transform, this);
            ((BendedTubeFragment) Child).StartAngle = startAngle;
            ((BendedTubeFragment) Child).StartUseSecondRadius = startUseSecondRadius;
        }

        /// <summary> Добавление нового прямого участка трубы. </summary>
        /// <remarks>
        /// При переопределении в потомке обязательно должна вызываться с помощью <c> base.AddDirectFragment()</c>.
        /// </remarks>
        public virtual void AddDirectFragment()
        {
            if (this is DirectTubeFragment) return;
            if (!HasChild) Child = TubeUnityManager.CreateDirectTubeFragment(Owner, EndPoint.transform, this);
        }
        
        /// <summary> Добавление нового прямого участка трубы с заданной длиной. </summary>
        /// <param name="startLength">Стартовая длина</param>
        /// <remarks>
        /// При переопределении в потомке обязательно должна вызываться с помощью <c> base.AddDirectFragment()</c>.
        /// </remarks>
        public virtual void AddDirectFragment(float startLength)
        {
            if (!HasChild) Child = TubeUnityManager.CreateDirectTubeFragment(Owner, EndPoint.transform, this);
            ((DirectTubeFragment) Child).StartLength = startLength;
        }

        /// <summary> Создание новой трубы. </summary>
        /// <remarks>
        /// При переопределении в потомке обязательно должна вызываться с помощью <c> base.CreateFragment()</c>.
        /// </remarks>
        public virtual void CreateTube()
        {
            var newTube = TubeManager.CreateTube(Owner);
            newTube.StartPlacing();
        }

        /// <summary> Удаление этого участка трубы. </summary>
        /// <remarks>
        /// При переопределении в потомке обязательно должна вызываться с помощью <c> base.RemoveThisFragment()</c>.
        /// </remarks>
        public virtual void RemoveThisFragment()
        {
            if (Parent != null)
            {
                Parent.SelectThis();
                Parent.Child = null;
            }
            Destroy(gameObject);
        }
        
        /// <summary> Функция, которая устанавлявает соответствующий цвет участку трубы. </summary>
        /// <remarks>
        /// При переопределении в потомке обязательно должна вызываться с помощью <c> base.SetColor()</c>.
        /// </remarks>
        protected virtual void SetColor()
        {
            Color gridColor;
            Color baseColor;

            if (IsSelected)
            {
                gridColor = SelectedTubeGridColor;
                baseColor = IsColliding ? CollidingTubeBaseColor : SelectedTubeBaseColor;
            }
            else
            {
                gridColor = IsColliding ? CollidingTubeGridColor : DefaultTubeGridColor;
                baseColor = DefaultTubeBaseColor;
            }

            Tube.GetComponent<MeshRenderer>().material.SetColor(GridColor, gridColor);
            Tube.GetComponent<MeshRenderer>().material.SetColor(BaseColor, baseColor);
        }

        public void TogglePlacing()
        {
            if (Owner.StartFragment.IsPlacing) Owner.StartFragment.StopPlacing();
            else                               Owner.StartFragment.StartPlacing();
        }
        
        public virtual void StartPlacing()
        {
            if (!Owner.StartFragment.IsPlacing)Owner.StartFragment.StartPlacing();
        }

        public virtual void StopPlacing()
        {
            if (Owner.StartFragment.IsPlacing) Owner.StartFragment.StopPlacing();
        }

        #region Notify property changed

        /// <summary> Событие, вызываемое при изменении какого-либо свойства. </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary> Обработчик изменения свойств. </summary>
        /// <param name="propertyName"> Имя изменившегося свойства. </param>
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
        
        #region Unity event functions

        /// <summary> Функция, инициализирующая участок трубы в Unity. </summary>
        /// <remarks>
        /// При переопределении в потомке обязательно должна вызываться с помощью <c> base.Awake()</c>.
        /// </remarks>
        protected virtual void Awake()
        {
            tag = "Tube";
            Tube = transform.Find("Tube").gameObject;
            TubeFragmentCollider tubeCollider = Tube.transform.Find("Collider")?.GetComponent<TubeFragmentCollider>();
            if (tubeCollider != null)
            {
                tubeCollider.Owner = this;
            }

            EndPoint = transform.Find("End Point").gameObject;
        }

        /// <summary> Функция, выполняющаяся после инициализизации участка трубы в Unity. </summary>
        /// <remarks>
        /// При переопределении в потомке обязательно должна вызываться с помощью <c> base.Start()</c>.
        /// </remarks>
        protected virtual void Start()
        {
            Diameter = Owner.Data.diameter;
            SelectThis();
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

        private bool _hasTransformError;
        private float _diameter;
        private TubeFragment _child;
        private TubeFragment _parent;
        private bool _isColliding;
        protected static readonly int GridColor = Shader.PropertyToID("_GridColor");
        protected static readonly int BaseColor = Shader.PropertyToID("_BaseColor");

        /// <summary> Цвет сетки участка трубы. </summary>
        protected static readonly Color DefaultTubeGridColor = new Color(1f, 1f, 0f, 1f);

        /// <summary> Цвет сетки участка трубы, когда она выбрана. </summary>
        protected static readonly Color SelectedTubeGridColor = new Color(0f, 1f, 0f, 1f);

        /// <summary> Цвет сетки участка трубы, когда она пересекается с другим участком трубы. </summary>
        protected static readonly Color CollidingTubeGridColor = new Color(1f, 0f, 0f, 1f);

        /// <summary> Цвет участка трубы. </summary>
        protected static readonly Color DefaultTubeBaseColor = new Color(1f, 1f, 0f, 0.25f);

        /// <summary> Цвет участка трубы, когда она выбрана. </summary>
        protected static readonly Color SelectedTubeBaseColor = new Color(0f, 1f, 0f, 0.25f);

        /// <summary> Цвет участка трубы, когда она пересекается с другим участком трубы. </summary>
        protected static readonly Color CollidingTubeBaseColor = new Color(1f, 0f, 0f, 0.25f);

        #endregion
    }
}
