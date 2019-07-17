using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MarkOnScene : MonoBehaviour
{
    public int Id;
    public bool IsSelected;
    public bool HasUpdate=true;

    private void Start()
    {
        HasUpdate = true;
    }

    public void ChangeIdOnTextMesh(int id)
    {
        GetComponentInChildren<TextMesh>().text = Convert.ToString(id);
    }
}
