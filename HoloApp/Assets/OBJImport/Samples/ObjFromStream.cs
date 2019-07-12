using Dummiesman;
using System.IO;
using System.Text;
using UnityEngine;
using System.Collections;
public class ObjFromStream : MonoBehaviour {
	void Start () {
        //make www
        var www = new WWW("https://people.sc.fsu.edu/~jburkardt/data/obj/lamp.obj");
        while (!www.isDone)
        {
	        StartCoroutine(Wait((float)0.001));
        }
//#if 
           // System.Threading.Thread.Sleep(1);
//#else

//#endif
        
        //create stream and load
        var textStream = new MemoryStream(Encoding.UTF8.GetBytes(www.text));
        var loadedObj = new OBJLoader().Load(textStream);
	}
	IEnumerator Wait(float delay)
	{
		
		yield return new WaitForSeconds(delay);
	}
}

