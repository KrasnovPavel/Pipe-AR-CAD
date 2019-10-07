using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class InputFromPanel : MonoBehaviour
{
    public MarkOnScene Mark { get; set; }
    public MarkParamPanel MarkParamPanel { get; set; }
    
    
    public void OnEndEdit()
    {
        InputField inputField = GetComponent<InputField>();
        if (inputField.text == "" || inputField.text == ".") inputField.text = "0";
    }
    
    private void ChangeMarkParams()
    {
        MarkParamPanel.SetParamsToMarkFromInputs();
    }

}
