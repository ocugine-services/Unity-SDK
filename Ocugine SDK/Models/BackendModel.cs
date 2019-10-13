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
//  Ocugine Content Models
//===================================================
/* Content List */
[System.Serializable]
public class ContentListModel : BaseModel
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
            public string content_slug = "";
            public double time = 0;
        }
    }
}

/* Content Model */
[System.Serializable]
public class ContentModel : BaseModel
{
    public DataModel data;

    [System.Serializable]
    public class DataModel
    {
        // Модель инфо
        public InfoModel info;

        [System.Serializable]
        public class InfoModel
        {
            public double uid = 0;
            public double project_id = 0;
            public string content_slug = "";
            public double content_size;
            public string content_url;
            public double time = 0;
        }
    }
}

//===================================================
//  Ocugine Live Configs Models
//===================================================
/* Live Config Model */
[System.Serializable]
public class LiveConfigModel : BaseModel
{
    public enum ConfigType { Text = 0, Double = 1, JSON = 2 }

    public DataModel data;

    [System.Serializable]
    public class DataModel
    {
        public double uid = 0;
        public string project_id = "";
        public string name = "";
        public string value = "";
        public ConfigType type = 0;
        public double time = 0;
    }
}

/* Live Configs List Model */
[System.Serializable]
public class LiveConfingListModel
{
    public DataModel data;

    [System.Serializable]
    public class DataModel
    {
        public LiveConfigModel.DataModel[] list;
    }
}

//===================================================
//  Ocugine Cloud Saves Model
//===================================================
/* Cloud Saves List */
[System.Serializable]
public class CloudSavesListModel
{
    public DataModel data;

    [System.Serializable]
    public class DataModel
    {
        public CloudSaveModel.DataModel[] list;
    }
}

/* Cloud Saves Data */
[System.Serializable]
public class CloudSaveModel : BaseModel
{
    public DataModel data;

    [System.Serializable]
    public class DataModel
    {
        public double uid = 0;
        public double project_id = 0;
        public double profile_uid = 0;
        public string data = "";
        public double time = 0;
    }
}