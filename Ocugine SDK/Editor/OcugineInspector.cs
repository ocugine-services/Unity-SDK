using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//===================================================
//  Ocugine SDK
//  Sofware Development Kit developed specially for
//  Ocugine Services. With this library you can
//  use all features of Ocugine Services
//
//  @name           Ocugine SDK
//  @developer      Ocugine Platform
//  @version        0.4.0a
//  @build          401
//  @url            https://ocugine.pro/
//  @docs           https://docs.ocugine.pro/
//  @license        MIT
//===================================================
//===================================================
//  Ocugine SDK Inspector Class for Unity Editor
//===================================================
[CustomEditor(typeof(OcugineSDK))]
public class OcugineInspector : Editor{
    private static readonly string[] _dontIncludeMe = new string[] { "m_Script" };
    public override void OnInspectorGUI(){
        // Draw Image
        //GUI.DrawTexture(new Rect(0, 0, 268, 105), Resources.Load("Ocugine/Logo") as Texture2D);
        GUILayout.Box(Resources.Load("Ocugine/Logo") as Texture2D);

        // Draw Ocugine Buttons
        GUILayout.Space(10f);
        if (GUILayout.Button("Open Ocugine Documentation")){
            Application.OpenURL("https://help.ocugine.pro/");
        }
        if (GUILayout.Button("Open Ocugine Dashboard")){
            Application.OpenURL("https://cp.ocugine.pro/");
        }

        // Draw Header
        GUILayout.Space(10f);
        GUILayout.Label("Setup Ocugine SDK:", EditorStyles.boldLabel);

        // Draw Object Params
        serializedObject.Update();
        DrawPropertiesExcluding(serializedObject, _dontIncludeMe);
        serializedObject.ApplyModifiedProperties();
    }
}
