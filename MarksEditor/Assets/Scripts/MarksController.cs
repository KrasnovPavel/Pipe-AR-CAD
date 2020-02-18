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

        /// <summary> Панель с данными о выбранной метке </summary>
        public MarkParamPanel ParamPanelOfSelectedMark { get; private set; }

        /// <summary> Список существующих меток </summary>
        [Tooltip("Список существующих меток")] public List<Mark> AllMarks;

        /// <summary> Импортируемый объект</summary>
        [Tooltip(" Импортируемый объект")] public Transform TargetTransform;

        /// <summary> Префаб метки </summary>
        [Tooltip("Префаб метки")] public GameObject MarkPrefab;

        /// <summary> Префаб панели с параметрами метки </summary>
        [Tooltip("Префаб панели с параметрами метки")]
        public GameObject MarkPanelPrefab;

        /// <summary> Родительнский объект для панели </summary>
        [Tooltip("Родительнский объект для панели")]
        public GameObject ParentOfPanels;

        /// <summary> Скорость перемещения метки на WASD </summary>
        [Tooltip("Скорость перемещения метки на WASD")]
        public float SelectedMarkSpeed;

        /// <summary>Удаляет метку с определенным id </summary>
        /// <param name="id">id метки</param>
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

        /// <summary> Удаляет все метки со сцены </summary>
        public void DeleteAllMarks()
        {
            MarkParamPanel[] panel = ParentOfPanels.GetComponentsInChildren<MarkParamPanel>();
            while (AllMarks.Count > 0)
            {
                int id = AllMarks[0].Id;
                Destroy(panel[id].gameObject);
                Destroy(AllMarks[id].gameObject);
                AllMarks.RemoveAt(id);
                panel.RemoveAt(id);
            }
        }

        /// <summary> Двигает метку в определенном направлении </summary>
        /// <param name="direction">Направление движения</param>
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

        /// <summary>Выбирает метку с определенным id </summary>
        /// <param name="id">id метки</param>
        public void SelectMark(int id)
        {
            if (AllMarks.Count < id) return;
            SelectedMark = AllMarks[id];
        }

        /// <summary> Добавляет метку на сцену </summary>
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

        /// <summary> Добавляет метку с указанными параметрами </summary>
        /// <param name="position">Вектор позиции на сцене</param>
        /// <param name="rotation">Вектор поворота</param>
        public void AddMark(Vector3 position, Quaternion rotation)
        {
            MarkParamPanel markParamPanel = Instantiate(MarkPanelPrefab).GetComponent<MarkParamPanel>();
            markParamPanel.transform.parent = ParentOfPanels.transform;

            Mark currentMark = Instantiate(MarkPrefab).GetComponent<Mark>();
            currentMark.Id = AllMarks.Count;
            SelectedMark = currentMark;
            AllMarks.Add(currentMark);
            markParamPanel.Mark = currentMark;
            Transform currentMarkTransform = currentMark.transform;
            TargetTransform.SetParent(currentMarkTransform, false);
            Vector3 lossyScale = currentMarkTransform.lossyScale;
            Vector3 currentMarkPosition = position;
            TargetTransform.localPosition = new Vector3(currentMarkPosition.x / lossyScale.x,
                currentMarkPosition.y / lossyScale.y,
                currentMarkPosition.z / lossyScale.z);
            TargetTransform.localRotation = rotation;
            TargetTransform.SetParent(null, true);
            TargetTransform.localScale = Vector3.one;
            currentMarkTransform.SetParent(TargetTransform, true);
            TargetTransform.position = Vector3.zero;
            TargetTransform.rotation = Quaternion.Euler(Vector3.zero);
            currentMarkTransform.SetParent(null, true);
        }

        #region Private defenitions

        private Mark _selectedMark;

        #endregion
    }
}