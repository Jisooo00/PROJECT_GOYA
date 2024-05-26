
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
        public string[] mListDialog;
        public bool bIsPlayed;
        public int mMoveScene;
        public int mIndex;

        public DialogData(string ID,int index, string[] list, bool b, int scene = -1)
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