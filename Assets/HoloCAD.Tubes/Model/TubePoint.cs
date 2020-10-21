// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using JetBrains.Annotations;
using UnityC3D;
using UnityEngine;

namespace HoloCAD.Tubes.Model
{
    /// <summary> Точка трубы. </summary>
    public class TubePoint : GCMPoint
    {
        /// <summary> Предыдущий отрезок трубы. </summary>
        [CanBeNull] public Segment Prev;

        /// <summary> Следующий отрезок трубы. </summary>
        [CanBeNull] public Segment Next;

        /// <summary> Труба - хозяин точки. </summary>
        public Tube Owner => Next?.Owner ?? Prev?.Owner;

        /// <summary> Находится ли точка во фланце. </summary>
        public bool IsInFlange => GetFlange() != null;

        /// <summary> Длина погиба. </summary>
        public float DeltaLength => BendRadius * Mathf.Tan(GetBendAngle() / 2 * Mathf.Deg2Rad);

        // ReSharper disable once PossibleNullReferenceException
        /// <summary> Находится ли точка в конечном фланце. </summary>
        public bool IsInEndFlange => IsInFlange && ReferenceEquals(Next.End, this);

        /// <summary> Диаметр трубы, изгибаемой в этой точке. </summary>
        public float Diameter => Owner.Diameter;

        /// <summary> Какой из двух доступных радиусов погиба использовать. </summary>
        public bool UseSecondRadius
        {
            get => _useSecondRadius;
            set
            {
                if (_useSecondRadius == value) return;
                
                _useSecondRadius = value;
                OnPropertyChanged(nameof(BendRadius));
            }
        }

        /// <summary> Радиус погиба. </summary>
        public float BendRadius => UseSecondRadius ? Owner.SecondBendRadius : Owner.FirstBendRadius;

        /// <summary> Конструктор точки. </summary>
        /// <param name="sys"> Система ограничений. </param>
        /// <param name="origin"> Местоположение. </param>
        /// <param name="parent"> Родительская ЛСК. </param>
        public TubePoint(GCMSystem sys, Vector3 origin, GCM_LCS parent = null) :
            base(sys, origin, parent)
        {
            RecalculateDiameter();
        }

        /// <summary> Возвращает угол погиба. </summary>
        /// <returns></returns>
        public float GetBendAngle()
        {
            if (Next == null || Prev == null) return 0;

            if (IsInEndFlange)
            {
                return 180 - Vector3.Angle(Next.Start.Origin - Origin, Prev.Start.Origin - Origin);
            }

            return 180 - Vector3.Angle(Next.End.Origin - Origin, Prev.Start.Origin - Origin);
        }

        /// <summary> Если точка находится в фланце - возвращает фланец. </summary>
        /// <returns></returns>
        [CanBeNull] public Flange GetFlange()
        {
            if (ReferenceEquals(Owner.StartFlange.EndPoint, this)) return Owner.StartFlange;
            if (ReferenceEquals(Owner.EndFlange.EndPoint,   this)) return Owner.EndFlange;
            return null;
        }

        /// <summary>  </summary>
        public void RecalculateDiameter()
        {
            OnPropertyChanged(nameof(Diameter));
        }

        #region Private definitions

        private bool _useSecondRadius;

        #endregion
    }
}
