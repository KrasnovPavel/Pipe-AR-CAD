// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using HoloCAD.UnityTubes;
using HoloCore.UI;
using JetBrains.Annotations;
using Microsoft.MixedReality.Toolkit;
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
        [CanBeNull] public TextMesh DetectionLabel;
        [CanBeNull] public GameObject ButtonBar;
        [CanBeNull] public MarksTarget Target;

        #region Unity event function
        private Mark _mark;

        private void Start()
        {
            _mainCameraPosition = Camera.main.transform.position;
            _mark = transform.parent.GetComponent<Mark>();
            _mark.PropertyChanged += delegate
            {
                if (DetectionLabel == null) return;
                
                if (_mark.IsActive)
                {
                    DetectionLabel.text = "Метка\nраспознана";
                    DetectionLabel.color = Color.green;
                }
                else
                {
                    DetectionLabel.text = "Метка\nне распознана";
                    DetectionLabel.color = Color.red;
                }
            };

            if (Target == null) return;
            
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
                ChangeTargetCollider.OnClick += delegate { Target.GetComponent<ViewModes>().Next(); };
            }

            if (Edit != null)
            {
                Edit.OnClick += delegate
                {
                    if (ButtonBar == null) return;
                    
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
            
            Target.PropertyChanged += delegate { SetText(); };
            
            SetText();
        }

        private void Update()
        {
            PushFromTargetAndSpatialMapping();
        }
        
        #endregion

        #region Private definitions
        
        /// <summary> Растояние между камерой и меткой, при которой активируется выталкивание </summary>
        private const float _triggerDistance = 2f;

        /// <summary> Глубина выталкивания из объекта коллизии </summary>
        private const float _pushDepth = 0.05f;
        
        /// <summary> Маска всех слоев, с которыми проверяется коллизия </summary>
        private const int _layerMask = (1<<31)|(1<<30);
        
        /// <summary> Позиция бъекта камеры, к которой привязаны все метки </summary>
        private static Vector3 _mainCameraPosition;


        /// <summary> "Выталкивает" панель с кнопками из сетки пространства и отображаемой модели  </summary>
        private void PushFromTargetAndSpatialMapping()
        {
            if(!_mark.IsActive) return;
            Vector3 markCameraVector = _mark.transform.position - _mainCameraPosition;
            if (markCameraVector.magnitude > _triggerDistance)
            {
                transform.localPosition = Vector3.zero;
                return;
            }
            RaycastHit hitInfo;
            if (Physics.Raycast(_mainCameraPosition, markCameraVector,
                out hitInfo, _triggerDistance * 2,
                _layerMask))
            {
                transform.position = hitInfo.point - markCameraVector * _pushDepth;
            }
            else
            {
                transform.localPosition = Vector3.zero;
            }
        }
        
        private void SetText()
        {
            if (Target == null || PositionLabel == null || RotationLabel == null) return;
            
            Vector3? position = Target.GetPosition(_mark);
            Vector3? rotation = Target.GetRotation(_mark);

            if (position == null || rotation == null) return;
            
            Vector3 p = position.Value;
            PositionLabel.text = $"x: {p.x:0.000}м.\ny: {p.y:0.000}м.\nz: {p.z:0.000}м.";
            Vector3 r = rotation.Value;
            RotationLabel.text = $"α: {r.x:0.0}°.\nβ: {r.y:0.0}°.\nγ: {r.z:0.0}°.";
        }
        
        #endregion
    }
}