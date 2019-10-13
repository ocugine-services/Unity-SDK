using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System.Net.Http;

//===================================================
//  Ocugine SDK
//  Sofware Development Kit developed specially for
//  Ocugine Services. With this library you can
//  use all features of Ocugine Services
//
//  @name           Ocugine SDK
//  @developer      Ocugine Platform
//  @version        0.4.1
//  @build          412
//  @url            https://ocugine.pro/
//  @docs           https://docs.ocugine.pro/
//  @license        MIT
//===================================================

// Todo: Remove warning disabletors
#pragma warning disable CS0618 // For usind deprecated method WWW

//===================================================
//  Ocugine Unity SDK General Class
//===================================================
[AddComponentMenu("Ocugine SDK/SDK Manager")]
public class OcugineSDK : MonoBehaviour{
    // General Class Constants
    public const bool SSL = true;                   // SSL
    public const string PROTOCOL = "https://";      // Requests Protocol
    public const string SERVER = "cp.ocugine.pro";  // Server URL   
    public const string API_GATE = "/api/";         // API Gateway
    public const string VERSION = "0.4.0a";         // Library Version
    public const double BUILD = 401;                // Library Build

    // Public Variables
    [HideInInspector] public static OcugineSDK instance = null;                 // Ocugine SDK Instance
    [HideInInspector] public bool initialized = false;                          // Ocugine SDK Initialization Status
    [Tooltip("Ocugine Application Settings")] public AppSettings application;   // Ocugine SDK Application Settings
    [Tooltip("Ocugine SDK Settings")] public SDKSettings settings;              // Ocugine SDK Settings
    [Tooltip("Ocugine Debug")] public bool debug_mode = false;                  // Ocugine Debug

    // Modules
    [HideInInspector] public Ocugine_Auth auth;                   // Ocugine Auth Module
    [HideInInspector] public Ocugine_Analytics analytics;         // Ocugine Analytics
    [HideInInspector] public Ocugine_GameServices game;           // Game Services
    [HideInInspector] public Ocugine_Monetization monetization;   // Monetization Services
    [HideInInspector] public Ocugine_Notifications notifications; // Notifications Services
    [HideInInspector] public Ocugine_Marketing marketing;         // Marketing Services
    [HideInInspector] public Ocugine_Ads ads;                     // Ads Services
    [HideInInspector] public Ocugine_Backend backend;             // Backend Services
    [HideInInspector] public Ocugine_Reports reports;             // Reporting Services
    [HideInInspector] public Ocugine_Localization locale;         // Locale Services
    [HideInInspector] public Ocugine_Users users;                 // Users Module
    [HideInInspector] public Ocugine_UI ui;                       // UI Module
    [HideInInspector] public Ocugine_Utils utils;                 // SDK Utils

    // Private Variables
    private static string _sdk_settings_path;   // Ocugine SDK Settings Path
    private static string _app_settings_path;   // Ocugine SDK Application Settings Path  

    // Public Class Params
    public const string STATE_OBJECT = "state";    // State Object
    public const string OAUTH_OBJECT = "oauth";
    public const string USERS_OBJECT = "users";
    public const string SETTINGS_OBJECT = "api_settings";
    public const string LOCALE_OBJECT = "localization"; 
    public const string ANALYTICS_OBJECT = "analytics";
    public const string GAMING_OBJECT = "gaming";
    public const string CLOUD_OBJECT = "cloud";
    public const string REPORTS_OBJECT = "reports";
    public const string UTILS_OBJECT = "utils";

    //============================================================
    //  @class      OcugineSDK
    //  @method     getServerURL()
    //  @type       Public
    //  @usage      Get Server URL
    //  @return     (string) url - server URL
    //============================================================
    public string getServerURL(){
        return PROTOCOL + SERVER + "/";
    }

    //============================================================
    //  @class      OcugineSDK
    //  @method     Awake()
    //  @type       Internal Void
    //  @usage      Call before scene initialized
    //============================================================
    void Awake(){
        // Set Settings Paths
        _sdk_settings_path = Application.persistentDataPath + "/sdk_settings.ocf"; // SDK Settings
        _app_settings_path = Application.persistentDataPath + "/app_settings.ocf"; // App Settings

        // Check Instance of Gamebase
        if (instance == null){ // Instance not found
            instance = this; // Set this object to instance
        } else if (instance == this){ // Has instance
            Destroy(gameObject); // Destroy this Gamebase
        }

        // Cant destroy this object when load another
        // scene for singleton working state
        DontDestroyOnLoad(gameObject); // Don't Destroy Gamebase

        // Initialize Settings
        _initializeSettings();  // Start Ocugine Initialization
        initialized = true;     // Set Initialized Flag
        if(debug_mode) Debug.Log("Ocugine SDK Successfully initialized"); // Write Log  
    }

    //============================================================
    //  @class      OcugineSDK
    //  @method     OnEnable()
    //  @type       Internal Void
    //  @usage      Component Enabled Callback
    //============================================================
    void OnEnable(){
        Application.logMessageReceived += HandleLog;
    }

    //============================================================
    //  @class      OcugineSDK
    //  @method     OnDisable()
    //  @type       Internal Void
    //  @usage      Component Disabled Callback
    //============================================================
    void OnDisable(){
        Application.logMessageReceived -= HandleLog;
    }

    //============================================================
    //  @class      OcugineSDK
    //  @method     HandleLog()
    //  @type       Private Void
    //  @usage      Unity Log Handler
    //============================================================
    private void HandleLog(string logString, string stackTrace, LogType type){
        if (application.auto_crash_reporting && type!=LogType.Warning && type!=LogType.Log && this.reports!=null){ // Auto Crash reporting
            bool critical = (type == LogType.Error || type == LogType.Assert) ? false : true; // Critical Error
            string message = logString + ". \n\n Stack Trace: " + stackTrace; // Log Message
            string code = "UERROR:";
            if (type == LogType.Error) code = code + " Error";
            if (type == LogType.Assert) code = code + " Assert";
            if (type == LogType.Exception) code = code + " Exception";
            this.reports.SendErrorReport("Unity Error", code, message, critical, settings.platform); // Send Error Report
        }
    }

    //============================================================
    //  @class      OcugineSDK
    //  @method     _initializeSettings()
    //  @type       Private Void
    //  @usage      Initialize SDK Settings
    //============================================================
    private void _initializeSettings(){
        // Load SDK Settings
        string _sdk_cfg = LoadSDKConfigs(_sdk_settings_path, true); // Load
        if(_sdk_cfg!=null) settings = JsonUtility.FromJson<SDKSettings>(_sdk_cfg); // Deserialize SDK Configs

        // Load App Settings
        string _app_cfg = LoadSDKConfigs(_app_settings_path, true); // Load
        if(_app_cfg != null) application = JsonUtility.FromJson<AppSettings>(_app_cfg); // Deserialize Application Configs

        // Initialize Platform
        if (settings.platform.Length < 1){ // Platform is not defined
            settings.platform = "other";
            if (Application.platform == RuntimePlatform.Android)
                settings.platform = "android";
            if (Application.platform == RuntimePlatform.IPhonePlayer) settings.platform = "ios";
            if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer)
                settings.platform = "mac";
            if (Application.platform == RuntimePlatform.FlashPlayer || Application.platform == RuntimePlatform.WebGLPlayer)
                settings.platform = "web";
            if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
                settings.platform = "windows";
            if (Application.platform == RuntimePlatform.XboxOne)
                settings.platform = "xbox";
            if (Application.platform == RuntimePlatform.LinuxEditor || Application.platform == RuntimePlatform.LinuxPlayer)
                settings.platform = "linux";
        }

        // Initialize Modules
        _initializeModules();
    }

    //============================================================
    //  @class      OcugineSDK
    //  @method     _initializeModules()
    //  @type       Private Void
    //  @usage      Initialize SDK Modules
    //============================================================
    private void _initializeModules(){

        // Utils Module
        this.gameObject.AddComponent<Ocugine_Utils>(); // Initialize Component
        utils = this.gameObject.GetComponent<Ocugine_Utils>(); // Set Component Link
        utils.InitializeModule(this.gameObject.GetComponent<OcugineSDK>()); // Initialize Module
        if (debug_mode)
            Debug.Log("Ocugine SDK: Utils Module successfully initialized."); // Write Log

        // Authentication Module
        if (settings.modules.Auth){
            this.gameObject.AddComponent<Ocugine_Auth>(); // Initialize Component
            auth = this.gameObject.GetComponent<Ocugine_Auth>(); // Set Component Link
            auth.InitializeModule(this.gameObject.GetComponent<OcugineSDK>()); // Initialize Module
            if (debug_mode)
                Debug.Log("Ocugine SDK: Authentication Module successfully initialized."); // Write Log
        }

        // Analytics Module
        if (settings.modules.Analytics){
            this.gameObject.AddComponent<Ocugine_Analytics>(); // Initialize Component
            analytics = this.gameObject.GetComponent<Ocugine_Analytics>(); // Set Component Link
            analytics.InitializeModule(this.gameObject.GetComponent<OcugineSDK>()); // Initialize Module
            if (debug_mode)
                Debug.Log("Ocugine SDK: Analytics Module successfully initialized."); // Write Log
        }

        // Ads Module
        if (settings.modules.Ads){
            this.gameObject.AddComponent<Ocugine_Ads>(); // Initialize Component
            ads = this.gameObject.GetComponent<Ocugine_Ads>(); // Set Component Link
            ads.InitializeModule(this.gameObject.GetComponent<OcugineSDK>()); // Initialize Module
            if (debug_mode)
                Debug.Log("Ocugine SDK: Ads Module successfully initialized."); // Write Log
        }

        // Notifications Module
        if (settings.modules.Notifications){
            this.gameObject.AddComponent<Ocugine_Notifications>(); // Initialize Component
            notifications = this.gameObject.GetComponent<Ocugine_Notifications>(); // Set Component Link
            notifications.InitializeModule(this.gameObject.GetComponent<OcugineSDK>()); // Initialize Module
            if (debug_mode)
                Debug.Log("Ocugine SDK: Notifications Module successfully initialized."); // Write Log
        }

        // Payments Module
        if (settings.modules.Payments){
            this.gameObject.AddComponent<Ocugine_Monetization>(); // Initialize Component
            monetization = this.gameObject.GetComponent<Ocugine_Monetization>(); // Set Component Link
            monetization.InitializeModule(this.gameObject.GetComponent<OcugineSDK>()); // Initialize Module
            if (debug_mode)
                Debug.Log("Ocugine SDK: Payments Module successfully initialized."); // Write Log
        }

        // Game Services Module
        if (settings.modules.GameServices){
            this.gameObject.AddComponent<Ocugine_GameServices>(); // Initialize Component
            game = this.gameObject.GetComponent<Ocugine_GameServices>(); // Set Component Link
            game.InitializeModule(this.gameObject.GetComponent<OcugineSDK>()); // Initialize Module
            if (debug_mode)
                Debug.Log("Ocugine SDK: Game Services Module successfully initialized."); // Write Log
        }

        // Marketing Module
        if (settings.modules.Marketing){
            this.gameObject.AddComponent<Ocugine_Marketing>(); // Initialize Component
            marketing = this.gameObject.GetComponent<Ocugine_Marketing>(); // Set Component Link
            marketing.InitializeModule(this.gameObject.GetComponent<OcugineSDK>()); // Initialize Module
            if (debug_mode)
                Debug.Log("Ocugine SDK: Marketing Module successfully initialized."); // Write Log
        }

        // Backend Module
        if (settings.modules.Backend){
            this.gameObject.AddComponent<Ocugine_Backend>(); // Initialize Component
            backend = this.gameObject.GetComponent<Ocugine_Backend>(); // Set Component Link
            backend.InitializeModule(this.gameObject.GetComponent<OcugineSDK>()); // Initialize Module
            if (debug_mode)
                Debug.Log("Ocugine SDK: Backend Module successfully initialized."); // Write Log
        }

        // Reports Module
        if (settings.modules.Reports){
            this.gameObject.AddComponent<Ocugine_Reports>(); // Initialize Component
            reports = this.gameObject.GetComponent<Ocugine_Reports>(); // Set Component Link
            reports.InitializeModule(this.gameObject.GetComponent<OcugineSDK>()); // Initialize Module
            if (debug_mode)
                Debug.Log("Ocugine SDK: Reports Module successfully initialized."); // Write Log
        }

        // Localization Module
        if (settings.modules.Localization){
            this.gameObject.AddComponent<Ocugine_Localization>(); // Initialize Component
            locale = this.gameObject.GetComponent<Ocugine_Localization>(); // Set Component Link
            locale.InitializeModule(this.gameObject.GetComponent<OcugineSDK>()); // Initialize Module
            if (debug_mode)
                Debug.Log("Ocugine SDK: Localization Module successfully initialized."); // Write Log
        }

        // Users Module
        if (settings.modules.Users)
        {
            this.gameObject.AddComponent<Ocugine_Users>(); // Initialize Component
            users = this.gameObject.GetComponent<Ocugine_Users>(); // Set Component Link
            users.InitializeModule(this.gameObject.GetComponent<OcugineSDK>()); // Initialize Module
            if (debug_mode)
                Debug.Log("Ocugine SDK: Users Module successfully initialized."); // Write Log
        }

        // UI Module
        if (settings.modules.UI){
            this.gameObject.AddComponent<Ocugine_UI>(); // Initialize Component
            ui = this.gameObject.GetComponent<Ocugine_UI>(); // Set Component Link
            ui.InitializeModule(this.gameObject.GetComponent<OcugineSDK>()); // Initialize Module
            if (debug_mode)
                Debug.Log("Ocugine SDK: UI Module successfully initialized."); // Write Log
        }
        
    }

    //============================================================
    //  @class      OcugineSDK
    //  @method     LoadSDKConfigs()
    //  @type       Public Void
    //  @usage      Load Configurations
    //  @args       (string) path - Path to Config Files
    //              (bool) encoded - Config Files Encoded?
    //  @return     (string/null) - Configuration Data
    //============================================================
    public string LoadSDKConfigs(string path, bool encoded = false){
        // Check File Exists
        if (!File.Exists(path)){ // No File
            return null;
        }

        // Load File
        string _data = File.ReadAllText(path); // Read All Text

        // Decode File
        if (encoded){ // Only For Encoded Files
            byte[] _decodedBytes = Convert.FromBase64String(_data); // Get Decoded Bytes
            string _decodedData = Encoding.UTF8.GetString(_decodedBytes); // Get Decoded Text
            return _decodedData; // Return Decoded Data
        }

        // Return Data
        if (debug_mode)
            Debug.Log("Ocugine SDK Settings successfully loaded: "+_data); 
        return _data;
    }

    //============================================================
    //  @class      OcugineSDK
    //  @method     SaveSDKConfigs()
    //  @type       Public Void
    //  @usage      Load Configurations
    //  @args       (string) path - Path to Config Files
    //              (string) data - Data to Save
    //              (bool) encoded - Encoding File Flag
    //============================================================
    public void SaveSDKConfigs(string path, string data, bool encoded = false){
        // Need Encoded
        if (encoded){ // Only if flag = true 
            byte[] _bytesToEncode = Encoding.UTF8.GetBytes(data); // Get Bytes to Encode
            data = Convert.ToBase64String(_bytesToEncode); // Encode Text
        }

        // Save File
        File.WriteAllText(path, data); // Save datas to file
        if (debug_mode)
            Debug.Log("Ocugine SDK Settings successfully saved to: " + path);
    }

    //============================================================
    //  @class      OcugineSDK
    //  @method     Start()
    //  @type       Internal Void
    //  @usage      Call when scene initialized
    //============================================================
    void Start(){
        /* Automatic Analytics */
        if (application.auto_analytics_events && this.analytics!=null){ // Enabled Auto Analytics
            this.analytics.SetUserFlag("first_open"); // Set User Flag
            this.analytics.SetUserFlag("session_open"); // Set User Flag
            this.analytics.SetUserFlag("session_update"); // Set User Flag
            if (debug_mode)
                Debug.Log("Ocugine SDK Analytics: Startup Events successfully sended.");
        }
    }

    //============================================================
    //  @class      OcugineSDK
    //  @method     Start()
    //  @type       Internal Void
    //  @usage      Call every tick
    //============================================================
    void Update(){
    }

    //============================================================
    //  @class      OcugineSDK
    //  @method     goToDocsPage()
    //  @type       Internal Void
    //  @usage      Go to documentation Page
    //============================================================
    [ContextMenu("Open Documentation")]
    void GoToDocsPage(){
        Application.OpenURL("https://help.ocugine.pro/en/ocugine-services/sdk-documentation/unity-sdk/");
        if (debug_mode)
            Debug.Log("Ocugine SDK: Documentation successfully loaded");
    }
}

//===================================================
//  Ocugine Unity SDK Authentication Class
//===================================================
[AddComponentMenu("Ocugine SDK/Authentication Component")]
[RequireComponent(typeof(OcugineSDK))]
public class Ocugine_Auth : MonoBehaviour{
    // Public Params
    public AuthenticationModel credentials = new AuthenticationModel();     // Authentication Credentials
    public ProfileModel profile = new ProfileModel();                       // Authenticated User Profile
    public ViewerModel viewer = new ViewerModel();                          // Authenticated User Viewer Model
    public bool test = true;

    // Private Params
    private OcugineSDK sdk_instance = null;     // Ocugine SDK Instance

    //============================================================
    //  @class      Ocugine_Auth
    //  @method     InitializeModule()
    //  @type       Public Void
    //  @usage      Initialize Module
    //  @args       (OcugineSDK) instance - Parent Instance Component
    //============================================================
    public void InitializeModule(OcugineSDK instance){
        sdk_instance = instance; // Set Ocugine SDK Instance
        credentials.token = sdk_instance.LoadSDKConfigs(Application.persistentDataPath + "/userdata.o", true);
        if(credentials.token == null || credentials.token == "") {
            // Do literally nothing because token is clean
        }else{
            credentials.is_auth = true;      
        }
    }

    //============================================================
    //  @class      Auth
    //  @method     GetLink()
    //  @type       Static Async Void
    //  @usage      Get oauth link
    //  @args       (string[] / string) grants - Grants for project
    //              (void) complete - Complete Callback
    //              (void) error - Error Callback
    //  @return     (bool) status
    //============================================================
    public delegate void OnGetLinkComplete(string data);
    public delegate void OnGetLinkError(string code);
    public void GetLink(OnGetLinkComplete complete, OnGetLinkError error = null, string[] grants = null){
        string stringgrants = "";
        if (grants != null){
            foreach (string s in grants)
                stringgrants += $"{s},";
            if (stringgrants.Contains("all"))
                stringgrants = "all";
        } else {
            stringgrants = "all";
        }
        
        WWWForm formContent = new WWWForm(); // Create WWW Form
        formContent.AddField("app_id", $"{sdk_instance.application.app_id}");      // Add Language
        formContent.AddField("app_key", $"{sdk_instance.application.app_key}");    // App Key
        formContent.AddField("grants", $"{stringgrants}");                         // Permissions
        formContent.AddField("lang", $"{sdk_instance.settings.language}");         // Language

        // Send Request
        StartCoroutine(sdk_instance.utils.SendRequest(OcugineSDK.PROTOCOL + OcugineSDK.SERVER + OcugineSDK.API_GATE + OcugineSDK.OAUTH_OBJECT + "/get_link", formContent, ((string response) => { // All Right
            OAuthModel state = JsonUtility.FromJson<OAuthModel>(response); // Get State Model from Response
            complete(state.data.auth_url); // Return Complete
        }), ((string msg) => { // Error
            if (error != null)
                error(msg); // Send Error
        })));
    }
    // With a string
    public void GetLink(OnGetLinkComplete complete, OnGetLinkError error = null, string grants = ""){
        WWWForm formContent = new WWWForm(); // Create WWW Form
        formContent.AddField("app_id", $"{sdk_instance.application.app_id}");      // Add Language
        formContent.AddField("app_key", $"{sdk_instance.application.app_key}");    // App Key
        formContent.AddField("grants", $"{(grants.ToLower().Contains("all") ? "all" : grants.TrimEnd(','))}");                         // Permissions
        formContent.AddField("lang", $"{sdk_instance.settings.language}");         // Language

        // Send Request
        StartCoroutine(sdk_instance.utils.SendRequest(OcugineSDK.PROTOCOL + OcugineSDK.SERVER + OcugineSDK.API_GATE + OcugineSDK.OAUTH_OBJECT + "/get_link", formContent, ((string response) => { // All Right
            OAuthModel state = JsonUtility.FromJson<OAuthModel>(response); // Get State Model from Response
            credentials.auth_key = state.data.auth_key; // Set the authentication key for get token
            complete(state.data.auth_url); // Return Complete
        }), ((string msg) => { // Error
            if (error != null)
                error(msg); // Send Error
        })));
    }

