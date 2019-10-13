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
//  SDK Settings Serializable Class
//===================================================
[System.Serializable]
public class SDKSettings{
    [Tooltip("SDK Platform")] public string platform = ""; // SDK Platform
    [Tooltip("Set SDK Language code. For example \"en\"")] public string language = "";    // SDK Language
    [Tooltip("Choose modules to preload for SDK")] public SDKModules modules;      // SDK Modules
    [Tooltip("Choose timeout for UI module")] public int auth_timeout = 30000; // Auth timeout
}

//===================================================
//  SDK Modules Serializable Class
//===================================================
[System.Serializable]
public class SDKModules{
    [Tooltip("Ocugine SDK Authentication Module")]
    public bool Auth = true;            // Authentication Module
    [Tooltip("Ocugine SDK Analytics Module")]
    public bool Analytics = true;       // Analytics Module
    [Tooltip("Ocugine SDK Game Services Module")]
    public bool GameServices = true;    // Game Services
    [Tooltip("Ocugine SDK Monetization Module")]
    public bool Payments = true;        // Monetization
    [Tooltip("Ocugine SDK PUSH-Notifications Module")]
    public bool Notifications = true;   // PUSH-Notifications
    [Tooltip("Ocugine SDK Marketing Tools Module")]
    public bool Marketing = true;       // Marketing
    [Tooltip("Ocugine SDK Ads & Traffic Exchange Module")]
    public bool Ads = true;             // Ads & Traffic Exchange
    [Tooltip("Ocugine SDK Backend-as-a-Service Module")]
    public bool Backend = true;         // Backend Services
    [Tooltip("Ocugine SDK Reporting Module")]
    public bool Reports = true;         // Reports
    [Tooltip("Ocugine SDK Perfomance Module")]
    public bool Performance = true;      // Perfomance
    [Tooltip("Ocugine SDK Back Office Module")]
    public bool Back_Office = true;     // Back Office
    [Tooltip("Ocugine SDK Localization Module")]
    public bool Localization = true;    // Localization
    [Tooltip("Ocugine SDK Users Module")]
    public bool Users = true;              // UI
    [Tooltip("Ocugine SDK UI Module")]
    public bool UI = true;              // UI
}

//===================================================
//  Application Settings Serializable Class
//===================================================
[System.Serializable]
public class AppSettings{
    [Header("Application Settings")]
    [Tooltip("Type your Ocugine Project ID")]
    public double app_id = 0;           // Application ID
    [Tooltip("Type your Ocugine Project Client Key")]
    public string app_key = "";         // Application Key
    [HideInInspector]
    public string app_name = "";        // Application Name
    [HideInInspector]
    public string app_desc = "";        // Application Desc
    [HideInInspector]
    public string app_version = "";     // Application Version
    [Header("Automatic Analytics & Reporting")]
    [Tooltip("Send analytic events automatically")]
    public bool auto_analytics_events = true;
    [Tooltip("Send crash reports automatically")]
    public bool auto_crash_reporting = true;
}