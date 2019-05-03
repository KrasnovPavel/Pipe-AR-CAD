using System;
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

        /// <summary> Увеличивает длину. </summary>
        /// <param name="delta"> Изменение длины. </param>
        public void IncreaseLength(float delta = 0.05f)
        {
            Length += delta;
        }
        
        /// <summary> Уменьшает длину. </summary>
        /// <param name="delta"> Изменение длины. </param>
        public void DecreaseLength(float delta = 0.05f)
        {
            Length -= delta;
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

        #endregion
    }
}
