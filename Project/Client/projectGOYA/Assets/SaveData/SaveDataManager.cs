using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveDataManager : MonoBehaviour
{
    [SerializeField] private SaveUserData data;

    public bool bLoginInfo
    {
        get { return !string.IsNullOrEmpty(data.SaveData.id) && !string.IsNullOrEmpty(data.SaveData.pw); }
    }

    public string LoginID
    {
        get { return data.SaveData.id; }
        set { data.SaveData.id = value; }
    }
    public string LoginPW
    {
        get { return data.SaveData.pw; }
        set { data.SaveData.pw = value; }
    }
    
    public int LoginUid
    {
        get { return data.SaveData.uid; }
        set { data.SaveData.uid = value; }
    }
    
    public string LoginNickname
    {
        get { return data.SaveData.nickname; }
        set { data.SaveData.nickname = value; }
    }
    public string ScriptVersion 
    { 
        get { return data.SaveData.dialogVersion; } 
        set { data.SaveData.dialogVersion = value; }
    }

    public void AddDialog(GameData.DialogData _data)
    {
        data.SaveData.listDialogData.Add(_data);
    }

    public void AddScript(GameData.ScriptData _data, string dialogID)
    {
        foreach (var dialog in data.SaveData.listDialogData)
        {
            if (dialog.m_strDialogID == dialogID)
            {
                dialog.m_scriptList.Add(_data);
            }
        }
    }

    public List<GameData.DialogData> GetDialogList()
    {
        return data.SaveData.listDialogData;
    }

    public GameData.DialogData GetDialogByID(string id)
    {
        foreach (var dialog in data.SaveData.listDialogData)
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
        data.Reset();
    }

    public void InitDialog()
    {
        data.ResetDialog();
    }
}
