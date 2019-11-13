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
//  @version        0.4.1
//  @build          413
//  @url            https://ocugine.pro/
//  @docs           https://docs.ocugine.pro/
//  @license        MIT
//===================================================
//===================================================
//  Ocugine SDK Error Report Data
//===================================================
[System.Serializable]
public class ErrorReportModel : BaseModel
{
    public DataModel data;

    [System.Serializable]
    public class DataModel
    {
        public double uid = 0;
        public string project_id = "";
        public string title = "";
        public string code = "";
        public string body = "";
        public string platform = "";
        public bool critical = false;
        public double time = 0;
    }
}

//===================================================
//  Ocugine SDK Performance Report Data
//===================================================
public class PerformanceReportModel : BaseModel
{
    public DataModel data;

    [System.Serializable]
    public class DataModel
    {
        public double uid = 0;
        public string project_id = "";
        public string title = "";
        public string body = "";
        public double time = 0;
    }
}