using System.Collections.Generic;
using System.ComponentModel;
using HoloCore;
using PiXYZ.Plugin.Unity;
using UnityEngine;

namespace GLTFConverter
{
    public class MarksController : Singleton<MarksController>
    {
        /// <summary> Направления движения метки</summary>
        public enum Directions
        {
            Forward,
            Backward,
            Left,
            Right,
            Up,
            Down,
            RotationRight,
            RotationLeft
        }
        
        /// <summary> Список существующих меток </summary>
        public List<Mark> AllMarks;

        /// <summary> Выбранная метка </summary>
        public Mark SelectedMark
        {
            get => _selectedMark;
            private set
            {
                _selectedMark = value;
                if (_selectedMark == null) return;
                _selectedMark.IsSelected = true;
                ParamPanelOfSelectedMark = ParentOfPanels.GetComponentsInChildren<MarkParamPanel>()[_selectedMark.Id];
            }
        }

        /// <summary> Панедб с данными о выбранной метке </summary>
        public MarkParamPanel ParamPanelOfSelectedMark { get; private set; }

        /// <summary> Префаб метки </summary>
        [Tooltip("text")] public GameObject MarkPrefab;

        /// <summary> Префаб панели с параметрами метки </summary>
        [Tooltip("text")] public GameObject MarkPanelPrefab;

        /// <summary> Родительнский объект для панели </summary>
        [Tooltip("text")] public GameObject ParentOfPanels;

        /// <summary> Скорость перемещения метки на WASD </summary>
        [Tooltip("text")] public float SelectedMarkSpeed;


        public void DeleteMark(int id)
        {
            if (AllMarks.Count < id) return;
            if (AllMarks[id].IsSelected) SelectedMark = null;
            MarkParamPanel[] panel = ParentOfPanels.GetComponentsInChildren<MarkParamPanel>();
            Destroy(panel[id].gameObject);
            Destroy(AllMarks[id].gameObject);
            AllMarks.RemoveAt(id);
            panel.RemoveAt(id);
            int newId = 0;
            foreach (Mark mark in AllMarks)
            {
                mark.Id = newId;
                panel[newId].IdText.text = $"{newId + 1}";
                newId++;
            }
        }

        public void MoveMark(Directions direction)
        {
            if (SelectedMark != null)
            {
                Transform markTransform = SelectedMark.transform;
                switch (direction)
                {
                    case Directions.Forward:
                        markTransform.position += Time.deltaTime * SelectedMarkSpeed * markTransform.forward;
                        break;
                    case Directions.Backward:
                        markTransform.position += Time.deltaTime * SelectedMarkSpeed * -markTransform.forward;
                        break;
                    case Directions.Left:
                        markTransform.position += Time.deltaTime * SelectedMarkSpeed * -markTransform.right;
                        break;
                    case Directions.Right:
                        markTransform.position += Time.deltaTime * SelectedMarkSpeed * markTransform.right;
                        break;
                    case Directions.Up:
                        markTransform.position += Time.deltaTime * SelectedMarkSpeed * markTransform.up;
                        break;
                    case Directions.Down:
                        markTransform.position += Time.deltaTime * SelectedMarkSpeed * -markTransform.up;
                        break;
                    case Directions.RotationRight:
                        markTransform.Rotate(markTransform.up, 90f, Space.World);
                        break;
                    case Directions.RotationLeft:
                        markTransform.Rotate(markTransform.up, -90f, Space.World);
                        break;
                }

                if (markTransform.hasChanged)
                {
                    ParamPanelOfSelectedMark.MarkTransformIntoInput();
                }
            }
        }

        public void SelectMark(int id)
        {
            if (AllMarks.Count < id) return;
            SelectedMark = AllMarks[id];
        }

        public void AddMark()
        {
            MarkParamPanel markParamPanel = Instantiate(MarkPanelPrefab).GetComponent<MarkParamPanel>();
            markParamPanel.transform.parent = ParentOfPanels.transform;
            
            Mark currentMark = Instantiate(MarkPrefab).GetComponent<Mark>();
            currentMark.Id = AllMarks.Count;
            SelectedMark = currentMark;
            AllMarks.Add(currentMark);
            
            markParamPanel.Mark = currentMark;
        }

        #region Private defenitions

        private Mark _selectedMark;

        #endregion
    }
}