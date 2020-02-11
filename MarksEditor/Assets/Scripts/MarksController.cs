using System.Collections.Generic;
using System.ComponentModel;
using HoloCore;
using PiXYZ.Plugin.Unity;
using UnityEngine;

namespace MarksEditor
{
    public class MarksController : Singleton<MarksController>, INotifyPropertyChanged
    {
        public List<Mark> AllMarks;

        public Mark SelectedMark
        {
            get => _selectedMark;
            private set
            {
                _selectedMark = value;
                if (_selectedMark == null) return;
                _selectedMark.IsSelected = true;
            }
        }

        /// <summary> Префаб метки </summary>
        [Tooltip("text")] public GameObject MarkPrefab;

        /// <summary> Префаб панели с параметрами метки </summary>
        [Tooltip("text")] public GameObject MarkPanelPrefab;

        /// <summary> Родительнский объект для панели </summary>
        [Tooltip("text")] public GameObject ParentOfPanels;

        /// <summary> Скорость перемещения метки на WASD </summary>
        [Tooltip("text")] public float SelectedMarkSpeed;

        /// <summary> Событие измененения свойств объекта </summary>
        public event PropertyChangedEventHandler PropertyChanged;

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

        public void MoveMark(MoveDirections.Directions direction)
        {
            if (SelectedMark != null)
            {
                Transform markTransform = SelectedMark.transform;
                switch (direction)
                {
                    case MoveDirections.Directions.Forward:
                        markTransform.position += Time.deltaTime * SelectedMarkSpeed * markTransform.forward;
                        break;
                    case MoveDirections.Directions.Backward:
                        markTransform.position += Time.deltaTime * SelectedMarkSpeed * -markTransform.forward;
                        break;
                    case MoveDirections.Directions.Left:
                        markTransform.position += Time.deltaTime * SelectedMarkSpeed * -markTransform.right;
                        break;
                    case MoveDirections.Directions.Right:
                        markTransform.position += Time.deltaTime * SelectedMarkSpeed * markTransform.right;
                        break;
                    case MoveDirections.Directions.Up:
                        markTransform.position += Time.deltaTime * SelectedMarkSpeed * markTransform.up;
                        break;
                    case MoveDirections.Directions.Down:
                        markTransform.position += Time.deltaTime * SelectedMarkSpeed * -markTransform.up;
                        break;
                    case MoveDirections.Directions.RotationRight:
                        markTransform.Rotate(markTransform.up, 90f, Space.World);
                        break;
                    case MoveDirections.Directions.RotationLeft:
                        markTransform.Rotate(markTransform.up, -90f, Space.World);
                        break;
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
            Mark currentMark = Instantiate(MarkPrefab).GetComponent<Mark>();
            currentMark.Id = AllMarks.Count;
            SelectedMark = currentMark;
            AllMarks.Add(currentMark);

            MarkParamPanel markParamPanel = Instantiate(MarkPanelPrefab).GetComponent<MarkParamPanel>();
            markParamPanel.transform.parent = ParentOfPanels.transform;
            markParamPanel.Mark = currentMark;
        }

        #region Private defenitions

        private Mark _selectedMark;

        #endregion
    }
}