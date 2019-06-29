// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace HoloCAD.UnityTubes
{
    /// <inheritdoc />
    /// <summary> Класс, реализующий прямой участок трубы. </summary>
    public class DirectTubeFragment : TubeFragment
    {
        /// <summary> Длина прямого участка трубы. </summary>
        public float Length
        {
            get => _length;
            set
            {
                if (value <= 0)
                {
                    return;
                }
                _length = value;

                Tube.transform.localScale = new Vector3(Diameter, _length, Diameter);
                EndPoint.transform.localPosition = new Vector3(0, 0, _length);
            }
        }

        /// <inheritdoc />
        public override float Diameter
        {
            get => base.Diameter;
            set
            {
                if (Math.Abs(base.Diameter - value) < float.Epsilon) return;

                base.Diameter = value;
                Tube.transform.localScale = new Vector3(Diameter, _length, Diameter);
            }
        }

        /// <inheritdoc/>
        public override bool IsPlacing
        {
            get => base.IsPlacing;
            set
            {
                base.IsPlacing = value;
                
                Transform tubeCollider = Tube.transform.Find("Collider"); 
                if (tubeCollider != null) tubeCollider.GetComponent<CapsuleCollider>().enabled = !IsPlacing;
            }
        }

        /// <summary> Список отростков из этого участка трубы. </summary>
        public ReadOnlyCollection<DirectTubeFragment> Outgrowths => _outgrowths.AsReadOnly();

        /// <summary> Увеличивает длину. </summary>
        /// <param name="delta"> Изменение длины. </param>
        public void ChangeLength(float delta)
        {
            Length += delta;
        }

        /// <summary> Добавляет новый отросток. </summary>
        public void AddOutgrowth()
        {
            _outgrowths.Add(TubeUnityManager.CreateOutgrowth(Owner, this).GetComponent<DirectTubeFragment>());
        }
        
        /// <summary> Удаляет указанный отросток из списка. </summary>
        /// <param name="outgrowth"> Удалённый отросток. </param>
        public void RemoveOutgrowth(DirectTubeFragment outgrowth)
        {
            _outgrowths.Remove(outgrowth);
        }

        /// <inheritdoc />
        public override void RemoveThisFragment()
        {
            if (Parent != null)
            {
                if (Parent.Child == this) Parent.Child = null;
                else                      ((DirectTubeFragment)Parent).RemoveOutgrowth(this);
            }
            Destroy(gameObject);
        }

        #region Unity event functions

        /// <inheritdoc />
        protected override void Start()
        {
            base.Start();
            Length = 0.5f;
            TubeManager.SelectTubeFragment(this);
        }

        #endregion

        #region Private definitions

        private float _length;
        private float _buttonBarOffset;
        private readonly List<DirectTubeFragment> _outgrowths = new List<DirectTubeFragment>();

        #endregion
    }
}
