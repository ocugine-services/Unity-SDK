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

/**================ SHORT MODELS =================**/

//===================================================
//  Ocugine User Base Fields Model
//===================================================
[System.Serializable]
public class ProfileModel
{
    public double uid = 0;
    public string first_name = "";
    public string last_name = "";
    public string avatar = "";
    public string email = "";
    // public ProfileDataModel[] profile_data; // TODO: Temporary empty
}

//===================================================
//  Ocugine Advanced Field Model
//===================================================
[System.Serializable]
public class AdvancedFieldModel
{
    public enum FieldType { TextLine = 0, Number = 1, Selectable = 2, Multiselect = 3, TextArea = 4, Phone = 5, Email = 6, Image = 7, URL = 8, Checkbox = 9 };
    //
    public double uid = 0;
    public string name = "";
    public double sorting = 0;
    public bool hidden = false;
    public FieldType type = 0; 
    public double project_id = 0;
    public bool required = false;
    public string default_value = "";
}

/**================== PROFILES ===================**/

//===================================================
//  Ocugine Ban State Model
//===================================================
[System.Serializable]
public class BanStateModel : BaseModel 
{
    public DataModel data;

    [System.Serializable]
    public class DataModel
    {      
        public bool ban_state = false;
        public string ban_reason = "";
        public double ban_escape_time = 0;
        public double ban_escape_days = 0;
    }
}

//===================================================
//  Ocugine Users List Model
//===================================================
[System.Serializable]
public class UsersListModel : BaseModel
{
    public DataModel data;

    [System.Serializable]
    public class DataModel
    {
        // Модель инфо
        public ListModel[] list;

        [System.Serializable]
        public class ListModel
        {
            public double uid = 0;
            public string first_name = "";
            public string last_name = "";
            public string avatar = "";
            public string email = "";
            //public ProfileDataModel[] profile_data; // TODO: Temporary empty
            public double profile_uid = 0;
            public double group_id = -1;
            public BanStateModel.DataModel ban_data;

            [System.Serializable]
            public class ProfileDataModel
            {
                // TODO: Temporary empty
            }                   
        }
    }
}

//===================================================
//  Ocugine Users Data Model
//===================================================
[System.Serializable]
public class UserModel : BaseModel
{
    public DataModel data;

    [System.Serializable]
    public class DataModel
    {
        public double group_id;
        public ProfileModel base_data;
        public AdvancedFieldModel[] advanced_fields;
        public BanStateModel.DataModel ban_data;     
    }
}

/**=================== GROUPS ====================**/

//===================================================
//  Ocugine Groups List Model
//===================================================
[System.Serializable]
public class GroupListModel : BaseModel
{
    public DataModel data;

    [System.Serializable]
    public class DataModel
    {
        public GroupModel.DataModel[] list;
    }
}

//===================================================
//  Ocugine Groups Info Model
//===================================================
[System.Serializable]
public class GroupModel : BaseModel
{
    public DataModel data;

    [System.Serializable]
    public class DataModel
    {
        public string uid = "";
        public string project_id = "";
        public string group_name = "";
        public string group_desc = "";
        public string time = "";
        public ConditionsData[] conditions;
        public bool can_select = false;
        public bool auto_detect = false;
    }

    [System.Serializable]
    public class ConditionsData
    {
        //TODO: Temporary empty
    }
}

/**=============== ADVANCED FIELDS ================**/

//===================================================
//  Ocugine Notification List Model
//===================================================
public class AdvancedFieldsListModel : BaseModel
{
    public DataModel data;

    [System.Serializable]
    public class DataModel
    {
        public AdvancedFieldModel[] list;
    }
}

/**=================== POLICY ====================**/

//===================================================
//  Ocugine Policy List Model
//===================================================
[System.Serializable]
public class PolicyListModel : BaseModel
{
    public DataModel data;

    [System.Serializable]
    public class DataModel
    {
        // Модель инфо
        public ListModel[] list;

        [System.Serializable]
        public class ListModel
        {
            public string policy_name = "";
            public int uid = 0;
            public string time = "";
        }

    }
}

//===================================================
//  Ocugine Policy Info Model
//===================================================
[System.Serializable]
public class PolicyModel : BaseModel
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
            public int uid = 0;
            public string policy_name = "";
            public string policy_text = "";
            public string project_id = "";
            public string time = "";
        }

    }
}

/**================== REVIEWS ====================**/

//===================================================
//  Ocugine Reviews List Model
//===================================================
[System.Serializable]
public class RewiewsListModel : BaseModel
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
            public string review = "";
            public double rate = 0;
            public double time = 0;
            public string platform = "";
        }
    }
}

/**================= CHAT ROOMS ==================**/

//===================================================
//  Ocugine Message Data Model
//===================================================
[System.Serializable]
public class MessageModel
{
    public double uid = 0;
    public double profile_uid = 0;
    public double project_id = 0;
    public double room_id = 0;
    public string message = "";
    public double time = 0;
    public string date = "";
    public ProfileModel sender;
}

//===================================================
//  Ocugine Chat Rooms List Model
//===================================================
[System.Serializable]
public class ChatRoomsListModel : BaseModel
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
            public string name = "";
            public string desc = "";
            public double project_id = 0;
            public double hidden = 0;
            public MessageModel last_msg;
        }
    }
}

//===================================================
//  Ocugine Messages List Model
//===================================================
[System.Serializable]
public class ChatMessagesListModel : BaseModel
{
    public DataModel data;

    [System.Serializable]
    public class DataModel
    {
        public MessageModel[] list;        
    }
}

/**=================== SUPPORT ===================**/

//===================================================
//  Ocugine Example List Model
//===================================================
[System.Serializable]
public class SupportCategoriesListModel : BaseModel
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
            public string name = "";
            public string desc = "";
            public bool closed = false;
            public double sorting = 0; 
            public double time = 0;
            public double topics_num = 0;
        }
    }
}

//===================================================
//  Ocugine Example List Model
//===================================================
public class SupportTopicsListModel : BaseModel
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
            public double profile_uid = 0;
            public double category_id = 0;
            public double project_id = 0;
            public double creation_time = 0;
            public double last_edit_time = 0;
            public string title = "";
            public bool is_closed = false;   
            public string body = "";  
            public double messages = 0;
        }
    }
}

//===================================================
//  Ocugine Example List Model
//===================================================
public class SupportMessagesListModel : BaseModel
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
            public double category_id = 0;
            public double profile_uid = 0;
            public double topic_id = 0;
            public double time = 0;
            public string message = "";
        }
    }
}

/**================ NOTIFICATIONS ================**/

//===================================================
//  Ocugine Notification Model
//===================================================
public class NotificationModel : BaseModel
{
    public enum PlatformType { All = 0, Android = 1, iOS = 2, WindowsStandalone = 3, WindowsUWP = 4, XboxOne = 5, Playstation4 = 6, NintendoSwitch = 7, Web = 8, MacLinux = 9 }
    public enum PriorityType { Low = 0, Normal = 1, High = 2, VeryHigh = 3 }

    public DataModel data;

    [System.Serializable]
    public class DataModel
    {
        public double uid = 0;
        public double project_id = 0;
        public string name = "";
        public string content = "";
        public bool for_authenticated = false;
        public PlatformType platform = 0;
        public double user_group = 0;
        public double time = 0;
        public double end_time = 0;
        public PriorityType priority = 0;
    }
}

//===================================================
//  Ocugine Notification List Model
//===================================================
public class NotificationsListModel : BaseModel
{
    public DataModel data;

    [System.Serializable]
    public class DataModel
    {
        public NotificationModel.DataModel[] list;
    }
}



