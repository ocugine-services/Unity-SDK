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
//  Ocugine SDK Authentication Models
//===================================================
/* Authentication Model */
[System.Serializable]
public class AuthenticationModel
{
    public double uid = 0;              // Authentication UID
    public bool is_auth = false;        // Authentication Status
    public string login = "";           // User Login
    public string token = "";           // User Token
    public string auth_key = "";        // User auth key after get link
    public string from = "";            // Authentication Method
    public double action_time = 0;      // Last Action Time (Unix)
    public double profile_uid = 0;      // Profile (Account) UID
}

/* Viewer Model */
[System.Serializable]
public class ViewerModel
{
    public double uid = 0;              // Viewer UID
    public string ip = "";              // Viewer IP
    public double profile_uid = 0;      // Profile (Account) UID
    public string user_agent = "";      // User Agent
    public string hash = "";            // User Hash
    public string location = "";        // User Location
    public string device = "";          // User Device
    public double time = 0;             // Last Action Time (Unix)
}

/* OAuth Model */
[System.Serializable]
public class OAuthModel : BaseModel
{
    public DataModel data;

    [System.Serializable]
    public class DataModel
    {
        public double app_id = 0;           // Project ID
        public string auth_url = "";        // Auth Link
        public string auth_key = "";        // Auth Key
        public double timeout = 0;          // UNIX Expire time
    }
}

/* OAuth Token Module */
[System.Serializable]
public class OAuthTokenModel : BaseModel
{
    public DataModel data;

    [System.Serializable]
    public class DataModel
    {
        public string access_token = "";    // Access token
        public string[] grants = null;      // UNIX Expire time
    }
}
