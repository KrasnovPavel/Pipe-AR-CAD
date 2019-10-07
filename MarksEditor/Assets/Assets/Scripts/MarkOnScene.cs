using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class MarkOnScene : MonoBehaviour
{

    public MarkParamPanel ParamPanelofThisMark;
    public TextMesh TextMeshOfNumber;

    public int Id
    {
        get => _id;
        set
        {
            _id = value;
            OnPropertyChanged();
        }
    }

    public bool IsSelected {get; set;}

    /// <summary> Событие измененения свойств объекта </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    void Start()
    {
        PropertyChanged += delegate(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == nameof(Id))
            {
                ChangeIdOnTextMesh();
                ParamPanelofThisMark.IdText.text = Convert.ToString(Id);
            }
        };
    }

    private void Update()
    {
        if (transform.hasChanged)
        {
            ParamPanelofThisMark.MarkTransformIntoInput();
            transform.hasChanged = false;
        }
    }

    public void ChangeIdOnTextMesh()
    {
        if (TextMeshOfNumber == null) return;
        TextMeshOfNumber.text = Convert.ToString(Id);
    }

    #region Private defenitions

    private int _id;
    
    #endregion
    
    #region Protected defenitions
        
    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
        
    #endregion
}
