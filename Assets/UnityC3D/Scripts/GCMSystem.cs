// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace UnityC3D
{
    public class GCMSystem : IDisposable
    {
        public GCM_LCS GroundLCS => new GCM_LCS(this, new GCMDescriptor {id = 0});

        public event Action Evaluated;

        public GCMSystem()
        {
            _gcmSystemPtr = GCM_CreateSystem();
        }

        public void Clear()
        {
            GCM_ClearSystem(_gcmSystemPtr);
        }

        public void Dispose()
        {
            GCM_RemoveSystem(_gcmSystemPtr);
        }

        public GCMResult Evaluate()
        {
            var res = GCM_Evaluate(_gcmSystemPtr);
            Evaluated?.Invoke();
            return res;
        }

        public void MakeCoincident(GCMPoint first, GCMPoint second, GCMAlignment alignment = GCMAlignment.NoAlignment)
        {
            MakeCoincident((GCMObject) first, (GCMObject) second, alignment);
        }

        public void MakeCoincident(GCMLine first, GCMLine second, GCMAlignment alignment = GCMAlignment.NoAlignment)
        {
            MakeCoincident((GCMObject) first, (GCMObject) second, alignment);
        }

        public void MakeCoincident(GCMLine first, GCMPoint second, GCMAlignment alignment = GCMAlignment.NoAlignment)
        {
            MakeCoincident((GCMObject) first, (GCMObject) second, alignment);
        }

        public void MakeCoincident(GCMPlane first, GCMPlane second, GCMAlignment alignment = GCMAlignment.NoAlignment)
        {
            MakeCoincident((GCMObject) first, (GCMObject) second, alignment);
        }

        public void MakeCoincident(GCMPoint first, GCMPlane second, GCMAlignment alignment = GCMAlignment.NoAlignment)
        {
            MakeCoincident((GCMObject) first, (GCMObject) second, alignment);
        }

        public void MakeCoincident(GCMCircle first, GCMCircle second, GCMAlignment alignment = GCMAlignment.NoAlignment)
        {
            MakeCoincident((GCMObject) first, (GCMObject) second, alignment);
        }

        public void MakeCoincident(GCMCircle first, GCMPlane second, GCMAlignment alignment = GCMAlignment.NoAlignment)
        {
            MakeCoincident((GCMObject) first, (GCMObject) second, alignment);
        }

        public void MakeCoincident(GCM_LCS first, GCM_LCS second, GCMAlignment alignment = GCMAlignment.NoAlignment)
        {
            MakeCoincident((GCMObject) first, (GCMObject) second, alignment);
        }

        public void MakeCoincident(GCMPoint first, GCM_LCS second, GCMAlignment alignment = GCMAlignment.NoAlignment)
        {
            MakeCoincident((GCMObject) first, (GCMObject) second, alignment);
        }

        public void MakeConcentric(GCMCircle first, GCMCircle second, GCMAlignment alignment = GCMAlignment.NoAlignment)
        {
            MakeConcentric((GCMObject) first, (GCMObject) second, alignment);
        }

        public void MakeConcentric(GCMCircle first, GCMLine second, GCMAlignment alignment = GCMAlignment.NoAlignment)
        {
            MakeConcentric((GCMObject) first, (GCMObject) second, alignment);
        }

        public void MakeConcentric(GCMCircle first, GCMPoint second, GCMAlignment alignment = GCMAlignment.NoAlignment)
        {
            MakeConcentric((GCMObject) first, (GCMObject) second, alignment);
        }

        public void MakeParallel(GCMLine first, GCMLine second, GCMAlignment alignment = GCMAlignment.NoAlignment)
        {
            MakeParallel((GCMObject) first, (GCMObject) second, alignment);
        }

        public void MakeParallel(GCMPlane first, GCMPlane second, GCMAlignment alignment = GCMAlignment.NoAlignment)
        {
            MakeParallel((GCMObject) first, (GCMObject) second, alignment);
        }

        public void MakeParallel(GCMLine first, GCMPlane second, GCMAlignment alignment = GCMAlignment.NoAlignment)
        {
            MakeParallel((GCMObject) first, (GCMObject) second, alignment);
        }

        public void MakePerpendicular(GCMLine first, GCMLine second, GCMAlignment alignment = GCMAlignment.NoAlignment)
        {
            MakePerpendicular((GCMObject) first, (GCMObject) second, alignment);
        }

        public void MakePerpendicular(GCMPlane first, GCMPlane second,
                                      GCMAlignment alignment = GCMAlignment.NoAlignment)
        {
            MakePerpendicular((GCMObject) first, (GCMObject) second, alignment);
        }

        public void MakePerpendicular(GCMLine first, GCMPlane second, GCMAlignment alignment = GCMAlignment.NoAlignment)
        {
            MakePerpendicular((GCMObject) first, (GCMObject) second, alignment);
        }

        public void SetDistance(GCMObject first, GCMObject second, float distance,
                                GCMAlignment alignment = GCMAlignment.NoAlignment)
        {
            GCM_AddDistance(_gcmSystemPtr, first.Descriptor, second.Descriptor, distance, alignment);
        }

        #region Internal definitions

        internal GCMDescriptor AddPoint(Vector3 point)
        {
            var o = MbVector3D.FromUnity(point);
            var p = GCM_Point(ref o);
            return GCM_AddGeom(_gcmSystemPtr, ref p);
        }

        internal GCMDescriptor AddLine(Vector3 origin, Vector3 direction)
        {
            var o = MbVector3D.FromUnity(origin);
            var d = MbVector3D.FromUnity(-direction);
            var line = GCM_Line(ref o, ref d);
            return GCM_AddGeom(_gcmSystemPtr, ref line);
        }

        internal GCMDescriptor AddPlane(Vector3 origin, Vector3 normal)
        {
            var o = MbVector3D.FromUnity(origin);
            var n = MbVector3D.FromUnity(-normal);
            var plane = GCM_Plane(ref o, ref n);
            return GCM_AddGeom(_gcmSystemPtr, ref plane);
        }

        internal GCMDescriptor AddCircle(Vector3 origin, Vector3 normal, float radius)
        {
            var o = MbVector3D.FromUnity(origin);
            var n = MbVector3D.FromUnity(-normal);
            var circle = GCM_Circle(ref o, ref n, radius);
            return GCM_AddGeom(_gcmSystemPtr, ref circle);
        }

        internal GCMDescriptor AddLCS(Transform tr)
        {
            var p = MbPlacement3D.FromUnity(tr);
            var lcs = GCM_SolidLCS(ref p);
            return GCM_AddGeom(_gcmSystemPtr, ref lcs);
        }

        internal MbPlacement3D GetPlacement(GCMObject obj)
        {
            return GCM_Placement(_gcmSystemPtr, obj.Descriptor);
        }

        internal void SetPlacement(GCMObject obj, MbPlacement3D newPlacement)
        {
            GCM_SetPlacement(_gcmSystemPtr, obj.Descriptor, ref newPlacement);
        }

        internal GCMDescriptor CreateRadiusConstraint(GCMCircle circle)
        {
            return GCM_FixRadius(_gcmSystemPtr, circle.Descriptor);
        }

        internal void SetRadius(GCMCircle circle, float newRadius)
        {
            GCM_ChangeDrivingDimension(_gcmSystemPtr, circle.RadiusConstraint, newRadius);
        }

        internal float GetRadius(GCMCircle circle)
        {
            return (float) GCM_RadiusA(_gcmSystemPtr, circle.Descriptor);
        }

        #endregion

        #region Private definitions

        private void MakeCoincident(GCMObject first, GCMObject second, GCMAlignment alignment)
        {
            GCM_AddBinConstraint(_gcmSystemPtr, GCMConstraintType.GCM_COINCIDENT, first.Descriptor,
                                 second.Descriptor, alignment, GCMTanChoice.GCM_TAN_NONE);
        }

        private void MakeConcentric(GCMObject first, GCMObject second, GCMAlignment alignment)
        {
            GCM_AddBinConstraint(_gcmSystemPtr, GCMConstraintType.GCM_CONCENTRIC, first.Descriptor,
                                 second.Descriptor, alignment, GCMTanChoice.GCM_TAN_NONE);
        }

        private void MakeParallel(GCMObject first, GCMObject second, GCMAlignment alignment)
        {
            GCM_AddBinConstraint(_gcmSystemPtr, GCMConstraintType.GCM_PARALLEL, first.Descriptor,
                                 second.Descriptor, alignment, GCMTanChoice.GCM_TAN_NONE);
        }

        private void MakePerpendicular(GCMObject first, GCMObject second, GCMAlignment alignment)
        {
            GCM_AddBinConstraint(_gcmSystemPtr, GCMConstraintType.GCM_PERPENDICULAR, first.Descriptor,
                                 second.Descriptor, alignment, GCMTanChoice.GCM_TAN_NONE);
        }

        private IntPtr _gcmSystemPtr;

        #endregion

        #region Extern functions

#if UNITY_EDITOR_64
        [DllImport("c3d", EntryPoint = "?GCM_Evaluate@@YA?AW4GCM_result@@PEAVMtGeomSolver@@@Z")]
#else
        [DllImport("c3d", EntryPoint = "?GCM_Evaluate@@YA?AW4GCM_result@@PAVMtGeomSolver@@@Z")]
#endif
        private static extern GCMResult GCM_Evaluate(IntPtr gSys);

#if UNITY_EDITOR_64
        [DllImport("c3d", EntryPoint = "?GCM_CreateSystem@@YAPEAVMtGeomSolver@@XZ")]
#else
        [DllImport("c3d", EntryPoint = "?GCM_CreateSystem@@YAPAVMtGeomSolver@@XZ")]
#endif
        private static extern IntPtr GCM_CreateSystem();

#if UNITY_EDITOR_64
        [DllImport("c3d", EntryPoint = "?GCM_RemoveSystem@@YAXPEAVMtGeomSolver@@@Z")]
#else
        [DllImport("c3d", EntryPoint = "?GCM_RemoveSystem@@YAXPAVMtGeomSolver@@@Z")]
#endif
        private static extern void GCM_RemoveSystem([In] IntPtr gcmSystem);

#if UNITY_EDITOR_64
        [DllImport("c3d", EntryPoint = "?GCM_ClearSystem@@YAXPEAVMtGeomSolver@@@Z")]
#else
        [DllImport("c3d", EntryPoint = "?GCM_ClearSystem@@YAXPAVMtGeomSolver@@@Z")]
#endif
        private static extern void GCM_ClearSystem([In] IntPtr gcmSystem);

#if UNITY_EDITOR_64
        [DllImport("c3d", EntryPoint = "?GCM_Placement@@YA?AVMbPlacement3D@@PEAVMtGeomSolver@@UMtObjectId@@@Z")]
#else
        [DllImport("c3d", EntryPoint = "?GCM_Placement@@YA?AVMbPlacement3D@@PAVMtGeomSolver@@UMtObjectId@@@Z")]
#endif
        private static extern MbPlacement3D GCM_Placement(IntPtr gSys, GCMDescriptor g);

#if UNITY_EDITOR_64
        [DllImport("c3d", EntryPoint = "?GCM_SetPlacement@@YAXPEAVMtGeomSolver@@UMtObjectId@@AEBVMbPlacement3D@@@Z")]
#else
        [DllImport("c3d", EntryPoint = "?GCM_SetPlacement@@YAXPAVMtGeomSolver@@UMtObjectId@@ABVMbPlacement3D@@@Z")]
#endif
        private static extern void GCM_SetPlacement(IntPtr gSys, GCMDescriptor g, ref MbPlacement3D newPlacement);

#if UNITY_EDITOR_64
        [DllImport("c3d", EntryPoint = "?GCM_RadiusA@@YANPEAVMtGeomSolver@@UMtObjectId@@@Z")]
#else
        [DllImport("c3d", EntryPoint = "?GCM_RadiusA@@YANPAVMtGeomSolver@@UMtObjectId@@@Z")]
#endif
        private static extern double GCM_RadiusA(IntPtr gSys, GCMDescriptor g);

#if UNITY_EDITOR_64
        [DllImport("c3d", EntryPoint = "?GCM_Point@@YA?AUGCM_g_record@@AEBVMbCartPoint3D@@@Z")]
#else
        [DllImport("c3d", EntryPoint = "?GCM_Point@@YA?AUGCM_g_record@@ABVMbCartPoint3D@@@Z")]
#endif
        private static extern GCMGeometry GCM_Point(ref MbVector3D origin);

#if UNITY_EDITOR_64
        [DllImport("c3d", EntryPoint = "?GCM_Line@@YA?AUGCM_g_record@@AEBVMbCartPoint3D@@AEBVMbVector3D@@@Z")]
#else
        [DllImport("c3d", EntryPoint = "?GCM_Line@@YA?AUGCM_g_record@@ABVMbCartPoint3D@@ABVMbVector3D@@@Z")]
#endif
        private static extern GCMGeometry GCM_Line(ref MbVector3D origin, ref MbVector3D direction);

#if UNITY_EDITOR_64
        [DllImport("c3d", EntryPoint = "?GCM_Plane@@YA?AUGCM_g_record@@AEBVMbCartPoint3D@@AEBVMbVector3D@@@Z")]
#else
        [DllImport("c3d", EntryPoint = "?GCM_Plane@@YA?AUGCM_g_record@@ABVMbCartPoint3D@@ABVMbVector3D@@@Z")]
#endif
        private static extern GCMGeometry GCM_Plane(ref MbVector3D origin, ref MbVector3D normal);

#if UNITY_EDITOR_64
        [DllImport("c3d", EntryPoint = "?GCM_Circle@@YA?AUGCM_g_record@@AEBVMbCartPoint3D@@AEBVMbVector3D@@N@Z")]
#else
        [DllImport("c3d", EntryPoint = "?GCM_Circle@@YA?AUGCM_g_record@@ABVMbCartPoint3D@@ABVMbVector3D@@N@Z")]
#endif
        private static extern GCMGeometry GCM_Circle(ref MbVector3D origin, ref MbVector3D normal, double radius);

#if UNITY_EDITOR_64
        [DllImport("c3d", EntryPoint = "?GCM_SolidLCS@@YA?AUGCM_g_record@@AEBVMbPlacement3D@@@Z")]
#else
        [DllImport("c3d", EntryPoint = "?GCM_SolidLCS@@YA?AUGCM_g_record@@ABVMbPlacement3D@@@Z")]
#endif
        private static extern GCMGeometry GCM_SolidLCS(ref MbPlacement3D position);

#if UNITY_EDITOR_64
        [DllImport("c3d", EntryPoint = "?GCM_AddGeom@@YA?AUMtObjectId@@PEAVMtGeomSolver@@AEBUGCM_g_record@@@Z")]
#else
        [DllImport("c3d", EntryPoint = "?GCM_AddGeom@@YA?AUMtObjectId@@PAVMtGeomSolver@@ABUGCM_g_record@@@Z")]
#endif
        private static extern GCMDescriptor GCM_AddGeom(IntPtr gSys, ref GCMGeometry g);

#if UNITY_EDITOR_64
        [DllImport("c3d", EntryPoint = "?GCM_FixRadius@@YA?AUMtObjectId@@PEAVMtGeomSolver@@U1@@Z")]
#else
        [DllImport("c3d", EntryPoint = "?GCM_FixRadius@@YA?AUMtObjectId@@PAVMtGeomSolver@@U1@@Z")]
#endif
        private static extern GCMDescriptor GCM_FixRadius(IntPtr gSys, GCMDescriptor g);

#if UNITY_EDITOR_64
        [DllImport("c3d",
                   EntryPoint =
                       "?GCM_AddBinConstraint@@YA?AUMtObjectId@@PEAVMtGeomSolver@@W4GCM_c_type@@U1@2W4GCM_alignment@@W4GCM_tan_choice@@@Z")]
#else
        [DllImport("c3d", 
                   EntryPoint =
                       "?GCM_AddBinConstraint@@YA?AUMtObjectId@@PAVMtGeomSolver@@W4GCM_c_type@@U1@2W4GCM_alignment@@W4GCM_tan_choice@@@Z")]
#endif
        private static extern GCMDescriptor GCM_AddBinConstraint(IntPtr gSys, GCMConstraintType cType,
                                                                 GCMDescriptor geom1, GCMDescriptor geom2,
                                                                 GCMAlignment aValue, GCMTanChoice tValue);

#if UNITY_EDITOR_64
        [DllImport("c3d", EntryPoint = "?GCM_AddDistance@@YA?AUMtObjectId@@PEAVMtGeomSolver@@U1@1NW4GCM_alignment@@@Z")]
#else
        [DllImport("c3d", EntryPoint = "?GCM_AddDistance@@YA?AUMtObjectId@@PAVMtGeomSolver@@U1@1NW4GCM_alignment@@@Z")]
#endif
        private static extern GCMDescriptor GCM_AddDistance(IntPtr gSys, GCMDescriptor geom1, GCMDescriptor geom2,
                                                            double value,
                                                            GCMAlignment aValue);

#if UNITY_EDITOR_64
        [DllImport("c3d",
                   EntryPoint = "?GCM_ChangeDrivingDimension@@YA?AW4GCM_result@@PEAVMtGeomSolver@@UMtObjectId@@N@Z")]
#else
        [DllImport("c3d", EntryPoint = "?GCM_ChangeDrivingDimension@@YA?AW4GCM_result@@PAVMtGeomSolver@@UMtObjectId@@N@Z")]
#endif
        private static extern GCMResult GCM_ChangeDrivingDimension(IntPtr gSys, GCMDescriptor c, double value);

        #endregion
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct GCMDescriptor
    {
        internal UInt32 id;
    }
}