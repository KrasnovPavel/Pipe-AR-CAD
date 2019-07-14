using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;
using UnityEngine.UI;
public class MarkParamPanel : MonoBehaviour
{
    public GameObject Mark;
   
    
    public void SetParamsToMarkFromInputs()
    {
        CultureInfo ci = new CultureInfo("en-US");
        Text idText = gameObject.transform.GetChild(0).GetComponent<Text>();
        idText.text = Convert.ToString(Mark.GetComponent<MarkOnScene>().Id);
        InputField inputFieldX = gameObject.transform.GetChild(1).GetChild(0).GetComponent<InputField>();
        InputField inputFieldY = gameObject.transform.GetChild(1).GetChild(1).GetComponent<InputField>();
        InputField inputFieldZ = gameObject.transform.GetChild(1).GetChild(2).GetComponent<InputField>();
        Debug.Log($"{inputFieldX.text},{inputFieldY.text},{inputFieldZ.text}");

        Mark.transform.position = new Vector3(float.Parse(inputFieldX.text,ci), float.Parse(inputFieldY.text,ci),float.Parse(inputFieldZ.text,ci));
        InputField inputFieldRotationX = gameObject.transform.GetChild(2).GetChild(0).GetComponent<InputField>();
        InputField inputFieldRotationY = gameObject.transform.GetChild(2).GetChild(1).GetComponent<InputField>();
        InputField inputFieldRotationZ = gameObject.transform.GetChild(2).GetChild(2).GetComponent<InputField>();
        Mark.transform.rotation = Quaternion.Euler(float.Parse(inputFieldRotationX.text,ci), float.Parse(inputFieldRotationY.text,ci),float.Parse(inputFieldRotationZ.text,ci));
      // Mark.transform.Rotate(new Vector3(float.Parse(inputFieldRotationX.text,ci), float.Parse(inputFieldRotationY.text,ci),float.Parse(inputFieldRotationZ.text,ci)));
        
    }

    private void Update()
    {
        /*
        Text idText = gameObject.transform.GetChild(0).GetComponent<Text>();
        idText.text = Convert.ToString(Mark.GetComponent<MarkOnScene>().Id);
        InputField inputFieldX = gameObject.transform.GetChild(1).GetChild(0).GetComponent<InputField>();
        inputFieldX.text =Convert.ToString(Mark.transform.position.x);
        InputField inputFieldY = gameObject.transform.GetChild(1).GetChild(1).GetComponent<InputField>();
        inputFieldY.text =Convert.ToString(Mark.transform.position.y);
        InputField inputFieldZ = gameObject.transform.GetChild(1).GetChild(2).GetComponent<InputField>();
        inputFieldZ.text =Convert.ToString(Mark.transform.position.z);
        InputField inputFieldRotationX = gameObject.transform.GetChild(2).GetChild(0).GetComponent<InputField>();
        inputFieldRotationX.text = Convert.ToString(Mark.transform.eulerAngles.x);
        InputField inputFieldRotationY = gameObject.transform.GetChild(2).GetChild(1).GetComponent<InputField>();
        inputFieldRotationY.text = Convert.ToString(Mark.transform.eulerAngles.y);
        InputField inputFieldRotationZ = gameObject.transform.GetChild(2).GetChild(2).GetComponent<InputField>();
        inputFieldRotationZ.text = Convert.ToString(Mark.transform.eulerAngles.z);
        */
    }
}
