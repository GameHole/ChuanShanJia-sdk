using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace Default
{
	public class Test
	{
        [MenuItem("Assets/Test")]
        static void AAA()
        {
            string m = "123456789";
            //Debug.Log(m.Remove(1, 2));
            Debug.Log(m.IndexOf("2345"));
          
        }
	}
}
