using System.IO;
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
   using static SFB.StandaloneFileBrowser;
#endif
namespace MarksEditor
{
   public static class FileSaverLoader
   {
      private static string ObjFilePath;

      public static void LoadObjModel()
      {
 
      }

      public static void SaveSceneFile()
      {

      }
      public static void SaveSceneJsonFile(string jsonText)
      {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
         string path;
         path=SaveFilePanel("","","","json");
         StreamWriter sw =  new StreamWriter(path); 
         sw.Write(jsonText);
         sw.Close();
#endif
      }
      public static void LoadSceneFile()
      {
    
      }
   
      private static string[] ReadFile(string path)
      {
         return null;
      }
   
      private static void CreateFile(string path, string[] filesToCompressPaths)
      {
     
      }
      public static string LoadJsonFile()
      {
         string jsonText = "";
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
         string[] paths;
         paths = OpenFilePanel("Open scene","","json",false);
         if (paths.Length == 0) return null;
         StreamReader sr = File.OpenText(paths[0]);
         jsonText = sr.ReadToEnd();
#endif
         return jsonText;
      }
   }
}
