using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SaveUserData", menuName = "NewSaveUserData")]
public class SaveUserData : ScriptableObject
{
    
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
    public UserData SaveData;


    public void Reset()
    {
        SaveData.id = "";
        SaveData.pw = "";
        SaveData.uid = -1;
        SaveData.nickname = "";
        SaveData.curMapID = "";
        SaveData.curPos = Vector3.zero;
        if (SaveData.listDialogData.Count > 0)
        {
            foreach (var data in SaveData.listDialogData)
            {
                data.m_bPlayed = false;
            }
        }
    }
    
    public void ResetDialog()
    {
        SaveData.nickname = "";
        SaveData.curMapID = "";
        SaveData.curPos = Vector3.zero;
        if (SaveData.listDialogData.Count > 0)
        {
            foreach (var data in SaveData.listDialogData)
            {
                data.m_bPlayed = false;
            }
        }
    }

}
