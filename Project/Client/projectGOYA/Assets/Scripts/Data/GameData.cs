
using System;
using System.Collections.Generic;
using UnityEngine;

public static class GameData
{
    #region Scene
    public enum eScene
    {
        None = -1,
        LoadingScene,
        IntroScene,
        MainScene,
        SanyeahScene,
        SanyeahGameScene,
    }

    #endregion

    #region Villan

    public enum eVillan
    {
        BOSS,
        SANYEAH,
    }
    

    #endregion

    #region Dialog

    public class DialogData
    {
        public string mObjectID;
        public KeyValuePair<string,string>[] mListDialog;
        public bool bIsPlayed;
        public int mMoveScene;
        public int mIndex;

        public DialogData(string ID,int index, KeyValuePair<string,string>[] list, bool b, int scene = -1)
        {
            mIndex = index;
            mObjectID = ID;
            mListDialog = list;
            bIsPlayed = b;
            mMoveScene = scene;
        }

    }

    public static Dictionary<string, DialogData[]> dicDialogData = new Dictionary<string, DialogData[]>();

    public static DialogData GetDialog(string ID)
    {
        if (dicDialogData.ContainsKey(ID))
        {
            foreach (var data in dicDialogData[ID])
            {
                if (!data.bIsPlayed)
                    return data;
            }
        }
        
        return null;
    }
    
    public static DialogData SetDialogEnd(string ID,int idx)
    {
        if (dicDialogData.ContainsKey(ID))
        {
            dicDialogData[ID][idx].bIsPlayed = true;
            return dicDialogData[ID][idx];
        }
        else
            return null;
    }

    #endregion
    
}

public class UserData
{
    public static UserData myData;

    public int user_uid = -1;
    public string user_id = "";
    public string user_pw = "";
}


public class Global
{
    public const string KEY_USER_ID = "USER_ID";
    public const string KEY_USER_PW = "USER_PW";
    public const string KEY_USER_NAME = "USER_NAME";
    public const string KEY_USER_UID = "USER_UID";
    public const int SANYEAH_NOTE_POOLING_CNT = 15;
    public const float SANYEAH_NOTE_DROP_SPEED = 550;
    public const float SANYEAH_NOTE_JUDGE_PERFECT = 30;
    public const float SANYEAH_NOTE_JUDGE_GREAT = 60;
    public const float SANYEAH_NOTE_JUDGE_GOOD = 90;
    public const float SANYEAH_NOTE_JUDGE_BAD = 120;
    public const float SANYEAH_NOTE_JUDGE_MISS = 300;

    public static void InitUserData()
    {
        if (UserData.myData != null)
            return;
        var data = new UserData();
        data.user_id = PlayerPrefs.HasKey(KEY_USER_ID) ? PlayerPrefs.GetString(KEY_USER_ID):"";
        data.user_uid = PlayerPrefs.HasKey(KEY_USER_UID) ? PlayerPrefs.GetInt(KEY_USER_UID):2;
        data.user_pw = PlayerPrefs.HasKey(KEY_USER_PW) ? PlayerPrefs.GetString(KEY_USER_PW):"";
        UserData.myData = data;

    }
}