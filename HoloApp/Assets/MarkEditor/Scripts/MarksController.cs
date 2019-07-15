using System.Collections;
using System.Collections.Generic;
using HoloCore;
using UnityEngine;

public class MarksController : Singleton<MarksController>
{
    public List<MarkOnScene> AllMarks;
    public MarkOnScene SelectedMark;
    public Material MaterialForSelected;
    public GameObject MarkPrefab;
    public GameObject MarkPanelPrefab;
    public GameObject ParentOfPanels;
    public void DeleteMark(int id)
    {
        
        if (AllMarks.Count <= id) return;
        if (AllMarks[id].IsSelected) SelectedMark = null;
        Destroy(ParentOfPanels.transform.GetChild(id).gameObject);
        Destroy(AllMarks[id].gameObject);
        AllMarks.RemoveAt(id);
        int newId = 0;
        foreach (var mark in AllMarks)
        {
            mark.Id = newId++;
            mark.ChangeIdOnTextMesh(mark.Id);
        }
    }

    public void MoveMark(Vector3 deltaPosition)
    {
        if (SelectedMark != null)
        {
            SelectedMark.transform.position += deltaPosition;
            SelectedMark.HasUpdate = true;
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
        currentMark.IsSelected = true;
        currentMark.ChangeIdOnTextMesh(currentMark.Id);
        SelectedMark = currentMark;
        AllMarks.Add(currentMark);
        MarkParamPanel markParamPanel = Instantiate(MarkPanelPrefab).GetComponent<MarkParamPanel>();
        markParamPanel.transform.parent = ParentOfPanels.transform;
        markParamPanel.Mark = currentMark.gameObject;
    }

    public SerializedMarks FormListOfAllMarksForSerialize()
    {
        return new SerializedMarks();
    }

    public void FormListOfAllMarksFromListOfSerialized(SerializedMarks serializedMarks)
    {
        
    }
}
