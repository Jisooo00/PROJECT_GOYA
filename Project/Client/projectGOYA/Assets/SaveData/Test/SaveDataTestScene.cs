using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveDataTestScene : MonoBehaviour
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
    
    public UserData data;
    
    private string fileName = "/SaveUserData.json";
    private void Awake()
    {
        LoadUserData();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SaveUserData();
        }
    }

    private void LoadUserData()
    {
        string str2 = File.ReadAllText(Application.persistentDataPath + fileName); 
        UserData _data = JsonUtility.FromJson<UserData>(str2);
        data = _data;
    }
    
    private void SaveUserData()
    {
        string _data = JsonUtility.ToJson(data);
        File.WriteAllText(Application.persistentDataPath + fileName,_data); 
    }


}
