using System;
using System.Collections.Generic;

namespace WpfApplication1
{
    [Serializable]
    public class Tube
    {
        public double diameter;
        public double width;
        public List<Fragment> fragments;
    }
}