    //============================================================
    //  @class      Auth
    //  @method     GetToken()
    //  @type       Static Void
    //  @usage      Get user token
    //              (void) complete - Complete Callback
    //              (void) error - Error Callback
    //  @return     none
    //============================================================
    public delegate void OnAPIInfoComplete(OAuthTokenModel data); // Returns OAuthTokenModel
    public delegate void OnAPIInfoError(string code); // Returns error code
    public void GetToken(OnAPIInfoComplete complete, OnAPIInfoError error = null, bool update = false){
        if (credentials.token != null && credentials.token != "" && update == false) // Write info about token
        {
            GetGrants(
                (OAuthTokenModel o) => 
                {
                    Debug.Log("[DEBUG] Token succesfully loaded");
                    complete(new OAuthTokenModel { data = new OAuthTokenModel.DataModel { access_token = credentials.token } });
                }, 
                (string s) => 
                {
                    Debug.Log("[DEBUG] Token not loaded\n" + s);
                    if (error != null)
                        error(s);
                });           
        }
        else
        {
            WWWForm formContent = new WWWForm(); // Create WWW Form
            formContent.AddField("app_id", $"{sdk_instance.application.app_id}");      // Add Language
            formContent.AddField("app_key", $"{sdk_instance.application.app_key}");    // App Key
            formContent.AddField("lang", $"{sdk_instance.settings.language}");         // Language
            formContent.AddField("auth_key", $"{credentials.auth_key}");               // Auth key

            // Send Request
            StartCoroutine(sdk_instance.utils.SendRequest(OcugineSDK.PROTOCOL + OcugineSDK.SERVER + OcugineSDK.API_GATE + OcugineSDK.OAUTH_OBJECT + "/get_token", formContent, ((string response) => { // All Right          
                OAuthTokenModel state = JsonUtility.FromJson<OAuthTokenModel>(response); // Deserialize Object
                credentials.token = state.data.access_token;
                sdk_instance.SaveSDKConfigs(Application.persistentDataPath + "/userdata.o", state.data.access_token, true); // Write info about token
                credentials.is_auth = true;
                complete(state); // Return Data 
            }), ((string msg) => { // Error
                if(error!=null) error(msg); // Send Error
            }) // NoDebug status
            ));
        }      
    }

    //============================================================
    //  @class      Auth
    //  @method     Logout()
    //  @type       Static Void
    //  @usage      Logout by user token
    //              (void) complete - Complete Callback
    //              (void) error - Error Callback
    //  @return     none
    //============================================================
    public delegate void OnLogoutComplete(BaseModel data);
    public delegate void OnLogoutError(string code);
    public void Logout(OnLogoutComplete complete = null, OnLogoutError error = null){
        if (sdk_instance.auth.credentials.token == "" || sdk_instance.auth.credentials.is_auth == false)
        {
            sdk_instance.auth.credentials.token = ""; sdk_instance.auth.credentials.is_auth = false;
            switch (sdk_instance.settings.language)
            {
                case "RU": { error("Ошибка деавторизации, приложение не авторизовано"); } break;
                default: { error("Logout error, application not authorized"); } break;
            }
            return;
        }

        WWWForm formContent = new WWWForm(); // Create WWW Form
        formContent.AddField("access_token", $"{sdk_instance.auth.credentials.token}");      // Add Language

        // Send Request
        StartCoroutine(sdk_instance.utils.SendRequest(OcugineSDK.PROTOCOL + OcugineSDK.SERVER + OcugineSDK.API_GATE + OcugineSDK.OAUTH_OBJECT + "/logout", formContent, ((string response) => { // All Right          
            sdk_instance.auth.credentials.token = "";
            sdk_instance.auth.credentials.is_auth = false;
            sdk_instance.SaveSDKConfigs(Application.persistentDataPath + "/userdata.o", "", true); // Write info about token
            BaseModel state = JsonUtility.FromJson<BaseModel>(response);
            if(complete!=null) complete(state);
        }), ((string msg) => { // Error
            sdk_instance.auth.credentials.token = "";
            sdk_instance.auth.credentials.is_auth = false;
            sdk_instance.SaveSDKConfigs(Application.persistentDataPath + "/userdata.o", "", true); // Write info about token
            if(error!=null) error(msg); // Send Error
        })));        
    }

    //============================================================
    //  @class      Auth
    //  @method     GetGrants()
    //  @type       Static Void
    //  @usage      Get application grants
    //              (void) complete - Complete Callback
    //              (void) error - Error Callback
    //  @return     none
    //============================================================
    public void GetGrants(OnAPIInfoComplete complete, OnAPIInfoError error = null)
    {
        if (sdk_instance.auth.credentials.token == "" || sdk_instance.auth.credentials.is_auth == false)
        {
            sdk_instance.auth.credentials.token = ""; sdk_instance.auth.credentials.is_auth = false;
            switch (sdk_instance.settings.language)
            {
                case "RU": { error("Ошибка, приложение не авторизовано"); } break;
                default: { error("Error, application not authorized"); } break;
            }
            return;
        }

        WWWForm formContent = new WWWForm(); // Create WWW Form
        formContent.AddField("app_id", $"{sdk_instance.application.app_id}");      // Add Language
        formContent.AddField("app_key", $"{sdk_instance.application.app_key}");    // App Key       
        formContent.AddField("access_token", $"{sdk_instance.auth.credentials.token}");      // Add Language

        // Send Request
        StartCoroutine(sdk_instance.utils.SendRequest(OcugineSDK.PROTOCOL + OcugineSDK.SERVER + OcugineSDK.API_GATE + OcugineSDK.OAUTH_OBJECT + "/get_grants", formContent,
        ((string data) => { // Response
            OAuthTokenModel state = JsonUtility.FromJson<OAuthTokenModel>(data); // Deserialize Object
            complete(state); // Return Data                       
        }),
        ((string code) => { // Error
            if(error!=null) error(code);
        })));
    }
}

//===================================================
//  Ocugine Unity SDK Analytics Class
//===================================================
[AddComponentMenu("Ocugine SDK/Analytics Component")]
[RequireComponent(typeof(OcugineSDK))]
public class Ocugine_Analytics : MonoBehaviour{
    // Private Params
    private OcugineSDK sdk_instance = null;     // Ocugine SDK Instance

    //============================================================
    //  @class      Ocugine_Analytics
    //  @method     InitializeModule()
    //  @type       Public Void
    //  @usage      Initialize Module
    //  @args       (OcugineSDK) instance - Parent Instance Component
    //============================================================
    public void InitializeModule(OcugineSDK instance){
        sdk_instance = instance; // Set Ocugine SDK Instance
    }

    //============================================================
    //  @class      Ocugine_Analytics
    //  @method     UpdateRetention()
    //  @type       Static Void
    //  @usage      Update user retention in app
    //  @args       (void) complete - Complete Callback
    //              (void) error - Error Callback
    //============================================================
    public delegate void OnUpdateRetentionSuccess(BaseModel data); // Returns BaseModel
    public delegate void OnUpdateRetentionError(string code); // Returns error code
    public void UpdateRetention(OnUpdateRetentionSuccess complete = null, OnUpdateRetentionError error = null){     
        WWWForm formContent = new WWWForm(); // Create WWW Form
        formContent.AddField("app_id", $"{sdk_instance.application.app_id}");      // Add Language
        formContent.AddField("app_key", $"{sdk_instance.application.app_key}");    // App Key
        if (sdk_instance.auth != null) formContent.AddField("access_token", $"{sdk_instance.auth.credentials.token}"); // Token  

        // Send Request
        StartCoroutine(sdk_instance.utils.SendRequest(OcugineSDK.PROTOCOL + OcugineSDK.SERVER + OcugineSDK.API_GATE + OcugineSDK.ANALYTICS_OBJECT + "/update_retention", formContent,
        ((string data) => { // Response
            BaseModel state = JsonUtility.FromJson<BaseModel>(data); // Deserialize Object
            if(complete!=null) complete(state); // Return Data                       
        }),
        ((string code) => { // Error
            if(error!=null) error(code);
        })));
    }

    //============================================================
    //  @class      Ocugine_Analytics
    //  @method     GetAvailableFlags()
    //  @type       Static Void
    //  @usage      Get available flags
    //  @args       (void) complete - Complete Callback
    //              (void) error - Error Callback
    //============================================================
    public delegate void OnGetAvailableFlagsSuccess(FlagsListModel data); // Returns FlagsListModel
    public delegate void OnGetAvailableFlagsError(string code); // Returns error code
    public void GetAvailableFlags(OnGetAvailableFlagsSuccess complete = null, OnGetAvailableFlagsError error = null){
        WWWForm formContent = new WWWForm(); // Create WWW Form
        formContent.AddField("app_id", $"{sdk_instance.application.app_id}");      // Add Language
        formContent.AddField("app_key", $"{sdk_instance.application.app_key}");    // App Key
        formContent.AddField("lang", $"{sdk_instance.settings.language}");         // Language

        // Send Request
        StartCoroutine(sdk_instance.utils.SendRequest(OcugineSDK.PROTOCOL + OcugineSDK.SERVER + OcugineSDK.API_GATE + OcugineSDK.ANALYTICS_OBJECT + "/get_available_flags", formContent,
        ((string data) => { // Response
            FlagsListModel state = JsonUtility.FromJson<FlagsListModel>(data); // Deserialize Object
            if(complete!=null) complete(state); // Return Data                       
        }),
        ((string code) => { // Error
            if(error!=null) error(code);
        })));
    }

    //============================================================
    //  @class      Ocugine_Analytics
    //  @method     SetUserFlag()
    //  @type       Static Void
    //  @usage      Set user flag
    //  @args       (string) flag_name - Flag that we should set
    //              (void) complete - Complete Callback
    //              (void) error - Error Callback
    //============================================================
    public delegate void OnSetUserFlagSuccess(BaseModel data); // Returns BaseModel
    public delegate void OnSetUserFlagError(string code); // Returns error code
    public void SetUserFlag(string flag_name, OnSetUserFlagSuccess complete = null, OnSetUserFlagError error = null){
        WWWForm formContent = new WWWForm(); // Create WWW Form
        formContent.AddField("app_id", $"{sdk_instance.application.app_id}");      // Add Language
        formContent.AddField("app_key", $"{sdk_instance.application.app_key}");    // App Key
        formContent.AddField("lang", $"{sdk_instance.settings.language}");         // Language
        formContent.AddField("flag", $"{flag_name}");         // Language
        if (sdk_instance.auth != null) formContent.AddField("access_token", $"{sdk_instance.auth.credentials.token}"); // Token       

        // Send Request
        StartCoroutine(sdk_instance.utils.SendRequest(OcugineSDK.PROTOCOL + OcugineSDK.SERVER + OcugineSDK.API_GATE + OcugineSDK.ANALYTICS_OBJECT + "/set_user_flag", formContent,
        ((string data) => { // Response
            BaseModel state = JsonUtility.FromJson<BaseModel>(data); // Deserialize Object
            if(complete!=null) complete(state); // Return Data                       
        }),
        ((string code) => { // Error
            if(error!=null) error(code);
        })));
    }

    //============================================================
    //  @class      Ocugine_Analytics
    //  @method     GetUserFlag()
    //  @type       Static Void
    //  @usage      Get user flag data by flag name
    //  @args       (string) flag_name - Flag that we should get
    //              (void) complete - Complete Callback
    //              (void) error - Error Callback
    //============================================================
    public delegate void OnGetUserFlagSuccess(FlagModel data); // Returns BaseModel
    public delegate void OnGetUserFlagError(string code); // Returns error code
    public void GetUserFlag(string flag_name, OnGetUserFlagSuccess complete = null, OnGetUserFlagError error = null){
        WWWForm formContent = new WWWForm(); // Create WWW Form
        formContent.AddField("app_id", $"{sdk_instance.application.app_id}");      // Add Language
        formContent.AddField("app_key", $"{sdk_instance.application.app_key}");    // App Key
        formContent.AddField("lang", $"{sdk_instance.settings.language}");         // Language
        formContent.AddField("flag", $"{flag_name}");         // Language
        if (sdk_instance.auth != null) formContent.AddField("access_token", $"{sdk_instance.auth.credentials.token}"); // Token

        // Send Request
        StartCoroutine(sdk_instance.utils.SendRequest(OcugineSDK.PROTOCOL + OcugineSDK.SERVER + OcugineSDK.API_GATE + OcugineSDK.ANALYTICS_OBJECT + "/get_user_flag", formContent,
        ((string data) => { // Response
            FlagModel state = JsonUtility.FromJson<FlagModel>(data); // Deserialize Object
            if(complete!=null) complete(state); // Return Data                       
        }),
        ((string code) => { // Error
            if(error!=null) error(code);
        })));
    }

    //============================================================
    //  @class      Ocugine_Analytics
    //  @method     GetLatestFlags()
    //  @type       Static Void
    //  @usage      Get all latest flags
    //  @args       (void) complete - Complete Callback
    //              (void) error - Error Callback
    //============================================================
    public delegate void OnGetLatestFlagsSuccess(LatestFlagsListModel data); // Returns BaseModel
    public delegate void OnGetLatestFlagsError(string code); // Returns error code
    public void GetLatestFlags(OnGetLatestFlagsSuccess complete = null, OnGetLatestFlagsError error = null){
        WWWForm formContent = new WWWForm(); // Create WWW Form
        formContent.AddField("app_id", $"{sdk_instance.application.app_id}");               // Add Language
        formContent.AddField("app_key", $"{sdk_instance.application.app_key}");             // App Key
        formContent.AddField("lang", $"{sdk_instance.settings.language}");                  // Language
        if (sdk_instance.auth != null) formContent.AddField("access_token", $"{sdk_instance.auth.credentials.token}"); // Token

        // Send Request
        StartCoroutine(sdk_instance.utils.SendRequest(OcugineSDK.PROTOCOL + OcugineSDK.SERVER + OcugineSDK.API_GATE + OcugineSDK.ANALYTICS_OBJECT + "/get_latest_user_flags", formContent,
        ((string data) => { // Response
            LatestFlagsListModel state = JsonUtility.FromJson<LatestFlagsListModel>(data); // Deserialize Object
            if(complete!=null) complete(state); // Return Data                       
        }),
        ((string code) => { // Error
            if(error!=null) error(code);
        })));
    }
}

//===================================================
//  Ocugine Unity SDK Game Services Class
//===================================================
[AddComponentMenu("Ocugine SDK/Game Services Component")]
[RequireComponent(typeof(OcugineSDK))]
public class Ocugine_GameServices : MonoBehaviour{
    // Public Params

    // Private Params
    private OcugineSDK sdk_instance = null;     // Ocugine SDK Instance

    //============================================================
    //  @class      Ocugine_GameServices
    //  @method     InitializeModule()
    //  @type       Public Void
    //  @usage      Initialize Module
    //  @args       (OcugineSDK) instance - Parent Instance Component
    //  @return     none
    //============================================================
    public void InitializeModule(OcugineSDK instance){
        sdk_instance = instance; // Set Ocugine SDK Instance
    }

    /*=====================[MISSIONS]=====================*/

    //============================================================
    //  @class      Ocugine_GameServices
    //  @method     GetMissionsList()
    //  @type       Static Void
    //  @usage      Get all missions
    //  @args       (double) page - Page
    //              (void) complete - Complete Callback
    //              (void) error - Error Callback
    //              (string) search - Search request              
    //  @return     none
    //============================================================
    public delegate void OnGetMissionsListSuccess(MissionsListModel data); // Returns MissionsListModel
    public delegate void OnGetMissionsListError(string code); // Returns error code
    public void GetMissionsList(string search = "", double page = 1, OnGetMissionsListSuccess complete = null, OnGetMissionsListError error = null){
        WWWForm formContent = new WWWForm(); // Create WWW Form
        formContent.AddField("app_id", $"{sdk_instance.application.app_id}");       // Add Language
        formContent.AddField("app_key", $"{sdk_instance.application.app_key}");     // App Key
        formContent.AddField("lang", $"{sdk_instance.settings.language}");          // Language
        formContent.AddField("search", $"{search}");                                // Language
        formContent.AddField("page", $"{page}");                                    // Language

        // Send Request
        StartCoroutine(sdk_instance.utils.SendRequest(OcugineSDK.PROTOCOL + OcugineSDK.SERVER + OcugineSDK.API_GATE + OcugineSDK.GAMING_OBJECT + "/get_missions", formContent,
        ((string data) => { // Response
            MissionsListModel state = JsonUtility.FromJson<MissionsListModel>(data); // Deserialize Object
            if(complete!=null) complete(state); // Return Data                       
        }),
        ((string code) => { // Error
            if (error != null)
                error(code);
        })));
    }

    //============================================================
    //  @class      Ocugine_GameServices
    //  @method     GetMissionInfo()
    //  @type       Static Void
    //  @usage      Get mission data
    //  @args       (double) mission_id - Id of mission
    //              (void) complete - Complete Callback
    //              (void) error - Error Callback
    //  @return     none
    //============================================================
    public delegate void OnGetMissionInfoSuccess(MissionModel data); // Returns MissionModel
    public delegate void OnGetMissionInfoError(string code); // Returns error code
    public void GetMissionInfo(double mission_id, OnGetMissionInfoSuccess complete = null, OnGetMissionInfoError error = null){
        WWWForm formContent = new WWWForm(); // Create WWW Form
        formContent.AddField("app_id", $"{sdk_instance.application.app_id}");      // Add Language
        formContent.AddField("app_key", $"{sdk_instance.application.app_key}");    // App Key
        formContent.AddField("lang", $"{sdk_instance.settings.language}");         // Language
        formContent.AddField("uid", $"{mission_id}");                              // Language
           
        // Send Request
        StartCoroutine(sdk_instance.utils.SendRequest(OcugineSDK.PROTOCOL + OcugineSDK.SERVER + OcugineSDK.API_GATE + OcugineSDK.GAMING_OBJECT + "/get_mission_info", formContent,
        ((string data) => { // Response
            MissionModel state = JsonUtility.FromJson<MissionModel>(data); // Deserialize Object
            if (complete != null)complete(state); // Return Data                       
        }),
        ((string code) => { // Error
            if (error != null)
                error(code);
        })));
    }

