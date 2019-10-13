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
//  Ocugine Languages Models
//===================================================
/* Info Model */
[System.Serializable]
public class LanguageModel : BaseModel
{
    public DataModel data;

    [System.Serializable]
    public class DataModel
    {
        public string uid = "";            // Locale language id
        public string code = "";           // Locale language short name
        public string name = "";           // Locale language name
        public string time = "";           // Locale language last edit time
        public string project_id = "";     // Project id
    }
}

/* List Model */
[System.Serializable]
public class LanguagesListModel : BaseModel
{
    public DataModel data;

    [System.Serializable]
    public class DataModel
    {
        public LanguageModel.DataModel[] list;
    }
}

//===================================================
//  Ocugine Locales Models
//===================================================
/* Locale Model */
[System.Serializable]
public class LocaleModel : BaseModel
{
    public DataModel data;

    [System.Serializable]
    public class DataModel
    {
        public string uid = "";            // Locale id
        public string code = "";           // Locale short name
        public string language = "";       // Locale language
        public string value = "";          // Locale value
        public string project_id = "";     // Project id
    }
}

/* Locales List Model */
[System.Serializable]
public class LocalesListModel : BaseModel
{
    public DataModel data;

    [System.Serializable]
    public class DataModel
    {
        public ListModel[] list;

        [System.Serializable]
        public class ListModel
        {
            public string lang_name = "";
            public LocaleModel.DataModel[] locales;
        }
    }
}