using System;
using System.Collections.Generic;

using UnityEngine;

[CreateAssetMenu(fileName = "QuestInfoData", menuName = "New Note QuestInfoData")]
public class QuestInfoData : ScriptableObject
{
    [Serializable]
    public class info
    {
        public string questID;
        public string startDialog;
        public string endDialog;
        public string reward;
    }
    public List<info> questInfoList;
}