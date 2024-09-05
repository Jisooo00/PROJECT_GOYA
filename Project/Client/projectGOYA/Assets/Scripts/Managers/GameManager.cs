using Unity.VisualScripting;
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
        AUTO_PLAY = -1, // 다이얼로그 반복
        NONE = 0,
        QUEST_CLEAR = 1,
        QUEST_FINISH = 2,
        QUEST_ACCEPT = 3,
        PLAY_SANYEAH = 4,
        WAKE_SANYEAH_UP = 5,
        POPUP_NOTICE_DEMO = 6,

    }
    
    void Start()
    {
        Init();
    }

    public bool bClearSanyeah = false;
    public static bool isGuestMode = false;

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

    public static void DialogAction(GameData.DialogData data)
    {
        switch (data.m_eAction)
        {
            case eDialogAction.QUEST_FINISH :
                foreach (var quest in data.m_listActionQuest)
                {
                    var req = new ReqQuestClear();
                    req.questId = quest;
                    WebReq.Instance.Request(req,delegate(ReqQuestClear.Res res){});
                }
                break;

            case eDialogAction.QUEST_ACCEPT :
                foreach (var quest in data.m_listActionQuest)
                {
                    var req = new ReqQuestAccept();
                    req.questId = quest;
                    WebReq.Instance.Request(req,delegate(ReqQuestAccept.Res res){});
                }
                break;
            case eDialogAction.PLAY_SANYEAH:
                Instance.Scene.LoadScene(GameData.eScene.SanyeahGameScene);
                break;
            case eDialogAction.WAKE_SANYEAH_UP :
                if (Instance.Scene.currentScene.m_eSceneType == GameData.eScene.SanyeahScene)
                {
                    foreach (var quest in data.m_listActionQuest)
                    {
                        var req = new ReqQuestAccept();
                        req.questId = quest;
                        WebReq.Instance.Request(req,delegate(ReqQuestAccept.Res res){});
                    }
                    Instance.Scene.currentScene.DelFunc();
                } 
                break;
            case eDialogAction.POPUP_NOTICE_DEMO:
                PopupManager.Instance.OpenPopupNotice("[돗가비의 꿈] 첫 번째.\n퀘스트가 끝났습니다.\n\n다음 퀘스트 업데이트를 \n기대해주세요.\n\n플레이해주셔서 감사합니다.\n-팀 고야-");
                break;
            default:
                break;
        }
        
        if (data.m_listActionDialog.Count>0)
        {
            foreach (var dialog in data.m_listActionDialog)
            {
                //PlayerPrefs.SetString(string.Format("{0}_{1}", GameData.myData.user_name, dialog), "true");
                //PlayerPrefs.Save();
                GameData.SetDialogPlayed(data.mObjectID,dialog);
            }

        }
    }
    
}


