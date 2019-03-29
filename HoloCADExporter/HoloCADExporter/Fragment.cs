using System;

namespace WpfApplication1
{
    [Serializable]
    public class Fragment
    {
        public double[] transform;

        public string type;
        public double length;
        public double radius;
        public int bendAngle;
    }
}