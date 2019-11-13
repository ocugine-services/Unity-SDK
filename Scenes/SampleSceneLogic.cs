using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
//  Example Scene Logic
//===================================================
public class SampleSceneLogic : MonoBehaviour{
    [Header("Ocugine SDK Instance")]
    public OcugineSDK SDK; // SDK Instance

    [Header("UI Panels")]
    public GameObject startupPanel; // Startup Panel
    public GameObject demoPanel; // Demo Panel

    [Header("Startup Panel UI")]
    public GameObject authPanel; // Auth Panel
    public GameObject welcomePanel; // Welcome Panel

    [Header("Demo Panel UI")]
    public GameObject generalScreen;
    public GameObject topScreen;
    public GameObject achivScreen;
    public GameObject analyticScreen;
    public GameObject policyScreen;
    public GameObject[] templates;

    // Start is called before the first frame update
    void Start(){
        // Check SDK Initialization
        if (SDK.auth == null || !SDK.settings.modules.Auth || SDK.ui==null || !SDK.settings.modules.UI){
            throw new System.Exception("Failed to initialize Ocugine Demo Scene. Please, enable all modules to continue.");
        }

        // Check Application Authentication
        demoPanel.SetActive(false);
        if (SDK.auth.credentials.is_auth && SDK.auth.credentials.token.Length>0){ // Authenticated
            SDK.auth.GetGrants((OAuthTokenModel token_data)=> {
                startupPanel.SetActive(true); // Startup Panel
                authPanel.SetActive(false); // Auth Panel
                welcomePanel.SetActive(true); // Welcome Panel
                doGetProfile(); // Get Profile
            }, (string error)=> {
                logout();
            });
        } else { // Non Authenticated
            startupPanel.SetActive(true); // Startup Panel
            authPanel.SetActive(true); // Auth Panel
            welcomePanel.SetActive(false); // Welcome Panel
        }
    }

    // Do Authenticate
    public void doAuthenticate(){
        Button _signin = authPanel.gameObject.transform.Find("LoginButton").GetComponent<Button>();
        _signin.interactable = false; // Disable Button

        // Authenticate
        SDK.ui.GetAuthForm(((OAuthTokenModel token_data) => { // Authenticated
            Debug.Log("Complete! Token Data: " + token_data.data.access_token);
            _signin.interactable = true; // Disable Button
            startupPanel.SetActive(true); // Startup Panel
            authPanel.SetActive(false); // Auth Panel
            welcomePanel.SetActive(true); // Welcome Panel
            doGetProfile(); // Get Profile
        }), ((string error) => { // Error
            _signin.interactable = true; // Disable Button
            throw new System.Exception(error); // Error
        }), "all");
    }

    // Get Profile
    public void doGetProfile(){
        // Get Components
        Text _profileLabel = welcomePanel.gameObject.transform.Find("ProfileLabel").GetComponent<Text>();
        Text _emailLabel = welcomePanel.gameObject.transform.Find("EmailLabel").GetComponent<Text>();
        Image _imgHolder = welcomePanel.gameObject.transform.Find("AvatarHolder").transform.Find("Avatar").GetComponent<Image>();

        // Get Datas
        if (SDK.users != null && SDK.settings.modules.Users){ // Enabled
            SDK.users.GetUserData((UserModel user) => { // Complete
                string name = (user.data.base_data.first_name!="")?user.data.base_data.first_name+" "+user.data.base_data.last_name:" User #"+user.data.base_data.uid; // User Name
                _profileLabel.text = "Hello, <color=\"#0082c8\"><b>"+ name + "</b></color>!"; // Set Name
                _emailLabel.text = user.data.base_data.email; // User Email
                StartCoroutine(SDK.utils.LoadImage(user.data.base_data.avatar, _imgHolder)); // Get Avatar
            }, (string error) => { // Error
                throw new System.Exception(error); // Error
            });
        } else { // Disabled
            throw new System.Exception("Failed to get access for Users Module. Please, enable this module in the SDK settings");
        }
    }

    // Go to the Profile Page
    public void goToProfile(){
        SDK.ui.showProfile(); // Show Profile Window
    }

    // Logout
    public void logout(){
        SDK.auth.Logout(); // Logout
        startupPanel.SetActive(true); // Startup Panel
        authPanel.SetActive(true); // Auth Panel
        welcomePanel.SetActive(false); // Welcome Panel
    }

    // Quit Game
    public void quitGame(){
        Application.Quit();
    }

