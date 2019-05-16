// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using HoloCAD.UI;
using UnityEngine;

namespace HoloCAD.UnityTubes
{
    /// <inheritdoc />
    /// <summary> Базовый класс участка трубы. От него наследуются все остальные классы участков труб. </summary>
    public class TubeFragment : MonoBehaviour
    {
        /// <summary> Объект, содержащий меш участка трубы. </summary>
        protected GameObject Tube;

        /// <summary> Объект, указывающий конечную точку участка трубы. </summary>
        public GameObject EndPoint { get; protected set; }

        /// <summary> Флаг, находится ли участок трубы в режиме перемещения. </summary>
        public virtual bool IsPlacing
        {
            get => _isPlacing;
            set
            {
                _isPlacing = value;
                Tube.GetComponent<MeshCollider>().enabled = !_isPlacing;
                Tube.transform.Find("Collider").GetComponent<MeshCollider>().enabled = !_isPlacing;
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
                
                var controlPanel = GetComponent<TubeFragmentControlPanel>();
                if (controlPanel != null) controlPanel.enabled = _isSelected;
            }
        }

        /// <summary> Диаметр участка трубы. </summary>
        public virtual float Diameter { get; set; }

        /// <summary> Выходит ли из этого участка трубы другой? </summary>
        public bool HasChild;

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
            Diameter = Owner.Data.diameter;
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
