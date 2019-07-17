using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonOfMarkPanel : MonoBehaviour
{
    public void DeleteMark()
    {
        MarksController.Instance.DeleteMark(transform.parent.parent.gameObject.GetComponent<MarkParamPanel>().Mark.GetComponent<MarkOnScene>().Id);
    }

    public void SelectMark()
    {
        MarksController.Instance.SelectMark(transform.parent.parent.gameObject.GetComponent<MarkParamPanel>().Mark.GetComponent<MarkOnScene>().Id);
    }
}
