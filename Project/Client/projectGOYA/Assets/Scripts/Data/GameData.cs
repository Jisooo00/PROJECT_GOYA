
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
        LoadingScene,
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

    public class DialogData
    {
        public enum eType
        {
            IN_ORDER = 1,
            RANDOM = 2,
        }
       
        public string m_strDialogID;
        public eType m_eType;
        public string mObjectID;
        public QuestData.eState m_eCondition;
        public string m_strQuestID;
        public bool m_bPlayed = false;
        public GameManager.eDialogAction m_eAction;
        
        public int mIndex;

        public DialogData(string ID,string type, string npcId, string condition,string questId,string action)
        {
            m_strDialogID = ID;
            m_eType = (eType) int.Parse(type);
            mObjectID = npcId;
            m_eCondition = (QuestData.eState) int.Parse(condition);
            m_strQuestID = questId;
            m_eAction = (GameManager.eDialogAction)int.Parse(action);
        }

    }
    
    #endregion

    #region ScriptData

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

        public bool bInitDialog = false;
    
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
    }
    
    public static UserData myData;
    
    #endregion
    
    
    public static Dictionary<string,QuestData> QuestDatas;
    public static Dictionary<string, List<DialogData>> DialogDatas;
    public static Dictionary<string, List<ScriptData>> ScriptDatas;

    #region InitDatas
    public static void InitDialogData(string sheetData)
    {
        string[] rows = sheetData.Split('\n');
        foreach (var row in rows)
        {
            string[] columns = row.Split('\t');
            string id = columns[0];
            string type= columns[1];
            string npcId= columns[2];
            string condition= columns[3];
            string questId= columns[4];
            string action= columns[5].Split('\r')[0];
            
            var data = new GameData.DialogData(id,type,npcId,condition,questId,action);
            if (PlayerPrefs.HasKey(string.Format("{0}_{1}", myData.user_uid, id)))
            {
                data.m_bPlayed = true;
            }

            if (!DialogDatas.ContainsKey(npcId))
            {
                DialogDatas.Add(npcId,new List<GameData.DialogData>(){data});
            }else
                DialogDatas[npcId].Add(data);
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

            var data = new GameData.ScriptData(Speaker, Portrait, Script);
            if (!ScriptDatas.ContainsKey(DialogId))
            {
                ScriptDatas.Add(DialogId, new List<GameData.ScriptData>() { data });
            }
            else
            {
                ScriptDatas[DialogId].Add(data);
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

    public static void InitUserData()
    {
        var data = new GameData.UserData();
        data.user_id = PlayerPrefs.HasKey(KEY_USER_ID) ? PlayerPrefs.GetString(KEY_USER_ID):"";
        data.user_uid = PlayerPrefs.HasKey(KEY_USER_UID) ? PlayerPrefs.GetInt(KEY_USER_UID):2;
        data.user_pw = PlayerPrefs.HasKey(KEY_USER_PW) ? PlayerPrefs.GetString(KEY_USER_PW):"";
        data.IS_BGM_ON = GameData.myData.IS_BGM_ON;
        data.IS_EFFECT_ON = GameData.myData.IS_EFFECT_ON;
        data.SET_VOLUME = GameData.myData.SET_VOLUME;
        data.cur_map = GameData.myData.cur_map;
        data.user_name = GameData.myData.user_name;
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