    // Play Demo Logic
    public void playDemoLogic(){
        startupPanel.SetActive(false);
        demoPanel.SetActive(true); // Activate panel

        // Get Components
        GameObject _topPanel = demoPanel.transform.Find("TopPanel").transform.gameObject; // Game Object
        GameObject _botPanel = demoPanel.transform.Find("BottomPanel").transform.gameObject; // Game Object
        Image _userAva = _topPanel.transform.Find("Avatar").GetComponent<Image>(); // Get Component
        Text _userLabel = _topPanel.transform.Find("UserLabel").transform.gameObject.GetComponent<Text>(); // Get Component
        if (SDK.users.current_user.data.base_data.avatar != null && SDK.users.current_user.data.base_data.avatar.Length > 0){
            StartCoroutine(SDK.utils.LoadImage(SDK.users.current_user.data.base_data.avatar, _userAva)); // Set User Avatar
        }
        string name = (SDK.users.current_user.data.base_data.first_name != "") ? SDK.users.current_user.data.base_data.first_name + " " + SDK.users.current_user.data.base_data.last_name : " User #" + SDK.users.current_user.data.base_data.uid; // User Name
        _userLabel.text = name; // Set Name
        switchTab("general");
    }

    // Switch Tab
    public void switchTab(string tab){
        // Hide All tabs
        generalScreen.SetActive(false); // General Screen
        topScreen.SetActive(false); // Top Screen
        achivScreen.SetActive(false); // Achievements Screen
        analyticScreen.SetActive(false); // Analytics Screen
        policyScreen.SetActive(false); // Policy Screen

        // Open Tab
        if (tab == "general"){
            generalScreen.SetActive(true); // Show
        }
        if (tab == "top"){
            topScreen.SetActive(true);
            initTOP();
        }
        if (tab == "dashboard"){
            Application.OpenURL("https://cp.ocugine.pro/");
        }
        if (tab == "achievements"){
            achivScreen.SetActive(true);
            initAchievements();
        }
        if (tab == "analytics"){
            analyticScreen.SetActive(true);
            initAnalytics();
        }
        if (tab == "policy"){
            policyScreen.SetActive(true);
            initPolicy();
        }
    }

    // Init Tops
    public void initTOP(){
        GameObject _currentContainer = topScreen.gameObject;
        GameObject _listTemplate = templates[0]; // List Template
        GameObject _List = _currentContainer.transform.Find("List").gameObject.transform.Find("Viewport").gameObject.transform.Find("Content").transform.gameObject;
        GameObject _List2 = _currentContainer.transform.Find("List2").gameObject.transform.Find("Viewport").gameObject.transform.Find("Content").transform.gameObject;

        // Clear List
        foreach (Transform child in _List.transform){
            GameObject.Destroy(child.gameObject);
        }
        foreach (Transform child in _List2.transform){
            GameObject.Destroy(child.gameObject);
        }

        // Get List
        SDK.game.GetLeaderboardsList((LeaderboardListModel list) => { // Get List
            foreach (LeaderboardModel.DataModel leaderboard in list.data.list){
                GameObject _itm = Instantiate(_listTemplate, _List.transform); // Instanite
                _itm.transform.Find("uid").transform.GetComponent<Text>().text = "#" + leaderboard.uid.ToString();
                _itm.transform.Find("policy_name").transform.GetComponent<Text>().text = "<b>" + leaderboard.name.ToString() + "</b>" + " ("+leaderboard.players.ToString()+" players)";
                _itm.GetComponent<Button>().onClick.RemoveAllListeners(); // Remove Listeners
                _itm.GetComponent<Button>().onClick.AddListener(() => {
                    // Clear
                    foreach (Transform child in _List2.transform){
                        GameObject.Destroy(child.gameObject);
                    }
                    // Get New Top
                    SDK.game.GetPlayersTop(leaderboard.uid, ((LeaderboardTopModel top)=> {
                        int _cnt = 1;
                        foreach (LeaderboardTopModel.DataModel.ListModel top_item in top.data.list){
                            GameObject _inner_itm = Instantiate(templates[1], _List2.transform); // Instaninte
                            _inner_itm.transform.Find("place").transform.GetComponent<Text>().text = "#" + _cnt.ToString();
                            string name = (top_item.profile.first_name.Length > 0) ? top_item.profile.first_name + " " + top_item.profile.last_name : "";
                            name += (name == "") ? "Player #" + top_item.profile.uid.ToString() : " (Player #" + top_item.profile.uid.ToString() + ")";
                            _inner_itm.transform.Find("player_name").transform.GetComponent<Text>().text = name;
                            _inner_itm.transform.Find("scores").transform.GetComponent<Text>().text = top_item.scores.ToString() + " scores";
                            Image _img = _inner_itm.transform.Find("avatar").GetComponent<Image>();
                            if (top_item.profile.avatar != null && top_item.profile.avatar.Length > 0){
                                StartCoroutine(SDK.utils.LoadImage(top_item.profile.avatar, _img));
                            }
                            _cnt++;
                        }
                    }), (string error)=> {
                        throw new Exception(error);
                    });
                });
            }
        }, (string error) => { // Error
            throw new System.Exception(error); // Error
        });
    }