    //============================================================
    //  @class      Ocugine_GameServices
    //  @method     GetPlayerMissions()
    //  @type       Static Void
    //  @usage      Get all player missions
    //  @args       (void) complete - Complete Callback
    //              (void) error - Error Callback
    //  @return     none
    //============================================================
    public delegate void OnGetPlayerMissionsSuccess(PlayeMissionsListModel data); // Returns PlayeMissionsListModel
    public delegate void OnGetPlayerMissionsError(string code); // Returns error code
    public void GetPlayerMissions(OnGetPlayerMissionsSuccess complete = null, OnGetPlayerMissionsError error = null){
        WWWForm formContent = new WWWForm(); // Create WWW Form
        formContent.AddField("app_id", $"{sdk_instance.application.app_id}");               // Add Language
        formContent.AddField("app_key", $"{sdk_instance.application.app_key}");             // App Key
        formContent.AddField("lang", $"{sdk_instance.settings.language}");                  // Language
        if (sdk_instance.auth != null) formContent.AddField("access_token", $"{sdk_instance.auth.credentials.token}"); // Token

        // Send Request
        StartCoroutine(sdk_instance.utils.SendRequest(OcugineSDK.PROTOCOL + OcugineSDK.SERVER + OcugineSDK.API_GATE + OcugineSDK.GAMING_OBJECT + "/get_player_missions", formContent,
        ((string data) => { // Response
            PlayeMissionsListModel state = JsonUtility.FromJson<PlayeMissionsListModel>(data); // Deserialize Object
            if (complete != null)
                complete(state); // Return Data                       
        }),
        ((string code) => { // Error
            if (error != null)
                error(code);
        })));
    }

    //============================================================
    //  @class      Ocugine_GameServices
    //  @method     SetPlayerMission()
    //  @type       Static Void
    //  @usage      Assing player to mission
    //  @args       (double) mission_id - Id of mission
    //              (void) complete - Complete Callback
    //              (void) error - Error Callback
    //  @return     none
    //============================================================
    public delegate void OnPlayerMissionChangeSuccess(BaseModel data); // Returns BaseModel
    public delegate void OnPlayerMissionChangeError(string code); // Returns error code
    public void SetPlayerMission(double mission_id, OnPlayerMissionChangeSuccess complete = null, OnPlayerMissionChangeError error = null){
        WWWForm formContent = new WWWForm(); // Create WWW Form
        formContent.AddField("app_id", $"{sdk_instance.application.app_id}");      // Add Language
        formContent.AddField("app_key", $"{sdk_instance.application.app_key}");    // App Key
        formContent.AddField("lang", $"{sdk_instance.settings.language}");         // Language
        if (sdk_instance.auth != null) formContent.AddField("access_token", $"{sdk_instance.auth.credentials.token}"); // Token
        formContent.AddField("uid", $"{mission_id}");                               // Mission ID

        // Send Request
        StartCoroutine(sdk_instance.utils.SendRequest(OcugineSDK.PROTOCOL + OcugineSDK.SERVER + OcugineSDK.API_GATE + OcugineSDK.GAMING_OBJECT + "/set_player_mission", formContent,
        ((string data) => { // Response
            BaseModel state = JsonUtility.FromJson<BaseModel>(data); // Deserialize Object
            if (complete != null)
                complete(state); // Return Data                       
        }),
        ((string code) => { // Error
            if (error != null)
                error(code);
        })));
    }

    //============================================================
    //  @class      Ocugine_GameServices
    //  @method     RemovePlayerMission()
    //  @type       Static Void
    //  @usage      Unassign player from mission
    //  @args       (double) mission_id - Id of mission
    //              (void) complete - Complete Callback
    //              (void) error - Error Callback
    //  @return     none
    //============================================================
    public void RemovePlayerMission(double mission_id, OnPlayerMissionChangeSuccess complete = null, OnPlayerMissionChangeError error = null){
        WWWForm formContent = new WWWForm(); // Create WWW Form
        formContent.AddField("app_id", $"{sdk_instance.application.app_id}");      // Add Language
        formContent.AddField("app_key", $"{sdk_instance.application.app_key}");    // App Key
        formContent.AddField("lang", $"{sdk_instance.settings.language}");         // Language
        if (sdk_instance.auth != null) formContent.AddField("access_token", $"{sdk_instance.auth.credentials.token}"); // Token
        formContent.AddField("uid", $"{mission_id}");                               // Mission ID

        // Send Request
        StartCoroutine(sdk_instance.utils.SendRequest(OcugineSDK.PROTOCOL + OcugineSDK.SERVER + OcugineSDK.API_GATE + OcugineSDK.GAMING_OBJECT + "/remove_player_mission", formContent,
        ((string data) => { // Response
            BaseModel state = JsonUtility.FromJson<BaseModel>(data); // Deserialize Object
            if (complete != null)
                complete(state); // Return Data                       
        }),
        ((string code) => { // Error
            if (error != null)
                error(code);
        })));
    }

    //============================================================
    //  @class      Ocugine_GameServices
    //  @method     RemovePlayerMission()
    //  @type       Static Void
    //  @usage      Set player scores in mission
    //  @args       (double) mission_id - Mission id
    //              (void) complete - Complete Callback
    //              (void) error - Error Callback
    //  @return     none
    //============================================================
    public delegate void OnSetPlayerMissionScoreComplete(MissionSetScoresModel data); // Returns BaseModel
    public delegate void OnSetPlayerMissionScoreError(string code); // Returns error code
    public void SetPlayerMissionScore(double mission_id, double scores, OnSetPlayerMissionScoreComplete complete = null, OnSetPlayerMissionScoreError error = null){
        WWWForm formContent = new WWWForm(); // Create WWW Form
        formContent.AddField("app_id", $"{sdk_instance.application.app_id}");               // Add Language
        formContent.AddField("app_key", $"{sdk_instance.application.app_key}");             // App Key
        formContent.AddField("lang", $"{sdk_instance.settings.language}");                  // Language
        if (sdk_instance.auth != null) formContent.AddField("access_token", $"{sdk_instance.auth.credentials.token}"); // Token
        formContent.AddField("uid", $"{mission_id}");                                       // Mission ID
        formContent.AddField("scores", $"{scores}");                                        // Mission ID

        // Send Request
        StartCoroutine(sdk_instance.utils.SendRequest(OcugineSDK.PROTOCOL + OcugineSDK.SERVER + OcugineSDK.API_GATE + OcugineSDK.GAMING_OBJECT + "/set_mission_scores", formContent,
        ((string data) => { // Response
            MissionSetScoresModel state = JsonUtility.FromJson<MissionSetScoresModel>(data); // Deserialize Object
            if (complete != null)
                complete(state); // Return Data                       
        }),
        ((string code) => { // Error
            if (error != null)
                error(code);
        })));
    }

    /*=====================[ACHIEVEMENTS]=====================*/

    //============================================================
    //  @class      Ocugine_GameServices
    //  @method     GetAchievemntsList()
    //  @type       Static Void
    //  @usage      Get all available achievements
    //  @args       (void) complete - Complete Callback
    //              (void) error - Error Callback
    //  @return     none
    //============================================================
    public delegate void OnGetAchievemntsListSuccess(AchievementsListModel data); // Returns MissionModel
    public delegate void OnGetAchievemntsListError(string code); // Returns error code
    public void GetAchievemntsList(OnGetAchievemntsListSuccess complete = null, OnGetAchievemntsListError error = null){
        WWWForm formContent = new WWWForm(); // Create WWW Form
        formContent.AddField("app_id", $"{sdk_instance.application.app_id}");      // Add Language
        formContent.AddField("app_key", $"{sdk_instance.application.app_key}");    // App Key
        formContent.AddField("lang", $"{sdk_instance.settings.language}");         // Language

        // Send Request
        StartCoroutine(sdk_instance.utils.SendRequest(OcugineSDK.PROTOCOL + OcugineSDK.SERVER + OcugineSDK.API_GATE + OcugineSDK.GAMING_OBJECT + "/get_achievements", formContent,
        ((string data) => { // Response
            AchievementsListModel state = JsonUtility.FromJson<AchievementsListModel>(data); // Deserialize Object
            if (complete != null)
                complete(state); // Return Data                       
        }),
        ((string code) => { // Error
            if (error != null)
                error(code);
        })));
    }

    //============================================================
    //  @class      Ocugine_GameServices
    //  @method     GetAchievement()
    //  @type       Static Void
    //  @usage      Get achievement by id
    //  @args       (double) achievement_id - id of achievement
    //              (void) complete - Complete Callback
    //              (void) error - Error Callback
    //  @return     none
    //============================================================
    public delegate void OnGetAchievementSuccess(AchievementModel data); // Returns MissionModel
    public delegate void OnGetAchievementError(string code); // Returns error code
    public void GetAchievement(double achievement_id, OnGetAchievementSuccess complete = null, OnGetAchievementError error = null){
        WWWForm formContent = new WWWForm(); // Create WWW Form
        formContent.AddField("app_id", $"{sdk_instance.application.app_id}");      // Add Language
        formContent.AddField("app_key", $"{sdk_instance.application.app_key}");    // App Key
        formContent.AddField("lang", $"{sdk_instance.settings.language}");         // Language
        formContent.AddField("uid", $"{achievement_id}");         // Language

        // Send Request
        StartCoroutine(sdk_instance.utils.SendRequest(OcugineSDK.PROTOCOL + OcugineSDK.SERVER + OcugineSDK.API_GATE + OcugineSDK.GAMING_OBJECT + "/get_achievement_info", formContent,
        ((string data) => { // Response
            AchievementModel state = JsonUtility.FromJson<AchievementModel>(data); // Deserialize Object
            if (complete != null)
                complete(state); // Return Data                       
        }),
        ((string code) => { // Error
            if (error != null)
                error(code);
        })));
    }

    //============================================================
    //  @class      Ocugine_GameServices
    //  @method     GetPlayerAchievements()
    //  @type       Static Void
    //  @usage      Get player achievements by token
    //  @args       (void) complete - Complete Callback
    //              (void) error - Error Callback
    //  @return     none
    //============================================================
    public delegate void OnGetPlayerAchievementsSuccess(PlayerAchievementListModel data); // Returns BaseModel
    public delegate void OnGetPlayerAchievementsError(string code); // Returns error code
    public void GetPlayerAchievements(OnGetPlayerAchievementsSuccess complete = null, OnGetPlayerAchievementsError error = null){
        WWWForm formContent = new WWWForm(); // Create WWW Form
        formContent.AddField("app_id", $"{sdk_instance.application.app_id}");               // Add Language
        formContent.AddField("app_key", $"{sdk_instance.application.app_key}");             // App Key
        formContent.AddField("lang", $"{sdk_instance.settings.language}");                  // Language
        if (sdk_instance.auth != null) formContent.AddField("access_token", $"{sdk_instance.auth.credentials.token}"); // Token

        // Send Request
        StartCoroutine(sdk_instance.utils.SendRequest(OcugineSDK.PROTOCOL + OcugineSDK.SERVER + OcugineSDK.API_GATE + OcugineSDK.GAMING_OBJECT + "/get_player_achievements", formContent,
        ((string data) => { // Response
            PlayerAchievementListModel state = JsonUtility.FromJson<PlayerAchievementListModel>(data); // Deserialize Object
            if (complete != null)
                complete(state); // Return Data                       
        }),
        ((string code) => { // Error
            if (error != null)
                error(code);
        })));
    }

    //============================================================
    //  @class      Ocugine_GameServices
    //  @method     UnlockPlayerAchievement()
    //  @type       Static Void
    //  @usage      Unlock achievement for player
    //  @args       (double) achievement_id - achievement id
    //              (void) complete - Complete Callback
    //              (void) error - Error Callback
    //  @return     none
    //============================================================
    public delegate void OnUnlockPlayerAchievementSuccess(BaseModel data); // Returns BaseModel
    public delegate void OnUnlockPlayerAchievementError(string code); // Returns error code
    public void UnlockPlayerAchievement(double achievement_id, OnPlayerMissionChangeSuccess complete = null, OnPlayerMissionChangeError error = null){
        WWWForm formContent = new WWWForm(); // Create WWW Form
        formContent.AddField("app_id", $"{sdk_instance.application.app_id}");               // Add Language
        formContent.AddField("app_key", $"{sdk_instance.application.app_key}");             // App Key
        formContent.AddField("lang", $"{sdk_instance.settings.language}");                  // Language
        if (sdk_instance.auth != null) formContent.AddField("access_token", $"{sdk_instance.auth.credentials.token}"); // Token
        formContent.AddField("uid", $"{achievement_id}");         // Language

        // Send Request
        StartCoroutine(sdk_instance.utils.SendRequest(OcugineSDK.PROTOCOL + OcugineSDK.SERVER + OcugineSDK.API_GATE + OcugineSDK.GAMING_OBJECT + "/unlock_player_achievement", formContent,
        ((string data) => { // Response
            BaseModel state = JsonUtility.FromJson<BaseModel>(data); // Deserialize Object
            if (complete != null)
                complete(state); // Return Data                       
        }),
        ((string code) => { // Error
            if (error != null)
                error(code);
        })));
    }

    /*=====================[LEADERBOARDS]=====================*/

    //============================================================
    //  @class      Ocugine_GameServices
    //  @method     GetLeaderboardsList()
    //  @type       Static Void
    //  @usage      Get all leaderboards
    //  @args       (void) complete - Complete Callback
    //              (void) error - Error Callback
    //  @return     none
    //============================================================
    public delegate void OnGetLeaderboardsListSuccess(LeaderboardListModel data); // Returns MissionModel
    public delegate void OnGetLeaderboardsListError(string code); // Returns error code
    public void GetLeaderboardsList(OnGetLeaderboardsListSuccess complete = null, OnGetLeaderboardsListError error = null){
        WWWForm formContent = new WWWForm(); // Create WWW Form
        formContent.AddField("app_id", $"{sdk_instance.application.app_id}");      // Add Language
        formContent.AddField("app_key", $"{sdk_instance.application.app_key}");    // App Key
        formContent.AddField("lang", $"{sdk_instance.settings.language}");         // Language

        // Send Request
        StartCoroutine(sdk_instance.utils.SendRequest(OcugineSDK.PROTOCOL + OcugineSDK.SERVER + OcugineSDK.API_GATE + OcugineSDK.GAMING_OBJECT + "/get_leaderboards", formContent,
        ((string data) => { // Response
            LeaderboardListModel state = JsonUtility.FromJson<LeaderboardListModel>(data); // Deserialize Object
            if (complete != null)
                complete(state); // Return Data                       
        }),
        ((string code) => { // Error
            if (error != null)
                error(code);
        })));
    }   

    //============================================================
    //  @class      Ocugine_GameServices
    //  @method     GetLeaderboard()
    //  @type       Static Void
    //  @usage      Get leadervoard by id
    //  @args       (double) leaderboard_id - leaderboard id
    //              (void) complete - Complete Callback
    //              (void) error - Error Callback
    //  @return     none
    //============================================================
    public delegate void OnGetLeaderboardSuccess(LeaderboardModel data); // Returns MissionModel
    public delegate void OnGetLeaderboardError(string code); // Returns error code
    public void GetLeaderboard(double leaderboard_id, OnGetLeaderboardSuccess complete = null, OnGetLeaderboardError error = null){
        WWWForm formContent = new WWWForm(); // Create WWW Form
        formContent.AddField("app_id", $"{sdk_instance.application.app_id}");      // Add Language
        formContent.AddField("app_key", $"{sdk_instance.application.app_key}");    // App Key
        formContent.AddField("lang", $"{sdk_instance.settings.language}");         // Language
        formContent.AddField("uid", $"{leaderboard_id}");         // Language

        // Send Request
        StartCoroutine(sdk_instance.utils.SendRequest(OcugineSDK.PROTOCOL + OcugineSDK.SERVER + OcugineSDK.API_GATE + OcugineSDK.GAMING_OBJECT + "/get_leaderboard_info", formContent,
        ((string data) => { // Response
            LeaderboardModel state = JsonUtility.FromJson<LeaderboardModel>(data); // Deserialize Object
            if (complete != null)
                complete(state); // Return Data                       
        }),
        ((string code) => { // Error
            if (error != null)
                error(code);
        })));
    }

    //============================================================
    //  @class      Ocugine_GameServices
    //  @method     GetPlayersTop()
    //  @type       Static Void
    //  @usage      Get top players from leaderboard
    //  @args       (double) leaderboard_id - leaderboard id
    //              (void) complete - Complete Callback
    //              (void) error - Error Callback
    //  @return     none
    //============================================================
    public delegate void OnGetPlayersTopSuccess(LeaderboardTopModel data); // Returns MissionModel
    public delegate void OnGetPlayersTopError(string code); // Returns error code
    public void GetPlayersTop(double leaderboard_id, OnGetPlayersTopSuccess complete = null, OnGetPlayersTopError error = null){
        WWWForm formContent = new WWWForm(); // Create WWW Form
        formContent.AddField("app_id", $"{sdk_instance.application.app_id}");      // Add Language
        formContent.AddField("app_key", $"{sdk_instance.application.app_key}");    // App Key
        formContent.AddField("lang", $"{sdk_instance.settings.language}");         // Language
        formContent.AddField("uid", $"{leaderboard_id}");         // Language

        // Send Request
        StartCoroutine(sdk_instance.utils.SendRequest(OcugineSDK.PROTOCOL + OcugineSDK.SERVER + OcugineSDK.API_GATE + OcugineSDK.GAMING_OBJECT + "/get_players_top", formContent,
        ((string data) => { // Response
            LeaderboardTopModel state = JsonUtility.FromJson<LeaderboardTopModel>(data); // Deserialize Object
            if (complete != null)
                complete(state); // Return Data                       
        }),
        ((string code) => { // Error
            if (error != null)
                error(code);
        })));
    }

    //============================================================
    //  @class      Ocugine_GameServices
    //  @method     GetPlayerLeaderboardScores()
    //  @type       Static Void
    //  @usage      Get user scores on leaderboard
    //  @args       (void) complete - Complete Callback
    //              (void) error - Error Callback
    //  @return     none
    //============================================================
    public delegate void OnGetPlayerLeaderboardScoresSuccess(LeaderboardPlayerScoresModel data); // Returns BaseModel
    public delegate void OnGetPlayerLeaderboardScoresError(string code); // Returns error code
    public void GetPlayerLeaderboardScores(double leaderboard_id, OnGetPlayerLeaderboardScoresSuccess complete = null, OnGetPlayerLeaderboardScoresError error = null){
        WWWForm formContent = new WWWForm(); // Create WWW Form
        formContent.AddField("app_id", $"{sdk_instance.application.app_id}");               // Add Language
        formContent.AddField("app_key", $"{sdk_instance.application.app_key}");             // App Key
        formContent.AddField("lang", $"{sdk_instance.settings.language}");                  // Language
        if (sdk_instance.auth != null) formContent.AddField("access_token", $"{sdk_instance.auth.credentials.token}"); // Token
        formContent.AddField("uid", $"{leaderboard_id}");         // Language

        // Send Request
        StartCoroutine(sdk_instance.utils.SendRequest(OcugineSDK.PROTOCOL + OcugineSDK.SERVER + OcugineSDK.API_GATE + OcugineSDK.GAMING_OBJECT + "/get_board_scores", formContent,
        ((string data) => { // Response
            LeaderboardPlayerScoresModel state = JsonUtility.FromJson<LeaderboardPlayerScoresModel>(data); // Deserialize Object
            if (complete != null)
                complete(state); // Return Data                       
        }),
        ((string code) => { // Error
            if (error != null)
                error(code);
        })));
    }

