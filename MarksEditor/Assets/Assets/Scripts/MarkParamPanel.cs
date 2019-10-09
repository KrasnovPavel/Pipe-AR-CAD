using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using System.Globalization;
using UnityEngine.UI;


public class MarkParamPanel : MonoBehaviour
{  
    public MarkOnScene Mark;
    CultureInfo ci = new CultureInfo("en-US");
    public InputField InputX;
    public InputField InputY;
    public InputField InputZ;
    public InputField InputRotationX;
    public InputField InputRotationY;
    public InputField InputRotationZ;
    public Text IdText;
    
    public void SetParamsToMarkFromInputs()
    {
        if (Mark == null) return;
        Mark.transform.position = new Vector3(float.Parse(InputX.text,ci), float.Parse(InputY.text,ci),
            float.Parse(InputZ.text,ci));
        Mark.transform.rotation = Quaternion.Euler(float.Parse(InputRotationX.text,ci), 
            float.Parse(InputRotationY.text,ci),float.Parse(InputRotationZ.text,ci));
    }

    public void MarkTransformIntoInput()
    {
        Vector3 markPosition = Mark.transform.position;
        Vector3 markRotation = Mark.transform.eulerAngles;
        InputX.text = Convert.ToString(markPosition.x, ci);
        InputY.text = Convert.ToString(markPosition.y, ci);
        InputZ.text = Convert.ToString(markPosition.z, ci);
        InputRotationX.text = Convert.ToString(markRotation.x, ci);
        InputRotationY.text = Convert.ToString(markRotation.y, ci);
        InputRotationZ.text = Convert.ToString(markRotation.z, ci);
    }
    
    private void Awake()
    {
        InputX.text = "0";
        InputY.text = "0";
        InputZ.text = "0";
        InputRotationX.text = "0";
        InputRotationY.text = "0";
        InputRotationZ.text = "0";
    }
    
    public void DeleteMark()
    {
        MarksController.Instance.DeleteMark(Mark.Id);
    }

    public void SelectMark()
    {
        MarksController.Instance.SelectMark(Mark.Id);
    }
}
