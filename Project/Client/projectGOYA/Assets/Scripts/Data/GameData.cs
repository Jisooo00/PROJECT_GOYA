
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class GameData
{
    #region Scene
    public enum eScene
    {
        None = -1,
        FirstScene,
        IntroScene,
        PrologScene,
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

    [Serializable]
    public class DialogData
    {
        public enum ePlayCondition
        {
            OBJECT_INTERACTION = -2,
            PREV_DIALOG = -1,
            OUEST_UNAVAILABLE = 0,
            OUEST_ACCOMPLISHING = 1,
            OUEST_COMPLETED = 2,
            OUEST_FINISHED = 3,
        }
       
        public string m_strDialogID;
        public string mObjectID;
        public ePlayCondition m_ePlayCondition;
        public string m_strConditionID = ""; //Quest이거나 Dialog
        public bool m_bPlayed = false;
        public GameManager.eDialogAction m_eAction;
        public List<string>m_listActionQuest = new List<string>(); 
        public List<string> m_listActionDialog = new List<string>(); 
        public bool m_bReplay = false;
        public List<ScriptData> m_scriptList = new List<ScriptData>();


        public DialogData(string ID,string npcId, string playCondition,  string condition,string actionQuest,string actionDialog,string action,string replay)
        {
            m_strDialogID = ID;
            mObjectID = npcId;
            m_ePlayCondition = (ePlayCondition) int.Parse(playCondition);
            m_strConditionID = condition;
            string[] quests = actionQuest.Split('/');
            if (quests.Length > 0)
            {
                foreach (var quest in quests)
                {
                    if(!string.IsNullOrEmpty(quest))
                        m_listActionQuest.Add(quest);
                }
            }
            string[] dialogs = actionDialog.Split('/');
            if (dialogs.Length > 0)
            {
                foreach (var dialog in dialogs)
                {
                    if(!string.IsNullOrEmpty(dialog))
                        m_listActionDialog.Add(dialog);
                }
            }
            m_eAction = (GameManager.eDialogAction)int.Parse(action);
            m_bReplay = replay == "1";
        }

    }
    
    #endregion

    #region ScriptData

    [Serializable]
    public class ScriptData
    {
        public string m_strSpeaker;
        public int m_iPortrait;
        public string m_strScript;

        public bool IsDokkabi
        {
            get
            {
                return m_strSpeaker == "dokkabi";
            }
        }

        public bool IsNPC
        {
            get
            {
                return m_strSpeaker.Contains("np_");
            }
        }

        public ScriptData(string speaker, string portrait, string script)
        {
            m_strSpeaker = speaker;
            m_iPortrait = int.Parse(portrait);
            m_strScript = script;
        }

        public string GetPortraitImgName()
        {
            return string.Format("{0}_{1:D2}", m_strSpeaker, m_iPortrait);
        }
    }

    

    #endregion
    
    #region QuestData
    
    public class QuestData
    {
        public string questID;
        public string state;
        
        public enum eState
        {
            UNAVAILABLE = 0,
            ACCOMPLISHING = 1,
            COMPLETED = 2,
            FINISHED = 3,

            
        }

        public QuestData(string questID,string state)
        {
            this.questID = questID;
            this.state = state;
        }


        public eState GetState()
        {
            return (eState)Enum.Parse(typeof(eState), state);
        }

    }

    #endregion
    
    #region userData
    public class UserData
    {
        public int user_uid = -1;
        public string user_id = "";
        public string user_pw = "";
        public string user_name = "";
        public string cur_map = "";
    
        public bool IS_BGM_ON = true;
        public bool IS_EFFECT_ON = true;
        public float SET_VOLUME = 1f;

        public float SET_CAM = 2.0f;
        public bool IS_SHOW_UI_BTN = true;

        public bool bInitDialog = false;

        public bool bUserInfoExist
        {
            get { return !string.IsNullOrEmpty(user_name); }
        }
    
        public void SetBgmOn(bool value)
        {

            IS_BGM_ON = value;
            PlayerPrefs.SetString(Global.KEY_BGM_SET_FLAG,IS_BGM_ON.ToString());
            PlayerPrefs.Save();
        
        }
    
        public void SetEffectOn(bool value)
        {

            IS_EFFECT_ON = value;
            PlayerPrefs.SetString(Global.KEY_EFFECT_SET_FLAG,IS_EFFECT_ON.ToString());
            PlayerPrefs.Save();
        
        }
    
        public void SetVolume(float value)
        {

            SET_VOLUME = value;
            PlayerPrefs.SetFloat(Global.KEY_VOLUME_SET_VALUE,SET_VOLUME);
            PlayerPrefs.Save();
        
        }

        public void SetCamSpeed(float value)
        {
            SET_CAM = value;
            PlayerPrefs.SetFloat(Global.KEY_CAMERA_SPEED,SET_CAM);
            PlayerPrefs.Save();
        }
        
        public void SetUIBtnOn(bool value)
        {
            IS_SHOW_UI_BTN = value;
            PlayerPrefs.SetString(Global.KEY_UI_BTN_FLAG,IS_SHOW_UI_BTN.ToString());
            PlayerPrefs.Save();
        }
    }
    
    public static UserData myData;
    
    #endregion

    #region NoticeData

    public static bool IsServerMaintainance = false;
    public static string MaintainanceNoticeMsg = "";

    public static bool IsNoticeExist = false;
    public static string NoticeMsg = "";
    
    public static bool NeedDownloadDialog = false;

    public static bool bInitNoticeData = false;

    #endregion
    
    public static Dictionary<string,QuestData> QuestDatas;
    public static Dictionary<string, List<DialogData>> DialogDatas = new Dictionary<string, List<DialogData>>();
    public static Dictionary<string, List<ScriptData>> ScriptDatas = new Dictionary<string, List<ScriptData>>() ;

    #region InitDatas
    public static void InitNoticeData(string sheetData)
    {
        string[] rows = sheetData.Split('\n');
        for (int i = 0; i < rows.Length; i++)
        {
            string[] columns = rows[i].Split('\t');
            if (i == 0) //서버 점검 확인
            {
                IsServerMaintainance = columns[0] == "1";
                MaintainanceNoticeMsg = columns[1];
            }else if (i == 1) //공지사항 확인
            {
                IsNoticeExist = columns[0] == "1";
                NoticeMsg = columns[1];
            }else if (i == 2)
            {
                if (GameManager.Instance.saveData.ScriptVersion != columns[0])
                {
                    GameManager.Instance.saveData.ScriptVersion = columns[0];
                    NeedDownloadDialog = true;
                }

            }
        }

        bInitNoticeData = true;
    }
    
    public static void InitDialogData(string sheetData)
    {
        string[] rows = sheetData.Split('\n');
        foreach (var row in rows)
        {
            string[] columns = row.Split('\t');
            
            string id = columns[0];
            string npcId = columns[1];
            string playCondition = columns[2];
            string condition = columns[3];
            string actionQuest = columns[4];
            string actionDialog = columns[5];
            string action = columns[6];
            string replay = columns[7].Split('\r')[0];
            
            var data = new DialogData(id,npcId,playCondition,condition,actionQuest,actionDialog,action,replay);
            
            GameManager.Instance.saveData.AddDialog(data);

        }
    }

    public static void InitScriptData(string sheetData)
    {
        string[] rows = sheetData.Split('\n');
        foreach (var row in rows)
        {
            string[] columns = row.Split('\t');
            string DialogId = columns[0];
            string Speaker = columns[2];
            string Portrait = columns[3];
            string Script = columns[4].Split('\r')[0];

            var data = new ScriptData(Speaker, Portrait, Script);
            GameManager.Instance.saveData.AddScript(data,DialogId);

        }
    }

    public static void SetGameDialogData()
    {
        var list = GameManager.Instance.saveData.GetDialogList();

        foreach (var data in list)
        {
            string npcId = data.mObjectID;
            string DialogId = data.m_strDialogID;
            if (!DialogDatas.ContainsKey(data.mObjectID))
            {
                DialogDatas.Add(npcId,new List<GameData.DialogData>(){data});
            }else
                DialogDatas[npcId].Add(data);

            if (data.m_scriptList.Count > 0)
            {
                foreach (var script in data.m_scriptList)
                {
                    if (!ScriptDatas.ContainsKey(DialogId))
                    {
                        ScriptDatas.Add(DialogId, new List<GameData.ScriptData>() { script });
                    }
                    else
                    {
                        ScriptDatas[DialogId].Add(script);
                    }
                }
            }
        }
        
    }
    #endregion

    public static QuestData GetQuestData(string id)
    {
        if (QuestDatas.ContainsKey(id))
            return QuestDatas[id];
        return null;
    }
    
    public static List<DialogData> GetDialog(string npcId)
    {
        if (DialogDatas.ContainsKey(npcId))
            return DialogDatas[npcId];
        return null;
    }

    public static void SetDialogPlayed(string npcId,string id, bool played = true)
    {
        if (DialogDatas.ContainsKey(npcId))
        {
            foreach (var dialog in DialogDatas[npcId])
            {
                if (dialog.m_strDialogID == id)
                    dialog.m_bPlayed = played;
            }
        }
        
        foreach (var dialog in GameManager.Instance.saveData.GetDialogList())
        {
            if (dialog.m_strDialogID == id)
                dialog.m_bPlayed = played;
        }
        
    }
    
    public static List<ScriptData> GetScript(string dialogId)
    {
        if (ScriptDatas.ContainsKey(dialogId))
        {
            return ScriptDatas[dialogId];
        }

        return null;
    }

}




public class Global
{
    public const string KEY_USER_ID = "USER_ID";
    public const string KEY_USER_PW = "USER_PW";
    public const string KEY_USER_NAME = "USER_NAME";
    public const string KEY_USER_UID = "USER_UID";
    public const string KEY_BGM_SET_FLAG = "BGM_SET";
    public const string KEY_EFFECT_SET_FLAG = "EFFECT_SET";
    public const string KEY_VOLUME_SET_VALUE = "VOLUME_SET";
    public const string KEY_CAMERA_SPEED = "CAMERA_SPEED";
    public const string KEY_UI_BTN_FLAG = "UI_BTN_SET";
    public const int SANYEAH_NOTE_POOLING_CNT = 15;
    public const float SANYEAH_NOTE_DROP_SPEED = 550;
    public const float SANYEAH_NOTE_JUDGE_PERFECT = 25;
    public const float SANYEAH_NOTE_JUDGE_GREAT = 50;
    public const float SANYEAH_NOTE_JUDGE_GOOD = 75;
    public const float SANYEAH_NOTE_JUDGE_BAD = 100;
    public const float SANYEAH_NOTE_JUDGE_MISS = 150;
    

    public static void InitSoundSet()
    {
        if (PlayerPrefs.HasKey(KEY_BGM_SET_FLAG))
            GameData.myData.IS_BGM_ON = Convert.ToBoolean(PlayerPrefs.GetString(KEY_BGM_SET_FLAG));
        if (PlayerPrefs.HasKey(KEY_EFFECT_SET_FLAG))
            GameData.myData.IS_EFFECT_ON = Convert.ToBoolean(PlayerPrefs.GetString(KEY_EFFECT_SET_FLAG));
        if (PlayerPrefs.HasKey(KEY_VOLUME_SET_VALUE))
            GameData.myData.SET_VOLUME = PlayerPrefs.GetFloat(KEY_VOLUME_SET_VALUE);
    }
    
    public static void InitUISet()
    {

        if (PlayerPrefs.HasKey(KEY_UI_BTN_FLAG))
            GameData.myData.IS_SHOW_UI_BTN = Convert.ToBoolean(PlayerPrefs.GetString(KEY_UI_BTN_FLAG));
        if (PlayerPrefs.HasKey(KEY_CAMERA_SPEED))
            GameData.myData.SET_CAM = PlayerPrefs.GetFloat(KEY_CAMERA_SPEED);
    }

    public static void InitUserData()
    {
        var data = new GameData.UserData();
        data.user_id = string.IsNullOrEmpty(GameManager.Instance.saveData.LoginID) ? "" : GameManager.Instance.saveData.LoginID;
        data.user_uid = GameManager.Instance.saveData.LoginUid;
        data.user_pw = string.IsNullOrEmpty(GameManager.Instance.saveData.LoginPW) ? "" : GameManager.Instance.saveData.LoginPW;
        data.IS_BGM_ON = GameData.myData.IS_BGM_ON;
        data.IS_EFFECT_ON = GameData.myData.IS_EFFECT_ON;
        data.SET_VOLUME = GameData.myData.SET_VOLUME;
        data.cur_map = GameData.myData.cur_map;
        data.user_name = GameData.myData.user_name;
        data.IS_SHOW_UI_BTN = GameData.myData.IS_SHOW_UI_BTN;
        data.SET_CAM = GameData.myData.SET_CAM;
        GameData.QuestDatas = new Dictionary<string, GameData.QuestData>();
        GameData.DialogDatas = new Dictionary<string, List<GameData.DialogData>>();
        GameData.ScriptDatas = new Dictionary<string, List<GameData.ScriptData>>();
        GameData.myData = data;
    }
    

    public static void DebugLogText(string msg, int error = 0)
    {
#if UNITY_EDITOR
        if(error < 0)
            Debug.LogError(msg);
        else
            Debug.Log(msg);
#endif
    }
}