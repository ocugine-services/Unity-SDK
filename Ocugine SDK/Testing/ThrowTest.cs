using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
//  Throw Testing Component
//  Use this component to test cloud reporting
//===================================================
[AddComponentMenu("Ocugine SDK/Tests/Throw Testing Component")]
public class ThrowTest : MonoBehaviour{
    public bool startup_throw = false; // Throw at startup

    // Start is called before the first frame update
    void Start(){
        if (startup_throw)
            throw new System.Exception("This is a Throw Exception test for Unity SDK");
    }

    // Test Generated Exception
    public void testGenerated(){
        throw new System.Exception("This is a generated Throw Exception");
    }

    // Test Object not found
    public void testUnknown(){
        GameObject.Find("UnknownObject").GetComponent<OcugineSDK>().settings.platform = "unknown_platform";
    }
}