    //============================================================
    //  @class      Ocugine_GameServices
    //  @method     SetPlayerLeaderboardScores()
    //  @type       Static Void
    //  @usage      Set player leaderboard scores
    //  @args       (void) complete - Complete Callback
    //              (void) error - Error Callback
    //  @return     none
    //============================================================
    public delegate void OnSetPlayerLeaderboardScoresSuccess(BaseModel data); // Returns BaseModel
    public delegate void OnSetPlayerLeaderboardScoresError(string code); // Returns error code
    public void SetPlayerLeaderboardScores(double leaderboard_id, double scores, OnSetPlayerLeaderboardScoresSuccess complete = null, OnSetPlayerLeaderboardScoresError error = null){
        WWWForm formContent = new WWWForm(); // Create WWW Form
        formContent.AddField("app_id", $"{sdk_instance.application.app_id}");               // Add Language
        formContent.AddField("app_key", $"{sdk_instance.application.app_key}");             // App Key
        formContent.AddField("lang", $"{sdk_instance.settings.language}");                  // Language
        if (sdk_instance.auth != null) formContent.AddField("access_token", $"{sdk_instance.auth.credentials.token}"); // Token
        formContent.AddField("uid", $"{leaderboard_id}");         // Language
        formContent.AddField("scores", $"{scores}");         // Language

        // Send Request
        StartCoroutine(sdk_instance.utils.SendRequest(OcugineSDK.PROTOCOL + OcugineSDK.SERVER + OcugineSDK.API_GATE + OcugineSDK.GAMING_OBJECT + "/set_board_scores", formContent,
        ((string data) => { // Response
            BaseModel state = JsonUtility.FromJson<BaseModel>(data); // Deserialize Object
            if (complete != null)
                complete(state); // Return Data                       
        }),
        ((string code) => { // Error
            if (error != null)
                error(code);
        })));
    }
}

//===================================================
//  Ocugine Unity SDK Monetization Class
//===================================================
[AddComponentMenu("Ocugine SDK/Monetization Component")]
[RequireComponent(typeof(OcugineSDK))]
public class Ocugine_Monetization : MonoBehaviour{
    // Public Params

    // Private Params
    private OcugineSDK sdk_instance = null;     // Ocugine SDK Instance

    //============================================================
    //  @class      Ocugine_Monetization
    //  @method     InitializeModule()
    //  @type       Public Void
    //  @usage      Initialize Module
    //  @args       (OcugineSDK) instance - Parent Instance Component
    //  @return     none
    //============================================================
    public void InitializeModule(OcugineSDK instance){
        sdk_instance = instance; // Set Ocugine SDK Instance
    }

    /* TODO: Monetization Class */
}

//===================================================
//  Ocugine Unity SDK Notifications Class
//===================================================
[AddComponentMenu("Ocugine SDK/Notifications Component")]
[RequireComponent(typeof(OcugineSDK))]
public class Ocugine_Notifications : MonoBehaviour{
    // Public Params

    // Private Params
    private OcugineSDK sdk_instance = null;     // Ocugine SDK Instance

    //============================================================
    //  @class      Ocugine_Notifications
    //  @method     InitializeModule()
    //  @type       Public Void
    //  @usage      Initialize Module
    //  @args       (OcugineSDK) instance - Parent Instance Component
    //  @return     none
    //============================================================
    public void InitializeModule(OcugineSDK instance){
        sdk_instance = instance; // Set Ocugine SDK Instance
    }

    //============================================================
    //  @class      Ocugine_Users
    //  @method     GetNotifications()
    //  @type       Static Void
    //  @usage      Get notifications list
    //  @args       (NotificationModel.PlatformType) platform - Platform
    //              (void) complete - Complete Callback
    //              (void) error - Error Callback
    //  @return     none
    //============================================================
    public delegate void OnGetNotificationsListSuccess(NotificationsListModel data); // Returns NotificationsListModel
    public delegate void OnGetNotificationsListError(string code); // Returns error code
    public void GetNotificationsList(NotificationModel.PlatformType platform, OnGetNotificationsListSuccess complete = null, OnGetNotificationsListError error = null)
    {
        WWWForm formContent = new WWWForm(); // Create WWW Form
        formContent.AddField("app_id", $"{sdk_instance.application.app_id}");      // Add Language
        formContent.AddField("app_key", $"{sdk_instance.application.app_key}");    // App Key
        formContent.AddField("lang", $"{sdk_instance.settings.language}");         // Language
        if (sdk_instance.auth != null)
            formContent.AddField("access_token", $"{sdk_instance.auth.credentials.token}"); // Token
        formContent.AddField("platform", $"{platform}");         // Language

        // Send Request
        StartCoroutine(sdk_instance.utils.SendRequest(OcugineSDK.PROTOCOL + OcugineSDK.SERVER + OcugineSDK.API_GATE + OcugineSDK.USERS_OBJECT + "/get_notifications", formContent,
        ((string data) => { // Response
            NotificationsListModel state = JsonUtility.FromJson<NotificationsListModel>(data); // Deserialize Object
            if (complete != null)
                complete(state); // Return Data                       
        }),
        ((string code) => { // Error
            if (error != null)
                error(code);
        })));
    }

    //============================================================
    //  @class      Ocugine_Users
    //  @method     GetNotification()
    //  @type       Static Void
    //  @usage      Get notification by id
    //  @args       (double) notification_uid - Notification id
    //              (void) complete - Complete Callback
    //              (void) error - Error Callback
    //  @return     none
    //============================================================
    public delegate void OnGetNotificationSuccess(NotificationModel data); // Returns NotificationModel
    public delegate void OnGetNotificationError(string code); // Returns error code
    public void GetNotification(double notification_id, OnGetNotificationSuccess complete = null, OnGetNotificationError error = null)
    {
        WWWForm formContent = new WWWForm(); // Create WWW Form
        formContent.AddField("app_id", $"{sdk_instance.application.app_id}");      // Add Language
        formContent.AddField("app_key", $"{sdk_instance.application.app_key}");    // App Key
        formContent.AddField("lang", $"{sdk_instance.settings.language}");         // Language
        formContent.AddField("uid", $"{notification_id}");         // Language     

        // Send Request
        StartCoroutine(sdk_instance.utils.SendRequest(OcugineSDK.PROTOCOL + OcugineSDK.SERVER + OcugineSDK.API_GATE + OcugineSDK.USERS_OBJECT + "/get_notification_data", formContent,
        ((string data) => { // Response
            NotificationModel state = JsonUtility.FromJson<NotificationModel>(data); // Deserialize Object
            if (complete != null)
                complete(state); // Return Data                       
        }),
        ((string code) => { // Error
            if (error != null)
                error(code);
        })));
    }

    //============================================================
    //  @class      Ocugine_Users
    //  @method     ReadNotification()
    //  @type       Static Void
    //  @usage      Show, that notification have been readen
    //  @args       (double) notification_uid - Id of notification
    //              (void) complete - Complete Callback
    //              (void) error - Error Callback
    //  @return     none
    //============================================================
    public delegate void OnReadNotificationSuccess(BaseModel data); // Returns NotificationModel
    public delegate void OnReadNotificationError(string code); // Returns error code
    public void ReadNotification(double notification_id, OnReadNotificationSuccess complete = null, OnReadNotificationError error = null)
    {
        WWWForm formContent = new WWWForm(); // Create WWW Form
        formContent.AddField("app_id", $"{sdk_instance.application.app_id}");      // Add Language
        formContent.AddField("app_key", $"{sdk_instance.application.app_key}");    // App Key
        formContent.AddField("lang", $"{sdk_instance.settings.language}");         // Language
        if (sdk_instance.auth != null)
            formContent.AddField("access_token", $"{sdk_instance.auth.credentials.token}"); // Token
        formContent.AddField("uid", $"{notification_id}");         // Language     

        // Send Request
        StartCoroutine(sdk_instance.utils.SendRequest(OcugineSDK.PROTOCOL + OcugineSDK.SERVER + OcugineSDK.API_GATE + OcugineSDK.USERS_OBJECT + "/read_notification", formContent,
        ((string data) => { // Response
            BaseModel state = JsonUtility.FromJson<BaseModel>(data); // Deserialize Object
            if (complete != null)
                complete(state); // Return Data                       
        }),
        ((string code) => { // Error
            if (error != null)
                error(code);
        })));
    }
}

//===================================================
//  Ocugine Unity SDK Marketing Class
//===================================================
[AddComponentMenu("Ocugine SDK/Marketing Component")]
[RequireComponent(typeof(OcugineSDK))]
public class Ocugine_Marketing : MonoBehaviour{
    // Public Params

    // Private Params
    private OcugineSDK sdk_instance = null;     // Ocugine SDK Instance

    //============================================================
    //  @class      Ocugine_Marketing
    //  @method     InitializeModule()
    //  @type       Public Void
    //  @usage      Initialize Module
    //  @args       (OcugineSDK) instance - Parent Instance Component
    //  @return     none
    //============================================================
    public void InitializeModule(OcugineSDK instance){
        sdk_instance = instance; // Set Ocugine SDK Instance
    }

    /* TODO: Marketing Class */
}

//===================================================
//  Ocugine Unity SDK Ads Class
//===================================================
[AddComponentMenu("Ocugine SDK/Ads Component")]
[RequireComponent(typeof(OcugineSDK))]
public class Ocugine_Ads : MonoBehaviour{
    // Public Params

    // Private Params
    private OcugineSDK sdk_instance = null;     // Ocugine SDK Instance

    //============================================================
    //  @class      Ocugine_Ads
    //  @method     InitializeModule()
    //  @type       Public Void
    //  @usage      Initialize Module
    //  @args       (OcugineSDK) instance - Parent Instance Component
    //  @return     none
    //============================================================
    public void InitializeModule(OcugineSDK instance){
        sdk_instance = instance; // Set Ocugine SDK Instance
    }

    /* TODO: Ads Class */
}

//===================================================
//  Ocugine Unity SDK Backend Class
//===================================================
[AddComponentMenu("Ocugine SDK/Backend Services Component")]
[RequireComponent(typeof(OcugineSDK))]
public class Ocugine_Backend : MonoBehaviour{
    // Public Params

    // Private Params
    private OcugineSDK sdk_instance = null;     // Ocugine SDK Instance

    //============================================================
    //  @class      Ocugine_Backend
    //  @method     InitializeModule()
    //  @type       Public Void
    //  @usage      Initialize Module
    //  @args       (OcugineSDK) instance - Parent Instance Component
    //  @return     none
    //============================================================
    public void InitializeModule(OcugineSDK instance){
        sdk_instance = instance; // Set Ocugine SDK Instance
    }

    //============================================================
    //  @class      Backend
    //  @method     GetContentList()
    //  @type       Static Async Void
    //  @usage      Get content list
    //  @args       (void) complete - Complete Callback
    //              (void) error - Error Callback
    //  @return     none
    //============================================================   
    public delegate void OnGetContentListComplete(ContentListModel data);
    public delegate void OnGetContentListError(string code);
    public void GetContentList(string search = "", double page = 1, OnGetContentListComplete complete = null, OnGetContentListError error = null) //
    {
        WWWForm formContent = new WWWForm(); // Create WWW Form
        formContent.AddField("app_id", $"{sdk_instance.application.app_id}");      // Add Language
        formContent.AddField("app_key", $"{sdk_instance.application.app_key}");    // App Key
        formContent.AddField("lang", $"{sdk_instance.settings.language}");         // Language
        formContent.AddField("search", $"{search}");                               // Search
        formContent.AddField("page", $"{page}");                                   // Page Index

        // Send Request
        StartCoroutine(sdk_instance.utils.SendRequest(OcugineSDK.PROTOCOL + OcugineSDK.SERVER + OcugineSDK.API_GATE + OcugineSDK.CLOUD_OBJECT + "/get_content_list", formContent,
        ((string data) => { // Response
            ContentListModel state = JsonUtility.FromJson<ContentListModel>(data); // Deserialize Object   
            if (complete != null)
                complete(state);
        }),
        ((string code) => { // Error
            if (error != null)
                error(code);
        })));

    }

    //============================================================
    //  @class      Backend
    //  @method     GetContentList()
    //  @type       Static Async Void
    //  @usage      Get content list
    //  @args       (double) content_id - Content id
    //              (void) complete - Complete Callback
    //              (void) error - Error Callback
    //  @return     none
    //============================================================   
    public delegate void OnGetContentComplete(ContentModel data);
    public delegate void OnGetContentError(string code);
    public void GetContent(double content_id, OnGetContentComplete complete = null, OnGetContentError error = null) //  (bool) Get lang
    {
        WWWForm formContent = new WWWForm(); // Create WWW Form
        formContent.AddField("app_id", $"{sdk_instance.application.app_id}");      // Add Language
        formContent.AddField("app_key", $"{sdk_instance.application.app_key}");    // App Key
        formContent.AddField("lang", $"{sdk_instance.settings.language}");         // Language
        formContent.AddField("cid", $"{content_id}");                              // Cid

        // Send Request
        StartCoroutine(sdk_instance.utils.SendRequest(OcugineSDK.PROTOCOL + OcugineSDK.SERVER + OcugineSDK.API_GATE + OcugineSDK.CLOUD_OBJECT + "/get_content", formContent,
        ((string data) => { // Response
                    ContentModel state = JsonUtility.FromJson<ContentModel>(data); // Deserialize Object   
            if (complete != null)
                complete(state);
            }),
        ((string code) => { // Error
            if (error != null)
                error(code);
        })));

    }

    //============================================================
    //  @class      Backend
    //  @method     GetLiveConfigsList()
    //  @type       Static Async Void
    //  @usage      Get live configs list
    //  @args       (void) complete - Complete Callback
    //              (void) error - Error Callback
    //  @return     none
    //============================================================   
    public delegate void OnGetLiveConfigsListComplete(LiveConfingListModel data);
    public delegate void OnGetLiveConfigsListError(string code);
    public void GetLiveConfigsList(OnGetLiveConfigsListComplete complete = null, OnGetLiveConfigsListError error = null) //  (bool) Get lang
    {
        WWWForm formContent = new WWWForm(); // Create WWW Form
        formContent.AddField("app_id", $"{sdk_instance.application.app_id}");      // Add Language
        formContent.AddField("app_key", $"{sdk_instance.application.app_key}");    // App Key
        formContent.AddField("lang", $"{sdk_instance.settings.language}");         // Language

        // Send Request
        StartCoroutine(sdk_instance.utils.SendRequest(OcugineSDK.PROTOCOL + OcugineSDK.SERVER + OcugineSDK.API_GATE + OcugineSDK.CLOUD_OBJECT + "/get_all_configs", formContent,
        ((string data) => { // Response
            LiveConfingListModel state = JsonUtility.FromJson<LiveConfingListModel>(data); // Deserialize Object   
            if (complete != null)
                complete(state);
        }),
        ((string code) => { // Error
            if (error != null)
                error(code);
        })));
    }

    //============================================================
    //  @class      Backend
    //  @method     GetLiveConfigsList()
    //  @type       Static Async Void
    //  @usage      Get live config data
    //  @args       (double) config_id - Config id
    //              (void) complete - Complete Callback
    //              (void) error - Error Callback
    //  @return     none
    //============================================================   
    public delegate void OnGetLiveConfigComplete(LiveConfigModel data);
    public delegate void OnGetLiveConfigError(string code);
    public void GetLiveConfig(double config_id, OnGetLiveConfigComplete complete = null, OnGetLiveConfigError error = null) //  (bool) Get lang
    {
        WWWForm formContent = new WWWForm(); // Create WWW Form
        formContent.AddField("app_id", $"{sdk_instance.application.app_id}");      // Add Language
        formContent.AddField("app_key", $"{sdk_instance.application.app_key}");    // App Key
        formContent.AddField("lang", $"{sdk_instance.settings.language}");         // Language
        formContent.AddField("uid", $"{config_id}");                                     // Cid

        // Send Request
        StartCoroutine(sdk_instance.utils.SendRequest(OcugineSDK.PROTOCOL + OcugineSDK.SERVER + OcugineSDK.API_GATE + OcugineSDK.CLOUD_OBJECT + "/get_config", formContent,
        ((string data) => { // Response
            LiveConfigModel state = JsonUtility.FromJson<LiveConfigModel>(data); // Deserialize Object   
            if (complete != null)
                complete(state);
        }),
        ((string code) => { // Error
            if (error != null)
                error(code);
        })));
    }

    //============================================================
    //  @class      Backend
    //  @method     GetCloudSavesList()
    //  @type       Static Async Void
    //  @usage      Get cloud saves list
    //  @args       (void) complete - Complete Callback
    //              (void) error - Error Callback
    //  @return     none
    //============================================================   
    public delegate void OnGetCloudSavesListComplete(CloudSavesListModel data);
    public delegate void OnGetCloudSavesListError(string code);
    public void GetCloudSavesList(OnGetCloudSavesListComplete complete = null, OnGetCloudSavesListError error = null){
        WWWForm formContent = new WWWForm(); // Create WWW Form
        formContent.AddField("app_id", $"{sdk_instance.application.app_id}");      // Add Language
        formContent.AddField("app_key", $"{sdk_instance.application.app_key}");    // App Key
        formContent.AddField("lang", $"{sdk_instance.settings.language}");         // Language
        if (sdk_instance.auth != null) formContent.AddField("access_token", $"{sdk_instance.auth.credentials.token}"); // Token

        // Send Request
        StartCoroutine(sdk_instance.utils.SendRequest(OcugineSDK.PROTOCOL + OcugineSDK.SERVER + OcugineSDK.API_GATE + OcugineSDK.CLOUD_OBJECT + "/get_player_saves", formContent,
        ((string data) => { // Response
            CloudSavesListModel state = JsonUtility.FromJson<CloudSavesListModel>(data); // Deserialize Object
            if (complete != null)
                complete(state);
        }),
        ((string code) => { // Error
            if (error != null)
                error(code);
        })));
    }

    //============================================================
    //  @class      Backend
    //  @method     GetCloudSaveData()
    //  @type       Static Async Void
    //  @usage      Get cloud save data
    //  @args       (double) save_id - Save uid
    //              (void) complete - Complete Callback
    //              (void) error - Error Callback
    //  @return     none
    //============================================================   
    public delegate void OnGetCloudSaveComplete(CloudSaveModel data);
    public delegate void OnGetCloudSaveError(string code);
    public void GetCloudSaveData(double save_id, OnGetCloudSaveComplete complete = null, OnGetCloudSaveError error = null) //  (bool) Get lang
    {
        WWWForm formContent = new WWWForm(); // Create WWW Form
        formContent.AddField("app_id", $"{sdk_instance.application.app_id}");      // Add Language
        formContent.AddField("app_key", $"{sdk_instance.application.app_key}");    // App Key
        formContent.AddField("lang", $"{sdk_instance.settings.language}");         // Language
        formContent.AddField("uid", $"{save_id}");                                 // UID
        if (sdk_instance.auth != null) formContent.AddField("access_token", $"{sdk_instance.auth.credentials.token}"); // Token

        // Send Request
        StartCoroutine(sdk_instance.utils.SendRequest(OcugineSDK.PROTOCOL + OcugineSDK.SERVER + OcugineSDK.API_GATE + OcugineSDK.CLOUD_OBJECT + "/get_save_data", formContent,
        ((string data) => { // Response
            CloudSaveModel state = JsonUtility.FromJson<CloudSaveModel>(data); // Deserialize Object   
            if (complete != null)
                complete(state);
        }),
        ((string code) => { // Error
            if (error != null)
                error(code);
        })));
    }

