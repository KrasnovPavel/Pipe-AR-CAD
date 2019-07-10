using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dummiesman;
using System.IO;
using System.IO.Compression;
using System.Linq;
using UnityEditor;
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
   using static SFB.StandaloneFileBrowser;
#endif
public static class FileSaverLoader
{
   public static void LoadObjModel()
   {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
      string[] paths;
      
      paths = OpenFilePanel("Open scheme","","obj",false);
      if (paths.Length == 0) return;
      string FilePath = paths[0];
#endif
      var loadedObj = new OBJLoader().Load(FilePath);
      loadedObj.transform.position= new Vector3(0,0,0);
      
   }

   public static void SaveSceneFile()
   {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
      string path;
      path = SaveFilePanel("","","","zip");
      if (path.Length == 0) return;
      string FilePath = path;
      CompressZip(path);
#endif
   }

   public static void LoadSceneFile()
   {
      
   }
   
   private static void DecompressZip()
   {
      
   }
   
   private static void CompressZip(string path)
   {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN      
      string[] pathSplits = path.Split('\\');
      string fileName = pathSplits.Last();
      string directoryPath = "";
      for(int i=0;i<pathSplits.Length-1;i++)
      {
         directoryPath += pathSplits[i]+"\\";
      }

      string jsonFilePath = Path.Combine(directoryPath, "settings.json");
      StreamWriter sw = new StreamWriter(jsonFilePath);
      sw.Write("aga");
      sw.Close();
      GameObject targetGameObject = GameObject.Find("Target");
      DirectoryInfo directoryInfo =  new DirectoryInfo(directoryPath);
     // AssetDatabase.CreateAsset(targetGameObject,directoryPath+"\\obj.asset");
      OBJExporter.MeshToFile(targetGameObject.GetComponent<MeshFilter>(),Path.Combine(directoryPath, "target.obj"));
     // foreach (FileInfo fileToCompress in directoryInfo.GetFiles())
    //  {
       
    //  }
      
#endif
   }
}
