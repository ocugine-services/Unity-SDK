# Ocugine Unity SDK
Welcome to the [Ocugine Platform](https://ocugine.net/). With this SDK you can develop your games faster, using dozens of ready-to-use modules for your projects. You can use our SDK for Unity games.

To get started, checkout examples and documentation at https://ocugine.net/. You also need to create your **Ocugine Account** and setup your project in the **Ocugine Dashboard**

## Build Information
This build tested on Unity 2018.3.1f1.

**Ocugine Unity SDK work features:**
- Currently our SDK tested only for Windows, WebGL, Android and iOS;
- This SDK version uses .NET 4.x equivalent for Unity;
- If you have a troubles with Prefabs - initialize SDK via Components for empty Object and move SDK Object up to top in the hierarchy;

## Installation
1) [Download](https://github.com/ocugine-services/Unity-SDK/releases) and unzip Ocugine SDK files in the Project Folder;
2) Go to the main menu and click **"Ocugine SDK"** => **"Instanite SDK"**. Don't forget to move SDK object up to top in the hierarchy;
- If you have some troubles with Prefab Instanite - Please, add new empty object, up them to top in the hierarchy and add component **"Ocugine SDK"** => **"SDK Manager"**;
3) Setup all modules in the Object Inspector;
4) Done. You can write code with Ocugine SDK for Unity;

## ## Setup SDK
In the inspector - please, fill **Project ID** and **Client Key**. After what you can work with SDK.

So, let's authtorize user via Ocugine SDK:
```
SDK.ui.GetAuthForm(((OAuthTokenModel token_data) => {
	Debug.Log("Complete! Token Data: " + token_data.data.access_token);
}), ((string error) => { // Error
    /* TODO: Error */
}), "all");
```

And that's all that you need to connect users zone for your project.
Now, your players can authenticate in your game by few clicks.

## Classes List
The list of available classes:
* **OcugineSDK** - General SDK Class;
* **Ocugine_Auth** - OAuth Authentication Class;
* **Ocugine_Analytics** - Analytics Class (for example: events and conversions);
* **Ocugine_GameServices** - Game Services Class (for example: achievements);
* **Ocugine_Monetization** - Monetization Class;
* **Ocugine_Notifications** - In-Game Notifications class;
* **Ocugine_Marketing** - Marketing Class;
* **Ocugine_Ads** - Advertising Class;
* **Ocugine_Backend** - Cloud Management Class;
* **Ocugine_Reports** - Errors and Performance Reporting Class;
* **Ocugine_Localization** - Localization Class;
* **Ocugine_Users** - Ocugine Users Class;
* **Ocugine_UI** - Ocugine UI Modules Class;
* **Ocugine_Utils** - Ocugine Utils Class

## Data Models
Then you work with Ocugine SDK - you need to use data models. For example:
```
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
```

This is a cloud-saves model. You need to use this model in the callbacks.

**Ocugine SDK has base models:**
```
[System.Serializable]
public class BaseModel{
    public bool complete = false;           // Complete Status
    public string message = null;           // Message Status
}
```

## What's next?
Learn more about Ocugine SDK and Platform [here](https://docs.ocugine.net/).