    //============================================================
    //  @class      Backend
    //  @method     SetCloudSaveData()
    //  @type       Static Async Void
    //  @usage      Get cloud save data
    //  @args       (double) save_id - Save UID for Update
    //              (string) body - Data to Save
    //              (void) complete - Complete Callback
    //              (void) error - Error Callback
    //  @return     none
    //============================================================   
    public delegate void OnSetCloudSaveComplete();
    public delegate void OnSetCloudSaveError(string code);
    public void SetCloudSaveData(double save_id = -1, string body = "", OnSetCloudSaveComplete complete = null, OnSetCloudSaveError error = null){ //  (bool) Get lang{
        WWWForm formContent = new WWWForm(); // Create WWW Form
        formContent.AddField("app_id", $"{sdk_instance.application.app_id}");      // Add Language
        formContent.AddField("app_key", $"{sdk_instance.application.app_key}");    // App Key
        formContent.AddField("lang", $"{sdk_instance.settings.language}");         // Language
        formContent.AddField("uid", $"{save_id}");                                 // UID
        formContent.AddField("data", $"{body}");                                   // Save Data
        if (sdk_instance.auth != null)
            formContent.AddField("access_token", $"{sdk_instance.auth.credentials.token}"); // Token

        // Send Request
        StartCoroutine(sdk_instance.utils.SendRequest(OcugineSDK.PROTOCOL + OcugineSDK.SERVER + OcugineSDK.API_GATE + OcugineSDK.CLOUD_OBJECT + "/set_save_data", formContent,
        ((string data) => { // Response
            BaseModel state = JsonUtility.FromJson<BaseModel>(data); // Deserialize Object
            if (complete != null)
                complete();
        }),
        ((string code) => { // Error
            if (error != null)
                error(code);
        })));
    }

    //============================================================
    //  @class      Backend
    //  @method     RemoveCloudSaveData()
    //  @type       Static Async Void
    //  @usage      Get cloud save data
    //  @args       (double) save_id - Save uid
    //              (void) complete - Complete Callback
    //              (void) error - Error Callback
    //  @return     none
    //============================================================   
    public delegate void OnRemoveCloudSaveComplete();
    public delegate void OnRemoveCloudSaveError(string code);
    public void RemoveCloudSaveData(double save_id, OnRemoveCloudSaveComplete complete = null, OnRemoveCloudSaveError error = null) //  (bool) Get lang
    {
        WWWForm formContent = new WWWForm(); // Create WWW Form
        formContent.AddField("app_id", $"{sdk_instance.application.app_id}");      // Add Language
        formContent.AddField("app_key", $"{sdk_instance.application.app_key}");    // App Key
        formContent.AddField("lang", $"{sdk_instance.settings.language}");         // Language
        formContent.AddField("uid", $"{save_id}");                                 // UID
        if (sdk_instance.auth != null)
            formContent.AddField("access_token", $"{sdk_instance.auth.credentials.token}"); // Token

        // Send Request
        StartCoroutine(sdk_instance.utils.SendRequest(OcugineSDK.PROTOCOL + OcugineSDK.SERVER + OcugineSDK.API_GATE + OcugineSDK.CLOUD_OBJECT + "/remove_save_data", formContent,
        ((string data) => { // Response
            BaseModel state = JsonUtility.FromJson<BaseModel>(data); // Deserialize Object
            if (complete != null)
                complete();
        }),
        ((string code) => { // Error
            if (error != null)
                error(code);
        })));
    }

    /* TODO: Migrate Backend Class */
}

//===================================================
//  Ocugine Unity SDK Reports Class
//===================================================
[AddComponentMenu("Ocugine SDK/Reporting Component")]
[RequireComponent(typeof(OcugineSDK))]
public class Ocugine_Reports : MonoBehaviour{
    // Public Params

    // Private Params
    private OcugineSDK sdk_instance = null;     // Ocugine SDK Instance

    //============================================================
    //  @class      Ocugine_Reports
    //  @method     InitializeModule()
    //  @type       Public Void
    //  @usage      Initialize Module
    //  @args       (OcugineSDK) instance - Parent Instance Component
    //  @return     none
    //============================================================
    public void InitializeModule(OcugineSDK instance){
        sdk_instance = instance; // Set Ocugine SDK Instance
    }

    //============================================================
    //  @class      Ocugine_Reports
    //  @method     Awake()
    //  @type       Internal Void
    //  @usage      Call before scene initialized
    //  @args       none
    //  @return     none
    //============================================================
    void Awake(){
        
    }

    //============================================================
    //  @class      Ocugine_Reports
    //  @method     Start()
    //  @type       Internal Void
    //  @usage      Call when scene initialized
    //  @args       none
    //  @return     none
    //============================================================
    void Start(){
        
    }

    //============================================================
    //  @class      Ocugine_Reports
    //  @method     Start()
    //  @type       Internal Void
    //  @usage      Call every tick
    //  @args       none
    //  @return     none
    //============================================================
    void Update(){
        
    }

    //============================================================
    //  @class      Ocugine_Reports
    //  @method     GetErrorReport()
    //  @type       Static Void
    //  @usage      Get error report by id
    //  @args       (double) report_id - Id of error report
    //              (void) complete - Complete Callback
    //              (void) error - Error Callback
    //  @return     none
    //============================================================
    public delegate void OnGetErrorReportSuccess(ErrorReportModel data); // Returns BaseModel
    public delegate void OnGetErrorReportError(string code); // Returns error code
    public void GetErrorReport(double report_id, OnGetErrorReportSuccess complete = null, OnGetErrorReportError error = null){
        WWWForm formContent = new WWWForm(); // Create WWW Form
        formContent.AddField("app_id", $"{sdk_instance.application.app_id}");      // Add Language
        formContent.AddField("app_key", $"{sdk_instance.application.app_key}");    // App Key
        formContent.AddField("uid", $"{report_id}");         // Language

        // Send Request
        StartCoroutine(sdk_instance.utils.SendRequest(OcugineSDK.PROTOCOL + OcugineSDK.SERVER + OcugineSDK.API_GATE + OcugineSDK.REPORTS_OBJECT + "/get_error", formContent,
        ((string data) => { // Response
            ErrorReportModel state = JsonUtility.FromJson<ErrorReportModel>(data); // Deserialize Object
            if (complete != null)
                complete(state); // Return Data                       
        }),
        ((string code) => { // Error
            if (error != null)
                error(code);
        })));
    }

    //============================================================
    //  @class      Ocugine_Reports
    //  @method     GetPerformanceReport()
    //  @type       Static Void
    //  @usage      Get performance report by id
    //  @args       (double) report_id - Performance report ID
    //              (void) complete - Complete Callback
    //              (void) error - Error Callback
    //  @return     none
    //============================================================
    public delegate void OnGetPerformanceReportSuccess(PerformanceReportModel data); // Returns BaseModel
    public delegate void OnGetPerformanceReportError(string code); // Returns error code
    public void GetPerformanceReport(double report_id, OnGetPerformanceReportSuccess complete = null, OnGetPerformanceReportError error = null){
        WWWForm formContent = new WWWForm(); // Create WWW Form
        formContent.AddField("app_id", $"{sdk_instance.application.app_id}");      // Add Language
        formContent.AddField("app_key", $"{sdk_instance.application.app_key}");    // App Key
        formContent.AddField("uid", $"{report_id}");         // Language

        // Send Request
        StartCoroutine(sdk_instance.utils.SendRequest(OcugineSDK.PROTOCOL + OcugineSDK.SERVER + OcugineSDK.API_GATE + OcugineSDK.REPORTS_OBJECT + "/get_performance", formContent,
        ((string data) => { // Response
            PerformanceReportModel state = JsonUtility.FromJson<PerformanceReportModel>(data); // Deserialize Object
            if (complete != null)
                complete(state); // Return Data                       
        }),
        ((string code) => { // Error
            if (error != null)
                error(code);
        })));
    }

    //============================================================
    //  @class      Ocugine_Reports
    //  @method     SendErrorReport()
    //  @type       Static Void
    //  @usage      Send error report
    //  @args       (string) name - Name of error report
    //              (string) code - Code of error report
    //              (string) body - Body of error report
    //              (int) critical - Criticality of error rebort
    //              (string) platform - Platform
    //              (void) complete - Complete Callback
    //              (void) error - Error Callback
    //  @return     none
    //============================================================
    public delegate void OnSendReportSuccess(BaseModel data); // Returns BaseModel
    public delegate void OnSendReportError(string code); // Returns error code
    public void SendErrorReport(string name, string report_code, string body, bool critical, string platform, OnSendReportSuccess complete = null, OnSendReportError error = null){
        WWWForm formContent = new WWWForm(); // Create WWW Form
        formContent.AddField("app_id", $"{sdk_instance.application.app_id}");      // Add Language
        formContent.AddField("app_key", $"{sdk_instance.application.app_key}");    // App Key
        formContent.AddField("lang", $"{sdk_instance.settings.language}");         // Language
        formContent.AddField("name", $"{name}");         // Variable
        formContent.AddField("code", $"{report_code}");         // Variable
        formContent.AddField("body", $"{body}");         // Variable
        formContent.AddField("critical", $"{critical}");         // Variable
        formContent.AddField("platform", $"{platform}");         // Variable

        // Send Request
        StartCoroutine(sdk_instance.utils.SendRequest(OcugineSDK.PROTOCOL + OcugineSDK.SERVER + OcugineSDK.API_GATE + OcugineSDK.REPORTS_OBJECT + "/send_error", formContent,
        ((string data) => { // Response
            BaseModel state = JsonUtility.FromJson<BaseModel>(data); // Deserialize Object
            if (complete != null)
                complete(state); // Return Data                       
        }),
        ((string code) => { // Error
            if (error != null)
                error(code);
        })));
    }

    //============================================================
    //  @class      Ocugine_Reports
    //  @method     SendErrorReport()
    //  @type       Static Void
    //  @usage      Send performance report
    //  @args       (string) name - Name of performance report
    //              (string) body - Text of performance report
    //              (void) complete - Complete Callback
    //              (void) error - Error Callback
    //  @return     none
    //============================================================
    public void SendPerformanceReport(string name, string body, OnSendReportSuccess complete = null, OnSendReportError error = null){
        WWWForm formContent = new WWWForm(); // Create WWW Form
        formContent.AddField("app_id", $"{sdk_instance.application.app_id}");      // Add Language
        formContent.AddField("app_key", $"{sdk_instance.application.app_key}");    // App Key
        formContent.AddField("lang", $"{sdk_instance.settings.language}");         // Language
        formContent.AddField("name", $"{name}");         // Variable
        formContent.AddField("body", $"{body}");         // Variable

        // Send Request
        StartCoroutine(sdk_instance.utils.SendRequest(OcugineSDK.PROTOCOL + OcugineSDK.SERVER + OcugineSDK.API_GATE + OcugineSDK.REPORTS_OBJECT + "/send_performance", formContent,
        ((string data) => { // Response
            BaseModel state = JsonUtility.FromJson<BaseModel>(data); // Deserialize Object
            if (complete != null)
                complete(state); // Return Data                       
        }),
        ((string code) => { // Error
            if (error != null)
                error(code);
        })));
    }
}

//===================================================
//  Ocugine Unity SDK Users Class
//===================================================
[AddComponentMenu("Ocugine SDK/Users Module")]
[RequireComponent(typeof(OcugineSDK))]
public class Ocugine_Users : MonoBehaviour{
    // Public Params
    public UserModel current_user = null; // Current User

    // Private Params
    private OcugineSDK sdk_instance = null;     // Ocugine SDK Instance

    //============================================================
    //  @class      Ocugine_Users
    //  @method     InitializeModule()
    //  @type       Public Void
    //  @usage      Initialize Module
    //  @args       (OcugineSDK) instance - Parent Instance Component
    //  @return     none
    //============================================================
    public void InitializeModule(OcugineSDK instance){
        sdk_instance = instance; // Set Ocugine SDK Instance
    }

    //============================================================
    //  @class      Ocugine_Users
    //  @method     Awake()
    //  @type       Internal Void
    //  @usage      Call before scene initialized
    //  @args       none
    //  @return     none
    //============================================================
    void Awake()
    {

    }

    //============================================================
    //  @class      Ocugine_Users
    //  @method     Start()
    //  @type       Internal Void
    //  @usage      Call when scene initialized
    //  @args       none
    //  @return     none
    //============================================================
    void Start()
    {

    }

    //============================================================
    //  @class      Ocugine_Users
    //  @method     Start()
    //  @type       Internal Void
    //  @usage      Call every tick
    //  @args       none
    //  @return     none
    //============================================================
    void Update()
    {

    }

    /*=====================[PROFILES]=====================*/

    //============================================================
    //  @class      Ocugine_Users
    //  @method     GetUsersList()
    //  @type       Static Void
    //  @usage      Get users list
    //  @args       (double) page - Page number    
    //              (void) complete - Complete Callback
    //              (void) error - Error Callback
    //  @return     none
    //============================================================
    public delegate void OnGetUsersListSuccess(UsersListModel data); // Returns OAuthTokenModel
    public delegate void OnGetUsersListError(string code); // Returns error code
    public void GetUsersList(double page = 1, OnGetUsersListSuccess complete = null, OnGetUsersListError error = null)
    {
        WWWForm formContent = new WWWForm(); // Create WWW Form
        formContent.AddField("app_id", $"{sdk_instance.application.app_id}");      // Add Language
        formContent.AddField("app_key", $"{sdk_instance.application.app_key}");    // App Key
        formContent.AddField("lang", $"{sdk_instance.settings.language}");         // Language
        formContent.AddField("page", $"{page}");    // 

        // Send Request
        StartCoroutine(sdk_instance.utils.SendRequest(OcugineSDK.PROTOCOL + OcugineSDK.SERVER + OcugineSDK.API_GATE + OcugineSDK.USERS_OBJECT + "/get_users_list", formContent,
        ((string data) => { // Response
            UsersListModel state = JsonUtility.FromJson<UsersListModel>(data); // Deserialize Object
            if (complete != null)
                complete(state); // Return Data                       
        }),
        ((string code) => { // Error
            if (error != null)
                error(code);
        })));
    }

    //============================================================
    //  @class      Ocugine_Users
    //  @method     FindUser()
    //  @type       Static Void
    //  @usage      Find user by Name and Surname
    //  @args       (string) search - Search request
    //              (double) page - Search page
    //              (void) complete - Complete Callback
    //              (void) error - Error Callback 
    //  @return     none
    //============================================================
    public void FindUser(string search = "", int page = 1, OnGetUsersListSuccess complete = null, OnGetUsersListError error = null)
    {
        WWWForm formContent = new WWWForm(); // Create WWW Form
        formContent.AddField("app_id", $"{sdk_instance.application.app_id}");      // Add Language
        formContent.AddField("app_key", $"{sdk_instance.application.app_key}");    // App Key
        formContent.AddField("lang", $"{sdk_instance.settings.language}");         // Language
        formContent.AddField("search", $"{search}");    //
        formContent.AddField("page", $"{page}");    // 

        // Send Request
        StartCoroutine(sdk_instance.utils.SendRequest(OcugineSDK.PROTOCOL + OcugineSDK.SERVER + OcugineSDK.API_GATE + OcugineSDK.USERS_OBJECT + "/find_user", formContent,
        ((string data) => { // Response
            UsersListModel state = JsonUtility.FromJson<UsersListModel>(data); // Deserialize Object
            if (complete != null)
                complete(state); // Return Data                       
        }),
        ((string code) => { // Error
            if (error != null)
                error(code);
        })));
    }

    //============================================================
    //  @class      Ocugine_Users
    //  @method     GetUserData()
    //  @type       Static Void
    //  @usage      Get data of current user
    //  @args       (void) complete - Complete Callback
    //              (void) error - Error Callback
    //  @return     none
    //============================================================
    public delegate void OnGetUserDataSuccess(UserModel data); // Returns OAuthTokenModel
    public delegate void OnGetUserDataError(string code); // Returns error code
    public void GetUserData(OnGetUserDataSuccess complete = null, OnGetUserDataError error = null)
    {
        WWWForm formContent = new WWWForm(); // Create WWW Form
        formContent.AddField("app_id", $"{sdk_instance.application.app_id}");      // Add Language
        formContent.AddField("app_key", $"{sdk_instance.application.app_key}");    // App Key
        formContent.AddField("lang", $"{sdk_instance.settings.language}");         // Language
        if(sdk_instance.auth != null) formContent.AddField("access_token", $"{sdk_instance.auth.credentials.token}"); // Token

        // Send Request
        StartCoroutine(sdk_instance.utils.SendRequest(OcugineSDK.PROTOCOL + OcugineSDK.SERVER + OcugineSDK.API_GATE + OcugineSDK.USERS_OBJECT + "/get_user_data", formContent,
        ((string data) => { // Response
            UserModel state = JsonUtility.FromJson<UserModel>(data); // Deserialize Object
            current_user = state;
            if (complete != null)
                complete(state); // Return Data                       
        }),
        ((string code) => { // Error
            if (error != null)
                error(code);
        })));
    }

    //============================================================
    //  @class      Ocugine_Users
    //  @method     GetUserByID()
    //  @type       Static Void
    //  @usage      Get data of user by id
    //  @args       (double) profile_uid - User id
    //              (void) complete - Complete Callback
    //              (void) error - Error Callback
    //  @return     none
    //============================================================
    public void GetUserByID(double profile_uid, OnGetUserDataSuccess complete = null, OnGetUserDataError error = null)
    {
        WWWForm formContent = new WWWForm(); // Create WWW Form
        formContent.AddField("app_id", $"{sdk_instance.application.app_id}");      // Add Language
        formContent.AddField("app_key", $"{sdk_instance.application.app_key}");    // App Key
        formContent.AddField("lang", $"{sdk_instance.settings.language}");         // Language
        formContent.AddField("profile_uid", $"{profile_uid}"); //

        // Send Request
        StartCoroutine(sdk_instance.utils.SendRequest(OcugineSDK.PROTOCOL + OcugineSDK.SERVER + OcugineSDK.API_GATE + OcugineSDK.USERS_OBJECT + "/get_user_by_id", formContent,
        ((string data) => { // Response
            UserModel state = JsonUtility.FromJson<UserModel>(data); // Deserialize Object
            if (complete != null)
                complete(state); // Return Data                       
        }),
        ((string code) => { // Error
            if (error != null)
                error(code);
        })));
    }

    //============================================================
    //  @class      Ocugine_Users
    //  @method     GetBanState()
    //  @type       Static Void
    //  @usage      Get data of ban by id
    //  @args       (double) profile_uid - User id
    //              (void) complete - Complete Callback
    //              (void) error - Error Callback
    //  @return     none
    //============================================================
    public delegate void OnGetBanStateSuccess(BanStateModel data); // Returns OAuthTokenModel
    public delegate void OnGetBanStateError(string code); // Returns error code
    public void GetBanState(double profile_uid, OnGetBanStateSuccess complete = null, OnGetBanStateError error = null)
    {
        WWWForm formContent = new WWWForm(); // Create WWW Form
        formContent.AddField("app_id", $"{sdk_instance.application.app_id}");      // Add Language
        formContent.AddField("app_key", $"{sdk_instance.application.app_key}");    // App Key
        formContent.AddField("lang", $"{sdk_instance.settings.language}");         // Language
        formContent.AddField("profile_uid", $"{profile_uid}"); //
        
        // Send Request
        StartCoroutine(sdk_instance.utils.SendRequest(OcugineSDK.PROTOCOL + OcugineSDK.SERVER + OcugineSDK.API_GATE + OcugineSDK.USERS_OBJECT + "/get_ban_state", formContent,
        ((string data) => { // Response
            BanStateModel state = JsonUtility.FromJson<BanStateModel>(data); // Deserialize Object
            if (complete != null)
                complete(state); // Return Data                       
        }),
        ((string code) => { // Error
            if (error != null)
                error(code);
        })));
    }

