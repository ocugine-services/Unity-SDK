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
//  Ocugine SDK State Model
//===================================================
[System.Serializable]
public class StateModel : BaseModel{
    public StateData data;
}

//===================================================
//  Ocugine SDK State Data
//===================================================
[System.Serializable]
public class StateData{
    public string version = "";         // API Version
    public double build = 0;            // API Build
    public string developer = "";       // API Developer
    public string url = "";             // API URL
    public bool online = false;         // API ONLINE STATE
}

//===================================================
//  Ocugine SDK API Status Data
//===================================================
[System.Serializable]
public class APIState{
    public int dashboard = 0;               // Dashboard State
    public APIServicesState services;       // Services State
    public APISDKState sdk;                 // SDK State
}
[System.Serializable]
public class APIServicesState{
    public APIServicesUsersState users;     // Users State
    public APIServicesAnalyticsState analytics; // Analytics State
    public APIServicesMarketingState marketing; // Marketing State
    public APIServicesAdsState ads;         // Ads State
    public APIServicesGamingState gaming;   // Game Services State
    public APIServicesCloudState cloud;     // Cloud Services State
    public APIServicesOfficeState office;   // Office Services State
    public APIServicesReportsState reports; // Reports Services State
    public APIServicesLocalizationState localization;   // Localization Services State
    public APIServicesOtherState other;     // Other Services State
}
[System.Serializable]
public class APIServicesUsersState{
    public int auth = 0;                    // Auth Module
    public int lists = 0;                   // Lists Module
    public int groups = 0;                  // Groups Module
    public int notifications = 0;           // Notifications Module
    public int support = 0;                 // Support Module
    public int chating = 0;                 // Chating Module
    public int reviews = 0;                 // Reviews Module
    public int policy = 0;                  // Policy Module
    public int settings = 0;                // Settings Module
}
[System.Serializable]
public class APIServicesAnalyticsState{
    public int general = 0;                 // General Analytics Module
    public int user_tracking = 0;           // Users Tracking Module
    public int events = 0;                  // Events Module
    public int funnels = 0;                 // Funnels Module
}
[System.Serializable]
public class APIServicesMarketingState{
    public int tools = 0;                   // General Marketing Tools
    public int promo = 0;                   // Promo Tools
    public int ab = 0;                      // AB Tools
    public int aso = 0;                     // ASO Tools
}
[System.Serializable]
public class APIServicesAdsState{
    public int exchange = 0;                // Traffic Exchange Module
    public int ads = 0;                     // Ads Module
}
[System.Serializable]
public class APIServicesGamingState{
    public int leaderboards = 0;            // Leaderboards Module
    public int achivements = 0;             // Achivements Module
    public int teams = 0;                   // Teams Module
    public int store = 0;                   // Store Module
    public int multiplayer = 0;             // Multiplayer Module
    public int security = 0;                // Security Module
    public int social = 0;                  // Social Module
    public int cross_play = 0;              // Cross Play Module
    public int missions = 0;                // Missions Module
}
[System.Serializable]
public class APIServicesCloudState{
    public int content = 0;                 // Content Delivery Module
    public int databases = 0;               // Databases Module
    public int backend = 0;                 // BaaS Module
    public int testing = 0;                 // Testing Module
    public int build = 0;                   // Cloud Build Module
    public int reporting = 0;               // Reporting Module
    public int perfomance = 0;              // Perfomance Module
}
[System.Serializable]
public class APIServicesOfficeState{
    public int chating = 0;                 // Office Chating Module
    public int notifications = 0;           // Notifications Module
    public int events = 0;                  // Events Module
    public int grants = 0;                  // Office Grants Module
    public int documents = 0;               // Documents Module
    public int payments = 0;                // Payments Module
    public int settings = 0;                // Office Settings Module
}
[System.Serializable]
public class APIServicesReportsState{
    public int payouts = 0;                 // Payouts Reporting Module
    public int errors = 0;                  // Errors Reporting Module
    public int usage = 0;                   // Usage Reporting Module
    public int perfomance = 0;              // Perfomance Reporting Module
    public int custom = 0;                  // Custom Reporting Module
}
[System.Serializable]
public class APIServicesLocalizationState{
    public int manager = 0;                 // Localization Manager Module
    public int languages = 0;               // Languages Module
}
[System.Serializable]
public class APIServicesOtherState{
    public int integrations = 0;            // Integrations Module
    public int sdk = 0;                     // SDK Module
    public int launcher_builder = 0;        // Launcher Builder Module
    public int api_manager = 0;             // API Manager Module
    public int engine = 0;                  // Engine Module
}
[System.Serializable]
public class APISDKState{
    public int net = 0;                     // NET SDK State
    public int unity = 0;                   // Unity SDK
    public int javascript = 0;              // JavaScript SDK State
    public int unreal = 0;                  // Unreal SDK State
    public int construct = 0;               // Construct SDK State
    public int phaser = 0;                  // Phaser SDK State
    public int gmaker = 0;                  // Game Maker SDK State
    public int xenko = 0;                   // Xenko Engine SDK State
    public int cryengine = 0;               // Cryengine SDK State
    public int ocugine = 0;                 // Ocugine SDK State
}

