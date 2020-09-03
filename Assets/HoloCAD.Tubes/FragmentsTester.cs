// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using HoloCAD.Tubes.C3D;
using HoloCAD.Tubes.Unity;
using UnityC3D;
using UnityEngine;

namespace HoloCAD.Tubes
{
    public class FragmentsTester : MonoBehaviour
    {
        public void AddNewTube()
        {
            _firstTube = TubeFragmentsFactory.CreateDirect();
            _firstTube.name = "Parent";
            _firstTubeFragment = _firstTube.GetComponent<DirectFragmentView>().Fragment;
        }

        public void ChangeLength()
        {
            _firstTubeFragment.Length += 0.1f;
        }

        public void AddChild()
        {
            _secondTube = TubeFragmentsFactory.CreateDirect(TubeFragmentsFactory.MainSys, _firstTubeFragment);
            _secondTube.name = "Second";
            _secondTubeFragment = _secondTube.GetComponent<DirectFragmentView>().Fragment;
        }

        public void Join()
        {
            TubeFragmentsFactory.MainSys.MakeCoincident(_firstTubeFragment.EndCircle, 
                _secondTubeFragment.EndCircle, GCMAlignment.Opposite);
        }

        public void Evaluate()
        {
            TubeFragmentsFactory.MainSys.Evaluate();
        }

        private GameObject _firstTube;
        private DirectFragment _firstTubeFragment;
        private GameObject _secondTube;
        private DirectFragment _secondTubeFragment;
    }
}