    /*=====================[GROUPS]=====================*/

    //============================================================
    //  @class      Ocugine_Users
    //  @method     GetGroupData()
    //  @type       Static Void
    //  @usage      Get user group data
    //  @args       (double) group_id - Id of group
    //              (void) complete - Complete Callback
    //              (void) error - Error Callback  
    //  @return     none
    //============================================================
    public delegate void OnGetGroupDataSuccess(GroupModel data); // Returns OAuthTokenModel
    public delegate void OnGetGroupDataError(string code); // Returns error code
    public void GetGroupData(double group_id, OnGetGroupDataSuccess complete = null, OnGetGroupDataError error = null)
    {
        WWWForm formContent = new WWWForm(); // Create WWW Form
        formContent.AddField("app_id", $"{sdk_instance.application.app_id}");      // Add Language
        formContent.AddField("app_key", $"{sdk_instance.application.app_key}");    // App Key
        formContent.AddField("group_id", $"{group_id}");         // Language
        formContent.AddField("lang", $"{sdk_instance.settings.language}");         // Language

        // Send Request
        StartCoroutine(sdk_instance.utils.SendRequest(OcugineSDK.PROTOCOL + OcugineSDK.SERVER + OcugineSDK.API_GATE + OcugineSDK.USERS_OBJECT + "/get_group_data", formContent,
        ((string data) => { // Response
            GroupModel state = JsonUtility.FromJson<GroupModel>(data); // Deserialize Object
            if (complete != null)
                complete(state); // Return Data                       
        }),
        ((string code) => { // Error
            if (error != null)
                error(code);
        })));
    }

    //============================================================
    //  @class      Ocugine_Users
    //  @method     GetGroupList()
    //  @type       Static Void
    //  @usage      Get user group list
    //  @args       (void) complete - Complete Callback
    //              (void) error - Error Callback
    //  @return     none
    //============================================================
    public delegate void OnGetGroupListSuccess(GroupListModel data); // Returns OAuthTokenModel
    public delegate void OnGetGroupListError(string code); // Returns error code
    public void GetGroupList(OnGetGroupListSuccess complete = null, OnGetGroupListError error = null)
    {
        WWWForm formContent = new WWWForm(); // Create WWW Form
        formContent.AddField("app_id", $"{sdk_instance.application.app_id}");      // Add Language
        formContent.AddField("app_key", $"{sdk_instance.application.app_key}");    // App Key
        formContent.AddField("lang", $"{sdk_instance.settings.language}");         // Language

        // Send Request
        StartCoroutine(sdk_instance.utils.SendRequest(OcugineSDK.PROTOCOL + OcugineSDK.SERVER + OcugineSDK.API_GATE + OcugineSDK.USERS_OBJECT + "/get_group_list", formContent,
        ((string data) => { // Response
            GroupListModel state = JsonUtility.FromJson<GroupListModel>(data); // Deserialize Object
            if (complete != null)
                complete(state); // Return Data                       
        }),
        ((string code) => { // Error
            if (error != null)
                error(code);
        })));
    }

    //============================================================
    //  @class      Ocugine_Users
    //  @method     SetGroup()
    //  @type       Static Void
    //  @usage      Set user group
    //  @args       (double) profile_uid - Id of user
    //              (double) group_id - Id of group
    //              (void) complete - Complete Callback
    //              (void) error - Error Callback
    //  @return     none
    //============================================================
    public delegate void OnSetGroupListSuccess(BaseModel data); // Returns OAuthTokenModel
    public delegate void OnSetGroupListError(string code); // Returns error code
    public void SetGroup(double profile_uid, double group_id, OnSetGroupListSuccess complete = null, OnSetGroupListError error = null)
    {
        WWWForm formContent = new WWWForm(); // Create WWW Form
        formContent.AddField("app_id", $"{sdk_instance.application.app_id}");      // Add Language
        formContent.AddField("app_key", $"{sdk_instance.application.app_key}");    // App Key
        formContent.AddField("lang", $"{sdk_instance.settings.language}");         // Language
        if (sdk_instance.auth != null) formContent.AddField("access_token", $"{sdk_instance.auth.credentials.token}");
        formContent.AddField("profile_uid", $"{profile_uid}");
        formContent.AddField("group_id", $"{group_id}");

        // Send Request
        StartCoroutine(sdk_instance.utils.SendRequest(OcugineSDK.PROTOCOL + OcugineSDK.SERVER + OcugineSDK.API_GATE + OcugineSDK.USERS_OBJECT + "/set_group", formContent,
        ((string data) => { // Response
            BaseModel state = JsonUtility.FromJson<BaseModel>(data); // Deserialize Object
            if (complete != null)
                complete(state); // Return Data                       
        }),
        ((string code) => { // Error
            if (error != null)
                error(code);
        })));
    }

    /*=====================[ADVANCED FIELDS]=====================*/

    //============================================================
    //  @class      Ocugine_Users
    //  @method     GetAdvancedFields()
    //  @type       Static Void
    //  @usage      Get advanced fields for users
    //  @args       (double) page - Page
    //              (void) complete - Complete Callback
    //              (void) error - Error Callback
    //              (string) search - Search request
    //  @return     none
    //============================================================
    public delegate void OnGetAdvancedFieldsSuccess(AdvancedFieldsListModel data); // Returns SupportTopicsListModel
    public delegate void OnGetAdvancedFieldsError(string code); // Returns error code
    public void GetAdvancedFields(string search = "", double page = 1, OnGetAdvancedFieldsSuccess complete = null, OnGetAdvancedFieldsError error = null)
    {
        WWWForm formContent = new WWWForm(); // Create WWW Form
        formContent.AddField("app_id", $"{sdk_instance.application.app_id}");       // Add Language
        formContent.AddField("app_key", $"{sdk_instance.application.app_key}");     // App Key
        formContent.AddField("page", $"{page}");                                    // 
        formContent.AddField("search", $"{search}");                                // 

        // Send Request
        StartCoroutine(sdk_instance.utils.SendRequest(OcugineSDK.PROTOCOL + OcugineSDK.SERVER + OcugineSDK.API_GATE + OcugineSDK.USERS_OBJECT + "/get_fields", formContent,
        ((string data) => { // Response
            AdvancedFieldsListModel state = JsonUtility.FromJson<AdvancedFieldsListModel>(data); // Deserialize Object
            if (complete != null)
                complete(state); // Return Data                       
        }),
        ((string code) => { // Error
            if (error != null)
                error(code);
        })));
    }

    //============================================================
    //  @class      Ocugine_Users
    //  @method     SetAdvancedField()
    //  @type       Static Void
    //  @usage      Set advanced field value
    //  @args       (double) profile_uid - Id of user
    //              (double) field_id - Id of field
    //              (string) value - Value for field
    //              (void) complete - Complete Callback
    //              (void) error - Error Callback
    //  @return     none
    //============================================================
    public delegate void OnSetAdvancedFieldSuccess(BaseModel data); // Returns BaseModel
    public delegate void OnSetAdvancedFieldError(string code); // Returns error code
    public void SetAdvancedField(double profile_uid, double field_id, string value, OnSetAdvancedFieldSuccess complete = null, OnSetAdvancedFieldError error = null)
    {    
        WWWForm formContent = new WWWForm(); // Create WWW Form
        formContent.AddField("app_id", $"{sdk_instance.application.app_id}");      // Add Language
        formContent.AddField("app_key", $"{sdk_instance.application.app_key}");    // App Key
        formContent.AddField("lang", $"{sdk_instance.settings.language}");         // Language
        if (sdk_instance.auth != null) formContent.AddField("access_token", $"{sdk_instance.auth.credentials.token}");
        formContent.AddField("profile_uid", $"{profile_uid}");  
        formContent.AddField("field_id", $"{field_id}");        
        formContent.AddField("value", $"{value}");                   

        // Send Request
        StartCoroutine(sdk_instance.utils.SendRequest(OcugineSDK.PROTOCOL + OcugineSDK.SERVER + OcugineSDK.API_GATE + OcugineSDK.USERS_OBJECT + "/set_field", formContent,
        ((string data) => { // Response
            BaseModel state = JsonUtility.FromJson<BaseModel>(data); // Deserialize Object
            if (complete != null)
                complete(state); // Return Data                       
        }),
        ((string code) => { // Error
            if (error != null)
                error(code);
        })));
    }

    /*=====================[SUPPORT]=====================*/

    //============================================================
    //  @class      Ocugine_Users
    //  @method     GetSupportCategories()
    //  @type       Static Void
    //  @usage      Get all support categories
    //  @args       (void) complete - Complete Callback
    //              (void) error - Error Callback
    //  @return     none
    //============================================================
    public delegate void OnGetSupportCategoriesSuccess(SupportCategoriesListModel data); // Returns SupportCategoriesListModel
    public delegate void OnGetSupportCategoriesError(string code); // Returns error code
    public void GetSupportCategories(OnGetSupportCategoriesSuccess complete = null, OnGetSupportCategoriesError error = null)
    {
        WWWForm formContent = new WWWForm(); // Create WWW Form
        formContent.AddField("app_id", $"{sdk_instance.application.app_id}");      // Add Language
        formContent.AddField("app_key", $"{sdk_instance.application.app_key}");    // App Key
        formContent.AddField("lang", $"{sdk_instance.settings.language}");         // Language

        // Send Request
        StartCoroutine(sdk_instance.utils.SendRequest(OcugineSDK.PROTOCOL + OcugineSDK.SERVER + OcugineSDK.API_GATE + OcugineSDK.USERS_OBJECT + "/get_support_categories", formContent,
        ((string data) => { // Response
            SupportCategoriesListModel state = JsonUtility.FromJson<SupportCategoriesListModel>(data); // Deserialize Object
            if (complete != null)
                complete(state); // Return Data                       
        }),
        ((string code) => { // Error
            if (error != null)
                error(code);
        })));
    }

    //============================================================
    //  @class      Ocugine_Users
    //  @method     GetSupportTopics()
    //  @type       Static Void
    //  @usage      Get all support topics
    //  @args       (category_uid) topic_uid - Category id
    //              (double) page - Page
    //              (void) complete - Complete Callback
    //              (void) error - Error Callback
    //              (string) search - Search request
    //  @return     none
    //============================================================
    public delegate void OnGetSupportTopicsSuccess(SupportTopicsListModel data); // Returns SupportTopicsListModel
    public delegate void OnGetSupportTopicsError(string code); // Returns error code
    public void GetSupportTopics(double category_uid, string search = "", double page = 1, OnGetSupportTopicsSuccess complete = null, OnGetSupportTopicsError error = null)
    {
        WWWForm formContent = new WWWForm(); // Create WWW Form
        formContent.AddField("app_id", $"{sdk_instance.application.app_id}");       // Add Language
        formContent.AddField("app_key", $"{sdk_instance.application.app_key}");     // App Key
        formContent.AddField("category_uid", $"{category_uid}");                    // 
        formContent.AddField("page", $"{page}");                                    // 
        formContent.AddField("search", $"{search}");                                // 

        // Send Request
        StartCoroutine(sdk_instance.utils.SendRequest(OcugineSDK.PROTOCOL + OcugineSDK.SERVER + OcugineSDK.API_GATE + OcugineSDK.USERS_OBJECT + "/get_support_topics", formContent,
        ((string data) => { // Response
            SupportTopicsListModel state = JsonUtility.FromJson<SupportTopicsListModel>(data); // Deserialize Object
            if (complete != null)
                complete(state); // Return Data                       
        }),
        ((string code) => { // Error
            if (error != null)
                error(code);
        })));
    }

    //============================================================
    //  @class      Ocugine_Users
    //  @method     GetSupportMessages()
    //  @type       Static Void
    //  @usage      Get all support messages of topic
    //  @args       (void) topic_uid - Topic id
    //              (double) page - Page
    //              (void) complete - Complete Callback
    //              (void) error - Error Callback
    //              (string) search - Search request
    //  @return     none
    //============================================================
    public delegate void OnGetSupportMessagesSuccess(SupportMessagesListModel data); // Returns SupportTopicsListModel
    public delegate void OnGetSupportMessagesError(string code); // Returns error code
    public void GetSupportMessages(double topic_uid, string search = "", double page = 1, OnGetSupportMessagesSuccess complete = null, OnGetSupportMessagesError error = null)
    {
        WWWForm formContent = new WWWForm(); // Create WWW Form
        formContent.AddField("app_id", $"{sdk_instance.application.app_id}");       // Add Language
        formContent.AddField("app_key", $"{sdk_instance.application.app_key}");     // App Key
        formContent.AddField("topic_uid", $"{topic_uid}");                    // 
        formContent.AddField("page", $"{page}");                                    // 
        formContent.AddField("search", $"{search}");                                // 

        // Send Request
        StartCoroutine(sdk_instance.utils.SendRequest(OcugineSDK.PROTOCOL + OcugineSDK.SERVER + OcugineSDK.API_GATE + OcugineSDK.USERS_OBJECT + "/get_support_messages", formContent,
        ((string data) => { // Response
            SupportMessagesListModel state = JsonUtility.FromJson<SupportMessagesListModel>(data); // Deserialize Object
            if (complete != null)
                complete(state); // Return Data                       
        }),
        ((string code) => { // Error
            if (error != null)
                error(code);
        })));
    }

    //============================================================
    //  @class      Ocugine_Users
    //  @method     CreateSupportTopic()
    //  @type       Static Void
    //  @usage      Create support topic in forum
    //  @args       (double) category_uid - Id of category
    //              (string) name - Title of topic
    //              (string) body - Main text of topic
    //              (void) complete - Complete Callback
    //              (void) error - Error Callback
    //  @return     none
    //============================================================
    public delegate void OnTopicActionSuccess(BaseModel data); // Returns BaseModel
    public delegate void OnTopicActionError(string code); // Returns error code
    public void CreateSupportTopic(double category_uid, string name, string body, OnTopicActionSuccess complete = null, OnTopicActionError error = null)
    {
        WWWForm formContent = new WWWForm(); // Create WWW Form
        formContent.AddField("app_id", $"{sdk_instance.application.app_id}");      // Add Language
        formContent.AddField("app_key", $"{sdk_instance.application.app_key}");    // App Key
        formContent.AddField("lang", $"{sdk_instance.settings.language}");         // Language
        if (sdk_instance.auth != null) formContent.AddField("access_token", $"{sdk_instance.auth.credentials.token}");
        formContent.AddField("category_uid", $"{category_uid}");      // 
        formContent.AddField("name", $"{name}");                    // 
        formContent.AddField("body", $"{body}");                    // 

        // Send Request
        StartCoroutine(sdk_instance.utils.SendRequest(OcugineSDK.PROTOCOL + OcugineSDK.SERVER + OcugineSDK.API_GATE + OcugineSDK.USERS_OBJECT + "/create_topic", formContent,
        ((string data) => { // Response
            BaseModel state = JsonUtility.FromJson<BaseModel>(data); // Deserialize Object
            if (complete != null)
                complete(state); // Return Data                       
        }),
        ((string code) => { // Error
            if (error != null)
                error(code);
        })));
    }

    //============================================================
    //  @class      Ocugine_Users
    //  @method     UpdateSupportTopic()
    //  @type       Static Void
    //  @usage      Update support topic data
    //  @args       (double) category_uid - Category id
    //              (double) topic_uid - Topic id
    //              (string) name - Topic title
    //              (string) body - Topic text
    //              (void) complete - Complete Callback
    //              (void) error - Error Callback
    //  @return     none
    //============================================================
    public void UpdateSupportTopic(double category_uid, double topic_uid, string name, string body, OnTopicActionSuccess complete = null, OnTopicActionError error = null)
    {
        WWWForm formContent = new WWWForm(); // Create WWW Form
        formContent.AddField("app_id", $"{sdk_instance.application.app_id}");      // Add Language
        formContent.AddField("app_key", $"{sdk_instance.application.app_key}");    // App Key
        formContent.AddField("lang", $"{sdk_instance.settings.language}");         // Language
        if (sdk_instance.auth != null) formContent.AddField("access_token", $"{sdk_instance.auth.credentials.token}");
        formContent.AddField("category_uid", $"{category_uid}");      // 
        formContent.AddField("topic_uid", $"{topic_uid}");      // 
        formContent.AddField("name", $"{name}");                    // 
        formContent.AddField("body", $"{body}");                    // 

        // Send Request
        StartCoroutine(sdk_instance.utils.SendRequest(OcugineSDK.PROTOCOL + OcugineSDK.SERVER + OcugineSDK.API_GATE + OcugineSDK.USERS_OBJECT + "/update_topic", formContent,
        ((string data) => { // Response
            BaseModel state = JsonUtility.FromJson<BaseModel>(data); // Deserialize Object
            if (complete != null)
                complete(state); // Return Data                       
        }),
        ((string code) => { // Error
            if (error != null)
                error(code);
        })));
    }

    //============================================================
    //  @class      Ocugine_Users
    //  @method     CloseSupportTopic()
    //  @type       Static Void
    //  @usage      Close support topic
    //  @args       (double) topic_uid - Id of topic
    //              (void) complete - Complete Callback
    //              (void) error - Error Callback
    //  @return     none
    //============================================================
    public void CloseSupportTopic(double topic_uid, OnTopicActionSuccess complete = null, OnTopicActionError error = null)
    {

        WWWForm formContent = new WWWForm(); // Create WWW Form
        formContent.AddField("app_id", $"{sdk_instance.application.app_id}");      // Add Language
        formContent.AddField("app_key", $"{sdk_instance.application.app_key}");    // App Key
        formContent.AddField("lang", $"{sdk_instance.settings.language}");         // Language
        if (sdk_instance.auth != null) formContent.AddField("access_token", $"{sdk_instance.auth.credentials.token}");
        formContent.AddField("topic_uid", $"{topic_uid}");                          // 

        // Send Request
        StartCoroutine(sdk_instance.utils.SendRequest(OcugineSDK.PROTOCOL + OcugineSDK.SERVER + OcugineSDK.API_GATE + OcugineSDK.USERS_OBJECT + "/close_topic", formContent,
        ((string data) => { // Response
            BaseModel state = JsonUtility.FromJson<BaseModel>(data); // Deserialize Object
            if (complete != null)
                complete(state); // Return Data                       
        }),
        ((string code) => { // Error
            if (error != null)
                error(code);
        })));
    }

    //============================================================
    //  @class      Ocugine_Users
    //  @method     SendSupportMessage()
    //  @type       Static Void
    //  @usage      Send support message
    //  @args       (double) topic_uid - Id of topic
    //              (string) message - Message for topic
    //              (void) complete - Complete Callback
    //              (void) error - Error Callback
    //  @return     none
    //============================================================
    public void SendSupportMessage(double topic_uid, string message, OnTopicActionSuccess complete = null, OnTopicActionError error = null)
    {
        WWWForm formContent = new WWWForm(); // Create WWW Form
        formContent.AddField("app_id", $"{sdk_instance.application.app_id}");      // Add Language
        formContent.AddField("app_key", $"{sdk_instance.application.app_key}");    // App Key
        formContent.AddField("lang", $"{sdk_instance.settings.language}");         // Language
        if (sdk_instance.auth != null) formContent.AddField("access_token", $"{sdk_instance.auth.credentials.token}");
        formContent.AddField("topic_uid", $"{topic_uid}");      // 
        formContent.AddField("message", $"{message}");      // 

        // Send Request
        StartCoroutine(sdk_instance.utils.SendRequest(OcugineSDK.PROTOCOL + OcugineSDK.SERVER + OcugineSDK.API_GATE + OcugineSDK.USERS_OBJECT + "/send_support_message", formContent,
        ((string data) => { // Response
            BaseModel state = JsonUtility.FromJson<BaseModel>(data); // Deserialize Object
            if (complete != null)
                complete(state); // Return Data                       
        }),
        ((string code) => { // Error
            if (error != null)
                error(code);
        })));
    }

