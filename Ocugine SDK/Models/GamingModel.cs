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
//  Ocugine SDK Missions Models
//===================================================
/* Mission Data Model */
[System.Serializable]
public class MissionModel : BaseModel
{
    public DataModel data;

    [System.Serializable]
    public class DataModel
    {
        public double uid = 0;
        public double project_id = 0;
        public string name = "";
        public string desc = "";
        public string image = "";
        public double counter = 0;
        public double next_mission = 0;
        public double time = 0;
        public double players = 0;
    }
}

/* Player Mission List Model */
[System.Serializable]
public class PlayeMissionsListModel : BaseModel
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
            public double mission_id = 0;
            public double counter = 0;
            public double completed = 0;
            public double time = 0;
            public MissionModel.DataModel mission_data;
        }    
    }
}

/* Missions List Model */
[System.Serializable]
public class MissionsListModel : BaseModel
{
    public DataModel data;

    [System.Serializable]
    public class DataModel
    {
        public MissionModel.DataModel[] list;
    }
}

/* Mission Set Scores Data Model */
[System.Serializable]
public class MissionSetScoresModel : BaseModel
{
    public DataModel data;

    [System.Serializable]
    public class DataModel
    {
        public bool mission_complete = false;
    }
}

//===================================================
//  Ocugine SDK Achievements Models
//===================================================
/* Achievement Model */
[System.Serializable]
public class AchievementModel : BaseModel
{
    public DataModel data;

    [System.Serializable]
    public class DataModel
    {
        public double uid = 0;
        public double project_id = 0;
        public string name = "";
        public string desc = "";
        public string image = "";
        public double time = 0;
        public double players = 0; // For list data model
    }
}

/* Achievements List Model */
[System.Serializable]
public class AchievementsListModel : BaseModel
{
    public DataModel data;

    [System.Serializable]
    public class DataModel
    {
        public AchievementModel.DataModel[] list;
    }
}

/* Player Achievements List Model */
[System.Serializable]
public class PlayerAchievementListModel : BaseModel
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
            public double achiv_id = 0;
            public double profile_uid = 0;
            public double time = 0;
            public AchievementModel.DataModel info;
        }
    }
}

//===================================================
//  Ocugine SDK Leaderboards Models
//===================================================
/* Leaderboard Model */
[System.Serializable]
public class LeaderboardModel : BaseModel
{
    public enum OrderType { ASC = 0, DESC = 1 };

    public DataModel data;

    [System.Serializable]
    public class DataModel
    {
        public double uid = 0;
        public double project_id = 0;
        public string name = "";
        public string desc = "";
        public string image = "";
        public OrderType ordering = OrderType.ASC;
        public double time = 0;
        public string players = "";
    }
}

/* Leaderboards List Model */
[System.Serializable]
public class LeaderboardListModel : BaseModel
{
    public DataModel data;

    [System.Serializable]
    public class DataModel
    {
        public LeaderboardModel.DataModel[] list;
    }
}

/* Leaderboards TOP List Model */
[System.Serializable]
public class LeaderboardTopModel : BaseModel
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
            public double profile_id = 0;
            public double scores = 0;
            public double board_id = 0;
            public double time = 0;
            public ProfileModel profile;
        }
    }
}

/* Leaderboard User Scores Data Model */
[System.Serializable]
public class LeaderboardPlayerScoresModel : BaseModel
{
    public DataModel data;

    [System.Serializable]
    public class DataModel
    {
        public double uid = 0;
        public double project_id = 0;
        public double profile_uid = 0;
        public double scores = 0;
        public double board_id = 0;
        public double time = 0;
    }
}
