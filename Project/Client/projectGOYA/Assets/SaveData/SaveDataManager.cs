using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveDataManager : MonoBehaviour
{
    private static SaveDataManager s_instance;
    public static SaveDataManager Instance
    {
        get
        {
            return s_instance;
        }
    }
    
    [Serializable]
    public class UserData
    {
        public string id;
        public string pw;
        public int uid = -1;
        public string nickname;
        public string curMapID;
        public Vector3 curPos;
        public string dialogVersion;
        public List<GameData.DialogData> listDialogData = new List<GameData.DialogData> ();
    }
    private string fileName = "SaveUserData.json";
    private void Awake()
    {
        DontDestroyOnLoad(this);
        LoadUserData();
    }
    
    private void LoadUserData()
    {
        string str = File.ReadAllText(Application.persistentDataPath + "/SaveUserData.json");
        if (string.IsNullOrEmpty(str))
        {
            data = new UserData();
            return;
        }

        UserData _data = JsonUtility.FromJson<UserData>(str);
        data = _data;
    }
    
    public void SaveUserData()
    {
        string _data = JsonUtility.ToJson(data);
        File.WriteAllText(Application.persistentDataPath + fileName,_data); 
    }

    private UserData data;
    public UserData saveData
    {
        get { return data; }
    }

    public bool bLoginInfo
    {
        get { return !string.IsNullOrEmpty(data.id) && !string.IsNullOrEmpty(data.pw); }
    }

    public string LoginID
    {
        get { return data.id; }
        set { data.id = value; }
    }
    public string LoginPW
    {
        get { return data.pw; }
        set { data.pw = value; }
    }
    
    public int LoginUid
    {
        get { return data.uid; }
        set { data.uid = value; }
    }
    
    public string LoginNickname
    {
        get { return data.nickname; }
        set { data.nickname = value; }
    }
    public string ScriptVersion 
    { 
        get { return data.dialogVersion; } 
        set { data.dialogVersion = value; }
    }

    public Vector3 CurPos
    {
        get { return data.curPos; }
        set
        {
            data.curPos = value; 
            SaveUserData();
        }
    }
    public string CurMapID
    {
        get { return data.curMapID; }
        set
        {
            if (GameData.myData != null)
            {
                GameData.myData.cur_map = value;
            }
            data.curMapID = value;
        }
    } 


    public void AddDialog(GameData.DialogData _data)
    {
        data.listDialogData.Add(_data);
    }

    public void AddScript(GameData.ScriptData _data, string dialogID)
    {
        foreach (var dialog in data.listDialogData)
        {
            if (dialog.m_strDialogID == dialogID)
            {
                dialog.m_scriptList.Add(_data);
            }
        }
    }

    public List<GameData.DialogData> GetDialogList()
    {
        return data.listDialogData;
    }

    public GameData.DialogData GetDialogByID(string id)
    {
        foreach (var dialog in data.listDialogData)
        {
            if (dialog.m_strDialogID == id)
            {
                return dialog;
            }
        }

        return null;
    }


    public void InitializeData()
    {
        data.id = "";
        data.pw = "";
        data.uid = -1;
        data.nickname = "";
        data.curMapID = "";
        data.curPos = Vector3.zero;
        if (data.listDialogData.Count > 0)
        {
            foreach (var data in data.listDialogData)
            {
                data.m_bPlayed = false;
            }
        }

        SaveUserData();
    }

    public void InitDialog()
    {
        data.nickname = "";
        data.curMapID = "";
        data.curPos = Vector3.zero;
        if (data.listDialogData.Count > 0)
        {
            foreach (var data in data.listDialogData)
            {
                data.m_bPlayed = false;
            }
        }

        SaveUserData();
    }
    
}
