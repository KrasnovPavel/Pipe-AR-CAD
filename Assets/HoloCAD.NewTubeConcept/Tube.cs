using System;
using System.Collections.Generic;
using UnityC3D;
using UnityEngine;

namespace HoloCAD.NewTubeConcept
{
    public class Tube : IDisposable
    {
        public readonly GCMPlane StartPlane;
        public readonly GCMPoint StartPoint;
        public readonly GCMLine  StartLine;
        
        public readonly GCMPlane EndPlane;
        public readonly GCMPoint EndPoint;
        public readonly GCMLine  EndLine;
        
        public readonly List<GCMPoint> MiddlePoints = new List<GCMPoint>();
        public readonly List<GCMLine> MiddleLines = new List<GCMLine>();

        public Tube(GCMSystem sys, Vector3 startPoint, Vector3 startNormal, Vector3 endPoint, Vector3 endNormal, GCM_LCS parentLCS)
        {
            StartPoint = new GCMPoint(sys, startPoint, parentLCS);
            StartPoint.Freeze();
            StartPlane = new GCMPlane(sys, startPoint, startNormal, parentLCS);
            StartPlane.Freeze();
            StartLine = new GCMLine(sys, startPoint, startNormal, parentLCS);
            StartLine.Freeze();
            
            EndPoint = new GCMPoint(sys, endPoint, parentLCS);
            EndPoint.Freeze();
            EndPlane = new GCMPlane(sys, endPoint, endNormal, parentLCS);
            EndPlane.Freeze();
            EndLine = new GCMLine(sys, endPoint, endNormal, parentLCS);
            EndLine.Freeze();
            
            // Пытаемся соединить фланцы
            
            var con = new GCMPoint(sys, (startPoint + endPoint) / 2, parentLCS);
            sys.MakeCoincident(con, StartLine);
            sys.MakeCoincident(con, EndLine);
            if (sys.Evaluate() == GCMResult.GCM_RESULT_Ok)
            {
                MiddlePoints.Add(con);
            }
            else
            {
                con.Dispose();
            }
        }

        public void TestDraw(string tubeName)
        {
            StartPoint.TestDraw($"{tubeName}-StartPoint");
            StartLine.TestDraw($"{tubeName}-StartLine");
            EndPoint.TestDraw($"{tubeName}-EndPoint");
            EndLine.TestDraw($"{tubeName}-EndLine");
        }

        public void Dispose()
        {
            StartPlane?.Dispose();
            StartPoint?.Dispose();
            StartLine?.Dispose();
            EndPlane?.Dispose();
            EndPoint?.Dispose();
            EndLine?.Dispose();
        }
    }
}