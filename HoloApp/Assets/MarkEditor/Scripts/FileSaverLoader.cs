using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dummiesman;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using UnityEditor;
using System.Text;
using Newtonsoft.Json;
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
   using static SFB.StandaloneFileBrowser;
#endif
public static class FileSaverLoader
{
   private static string ObjFilePath;
   private static string JsonText;
   public static void LoadObjModel()
   {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
      string[] paths;
      
      paths = OpenFilePanel("Open scheme","","obj",false);
      if (paths.Length == 0) return;
      string FilePath = paths[0];
      ObjFilePath = FilePath;
      var loadedObj = new OBJLoader().Load(FilePath);
      loadedObj.transform.position= new Vector3(0,0,0);
#endif
   }

   public static void SaveSceneFile()
   {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
      string path;
      path = SaveFilePanel("","","","mscene");
      if (path.Length == 0) return;
      string[] pathSplits = path.Split('\\');
      string directoryPath = "";
      for(int i=0;i<pathSplits.Length-1;i++)
      {
         directoryPath += pathSplits[i]+"\\";
      }

      string jsonFilePath = Path.Combine(directoryPath, "MarkSettings.json");
      StreamWriter sw = new StreamWriter(jsonFilePath);
      sw.Write("aga");
      sw.Close();
      // GameObject targetGameObject = GameObject.Find("Target");
     // OBJExporter.MeshToFile(targetGameObject.GetComponent<MeshFilter>(),Path.Combine(directoryPath, "target.obj"));
      OBJExporter.FileToFile(ObjFilePath,Path.Combine(directoryPath, "target.obj"));
      string[] filesToCompressPaths = {jsonFilePath, Path.Combine(directoryPath, "target.obj")};
      CreateFile(path, filesToCompressPaths);
      foreach (string fileToCompress in filesToCompressPaths)
      {
         if(File.Exists(fileToCompress)) File.Delete(fileToCompress);
      }
#endif
   }

   public static void LoadSceneFile()
   {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
      string[] paths;
      
      paths = OpenFilePanel("Open scheme","","mscene",false);
      if (paths.Length == 0) return;
      string FilePath = paths[0];

      string[] pathsToFiles = ReadFile(FilePath);
      
      foreach (string pathToFile in pathsToFiles)
      {
         FileInfo fileInfo = new FileInfo(pathToFile);
         switch(fileInfo.Extension){
         case "obj" :
            var loadedObj = new OBJLoader().Load(pathToFile);
            loadedObj.transform.position= new Vector3(0,0,0);
            break;
         case "json":
            //TODO:LLL
            Debug.Log(fileInfo.OpenText().ReadToEnd());
            break;
         }
         
      }
#endif
   }
   
   private static string[] ReadFile(string path)
   {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
      List<string> pathsToFiles = new List<string>();
      string[] pathSplits = path.Split('\\');
      string directoryPath = "";
      for(int i=0;i<pathSplits.Length-1;i++)
      {
         directoryPath += pathSplits[i]+"\\";
      }
      
      FileInfo fileToRead = new FileInfo(path);
      string fileName = "";
      int lineCount = 0;
      using (StreamReader fileReader = fileToRead.OpenText()){
         
         while (!fileReader.EndOfStream)
         {
            fileName = fileReader.ReadLine();
            lineCount = Convert.ToInt32(fileReader.ReadLine());
            StreamWriter  FileWriter = File.CreateText(Path.Combine(directoryPath,fileName));
            for (int i = 0; i < lineCount; i++)
            {
               FileWriter.WriteLine(fileReader.ReadLine()+"\n");
            }
            FileWriter.Close();
         }
      }
      return pathsToFiles.ToArray();
#endif
      return null;
   }
   
   private static void CreateFile(string path, string[] filesToCompressPaths)
   {
      
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
      
      StreamWriter  FileWriter = File.CreateText(path);
      foreach (string fileToCompressPath in filesToCompressPaths)
      {
         
       //  Debug.Log(fileToCompressPath);
         FileInfo fileToRead = new FileInfo(fileToCompressPath);
         using (StreamReader fileReader = fileToRead.OpenText())
         {
            string outputString = "";
            int lineCount = 0;
            while (!fileReader.EndOfStream)
            {
               lineCount++;
               outputString += fileReader.ReadLine()+"\n";
            }

            FileWriter.WriteLine(fileToRead.Name);
            FileWriter.WriteLine(lineCount);
            FileWriter.Write(outputString);
         }
        /* using (FileStream fileToCompressStream = fileToCompress.OpenRead())
         {
            FileToSave.WriteLine((fileToCompressStream.Length).ToString());
            byte[] fileToCompressBytes =new byte[fileToCompressStream.Length];
            fileToCompressStream.Read(fileToCompressBytes, 0, (int)(fileToCompressStream.Length));
            FileToSave.Write(fileToCompressBytes);
            FileToSave.Write();
         }*/
         // break;
      }
      FileWriter.Close();
#endif
   }
}
