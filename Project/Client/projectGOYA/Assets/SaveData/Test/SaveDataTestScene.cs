using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveDataTestScene : MonoBehaviour
{
    public SaveUserData saveData;
    // Start is called before the first frame update
    void Start()
    {
        /*
        WebReq.Instance.LoadDialogDataTest(delegate(string str, string str2)
        {
            InitDialogData(str,str2);
        });
        
        saveData.SaveData.id = PlayerPrefs.GetString(Global.KEY_USER_ID);
        saveData.SaveData.pw = PlayerPrefs.GetString(Global.KEY_USER_PW);
        saveData.SaveData.uid = PlayerPrefs.GetInt(Global.KEY_USER_UID);
        saveData.SaveData.nickname = PlayerPrefs.GetString(Global.KEY_USER_NAME);
*/

/*
        foreach (var kv in saveData.dicUserData)
        {
            Debug.Log(kv);
        }
        */

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void InitDialogData(string sheetData, string scriptData)
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
            
            var data = new GameData.DialogData(id,npcId,playCondition,condition,actionQuest,actionDialog,action,replay);
            saveData.SaveData.listDialogData.Add(data);
        }
        
        string[] rows2 = scriptData.Split('\n');
        foreach (var row in rows2)
        {
            string[] columns = row.Split('\t');
            string DialogId = columns[0];
            string Speaker = columns[2];
            string Portrait = columns[3];
            string Script = columns[4].Split('\r')[0];

            var data = new GameData.ScriptData(Speaker, Portrait, Script);
            foreach (var dialog in saveData.SaveData.listDialogData)
            {
                if (dialog.m_strDialogID == DialogId)
                {
                    dialog.m_scriptList.Add(data);
                }
            }

        }
    }
    
}
