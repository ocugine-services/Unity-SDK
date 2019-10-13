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
//  @developer      CodeBits Interactive
//  @version        0.4.1
//  @build          412
//  @url            https://ocugine.pro/
//  @docs           https://docs.ocugine.pro/
//  @license        MIT
//===================================================
//===================================================
//  Ocugine SDK Flags Models
//===================================================
/* Flag Model */
[System.Serializable]
public class FlagModel : BaseModel
{
    public DataModel data;

    [System.Serializable]
    public class DataModel
    {
        public string flag = "";
        public bool exists = false;
        public double time = 0;
        public double counter = 0;
    }
}

/* Flags List Model */
[System.Serializable]
public class FlagsListModel : BaseModel
{
    public DataModel data;

    [System.Serializable]
    public class DataModel
    {
        public bool complete = false;
        public string[] list;
    }
}

/* Latest Flags List Model */
[System.Serializable]
public class LatestFlagsListModel : BaseModel
{
    public DataModel data;

    [System.Serializable]
    public class DataModel
    {
        public ListModel[] list;

        [System.Serializable]
        public class ListModel
        {
            public double uid = 0;
            public double project_id = 0;          
            public double profile_uid = 0;
            public string flag = "";
            public string viewer = "";         
            public double time = 0;
        }
    }
}