    /*=====================[CHATS]=====================*/

    //============================================================
    //  @class      Ocugine_Users
    //  @method     GetChatRooms()
    //  @type       Static Void
    //  @usage      Get all chat rooms
    //  @args       (void) complete - Complete Callback
    //              (void) error - Error Callback
    //  @return     none
    //============================================================
    public delegate void OnGetChatRoomsSuccess(ChatRoomsListModel data); // Returns ChatRoomsListModel
    public delegate void OnGetChatRoomsError(string code); // Returns error code
    public void GetChatRooms(OnGetChatRoomsSuccess complete = null, OnGetChatRoomsError error = null)
    {
        WWWForm formContent = new WWWForm(); // Create WWW Form
        formContent.AddField("app_id", $"{sdk_instance.application.app_id}");      // Add Language
        formContent.AddField("app_key", $"{sdk_instance.application.app_key}");    // App Key
        formContent.AddField("lang", $"{sdk_instance.settings.language}");         // Language

        // Send Request
        StartCoroutine(sdk_instance.utils.SendRequest(OcugineSDK.PROTOCOL + OcugineSDK.SERVER + OcugineSDK.API_GATE + OcugineSDK.USERS_OBJECT + "/get_available_rooms", formContent,
        ((string data) => { // Response
            ChatRoomsListModel state = JsonUtility.FromJson<ChatRoomsListModel>(data); // Deserialize Object
            if (complete != null)
                complete(state); // Return Data                       
        }),
        ((string code) => { // Error
            if (error != null)
                error(code);
        })));
    }

    //============================================================
    //  @class      Ocugine_Users
    //  @method     GetChatMessages()
    //  @type       Static Void
    //  @usage      Get chat messages
    //  @args       (double) room_id - Id of room
    //              (void) complete - Complete Callback
    //              (void) error - Error Callback
    //  @return     none
    //============================================================
    public delegate void OnGetChatMessagesSuccess(ChatMessagesListModel data); // Returns ChatMessagesListModel
    public delegate void OnGetChatMessagesError(string code); // Returns error code
    public void GetChatMessages(double room_id, OnGetChatMessagesSuccess complete = null, OnGetChatMessagesError error = null)
    {
        WWWForm formContent = new WWWForm(); // Create WWW Form
        formContent.AddField("app_id", $"{sdk_instance.application.app_id}");      // Add Language
        formContent.AddField("app_key", $"{sdk_instance.application.app_key}");    // App Key
        formContent.AddField("lang", $"{sdk_instance.settings.language}");         // Language
        formContent.AddField("room_id", $"{room_id}");         // Language

        // Send Request
        StartCoroutine(sdk_instance.utils.SendRequest(OcugineSDK.PROTOCOL + OcugineSDK.SERVER + OcugineSDK.API_GATE + OcugineSDK.USERS_OBJECT + "/get_chat_messages", formContent,
        ((string data) => { // Response
            ChatMessagesListModel state = JsonUtility.FromJson<ChatMessagesListModel>(data); // Deserialize Object
            if (complete != null)
                complete(state); // Return Data                       
        }),
        ((string code) => { // Error
            if (error != null)
                error(code);
        })));
    }

    //============================================================
    //  @class      Ocugine_Users
    //  @method     SendChatMessage()
    //  @type       Static Void
    //  @usage      Send chat room message
    //  @args       (double) room_id - Id of room
    //              (string) message - Message for chat
    //              (void) complete - Complete Callback
    //              (void) error - Error Callback
    //  @return     none
    //============================================================
    public delegate void OnSendChatMessageSuccess(BaseModel data); // Returns BaseModel
    public delegate void OnSendChatMessageError(string code); // Returns error code
    public void SendChatMessage(double room_id, string message, OnSendChatMessageSuccess complete =  null, OnSendChatMessageError error = null)
    {
        WWWForm formContent = new WWWForm(); // Create WWW Form
        formContent.AddField("app_id", $"{sdk_instance.application.app_id}");      // Add Language
        formContent.AddField("app_key", $"{sdk_instance.application.app_key}");    // App Key
        formContent.AddField("lang", $"{sdk_instance.settings.language}");         // Language
        if (sdk_instance.auth != null) formContent.AddField("access_token", $"{sdk_instance.auth.credentials.token}");
        formContent.AddField("room_id", $"{room_id}");         // 
        formContent.AddField("message", $"{message}");         // 

        // Send Request
        StartCoroutine(sdk_instance.utils.SendRequest(OcugineSDK.PROTOCOL + OcugineSDK.SERVER + OcugineSDK.API_GATE + OcugineSDK.USERS_OBJECT + "/send_cmessage", formContent,
        ((string data) => { // Response
            BaseModel state = JsonUtility.FromJson<BaseModel>(data); // Deserialize Object
            if (complete != null)
                complete(state); // Return Data                       
        }),
        ((string code) => { // Error
            if (error != null)
                error(code);
        })));
    }

    /*=====================[REVIEWS]=====================*/

    //============================================================
    //  @class      Ocugine_Users
    //  @method     SendReview()
    //  @type       Static Void
    //  @usage      Send in-game review
    //  @args       (string) message - Message of review
    //              (double) stars - Estimation
    //              (void) complete - Complete Callback
    //              (void) error - Error Callback
    //  @return     none
    //============================================================
    public delegate void OnSendReviewSuccess(BaseModel data); // Returns BaseModel
    public delegate void OnSendReviewError(string code); // Returns error code
    public void SendReview(string message, double stars, OnSendReviewSuccess complete = null, OnSendReviewError error = null)
    {
        WWWForm formContent = new WWWForm(); // Create WWW Form
        formContent.AddField("app_id", $"{sdk_instance.application.app_id}");      // Add Language
        formContent.AddField("app_key", $"{sdk_instance.application.app_key}");    // App Key
        formContent.AddField("lang", $"{sdk_instance.settings.language}");         // Language
        if (sdk_instance.auth != null) formContent.AddField("access_token", $"{sdk_instance.auth.credentials.token}");
        formContent.AddField("message", $"{message}");
        formContent.AddField("stars", $"{stars}");

        // Send Request
        StartCoroutine(sdk_instance.utils.SendRequest(OcugineSDK.PROTOCOL + OcugineSDK.SERVER + OcugineSDK.API_GATE + OcugineSDK.USERS_OBJECT + "/send_review", formContent,
        ((string data) => { // Response
            RewiewsListModel state = JsonUtility.FromJson<RewiewsListModel>(data); // Deserialize Object
            if (complete != null)
                complete(state); // Return Data                       
        }),
        ((string code) => { // Error
            if (error != null)
                error(code);
        })));
    }

    //============================================================
    //  @class      Ocugine_Users
    //  @method     GetReviewsList()
    //  @type       Static Void
    //  @usage      Get all reviews
    //  @args       (double) page - Page
    //              (void) complete - Complete Callback
    //              (void) error - Error Callback
    //              (string) search - Search request
    //  @return     none
    //============================================================
    public delegate void OnGetReviewsListSuccess(RewiewsListModel data); // Returns RewiewsListModel
    public delegate void OnGetReviewsListError(string code); // Returns error code
    public void GetReviewsList(string search = "", double page = 1, OnGetReviewsListSuccess complete = null, OnGetReviewsListError error = null)
    {
        WWWForm formContent = new WWWForm(); // Create WWW Form
        formContent.AddField("app_id", $"{sdk_instance.application.app_id}");      // Add Language
        formContent.AddField("app_key", $"{sdk_instance.application.app_key}");    // App Key
        formContent.AddField("lang", $"{sdk_instance.settings.language}");         // Language
        formContent.AddField("page", $"{page}");         
        formContent.AddField("search", $"{search}");        

        // Send Request
        StartCoroutine(sdk_instance.utils.SendRequest(OcugineSDK.PROTOCOL + OcugineSDK.SERVER + OcugineSDK.API_GATE + OcugineSDK.USERS_OBJECT + "/get_reviews", formContent,
        ((string data) => { // Response
            RewiewsListModel state = JsonUtility.FromJson<RewiewsListModel>(data); // Deserialize Object
            if (complete != null)
                complete(state); // Return Data                       
        }),
        ((string code) => { // Error
            if (error != null)
                error(code);
        })));
    }

    /*=====================[POLICY]=====================*/

    //============================================================
    //  @class      Ocugine_Users
    //  @method     GetPolicyList()
    //  @type       Static Void
    //  @usage      Get policy list
    //              (void) complete - Complete Callback
    //              (void) error - Error Callback
    //  @return     none
    //============================================================
    public delegate void OnGetPolicyListSuccess(PolicyListModel data); // Returns OAuthTokenModel
    public delegate void OnGetPolicyListError(string code); // Returns error code
    public void GetPolicyList(OnGetPolicyListSuccess complete = null, OnGetPolicyListError error = null)
    {
        WWWForm formContent = new WWWForm(); // Create WWW Form
        formContent.AddField("app_id", $"{sdk_instance.application.app_id}");      // Add Language
        formContent.AddField("app_key", $"{sdk_instance.application.app_key}");    // App Key
        formContent.AddField("lang", $"{sdk_instance.settings.language}");         // Language

        // Send Request
        StartCoroutine(sdk_instance.utils.SendRequest(OcugineSDK.PROTOCOL + OcugineSDK.SERVER + OcugineSDK.API_GATE + OcugineSDK.USERS_OBJECT + "/get_policy_list", formContent,
        ((string data) => { // Response
            PolicyListModel state = JsonUtility.FromJson<PolicyListModel>(data); // Deserialize Object
            if (complete != null)
                complete(state); // Return Data                       
        }),
        ((string code) => { // Error
            if (error != null)
                error(code);
        })));
    }

    //============================================================
    //  @class      Ocugine_Users
    //  @method     GetPolicyInfo()
    //  @type       Static Void
    //  @usage      Get policy info
    //              (double) pid - Id of policy
    //              (void) complete - Complete Callback
    //              (void) error - Error Callback
    //  @return     none
    //============================================================
    public delegate void OnGetPolicyInfoSuccess(PolicyModel data); // Returns OAuthTokenModel
    public delegate void OnGetPolicyInfoError(string code); // Returns error code
    public void GetPolicyInfo(double pid, OnGetPolicyInfoSuccess complete = null, OnGetPolicyInfoError error = null)
    {
        WWWForm formContent = new WWWForm(); // Create WWW Form
        formContent.AddField("app_id", $"{sdk_instance.application.app_id}");      // Add Language
        formContent.AddField("app_key", $"{sdk_instance.application.app_key}");    // App Key
        formContent.AddField("lang", $"{sdk_instance.settings.language}");         // Language
        formContent.AddField("pid", $"{pid}");                                     // Policy id

        // Send Request
        StartCoroutine(sdk_instance.utils.SendRequest(OcugineSDK.PROTOCOL + OcugineSDK.SERVER + OcugineSDK.API_GATE + OcugineSDK.USERS_OBJECT + "/get_policy_info", formContent,
        ((string data) => { // Response
            PolicyModel state = JsonUtility.FromJson<PolicyModel>(data); // Deserialize Object
            if (complete != null)
                complete(state); // Return Data                       
        }),
        ((string code) => { // Error
            if (error != null)
                error(code);
        })));
    }

}

//===================================================
//  Ocugine Unity SDK Localization Class
//===================================================
[AddComponentMenu("Ocugine SDK/Localization Component")]
[RequireComponent(typeof(OcugineSDK))]
public class Ocugine_Localization : MonoBehaviour{
    // Public Params

    // Private Params
    private OcugineSDK sdk_instance = null;     // Ocugine SDK Instance
    private Dictionary<string, LanguageModel> LangInfoCache = new Dictionary<string, LanguageModel>(); // Localization cache
    private Dictionary<string, Dictionary<string, LocaleModel>> LocInfoCache = new Dictionary<string, Dictionary<string, LocaleModel>>(); // Localization cache
                                                                                                                    //
    private LanguagesListModel.DataModel LangListCache = new LanguagesListModel.DataModel(); // Languahe list cache
    private LocalesListModel.DataModel LocListCache = new LocalesListModel.DataModel(); // Localization list cache

    //============================================================
    //  @class      Ocugine_Localization
    //  @method     InitializeModule()
    //  @type       Public Void
    //  @usage      Initialize Module
    //  @args       (OcugineSDK) instance - Parent Instance Component
    //  @return     none
    //============================================================
    public void InitializeModule(OcugineSDK instance){
        sdk_instance = instance; // Set Ocugine SDK Instance
    }

    //============================================================
    //  @class      Localization
    //  @method     GetLang()
    //  @type       Static Async Void
    //  @usage      Get language info
    //  @args       (void) complete - Complete Callback
    //              (void) error - Error Callback
    //              (string) lang_code - Lang code
    //  @return     none
    //============================================================   
    public delegate void OnGetLangComplete(LanguageModel data);
    public delegate void OnGetLangError(string code);
    public void GetLang(string lang_code, OnGetLangComplete complete = null, OnGetLangError error = null) //  (bool) Get lang
    {
        if (LangInfoCache.ContainsKey(lang_code)) // If cached
        {
            if (complete != null)
                complete(LangInfoCache[lang_code]);
        }
        else // If not cached
        {
            WWWForm formContent = new WWWForm(); // Create WWW Form
            formContent.AddField("app_id", $"{sdk_instance.application.app_id}"); // App Id
            formContent.AddField("app_key", $"{sdk_instance.application.app_key}"); // App key
            formContent.AddField("code", $"{lang_code}"); // Code language          

            // Send Request
            StartCoroutine(sdk_instance.utils.SendRequest(OcugineSDK.PROTOCOL + OcugineSDK.SERVER + OcugineSDK.API_GATE + OcugineSDK.LOCALE_OBJECT + "/get_lang", formContent, ((string data) => { // All Right
                LanguageModel state = JsonUtility.FromJson<LanguageModel>(data); // Deserialize Object    
                LangInfoCache[lang_code] = state;
                if (complete != null)
                    complete(state);
            }), ((string msg) => { // Error
                if (error != null)
                    error(msg); // Send Error
            })));
        }
    }

    //============================================================
    //  @class      Localization
    //  @method     GetLangList()
    //  @type       Static Async Void
    //  @usage      Get lang list
    //  @args       (void) complete - Complete Callback
    //              (void) error - Error Callback
    //  @return     none
    //============================================================   
    public delegate void OnGetLangListComplete(LanguagesListModel data);
    public delegate void OnGetLangListError(string code);
    public void GetLangList(OnGetLangListComplete complete = null, OnGetLangListError error = null) // (bool) Get locale
    {
        if (LangListCache.list != null) // If has lang and locale in thot lang
        {
            if (complete != null)
                complete(new LanguagesListModel() { message = "Cache", data = LangListCache });
        }
        else
        {
            WWWForm formContent = new WWWForm(); // Create WWW Form
            formContent.AddField("app_id", $"{sdk_instance.application.app_id}"); // App Id
            formContent.AddField("app_key", $"{sdk_instance.application.app_key}"); // App key

            StartCoroutine(sdk_instance.utils.SendRequest(OcugineSDK.PROTOCOL + OcugineSDK.SERVER + OcugineSDK.API_GATE + OcugineSDK.LOCALE_OBJECT + "/get_lang_list", formContent, ((string data) => { // All Right
                LanguagesListModel state = JsonUtility.FromJson<LanguagesListModel>(data); // Deserialize Object    
                LangListCache = state.data;
                if (complete != null)
                    complete(state);
            }), ((string msg) => { // Error
                if (error != null)
                    error(msg); // Send Error
            })));
        }
    }

    //============================================================
    //  @class      Localization
    //  @method     GetLang()
    //  @type       Static Async Void
    //  @usage      Get locale info
    //  @args       (void) complete - Complete Callback
    //              (void) error - Error Callback
    //              (string) lang_code - Lang code
    //              (string) locale_code - Locale code
    //  @return     none
    //============================================================   
    public delegate void OnGetLocaleComplete(LocaleModel data);
    public delegate void OnGetLocaleError(string code);
    public void GetLocale(string lang_code, string locale_code, OnGetLocaleComplete complete = null, OnGetLocaleError error = null) // (bool) Get locale
    {
        if (LocInfoCache.ContainsKey(lang_code) && LocInfoCache[lang_code].ContainsKey(locale_code)) // If has lang and locale in thot lang
        {
            if (complete != null)
                complete(LocInfoCache[lang_code][locale_code]);
        }
        else
        {

            WWWForm formContent = new WWWForm(); // Create WWW Form
            formContent.AddField("app_id", $"{sdk_instance.application.app_id}"); // App Id
            formContent.AddField("app_key", $"{sdk_instance.application.app_key}"); // App key
            formContent.AddField("lang", $"{lang_code}"); // Code language
            formContent.AddField("code", $"{locale_code}"); // Code locale   

            StartCoroutine(sdk_instance.utils.SendRequest(OcugineSDK.PROTOCOL + OcugineSDK.SERVER + OcugineSDK.API_GATE + OcugineSDK.LOCALE_OBJECT + "/get_locale", formContent, ((string data) => { // All Right
                LocaleModel state = JsonUtility.FromJson<LocaleModel>(data); // Deserialize Object   
                if (LocInfoCache.ContainsKey(lang_code))
                    LocInfoCache[lang_code][locale_code] = state;
                else
                    LocInfoCache[lang_code] = new Dictionary<string, LocaleModel> { { locale_code, state } };
                if (complete != null)
                    complete(state);
            }), ((string msg) => { // Error
                if (error != null)
                    error(msg); // Send Error
            })));
            
        }
    }

    //============================================================
    //  @class      Localization
    //  @method     GetLocaleList()
    //  @type       Static Async Void
    //  @usage      Get locale list
    //  @args       (void) complete - Complete Callback
    //              (void) error - Error Callback
    //  @return     none
    //============================================================  
    public delegate void OnGetLocaleListComplete(LocalesListModel data);
    public delegate void OnGetLocaleListError(string code);
    public void GetLocaleList(string search = "", double page = 1, OnGetLocaleListComplete complete = null, OnGetLocaleListError error = null) // (bool) Get locale
    {
        if (LocListCache.list != null) // If has lang and locale in thot lang
        {
            if (complete != null)
                complete(new LocalesListModel() { message = "Cache", data = LocListCache });
        }
        else
        {
            WWWForm formContent = new WWWForm(); // Create WWW Form
            formContent.AddField("app_id", $"{sdk_instance.application.app_id}"); // App Id
            formContent.AddField("app_key", $"{sdk_instance.application.app_key}"); // App key
            formContent.AddField("search", $"{search}"); // Search
            formContent.AddField("page", $"{page}"); // Page

            StartCoroutine(sdk_instance.utils.SendRequest(OcugineSDK.PROTOCOL + OcugineSDK.SERVER + OcugineSDK.API_GATE + OcugineSDK.LOCALE_OBJECT + "/get_locale_list", formContent, ((string data) => { // All Right
                LocalesListModel state = JsonUtility.FromJson<LocalesListModel>(data); // Deserialize Object    
                LocListCache = state.data;
                if (complete != null)
                    complete(state);
            }), ((string msg) => { // Error
                if (error != null)
                    error(msg); // Send Error
            })));
        }
    }
}

