using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager s_instance;
    public static GameManager Instance
    {
        get
        {
            Init();
            return s_instance;
        }
    }

    private SceneMgr sceneMgr = new SceneMgr();
    public SceneMgr Scene { get { return Instance.sceneMgr; } }
    
    public enum eDialogAction
    {
        SAVE_EXCEPTION = -2, // 다이얼로그 반복
        NONE = -1,
        QUEST_CLEAR = 0,
        QUEST_ACCEPT = 1,
        PLAY_SANYEAH = 2,
        SAVE_DIALOG_01 = 3,
        WAKE_SANYEAH_UP = 4,

    }
    
    void Start()
    {
        Init();
    }

    public bool bClearSanyeah = false;

    static void Init()
    {
        if(s_instance == null)
        {
            GameObject obj = GameObject.Find("GameManager");
            if (obj == null)
            {
                obj = new GameObject { name = "@GameManager" };
                obj.AddComponent<GameManager>();
            }
            
            DontDestroyOnLoad(obj);
            s_instance = obj.GetComponent<GameManager>();
        }
    }

    public static void DialogAction(string questId, eDialogAction eAction)
    {
        switch (eAction)
        {
            case eDialogAction.QUEST_CLEAR :
                var req0 = new ReqQuestClear();
                req0.questId = questId;
                if (questId == "Qu_0002")
                {
                    PlayerPrefs.SetString(string.Format("{0}_{1}", GameData.myData.user_uid, "Dl_0006"), "true");
                    PlayerPrefs.Save();
                    GameData.SetDialogPlayed("np_0002","Dl_0006");
                }
                WebReq.Instance.Request(req0,delegate(ReqQuestClear.Res res){});
                break;
            case eDialogAction.QUEST_ACCEPT :
                var req1 = new ReqQuestAccept();
                req1.questId = questId;
                WebReq.Instance.Request(req1, delegate(ReqQuestAccept.Res res) { });
                if(questId == "Qu_0002" && Instance.Scene.currentScene.m_eSceneType == GameData.eScene.MainScene)
                    Instance.Scene.currentScene.DelFunc();
                break;
            case eDialogAction.PLAY_SANYEAH:
                if (PlayerPrefs.HasKey(string.Format("{0}_{1}", GameData.myData.user_uid, "Dl_0006")))
                {
                    PlayerPrefs.DeleteKey(string.Format("{0}_{1}", GameData.myData.user_uid, "Dl_0006"));
                    GameData.SetDialogPlayed("np_0002","Dl_0006",false);
                }
                Instance.Scene.LoadScene(GameData.eScene.SanyeahGameScene);
            
                break;
            case eDialogAction.SAVE_DIALOG_01 :
                PlayerPrefs.SetString(string.Format("{0}_{1}", GameData.myData.user_uid, "Dl_0003"), "true");
                PlayerPrefs.Save();
                GameData.SetDialogPlayed("np_0001","Dl_0003");
                Instance.Scene.LoadScene(GameData.eScene.MainScene);
                break;
            case eDialogAction.WAKE_SANYEAH_UP :
                if (Instance.Scene.currentScene.m_eSceneType == GameData.eScene.SanyeahScene)
                {
                    var req2 = new ReqQuestAccept();
                    req2.questId = questId;
                    WebReq.Instance.Request(req2,delegate(ReqQuestAccept.Res res){});
                    Instance.Scene.currentScene.DelFunc();
                } 
                break;
            default:
                break;
        }
    }
    
}


