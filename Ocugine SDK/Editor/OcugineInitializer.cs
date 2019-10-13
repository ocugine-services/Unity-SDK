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
//  Ocugine SDK Initializer Class for Unity Editor
//===================================================
public class OcugineInitializer : EditorWindow{
    [MenuItem("Ocugine SDK/Instanite SDK")]
    static void loadSDKPrefab(){
        GameObject sdk_prefab = PrefabUtility.InstantiatePrefab(Resources.Load("Ocugine/OcugineSDK") as GameObject) as GameObject; // Initialize
        Selection.activeGameObject = sdk_prefab; // Set Active Game Object
    }

    [MenuItem("Ocugine SDK/Open Documentation Page")]
    static void showDocs(){
        Application.OpenURL("https://help.ocugine.pro/");
    }

    [MenuItem("Ocugine SDK/Open Dashboard")]
    static void showDashboard(){
        Application.OpenURL("https://cp.ocugine.pro/");
    }
}
