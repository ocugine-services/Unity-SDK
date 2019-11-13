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
//  Localization Component (Unity UI (Text))
//===================================================
[RequireComponent(typeof(UnityEngine.UI.Text))]
[AddComponentMenu("Ocugine SDK/Localization/UI Text Locale")]
public class LocaleString : MonoBehaviour{
    [Header("Component Settings")]
    public string language = "";            // Language
    public string locale_code = "";         // Locale Code

    [Header("Additional Components")]
    public bool update_every_frame = false; // Update Every Frame

    // Private Objects
    private Text _text;                     // Text Component
    private string _default_text = "";      // Default Text
    private OcugineSDK _sdk = null;         // SDK Instance

    // Awake
    void Awake(){
        // Work with Component
        _text = GetComponent<Text>(); // Get Component
        if (_text == null) throw new System.Exception("Failed to use this component without Unity UI"); // UI Component
        _default_text = _text.text; // Set Default Text

        // Work with SDK Instance
        if(_sdk==null) _sdk = _findSDKInstance(); // Find SDK Instance
    }

    // Start is called before the first frame update
    void Start(){
        if (locale_code.Length > 0){
            _setLocale(language, locale_code);
        }
    }

    // Update is called once per frame
    void Update(){
        if (update_every_frame && locale_code.Length > 0){
            _setLocale(language, locale_code);
        }
    }

    // Set Locale
    private void _setLocale(string language, string locale_code){
        string _lng = (language.Length > 0) ? language : _sdk.settings.language;
        _sdk.locale.GetLocale(language, locale_code, (LocaleModel locale) => { // All Right
            _text.text = locale.data.value; // Set Value
        }, (string error) => { // Error
            if (_sdk.debug_mode)
                Debug.Log("Ocugine Localization Component Error: " + error);
            _text.text = _default_text; // Set Default Value
        });
    }

    // Find SDK Instance
    private OcugineSDK _findSDKInstance(){
        var _instance = FindObjectsOfType<OcugineSDK>(); // Find Ocugine SDK Instances
        if (_instance.Length < 1){ // Instance not found
            throw new System.Exception("Failed to found an Ocugine SDK instance in this scene"); // Error
        }

        // Has Instances
        return _instance[0]; // Return Instance
    }
}