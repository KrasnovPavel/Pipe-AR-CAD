using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using Kompas6API5;
using Kompas6Constants3D;
using KompasAPI7;

namespace WpfApplication1
{
    public class KompasNotInstalledException : Exception
    {
        public KompasNotInstalledException() : base("Kompas not found, maybe it is not installed.")
        {
            
        }
    }

    public static class KompasConverter
    {
        public static async Task ConvertToKompas(string jsonFile, string outputDir)
        {
            await OpenKompas();

            await Task.Run(() =>
            {
                List<Tube> tubes = JsonReader.ReadFile(jsonFile);
                for (int i = 0; i < tubes.Count; i++)
                {
                    CreateParts(tubes[i], outputDir);
                    CreateAssembly(tubes[i], outputDir, $"Труба №{i+1}.a3d");
                }
            });
        }

        #region Private definitions
        
        private static async Task OpenKompas()
        {
            try
            {
                _kompas = (KompasObject) Marshal.GetActiveObject(ProgramId);
                if (_kompas != null)
                {
                    await ShowKompasWindow();
                }
                else
                {
                    await OpenNewKompasWindow();
                }
            }
            catch (COMException)
            {
                await OpenNewKompasWindow();
            }
            finally
            {
                if (_kompas == null)
                {
                    throw new KompasNotInstalledException();
                }
            }
        }
        
        private static async Task OpenNewKompasWindow()
        {
            Type t = Type.GetTypeFromProgID(ProgramId);
            _kompas = await Task.Run(() => (KompasObject)Activator.CreateInstance(t));
            if (_kompas != null)
            {
                await ShowKompasWindow();
            }
        }
        
        private static async Task ShowKompasWindow()
        {
            await Task.Run(() =>
            {
                _kompas.Visible = true;
                _kompas.ActivateControllerAPI();
            });
        }
        
        private static void CreateParts(Tube tube, string outputDir)
        {
            foreach (Fragment fragment in tube.fragments)
            {
                Document3D doc = _kompas.Document3D();
                doc.Create();
                
                ksPart part = doc.GetPart((int)Part_Type.pTop_Part);

                ksEntity entityBaseSketch = CreateBaseSketch(tube.diameter, tube.width, part);
                ksEntity trajectorySketch = CreateTrajectorySketches(fragment, part);
                MakeEvolution(entityBaseSketch, trajectorySketch, part);
                
                doc.SaveAs(Path.Combine(outputDir, $"./{fragment.GetHashCode()}.m3d"));
            }
        }
        
        private static void CreateAssembly(Tube tube, string outputDir, string fileName)
        {
            Document3D doc = (Document3D) _kompas.Document3D();
            doc.Create(false, false);
            
            Part7 topPart = ((KompasDocument3D) ((IApplication) _kompas.ksGetApplication7()).ActiveDocument).TopPart;
            
            foreach (Fragment fragment in tube.fragments)
            {
                Part7 newPart = topPart.Parts.AddFromFile(Path.Combine(outputDir, $"./{fragment.GetHashCode()}.m3d"));

                newPart.Placement.InitByMatrix3D(fragment.transform);
                newPart.UpdatePlacement(true);
                newPart.Update();
            }
            
            doc.RebuildDocument();
            doc.SaveAs(Path.Combine(outputDir, fileName));
        }
        
        private static ksEntity CreateBaseSketch(double diameter, double width, ksPart part)
        {
            ksEntity entitySketch = (ksEntity)part.NewEntity((short)Obj3dType.o3d_sketch);
            if (entitySketch == null) throw new Exception("Can't create sketch");
            
            ksSketchDefinition sketchBaseDef = (ksSketchDefinition)entitySketch.GetDefinition();
            if (sketchBaseDef == null) throw new Exception("Can't get sketchDef");
            
            ksEntity basePlane = (ksEntity)part.GetDefaultEntity((short)Obj3dType.o3d_planeXOY);
            sketchBaseDef.SetPlane(basePlane);
            entitySketch.Create();

            ksDocument2D sketchEdit = (ksDocument2D)sketchBaseDef.BeginEdit();
            sketchEdit.ksCircle(0, 0, diameter / 2, 1);
            sketchEdit.ksCircle(0, 0, (diameter - width) / 2, 1);
            sketchBaseDef.EndEdit();

            return entitySketch;
        }   
        
        private static ksEntity CreateTrajectorySketches(Fragment fragment, ksPart part)
        {     
            ksEntity entity = (ksEntity) part.NewEntity((short) Obj3dType.o3d_sketch);
            if (entity == null) throw new Exception("Can't create sketch");
        
            ksSketchDefinition sketchDef = (ksSketchDefinition)entity.GetDefinition();
            if (sketchDef == null) throw new Exception("Can't get sketchDef");
        
            ksEntity basePlane = (ksEntity)part.GetDefaultEntity((short)Obj3dType.o3d_planeXOZ);
            if (basePlane == null) throw new Exception("Can't create plane3points");

            sketchDef.SetPlane(basePlane);
            entity.Create();
            
            ksDocument2D sketchEdit = (ksDocument2D)sketchDef.BeginEdit();
            switch (fragment.type)
            {
                case "Direct":
                    sketchEdit.ksLineSeg(0, 0, 0, -fragment.length, 1);
                    break;
                case "Bended":
                    double endX = fragment.radius - fragment.radius * Math.Cos(fragment.bendAngle * Math.PI / 180.0);
                    double endY = fragment.radius * Math.Sin(fragment.bendAngle * Math.PI / 180.0);
                    sketchEdit.ksArcByPoint(-fragment.radius, 0,
                        fragment.radius,
                        0, 0,
                        -endX, -endY,-1, 1);
                    break;
            }
            sketchDef.EndEdit();
            return entity;
        }
        
        private static void MakeEvolution(ksEntity baseSketch, ksEntity trajectorySketch, ksPart part)
        {
            ksEntity entityEvolution = (ksEntity)part.NewEntity((short)Obj3dType.o3d_bossEvolution);
            if (entityEvolution == null) throw new Exception("Can't create evolution");
            ksBossEvolutionDefinition evolutionDef = (ksBossEvolutionDefinition)entityEvolution.GetDefinition();
            if (evolutionDef == null) throw new Exception("Can't get evolutionDef");
            
            evolutionDef.SetSketch(baseSketch);
            evolutionDef.PathPartArray().Add(trajectorySketch);
            entityEvolution.Create();
        }

        #endregion
        
        private static KompasObject _kompas;
        private const string ProgramId = "KOMPAS.Application.5";
    }
}