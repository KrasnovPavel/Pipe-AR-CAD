using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;
using UnityEngine.UI;
public class MarkParamPanel : MonoBehaviour
{
    public GameObject Mark;
    CultureInfo ci = new CultureInfo("en-US");
    
    public void SetParamsToMarkFromInputs()
    {
        if (Mark == null) return;
        Text idText = gameObject.transform.GetChild(0).GetComponent<Text>();
        idText.text = Convert.ToString(Mark.GetComponent<MarkOnScene>().Id);
        InputField inputFieldX = gameObject.transform.GetChild(1).GetChild(0).GetComponent<InputField>();
        InputField inputFieldY = gameObject.transform.GetChild(1).GetChild(1).GetComponent<InputField>();
        InputField inputFieldZ = gameObject.transform.GetChild(1).GetChild(2).GetComponent<InputField>();

        Mark.transform.position = new Vector3(float.Parse(inputFieldX.text,ci), float.Parse(inputFieldY.text,ci),float.Parse(inputFieldZ.text,ci));
        InputField inputFieldRotationX = gameObject.transform.GetChild(2).GetChild(0).GetComponent<InputField>();
        InputField inputFieldRotationY = gameObject.transform.GetChild(2).GetChild(1).GetComponent<InputField>();
        InputField inputFieldRotationZ = gameObject.transform.GetChild(2).GetChild(2).GetComponent<InputField>();
        Mark.transform.rotation = Quaternion.Euler(float.Parse(inputFieldRotationX.text,ci), float.Parse(inputFieldRotationY.text,ci),float.Parse(inputFieldRotationZ.text,ci));
      // Mark.transform.Rotate(new Vector3(float.Parse(inputFieldRotationX.text,ci), float.Parse(inputFieldRotationY.text,ci),float.Parse(inputFieldRotationZ.text,ci)));
        
    }

    private void Update()
    {
        if (Mark == null) return;
        if (!Mark.GetComponent<MarkOnScene>().HasUpdate) return;
        Text idText = gameObject.transform.GetChild(0).GetComponent<Text>();
        idText.text = Convert.ToString(Mark.GetComponent<MarkOnScene>().Id);
        InputField inputFieldX = gameObject.transform.GetChild(1).GetChild(0).GetComponent<InputField>();
        inputFieldX.text =Mark.transform.position.x.ToString(ci);
        InputField inputFieldY = gameObject.transform.GetChild(1).GetChild(1).GetComponent<InputField>();
        inputFieldY.text =Mark.transform.position.y.ToString(ci);
        InputField inputFieldZ = gameObject.transform.GetChild(1).GetChild(2).GetComponent<InputField>();
        inputFieldZ.text =Mark.transform.position.z.ToString(ci);
        InputField inputFieldRotationX = gameObject.transform.GetChild(2).GetChild(0).GetComponent<InputField>();
        inputFieldRotationX.text = Mark.transform.eulerAngles.x.ToString(ci);
        InputField inputFieldRotationY = gameObject.transform.GetChild(2).GetChild(1).GetComponent<InputField>();
        inputFieldRotationY.text = Mark.transform.eulerAngles.y.ToString(ci);
        InputField inputFieldRotationZ = gameObject.transform.GetChild(2).GetChild(2).GetComponent<InputField>();
        inputFieldRotationZ.text = Mark.transform.eulerAngles.z.ToString(ci);
        Mark.GetComponent<MarkOnScene>().HasUpdate = false;
    }


}