    // Init Achievements
    public void initAchievements(){
        _updatePlayerAchievements(); // Update Player Achievements
        GameObject _currentContainer = achivScreen.gameObject;
        GameObject _listTemplate = templates[1]; // List Template
        GameObject _List = _currentContainer.transform.Find("List").gameObject.transform.Find("Viewport").gameObject.transform.Find("Content").transform.gameObject;
        GameObject _AchivInfo = _currentContainer.transform.Find("AchivInfo").gameObject;
        GameObject _UnlockBTN = _currentContainer.transform.Find("Unlock").gameObject;

        // Set Unlock Interatible
        _UnlockBTN.GetComponent<Button>().interactable = false;
        _AchivInfo.GetComponent<Text>().text = "<color=\"#999\">Please, choose an achievement</color>";

        // Clear List
        foreach (Transform child in _List.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        // Get List
        SDK.game.GetAchievemntsList((AchievementsListModel list) => { // Get List
            foreach (AchievementModel.DataModel achiv in list.data.list){
                GameObject _itm = Instantiate(_listTemplate, _List.transform); // Instanite
                _itm.transform.Find("place").transform.GetComponent<Text>().text = achiv.name.ToString();
                _itm.transform.Find("player_name").transform.GetComponent<Text>().text = achiv.desc.ToString();
                _itm.transform.Find("scores").transform.GetComponent<Text>().text = "<b>Unlocked by</b> " + achiv.players.ToString() + " players";
                Image _img = _itm.transform.Find("avatar").transform.GetComponent<Image>();
                if (achiv.image != null && achiv.image.Length > 0){
                    StartCoroutine(SDK.utils.LoadImage(achiv.image, _img));
                }
                _itm.GetComponent<Button>().onClick.RemoveAllListeners(); // Remove Listeners
                _itm.GetComponent<Button>().onClick.AddListener(() => {
                    _UnlockBTN.GetComponent<Button>().interactable = true;
                    _AchivInfo.GetComponent<Text>().text = "<b>"+achiv.name+"</b>\nAchievement #"+achiv.uid;
                    _UnlockBTN.GetComponent<Button>().onClick.RemoveAllListeners();
                    _UnlockBTN.GetComponent<Button>().onClick.AddListener(() => {
                        SDK.game.UnlockPlayerAchievement(achiv.uid, (BaseModel data) => {
                            _updatePlayerAchievements();
                            _UnlockBTN.GetComponent<Button>().interactable = false;
                        });
                    });
                });
            }
        }, (string error) => { // Error
            throw new System.Exception(error); // Error
        });
    }

    // Update Player Achievements
    private void _updatePlayerAchievements(){
        GameObject _currentContainer = achivScreen.gameObject;
        GameObject _listTemplate = templates[1]; // List Template
        GameObject _List = _currentContainer.transform.Find("List2").gameObject.transform.Find("Viewport").gameObject.transform.Find("Content").transform.gameObject;

        // Clear List
        foreach (Transform child in _List.transform){
            GameObject.Destroy(child.gameObject);
        }

        // Get List
        SDK.game.GetPlayerAchievements((PlayerAchievementListModel list) => { // Get List
            foreach (PlayerAchievementListModel.DataModel.ListModel achiv in list.data.list){
                GameObject _itm = Instantiate(_listTemplate, _List.transform); // Instanite
                _itm.transform.Find("place").transform.GetComponent<Text>().text = achiv.info.name.ToString();
                _itm.transform.Find("player_name").transform.GetComponent<Text>().text = achiv.info.desc.ToString();
                _itm.transform.Find("scores").transform.GetComponent<Text>().text = "<b>Unlocked at</b> " + UnixTimeStampToDateTime((long)achiv.time).ToString();
                Image _img = _itm.transform.Find("avatar").transform.GetComponent<Image>();
                if (achiv.info.image != null && achiv.info.image.Length > 0)
                {
                    StartCoroutine(SDK.utils.LoadImage(achiv.info.image, _img));
                }
                _itm.GetComponent<Button>().onClick.RemoveAllListeners(); // Remove Listeners
                _itm.GetComponent<Button>().onClick.AddListener(() => {
                });
            }
        }, (string error) => { // Error
            throw new System.Exception(error); // Error
        });
    }

    // Init Analytics
    public void initAnalytics(){
        GameObject _currentContainer = analyticScreen.gameObject;
        GameObject _listTemplate = templates[0]; // List Template
        GameObject _List = _currentContainer.transform.Find("List").gameObject.transform.Find("Viewport").gameObject.transform.Find("Content").transform.gameObject;
        GameObject _Content = _currentContainer.transform.Find("ContentArea").gameObject.transform.Find("Viewport").gameObject.transform.Find("Content").transform.gameObject;
        _currentContainer.transform.Find("ContentArea").transform.gameObject.SetActive(false);

        // Clear List
        foreach (Transform child in _List.transform){
            GameObject.Destroy(child.gameObject);
        }

        // Get List
        SDK.analytics.GetLatestFlags((LatestFlagsListModel list) => { // Get List
            foreach (LatestFlagsListModel.DataModel.ListModel flag in list.data.list){
                GameObject _itm = Instantiate(_listTemplate, _List.transform); // Instanite
                _itm.transform.Find("uid").transform.GetComponent<Text>().text = "#" + flag.uid.ToString();
                _itm.transform.Find("policy_name").transform.GetComponent<Text>().text = flag.flag.ToString();
                _itm.GetComponent<Button>().onClick.RemoveAllListeners(); // Remove Listeners
                _itm.GetComponent<Button>().onClick.AddListener(() => {
                    _currentContainer.transform.Find("ContentArea").transform.gameObject.SetActive(true);
                    string _txt = "<b>Analytic Action #"+flag.uid+" Overview:</b>\n";
                    _txt += "<b>Analytic Flag Name: </b>" + flag.flag.ToString();
                    _txt += "\n<b>Profile UID (If Authenticated): </b>"+flag.profile_uid.ToString();
                    _txt += "\n<b>Viewer Hash: </b>" + flag.viewer.ToString();
                    _txt += "\n<b>Project ID: </b>" + flag.project_id.ToString();
                    _txt += "\n<b>Action Time: </b>" + UnixTimeStampToDateTime((long)flag.time).ToString() + "("+flag.time.ToString()+")";
                    _Content.transform.Find("PolicyData").transform.gameObject.GetComponent<Text>().text = _txt;
                    _currentContainer.transform.Find("ContentArea").GetComponent<ScrollRect>().normalizedPosition = new Vector2(0, 1);
                });
            }
        }, (string error) => { // Error
            throw new System.Exception(error); // Error
        });
    }

    // Init Policy
    public void initPolicy(){
        GameObject _currentContainer = policyScreen.gameObject;
        GameObject _listTemplate = templates[0]; // List Template
        GameObject _List = _currentContainer.transform.Find("List").gameObject.transform.Find("Viewport").gameObject.transform.Find("Content").transform.gameObject;
        GameObject _Content = _currentContainer.transform.Find("ContentArea").gameObject.transform.Find("Viewport").gameObject.transform.Find("Content").transform.gameObject;
        _currentContainer.transform.Find("ContentArea").transform.gameObject.SetActive(false);

        // Clear List
        foreach (Transform child in _List.transform){
            GameObject.Destroy(child.gameObject);
        }

        // Get List
        SDK.users.GetPolicyList((PolicyListModel list) => { // Get List
            foreach (PolicyListModel.DataModel.ListModel policy in list.data.list){
                GameObject _itm = Instantiate(_listTemplate, _List.transform); // Instanite
                _itm.transform.Find("uid").transform.GetComponent<Text>().text = "#" + policy.uid.ToString();
                _itm.transform.Find("policy_name").transform.GetComponent<Text>().text = policy.policy_name.ToString();
                _itm.GetComponent<Button>().onClick.RemoveAllListeners(); // Remove Listeners
                _itm.GetComponent<Button>().onClick.AddListener(() => {
                    SDK.users.GetPolicyInfo(policy.uid, (PolicyModel policy_data)=> {
                        _currentContainer.transform.Find("ContentArea").transform.gameObject.SetActive(true);
                        _Content.transform.Find("PolicyData").transform.gameObject.GetComponent<Text>().text = policy_data.data.info.policy_text;
                        _currentContainer.transform.Find("ContentArea").GetComponent<ScrollRect>().normalizedPosition = new Vector2(0, 1);
                    }, (string error) => { // Error
                        throw new System.Exception(error); // Error
                    });
                });
            }
        }, (string error) => { // Error
            throw new System.Exception(error); // Error
        });
    }

    // Date Time Converter
    private static DateTime UnixTimeStampToDateTime(long unixTimeStamp){
        System.DateTime dtDateTime = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);
        dtDateTime = dtDateTime.AddSeconds(unixTimeStamp);
        return dtDateTime;
    }
}
