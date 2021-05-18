using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;

namespace TTSDK
{
    //[CustomEditor(typeof(TPPama))]
    //public class TTEditor:Editor
    //{
    //    public List<SerializedProperty> ps = new List<SerializedProperty>();

    //    private void OnEnable()
    //    {
    //        foreach (var item in typeof(TPPama).GetFields(BindingFlags.Instance| BindingFlags.Public| BindingFlags.NonPublic))
    //        {
    //            ps.Add(serializedObject.FindProperty(item.Name));
    //        }
    //    }

    //    public override void OnInspectorGUI()
    //    {
    //        serializedObject.Update();

    //        EditorGUI.indentLevel = 1;
    //        foreach (var item in ps)
    //        {
    //            EditorGUILayout.PropertyField(item, true/* new GUIContent(item.name)*/);
    //            GUILayout.Space(5);
    //        }
    //        if (GUI.changed)
    //        {
    //            EditorUtility.SetDirty(target);
    //        }
    //        //GUILayout.Space(5);
    //        if (GUILayout.Button("Apply"))
    //        {
    //            TPHelper.apply();
    //        }
          
    //        serializedObject.ApplyModifiedProperties();
    //    }
    //}
}
