using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MarkOnScene : MonoBehaviour
{
    public int Id;
    public bool IsSelected;

    public void ChangeIdOnTextMesh(int id)
    {
        GetComponentInChildren<Text>().text = Convert.ToString(id);
    }
}