//===================================================
//  Ocugine Unity SDK Utils Class
//===================================================
[AddComponentMenu("Ocugine SDK/Utils Component")]
[RequireComponent(typeof(OcugineSDK))]
public class Ocugine_Utils : MonoBehaviour{
    // Public Params

    // Private Params
    private OcugineSDK sdk_instance = null;     // Ocugine SDK Instance

    //============================================================
    //  @class      Ocugine_Utils
    //  @method     InitializeModule()
    //  @type       Public Void
    //  @usage      Initialize Module
    //  @args       (OcugineSDK) instance - Parent Instance Component
    //  @return     none
    //============================================================
    public void InitializeModule(OcugineSDK instance){
        sdk_instance = instance; // Set Ocugine SDK Instance
    }

    //============================================================
    //  @class      Ocugine_Utils
    //  @method     SendRequest()
    //  @type       Public IEnumerator
    //  @usage      Send Request to Web Server
    //  @args       none
    //  @return     none
    //============================================================
    public delegate void OnRequestCompleted(string data);
    public delegate void OnRequestError(string code);
    public IEnumerator SendRequest(string url, WWWForm data, OnRequestCompleted complete = null, OnRequestError error = null){
        // Send Data
        var request = new WWW(url, data); // Create WWW Request
        yield return request; // Send Request

        // Work with Response
        if (request.error != null){ // Request has error
            error("Failed to send request at the Ocugine server. Please, check your internet connection and try again."); // Do error
        } else{ // No Errors
            try{ // Trying to Parse Response
                BaseModel response = JsonUtility.FromJson<BaseModel>(request.text); // Get Base Model from Response Text
                if (sdk_instance.debug_mode) Debug.Log(response);
                if (response.complete){ // Response not has an errors
                    if (complete != null)
                        complete(request.text); // Return Complete
                } else
                { // Reponse has errors
                    if (error != null)
                        error(response.message); // Show Error
                    if (sdk_instance.debug_mode)
                        Debug.Log("Ocugine SDK Error: " + response.message);
                }
            } catch (Exception ex) { // All Right
                if (error != null)
                    error("Failed to parse server response. Please, try again later or contact with game administration."); // Do error
                if (sdk_instance.debug_mode)
                    Debug.Log("Ocugine SDK Error. Failed to convert responce. Responce Data: " + request.text + "\n" + ex);
           }
        }
    }

    //============================================================
    //  @class      Ocugine_Utils
    //  @method     LoadImage()
    //  @type       Public IEnumerator
    //  @usage      Load an Image to the Container
    //  @args       none
    //  @return     none
    //============================================================
    public delegate void OnImageLoaded(); // Image Loaded
    public delegate void OnImageLoadError(string code); // Image Loading Error
    public IEnumerator LoadImage(string url, Image image, OnImageLoaded complete = null, OnImageLoadError error = null){
        // Send Data
        var request = new WWW(url); // Create WWW Request
        yield return request; // Send Request

        // Work with Response
        if (request.error != null){ // Request has error
            error("Failed to load image from Ocugine server. Please, check your internet connection and try again."); // Do error
        } else { // No Errors
            try{ // Trying to Parse Response
                image.sprite = Sprite.Create(request.texture, new Rect(0, 0, request.texture.width, request.texture.height), new Vector2(0, 0));
            } catch (Exception ex){ // All Right
                if (error != null)
                    error("Failed to download image to the container. Please, try again later or contact with game administration."); // Do error
            }
        }
    }

    //============================================================
    //  @class      Ocugine_Utils
    //  @method     getAPIInfo()
    //  @type       Public IEnumerator
    //  @usage      Get API Information
    //  @args       (void) complete - Request Complete Callback
    //              (void) error - Request Error Callback
    //  @return     none
    //============================================================
    public delegate void OnAPIInfoComplete(StateModel data);    // Api Info Successfully Received
    public delegate void OnAPIInfoError(string code);           // API Info Request Error
    public void GetAPIInfo(OnAPIInfoComplete complete = null, OnAPIInfoError error = null)
    {
        // Create Data for Request
        WWWForm data = new WWWForm(); // Create WWW Form
        data.AddField("lang", $"{sdk_instance.settings.language}");         // Language

        // Send Request
        StartCoroutine(SendRequest(OcugineSDK.PROTOCOL + OcugineSDK.SERVER + OcugineSDK.API_GATE + OcugineSDK.STATE_OBJECT + "/", data, ((string response) => { // All Right
            StateModel state = JsonUtility.FromJson<StateModel>(response); // Get State Model from Response
            if (complete != null)
                complete(state); // Return Complete
        }), ((string msg) => { // Error
            if (error != null)
                error(msg); // Send Error
        })));
    }

    //============================================================
    //  @class      Ocugine_Utils
    //  @method     getAPIStatus()
    //  @type       Public IEnumerator
    //  @usage      Get API Information
    //  @args       (void) complete - Request Complete Callback
    //              (void) error - Request Error Callback
    //  @return     none
    //============================================================
    public delegate void OnAPIStatusComplete(APIState data);
    public delegate void OnAPIStatusError(string code);
    public void GetAPIStatus(OnAPIStatusComplete complete = null, OnAPIStatusError error = null)
    {
        // Create Data for Request
        WWWForm data = new WWWForm(); // Create WWW Form
        data.AddField("lang", $"{sdk_instance.settings.language}");         // Language

        // Send Request
        StartCoroutine(SendRequest(OcugineSDK.PROTOCOL + OcugineSDK.SERVER + OcugineSDK.API_GATE + OcugineSDK.STATE_OBJECT + "/get_state/", data, ((string response) => { // All Right
            APIState state = JsonUtility.FromJson<APIState>(response); // Get State Model from Response
            if (complete != null)
                complete(state); // Return Complete
        }), ((string msg) => { // Error
            if (error != null)
                error(msg); // Send Error
        })));
    }

    //============================================================
    //  @class      Ocugine_Utils
    //  @method     GetSettings()
    //  @type       Static Void
    //  @usage      Get settings info
    //              (void) complete - Complete Callback
    //              (void) error - Error Callback
    //  @return     none
    //============================================================
    public delegate void OnGetSettingsSuccess(APISettingsModel data); // Returns OAuthTokenModel
    public delegate void OnGetSettingsError(string code); // Returns error code
    public void GetSettings(OnGetSettingsSuccess complete, OnGetSettingsError error)
    {
        WWWForm formContent = new WWWForm();                                       // Create WWW Form
        formContent.AddField("app_id", $"{sdk_instance.application.app_id}");      // Add Language
        formContent.AddField("app_key", $"{sdk_instance.application.app_key}");    // App Key
        formContent.AddField("lang", $"{sdk_instance.settings.language}");         // Language

        // Send Request
        StartCoroutine(sdk_instance.utils.SendRequest(OcugineSDK.PROTOCOL + OcugineSDK.SERVER + OcugineSDK.API_GATE + OcugineSDK.SETTINGS_OBJECT + "/get_settings", formContent,
        ((string data) => { // Response
                    APISettingsModel state = JsonUtility.FromJson<APISettingsModel>(data); // Deserialize Object
                    complete(state); // Return Data                       
                }),
        ((string code) => { // Error
                error(code);
        })));
    }

    //============================================================
    //  @class      Ocugine_Utils
    //  @method     GetUserAgent()
    //  @type       Static Void
    //  @usage      Get user-agent string
    //  @args       (void) complete - Complete Callback
    //              (void) error - Error Callback
    //  @return     none
    //============================================================
    public delegate void OnGetUserAgentSuccess(APIUserAgentModel data); // Returns APIUserAgentModel
    public delegate void OnGetUserAgentError(string code); // Returns error code
    public void GetUserAgent(OnGetUserAgentSuccess complete = null, OnGetUserAgentError error = null)
    {
        WWWForm formContent = new WWWForm(); // Create WWW Form
        formContent.AddField("lang", $"{sdk_instance.settings.language}");         // Language

        // Send Request
        StartCoroutine(sdk_instance.utils.SendRequest(OcugineSDK.PROTOCOL + OcugineSDK.SERVER + OcugineSDK.API_GATE + OcugineSDK.UTILS_OBJECT + "/get_ua", formContent,
        ((string data) => { // Response
            APIUserAgentModel state = JsonUtility.FromJson<APIUserAgentModel>(data); // Deserialize Object
            if (complete != null)
                complete(state); // Return Data                       
        }),
        ((string code) => { // Error
            if (error != null)
                error(code);
        })));
    }

    //============================================================
    //  @class      Ocugine_Utils
    //  @method     GetIP()
    //  @type       Static Void
    //  @usage      Get IP data
    //  @args       (void) complete - Complete Callback
    //              (void) error - Error Callback
    //  @return     none
    //============================================================
    public delegate void OnGetIPSuccess(APIIPModel data); // Returns APIIPModel
    public delegate void OnGetIPError(string code); // Returns error code
    public void GetIP(OnGetIPSuccess complete = null, OnGetIPError error = null)
    {
        WWWForm formContent = new WWWForm(); // Create WWW Form
        formContent.AddField("lang", $"{sdk_instance.settings.language}");         // Language

        // Send Request
        StartCoroutine(sdk_instance.utils.SendRequest(OcugineSDK.PROTOCOL + OcugineSDK.SERVER + OcugineSDK.API_GATE + OcugineSDK.UTILS_OBJECT + "/get_ip", formContent,
        ((string data) => { // Response
            APIIPModel state = JsonUtility.FromJson<APIIPModel>(data); // Deserialize Object
            if (complete != null)
                complete(state); // Return Data                       
        }),
        ((string code) => { // Error
            if (error != null)
                error(code);
        })));
    }

    //============================================================
    //  @class      Ocugine_Utils
    //  @method     GetDevice()
    //  @type       Static Void
    //  @usage      Get player device
    //  @args       (void) complete - Complete Callback
    //              (void) error - Error Callback
    //  @return     none
    //============================================================
    public delegate void OnGetDeviceSuccess(APIDeviceModel data); // Returns APIDeviceModel
    public delegate void OnGetDeviceError(string code); // Returns error code
    public void GetDevice(OnGetDeviceSuccess complete = null, OnGetDeviceError error = null)
    {
        WWWForm formContent = new WWWForm(); // Create WWW Form
        formContent.AddField("lang", $"{sdk_instance.settings.language}");         // Language

        // Send Request
        StartCoroutine(sdk_instance.utils.SendRequest(OcugineSDK.PROTOCOL + OcugineSDK.SERVER + OcugineSDK.API_GATE + OcugineSDK.UTILS_OBJECT + "/get_device", formContent,
        ((string data) => { // Response
            APIDeviceModel state = JsonUtility.FromJson<APIDeviceModel>(data); // Deserialize Object
            if (complete != null)
                complete(state); // Return Data                       
        }),
        ((string code) => { // Error
            if (error != null)
                error(code);
        })));
    }

    //============================================================
    //  @class      Ocugine_Utils
    //  @method     CheckAPIKeys()
    //  @type       Static Void
    //  @usage      Check API keys
    //  @args       (void) complete - Complete Callback
    //              (void) error - Error Callback
    //  @return     none
    //============================================================
    public delegate void OnCheckAPIKeysSuccess(BaseModel data); // Returns APIDeviceModel
    public delegate void OnCheckAPIKeysError(string code); // Returns error code
    public void CheckAPIKeys(OnCheckAPIKeysSuccess complete = null, OnCheckAPIKeysError error = null)
    {
        WWWForm formContent = new WWWForm(); // Create WWW Form
        formContent.AddField("app_id", $"{sdk_instance.application.app_id}");   // App Id
        formContent.AddField("app_key", $"{sdk_instance.application.app_key}"); // App key

        // Send Request
        StartCoroutine(sdk_instance.utils.SendRequest(OcugineSDK.PROTOCOL + OcugineSDK.SERVER + OcugineSDK.API_GATE + OcugineSDK.UTILS_OBJECT + "/check_keys", formContent,
        ((string data) => { // Response
            BaseModel state = JsonUtility.FromJson<BaseModel>(data); // Deserialize Object
            if (complete != null)
                complete(state); // Return Data                       
        }),
        ((string code) => { // Error
            if (error != null)
                error(code);
        })));
    }
}

//===================================================
//  Ocugine Unity SDK UI Class
//===================================================
[AddComponentMenu("Ocugine SDK/UI Component")]
[RequireComponent(typeof(OcugineSDK))]
public class Ocugine_UI : MonoBehaviour{
    // Public Params

    // Private Params
    private OcugineSDK sdk_instance = null;     // Ocugine SDK Instance
    private WebViewObject webView = null; // Web View Object

    //============================================================
    //  @class      Ocugine_UI
    //  @method     InitializeModule()
    //  @type       Public Void
    //  @usage      Initialize Module
    //  @args       (OcugineSDK) instance - Parent Instance Component
    //  @return     none
    //============================================================
    public void InitializeModule(OcugineSDK instance){
        sdk_instance = instance; // Set Ocugine SDK Instance
        if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer){
            webView = null;
        } else {
            if(webView==null) webView = (new GameObject("WebViewObject")).AddComponent<WebViewObject>(); // Create Web View
            webView.transform.gameObject.SetActive(false); // Hide Window
            // Initialize WebView
            webView.Init(cb: (msg) => {
                    Debug.Log(string.Format("CallFromJS[{0}]", msg));
                },
                err: (msg) => {
                    Debug.Log(string.Format("CallOnError[{0}]", msg));
                },
                started: (msg) => {
                    Debug.Log(string.Format("CallOnStarted[{0}]", msg));
                },
                ld: (msg) => {
                    Debug.Log(string.Format("CallOnLoaded[{0}]", msg));
                    #if UNITY_EDITOR_OSX || !UNITY_ANDROID
                        #if true
                            webView.EvaluateJS(@"
                              if (window && window.webkit && window.webkit.messageHandlers && window.webkit.messageHandlers.unityControl) {
                                window.Unity = {
                                  call: function(msg) {
                                    window.webkit.messageHandlers.unityControl.postMessage(msg);
                                  }
                                }
                              } else {
                                window.Unity = {
                                  call: function(msg) {
                                    window.location = 'unity:' + msg;
                                  }
                                }
                              }
                            ");
                        #else
                            webView.EvaluateJS(@"
                              if (window && window.webkit && window.webkit.messageHandlers && window.webkit.messageHandlers.unityControl) {
                                window.Unity = {
                                  call: function(msg) {
                                    window.webkit.messageHandlers.unityControl.postMessage(msg);
                                  }
                                }
                              } else {
                                window.Unity = {
                                  call: function(msg) {
                                    var iframe = document.createElement('IFRAME');
                                    iframe.setAttribute('src', 'unity:' + msg);
                                    document.documentElement.appendChild(iframe);
                                    iframe.parentNode.removeChild(iframe);
                                    iframe = null;
                                  }
                                }
                              }
                            ");
                        #endif
                        webView.EvaluateJS(@"Unity.call('ua=' + navigator.userAgent)");
                    #endif
                }, enableWKWebView: true);

            #if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
                webView.bitmapRefreshCycle = 1;
            #endif
            webView.SetMargins(0, 0, 0, 0);
            webView.SetVisibility(false);
        }
    }

    //============================================================
    //  @class      Ocugine_UI
    //  @method     Update()
    //============================================================
    void Update(){
        if (Input.GetKeyDown(KeyCode.Escape)){
            if (webView != null){
                webView.SetVisibility(false);
                webView.transform.gameObject.SetActive(false); // Show Window
            }
        }
    }

    //============================================================
    //  @class      UI
    //  @method     GetToken()
    //  @type       Static Void
    //  @usage      Get token by Oauth protocol
    //  @args       (string / string[]) grants - Grants for project
    //              (void) complete - Complete Callback
    //              (void) error - Error Callback
    //  @return     none
    //============================================================   
    public delegate void OnTokenComplete(OAuthTokenModel data);
    public delegate void OnTokenError(string code);
    public void GetAuthForm(OnTokenComplete complete, OnTokenError error, string grants = "") // Get and return login form with string or empty grants
    {
        /** checking Auth module **/
        if (sdk_instance.auth == null)
        {
            if (error != null)
                error("Auth module required");
            return;
        }
        else
        {
            /** Get link and open brower **/
            sdk_instance.auth.GetLink(
                (string c) => {
                    if (webView == null){
                        Application.OpenURL(c);
                    } else {
                        webView.SetVisibility(true);
                        webView.transform.gameObject.SetActive(true); // Show Window
                        webView.LoadURL(c);
                    }
                    
                    StartCoroutine(CheckLoginDuringTimeout((OAuthTokenModel model)=> {
                        if (webView != null){
                            webView.SetVisibility(false);
                            webView.transform.gameObject.SetActive(false); // Show Window
                        }
                        complete(model);
                    }, (string err)=> {
                        if (error != null)
                            error(err);
                    }));
                },
                (string e) => {
                    if (error != null)
                        error(e);
                }, grants);
        }              
    }
    public void GetAuthForm(OnTokenComplete complete, OnTokenError error = null, string[] grants = null) // Get and return login form with an array of string grants
    {
        /** checking Auth module **/
        if (sdk_instance.auth == null){
            if (error != null)
                error("Auth module required");
            return;
        }
        else
        {
            /* Prepare Grants */
            if (grants == null)
                grants = new string[] { "all" };

            /** Get link and open brower **/
            sdk_instance.auth.GetLink(
                (string c) => {
                    if (webView == null){
                        Application.OpenURL(c);
                    } else {
                        webView.SetVisibility(true);
                        webView.transform.gameObject.SetActive(true); // Show Window
                        webView.LoadURL(c);
                    }
                    StartCoroutine(CheckLoginDuringTimeout((OAuthTokenModel model) => {
                        if (webView != null){
                            webView.SetVisibility(false);
                            webView.transform.gameObject.SetActive(false); // Show Window
                        }
                        complete(model);
                    }, (string err)=> {
                        if (error != null)
                            error(err);
                    }));
                },
                (string e) => {
                    if (error != null)
                        error(e);
                }, grants);
        }       
    }
    
    // You do not need this, do not touch this, forget this
    private IEnumerator CheckLoginDuringTimeout(OnTokenComplete complete, OnTokenError error){
        int timeout = 0; bool GotToken = false; string lasterror = "";
        /** Iteration for auth checking **/
        while (!GotToken && timeout != sdk_instance.settings.auth_timeout) // Set auth waiting time (30 sec)
        {
            sdk_instance.auth.GetToken((OAuthTokenModel token) => { complete(token); GotToken = true; }, (string err) => lasterror = err, true); // Wait for result
            timeout++; yield return new WaitForSeconds(1f); // Increase counter and wait 1 sec
        }
        if (!GotToken) // Timeout
        {
            switch (sdk_instance.settings.language)
            {
                case "RU": { error("Время на аутентификацию вышло\n" + lasterror); } break;
                default: { error("Authefication timed out\n" + lasterror); } break;
            }
        }
    }

    //============================================================
    //  @class      UI
    //  @method     showProfile()
    //  @type       Public Void
    //  @usage      Show Profile Page
    //============================================================ 
    public void showProfile(){
        string url = sdk_instance.getServerURL() + "profile/";

        // Open Profile Page
        if (webView == null){
            Application.OpenURL(url);
        } else{
            webView.SetVisibility(true);
            webView.transform.gameObject.SetActive(true); // Show Window
            webView.LoadURL(url);
        }
    }
}