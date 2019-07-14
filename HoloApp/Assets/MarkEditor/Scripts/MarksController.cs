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
    public void DeleteMark(int id)
    {
        if (AllMarks.Count <= id) return;
        if (AllMarks[id].IsSelected) SelectedMark = null;
        AllMarks.RemoveAt(id);
        int newId = 0;
        foreach (var mark in AllMarks)
        {
            mark.Id = newId++;
        }
    }

    public void MoveMark(Vector3 deltaPosition)
    {
        if(SelectedMark!=null)
            SelectedMark.transform.position += deltaPosition;
    }
    
    public void AddMark()
    {
        MarkOnScene currentMark = Instantiate(MarkPrefab).GetComponent<MarkOnScene>();
        currentMark.Id = AllMarks.Count;
        currentMark.IsSelected = true;
        SelectedMark = currentMark;
        AllMarks.Add(currentMark.GetComponent<MarkOnScene>());
        
    }

    public SerializedMarks FormListOfAllMarksForSerialize()
    {
        return new SerializedMarks();
    }

    public void FormListOfAllMarksFromListOfSerialized(SerializedMarks serializedMarks)
    {
        
    }
}
