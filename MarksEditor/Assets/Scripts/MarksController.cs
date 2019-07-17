using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HoloCore;
using UnityEngine;
using UnityEngine.Rendering;

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
        DestroyImmediate(ParentOfPanels.transform.GetChild(id).gameObject);
        Destroy(AllMarks[id].gameObject);
        AllMarks.RemoveAt(id);
        int newId = 0;
        foreach (var mark in AllMarks)
        {
            mark.Id = newId++;
            mark.ChangeIdOnTextMesh(mark.Id);
        }
    }

    public void MoveMark(int direction, float speed)
    {
        
        if (SelectedMark != null)
        {
            var markTransform =SelectedMark.transform;
            switch (direction)
            {
                case 0:
                    markTransform.position += markTransform.forward * speed;
                    break;
                case 1:
                    markTransform.position += -markTransform.forward * speed;
                    break;
                case 2:
                    markTransform.position += -markTransform.right * speed;
                    break;
                case 3:
                    markTransform.position += markTransform.right * speed;
                    break;
                case 4:
                    markTransform.position += markTransform.up * speed;
                    break;
                case 5:
                    markTransform.position += -markTransform.up * speed;
                    break;
            }
            
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

    public string CreateJsonString()
    {
        string jsonText = "";
        List<SerializedMark> serializedMarksList = new List<SerializedMark>();
        int i = 0;
        foreach (MarkOnScene mark in AllMarks)
        {
            serializedMarksList.Add(new SerializedMark(mark.gameObject.transform.position.x,
                mark.gameObject.transform.position.y, mark.gameObject.transform.position.z,
                mark.gameObject.transform.eulerAngles.x, mark.gameObject.transform.eulerAngles.y, mark.gameObject.transform.eulerAngles.z, $"ImageTarget{i}",
                "Target"));
            Debug.Log($"{gameObject.transform.eulerAngles.x}, {gameObject.transform.eulerAngles.y}, {gameObject.transform.eulerAngles.z}");
         //   Debug.Log($"{gameObject.transform.rotation.x}, {gameObject.transform.rotation.y}, {gameObject.transform.rotation.z}");

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
           AllMarks[i].gameObject.transform.position = new Vector3(serializedMarks.AllMarks[i].X,
               serializedMarks.AllMarks[i].Y, serializedMarks.AllMarks[i].Z);
           AllMarks[i].gameObject.transform.rotation = Quaternion.Euler(serializedMarks.AllMarks[i].RotationX,
               serializedMarks.AllMarks[i].RotationY, serializedMarks.AllMarks[i].RotationZ);
           AllMarks[i].HasUpdate = true;
       }
    }
}
