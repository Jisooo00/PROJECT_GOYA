
using System;
using System.Collections.Generic;

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

    
}

public class Global
{
    public const string KEY_USER_ID = "USER_ID";
    public const string KEY_USER_PW = "USER_PW";
    public const string KEY_USER_UID = "USER_UID";
    public const int SANYEAH_NOTE_POOLING_CNT = 15;
    public const float SANYEAH_NOTE_DROP_SPEED = 550;
    public const float SANYEAH_NOTE_JUDGE_PERFECT = 30;
    public const float SANYEAH_NOTE_JUDGE_GREAT = 60;
    public const float SANYEAH_NOTE_JUDGE_GOOD = 90;
    public const float SANYEAH_NOTE_JUDGE_BAD = 120;
    public const float SANYEAH_NOTE_JUDGE_MISS = 300;
    
}