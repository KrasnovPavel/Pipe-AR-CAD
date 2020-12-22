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
        public bool IsInFlange => flange != null;

        /// <summary> Длина погиба. </summary>
        public float DeltaLength => BendRadius * Mathf.Tan(GetBendAngle() / 2 * Mathf.Deg2Rad);

        // ReSharper disable once PossibleNullReferenceException
        /// <summary> Находится ли точка в конечном фланце. </summary>
        public bool IsInEndFlange => IsInFlange && ReferenceEquals(Next.End, this);

        /// <summary> Диаметр трубы, изгибаемой в этой точке. </summary>
        public float Diameter => Owner.Diameter;

        /// <summary> Фланец к которому относится точка. (Может быть null!!!) </summary>
        // ReSharper disable once InconsistentNaming
        [CanBeNull] public readonly Flange flange;

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
        /// <param name="flange"> Фланец к которому относится точка. </param>
        /// <param name="parent"> Родительская ЛСК. </param>
        public TubePoint(GCMSystem sys, Vector3 origin, Flange flange, GCM_LCS parent = null) :
            base(sys, origin, parent)
        {
            this.flange = flange;
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

        /// <summary>  </summary>
        public void RecalculateDiameter()
        {
            OnPropertyChanged(nameof(Diameter));
        }

        public void UpdateOwner()
        {
            OnPropertyChanged(nameof(Owner));
        }

        /// <summary> Отсоединяется от переданного сегмента. </summary>
        /// <param name="segment"></param>
        public void DetachSegment(Segment segment)
        {
            if (Next == segment)
            {
                Next = null;
                OnPropertyChanged(nameof(Next));
            }
            else if (Prev == segment)
            {
                Prev = null;
                OnPropertyChanged(nameof(Prev));
            }
        }

        public void Move(Vector3 newPos)
        {
            var            lastPos       = Origin;
            MbPlacement3D? lastPlacePrev = Prev?.Line.Placement;
            MbPlacement3D? lastPlaceNext = Next?.Line.Placement;

            if (IsInFlange)
            {
                var projection = Vector3.Project(newPos - flange.Origin, flange.Normal); //-V3080
                if (Vector3.Angle(projection, flange.Normal) > 90)
                {
                    return;
                }

                Origin = flange.Origin + projection;
                Prev?.ResetLine();
                Next?.ResetLine();
                GCMSys.Evaluate();
                return;
            }

            Origin = newPos;
            Prev?.ResetLine();
            Next?.ResetLine();

            var res = GCMSys.Evaluate();
            if (res != GCMResult.GCM_RESULT_Ok)
            {
                Debug.LogWarning(res);
                Origin = lastPos;
                if (Prev != null && lastPlacePrev != null)
                {
                    Prev.Line.Placement = lastPlacePrev.Value;
                }

                if (Next != null && lastPlaceNext != null)
                {
                    Next.Line.Placement = lastPlaceNext.Value;
                }

                GCMSys.Evaluate();
            }

            Owner.FixErrors();
        }

        #region Private definitions

        private bool _useSecondRadius;

        #endregion
    }
}
