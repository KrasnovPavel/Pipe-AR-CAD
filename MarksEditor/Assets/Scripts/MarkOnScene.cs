using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UnityEngine;

namespace MarksEditor
{
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
        
            ChangeIdOnTextMesh();
            ParamPanelofThisMark.IdText.text = Convert.ToString(Id+1);
        
            PropertyChanged += delegate(object sender, PropertyChangedEventArgs args)
            {
                if (args.PropertyName == nameof(Id))
                {
                    ChangeIdOnTextMesh();
                    ParamPanelofThisMark.IdText.text = Convert.ToString(Id+1);
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
            TextMeshOfNumber.text = Convert.ToString(Id+1);
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
}
