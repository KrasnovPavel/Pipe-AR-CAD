using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using HoloCore;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class MarksController : Singleton<MarksController>
{
    public List<MarkOnScene> AllMarks;
    public MarkOnScene SelectedMark {
        get => _selectedMark;
        set
        {
            _selectedMark = value;
            OnPropertyChanged();  
        }
        
    }
    
    
    public Material MaterialForSelected;
    public GameObject MarkPrefab;
    public GameObject MarkPanelPrefab;
    public GameObject ParentOfPanels;
    public float SelectedMarkSpeed;
   
    /// <summary> Событие измененения свойств объекта </summary>
    public event PropertyChangedEventHandler PropertyChanged;
    
    public void DeleteMark(int id)
    {
        if (AllMarks.Count <= id) return;
        if (AllMarks[id].IsSelected) SelectedMark = null;
        DestroyImmediate(AllMarks[id].ParamPanelofThisMark.gameObject);
        Destroy(AllMarks[id].gameObject);
        AllMarks.RemoveAt(id);
        int newId = 0;
        foreach (MarkOnScene mark in AllMarks) mark.Id = newId++;
    }

    public void MoveMark(int direction)
    {
        
        if (SelectedMark != null)
        {
            Transform markTransform = SelectedMark.transform;
            switch (direction)
            {
                case 0:
                    markTransform.position += markTransform.forward * SelectedMarkSpeed * Time.deltaTime;
                    break;
                case 1:
                    markTransform.position += -markTransform.forward * SelectedMarkSpeed * Time.deltaTime;
                    break;
                case 2:
                    markTransform.position += -markTransform.right * SelectedMarkSpeed * Time.deltaTime;
                    break;
                case 3:
                    markTransform.position += markTransform.right * SelectedMarkSpeed * Time.deltaTime;
                    break;
                case 4:
                    markTransform.position += markTransform.up * SelectedMarkSpeed * Time.deltaTime;
                    break;
                case 5:
                    markTransform.position += -markTransform.up * SelectedMarkSpeed * Time.deltaTime;
                    break;
            }
        }
    }

    public void SelectMark(int id)
    {
        if (AllMarks.Count <= id) return;
        SelectedMark = AllMarks[id];
    }
    
    public void AddMark()
    {
        MarkOnScene currentMark = Instantiate(MarkPrefab).GetComponent<MarkOnScene>();
        currentMark.Id = AllMarks.Count;
        SelectedMark = currentMark;
        AllMarks.Add(currentMark);
        
        MarkParamPanel markParamPanel = Instantiate(MarkPanelPrefab).GetComponent<MarkParamPanel>();
        markParamPanel.transform.parent = ParentOfPanels.transform;

        currentMark.ParamPanelofThisMark = markParamPanel;
        markParamPanel.IdText.text = Convert.ToString(currentMark.Id);
        markParamPanel.Mark = currentMark;
    }

    public string CreateJsonString()
    {
        string jsonText = "";
        List<SerializedMark> serializedMarksList = new List<SerializedMark>();
        int i = 0;
        foreach (MarkOnScene mark in AllMarks)
        {
            var markTransform = mark.transform;
            serializedMarksList.Add(new SerializedMark(markTransform.position.x,
                markTransform.position.y, markTransform.position.z,
                markTransform.eulerAngles.x,markTransform.eulerAngles.y, markTransform.eulerAngles.z, $"ImageTarget{i}",
                "Target"));

            i++;
        }
        SerializedMarks serializedMarks = new SerializedMarks(serializedMarksList.ToArray());
        jsonText = JsonUtility.ToJson(serializedMarks);
        return jsonText;
    }
    
    
    
    public void ReadJsonString(string jsonText)
    {
       SerializedMarks serializedMarks = JsonUtility.FromJson<SerializedMarks>(jsonText);
       for (; AllMarks.Count > 0;)
       {
           DeleteMark(0);
       }
       for (int i = 0; i < serializedMarks.AllMarks.Length; i++)
       {
           AddMark();
           AllMarks[i].gameObject.transform.localPosition = new Vector3(serializedMarks.AllMarks[i].X,
               serializedMarks.AllMarks[i].Y, serializedMarks.AllMarks[i].Z);
           AllMarks[i].gameObject.transform.localRotation = Quaternion.Euler(serializedMarks.AllMarks[i].RotationX,
               serializedMarks.AllMarks[i].RotationY, serializedMarks.AllMarks[i].RotationZ);
       }
    }
    
    #region Unity events

    private MarkOnScene _selectedMark;
    
    private void Start()
    {
        PropertyChanged += delegate(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == nameof(SelectedMark))
            {
                if (SelectedMark == null) return;
                SelectedMark.IsSelected = true;
            }
        };
    }
    
    #endregion
    
    
    #region Protected defenitions
        
    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
        
    #endregion
}
