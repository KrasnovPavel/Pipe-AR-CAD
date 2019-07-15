using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloCore.UI;
using HoloCAD.UnityTubes;
public class MarkButton : MonoBehaviour
{
    /// <summary> Трехмерная кнопка для смены цели привязки со стен на модель и наоборот. </summary>
    [Tooltip("Трехмерная кнопка для смены цели привязки со стен на модель и наоборот")]
    public Button3D ChangeTargetColliderButton;

    private bool ActiveWalls = true;
    void Start()
    {
        if (ChangeTargetColliderButton != null)   ChangeTargetColliderButton.OnClick   += ChangeTargetCollider;
    }
    //TODO: исправить ситуацию, когда активен collider и стен, и модели
    public void ChangeTargetCollider(MonoBehaviour sender = null)
    {
        if (ActiveWalls)
        {
            MarksHandler.Instance.transform.GetChild(0).GetChild(0).gameObject.GetComponent<MeshCollider>().enabled = true;
            TubeUnityManager.ShowGrid(false);
        }
        else
        {
            MarksHandler.Instance.transform.GetChild(0).GetChild(0).gameObject.GetComponent<MeshCollider>().enabled = false;
            TubeUnityManager.ShowGrid(true);
        }
        ActiveWalls = !ActiveWalls;
    }
}
