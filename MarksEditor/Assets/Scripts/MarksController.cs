using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using HoloCore;
using JetBrains.Annotations;
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
            if (AllMarks.Count <= id) return;
            if (AllMarks[id].IsSelected) SelectedMark = null;
            DestroyImmediate(AllMarks[id].ParamPanelOfThisMark.gameObject);
            Destroy(AllMarks[id].gameObject);
            AllMarks.RemoveAt(id);
            int newId = 0;
            foreach (Mark mark in AllMarks) mark.Id = newId++;
        }

        public void MoveMark(int direction)
        {
            if (SelectedMark != null)
            {
                Transform markTransform = SelectedMark.transform;
                switch (direction)
                {
                    case 0:
                        markTransform.position += Time.deltaTime * SelectedMarkSpeed * markTransform.forward;
                        break;
                    case 1:
                        markTransform.position += Time.deltaTime * SelectedMarkSpeed * -markTransform.forward;
                        break;
                    case 2:
                        markTransform.position += Time.deltaTime * SelectedMarkSpeed * -markTransform.right;
                        break;
                    case 3:
                        markTransform.position += Time.deltaTime * SelectedMarkSpeed * markTransform.right;
                        break;
                    case 4:
                        markTransform.position += Time.deltaTime * SelectedMarkSpeed * markTransform.up;
                        break;
                    case 5:
                        markTransform.position += Time.deltaTime * SelectedMarkSpeed * -markTransform.up;
                        break;
                    case 6:
                        markTransform.Rotate(markTransform.up, Mathf.Deg2Rad * 90f);
                        break;
                    case 7:
                        markTransform.Rotate(markTransform.up, Mathf.Deg2Rad * -90f);
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

            currentMark.ParamPanelOfThisMark = markParamPanel;
            markParamPanel.Mark = currentMark;
        }

        #region Private defenitions

        private Mark _selectedMark;

        #endregion
    }
}