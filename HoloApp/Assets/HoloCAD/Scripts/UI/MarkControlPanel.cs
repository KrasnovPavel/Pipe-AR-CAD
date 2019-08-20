using System;
using HoloCAD.UnityTubes;
using HoloCore.UI;
using JetBrains.Annotations;
using UnityEngine;

namespace HoloCAD.UI
{
    public sealed class MarkControlPanel : MonoBehaviour
    {
        [CanBeNull] public Button3D MoveLeft;
        [CanBeNull] public Button3D MoveRight;
        [CanBeNull] public Button3D MoveUp;
        [CanBeNull] public Button3D MoveDown;
        [CanBeNull] public Button3D MoveForward;
        [CanBeNull] public Button3D MoveBackward;
        [CanBeNull] public Button3D TurnRollPlus;
        [CanBeNull] public Button3D TurnRollMinus;
        [CanBeNull] public Button3D TurnPitchPlus;
        [CanBeNull] public Button3D TurnPitchMinus;
        [CanBeNull] public Button3D TurnYawPlus;
        [CanBeNull] public Button3D TurnYawMinus;
        [CanBeNull] public Button3D ChangeTargetCollider;
        [CanBeNull] public Button3D Edit;
        [CanBeNull] public TextMesh PositionLabel;
        [CanBeNull] public TextMesh RotationLabel;
        public GameObject ButtonBar;

        public MarksTarget Target;

        #region Unity event function

        private Mark _mark;

        private void Start()
        {
            _mark = transform.parent.GetComponent<Mark>();
            if (MoveLeft != null)
            {
                MoveLeft.OnClick += delegate { Target.ChangePosition(_mark, Vector3.left * Steps.Linear); };
            }
            if (MoveRight != null)
            {
                MoveRight.OnClick += delegate { Target.ChangePosition(_mark, Vector3.right * Steps.Linear); };
            }
            if (MoveUp != null)
            {
                MoveUp.OnClick += delegate { Target.ChangePosition(_mark, Vector3.up * Steps.Linear); };
            }
            if (MoveDown != null)
            {
                MoveDown.OnClick += delegate { Target.ChangePosition(_mark, Vector3.down * Steps.Linear); };
            }
            if (MoveForward != null)
            {
                MoveForward.OnClick += delegate { Target.ChangePosition(_mark, Vector3.forward * Steps.Linear); };
            }
            if (MoveBackward != null)
            {
                MoveBackward.OnClick += delegate { Target.ChangePosition(_mark, Vector3.back * Steps.Linear); };
            }
            if (TurnRollPlus != null)
            {
                TurnRollPlus.OnClick += delegate { Target.ChangeRotation(_mark, Vector3.left * Steps.Angular); };
            }
            if (TurnRollMinus != null)
            {
                TurnRollMinus.OnClick += delegate { Target.ChangeRotation(_mark, Vector3.right * Steps.Angular); };
            }
            if (TurnPitchPlus != null)
            {
                TurnPitchPlus.OnClick += delegate { Target.ChangeRotation(_mark, Vector3.up * Steps.Angular); };
            }
            if (TurnPitchMinus != null)
            {
                TurnPitchMinus.OnClick += delegate { Target.ChangeRotation(_mark, Vector3.down * Steps.Angular); };
            }
            if (TurnYawPlus != null)
            {
                TurnYawPlus.OnClick += delegate { Target.ChangeRotation(_mark, Vector3.forward * Steps.Angular); };
            }
            if (TurnYawMinus != null)
            {
                TurnYawMinus.OnClick += delegate { Target.ChangeRotation(_mark, Vector3.back * Steps.Angular); };
            }
            if (ChangeTargetCollider != null)
            {
                ChangeTargetCollider.OnClick += delegate { TubeUnityManager.UseSpatialMapping = !TubeUnityManager.UseSpatialMapping; };
            }

            if (Edit != null)
            {
                Edit.OnClick += delegate
                {
                    if (ButtonBar.activeSelf)
                    {
                        ButtonBar.SetActive(false);
                        Target.MakeTransparent(false);
                    }
                    else
                    {
                        ButtonBar.SetActive(true);
                        Target.MakeTransparent(true);
                    }
                };
            }
            
            SetText();

            Target.PropertyChanged += delegate { SetText(); };
        }

        #endregion

        #region Private definitions

        private void SetText()
        {
            Vector3? position = Target.GetPosition(_mark);
            Vector3? rotation = Target.GetRotation(_mark);

            if (position != null && PositionLabel != null)
            {
                Vector3 p = position.Value;
                PositionLabel.text = $"x: {p.x:0.000}м.\ny: {p.y:0.000}м.\nz: {p.z:0.000}м.";
            }
            if (rotation != null && RotationLabel != null)
            {
                Vector3 r = rotation.Value;
                RotationLabel.text = $"α: {r.x:0.0}°.\nβ: {r.y:0.0}°.\nγ: {r.z:0.0}°.";
            }
        }
        
        #endregion
    }
}