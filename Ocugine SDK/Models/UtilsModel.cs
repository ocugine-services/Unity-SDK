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
//  Ocugine API Settings List Model
//===================================================
[System.Serializable]
public class APISettingsModel : BaseModel
{
    public DataModel data;

    [System.Serializable]
    public class DataModel
    {
        // Модель инфо
        public ConfingsModel configs;

        [System.Serializable]
        public class ConfingsModel
        {
            public double uid = 0;
            public double project_id = 0;
            public bool enabled = true;
            public double limitation = 0;
        }

    }
}

//===================================================
//  Ocugine User Agent Data Model
//===================================================
[System.Serializable]
public class APIUserAgentModel : BaseModel
{
    public DataModel data;

    [System.Serializable]
    public class DataModel
    {
        public string user_agent = "";
    }
}

//===================================================
//  Ocugine IP Data Model
//===================================================
[System.Serializable]
public class APIIPModel : BaseModel
{
    public DataModel data;

    [System.Serializable]
    public class DataModel
    {
        public string ip = "";
    }
}

//===================================================
//  Ocugine Device Data Model
//===================================================
[System.Serializable]
public class APIDeviceModel : BaseModel
{
    public DataModel data;

    [System.Serializable]
    public class DataModel
    {
        public string user_device = "";
    }
}