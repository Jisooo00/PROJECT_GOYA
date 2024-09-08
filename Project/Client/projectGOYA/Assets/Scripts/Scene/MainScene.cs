
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class MainScene : BaseScene
{
    //public GameObject SceneMoveBox;
    public GameObject m_uiLoading;
    public Transform m_tmPosFromCave;
    public Transform m_tmPosFromSanyeah;
    public Monster_np0003 m_npcFox;
    protected override void InitScene()
    {
        base.InitScene();
        m_eSceneType = GameData.eScene.MainScene;
        WebReq.Instance.Request(new ReqMapEnter(), delegate(ReqMapEnter.Res res)
        {
        });
        SetUIManager();
        AudioManager.Instance.StopBgm();
        AudioManager.Instance.PlayBgm();
        //Player.instance.PlayEffect("Chara_APPEAR");
    }

    public void Start()
    {
        if (Player.instance.bIsOnGoingTutorial)
        {
            StartCoroutine(StartTutorial());
            m_uiManager.SetPrologScene();
        }
        else
        {
            //m_npcFox.gameObject.SetActive(false);
            StartCoroutine(StartAfter());
        }
        
    }

    public override void Clear(Action del)
    {
        //Player.instance.PlayEffect("Chara_DISAPPEAR");
        if (del != null)
        {
            StartCoroutine(ClearSceneAfter(del));
        }
        Debug.Log("MainScene is clear.");

    }
        
    IEnumerator ClearSceneAfter(Action del)
    {
        yield return new WaitForSeconds(0.5f);
        del();
    }

    IEnumerator StartAfter()
    {
        m_uiLoading.gameObject.SetActive(true);

        var moveFirst = new Vector2(0, 0);



        if (GameManager.Instance.Scene.prevScene == GameData.eScene.SanyeahScene)
        {
            Player.instance.transform.localPosition = m_tmPosFromSanyeah.localPosition;
            moveFirst = Vector2.right;
        }
        else
        {
            Player.instance.transform.localPosition = m_tmPosFromCave.localPosition;
            moveFirst = Vector2.up;
        }
        
        m_npcFox.transform.localPosition = new Vector2(-2.0f,-11.0f);
        m_npcFox.MoveTo(new Vector2(-2.0f,-11.1f));
        
        yield return new WaitForSeconds(0.25f);
        m_uiLoading.gameObject.SetActive(false);

        if (moveFirst != Vector2.zero)
        {
            Player.instance.SetInputPos(moveFirst);
            yield return new WaitForSeconds(0.5f);
            Player.instance.SetInputPos(new Vector2(0,0f));
        }

        
        /*if (GameData.QuestDatas.ContainsKey("Qu_0001") && GameData.QuestDatas["Qu_0001"].GetState() ==
            GameData.QuestData.eState.UNAVAILABLE)
        {
            var req = new ReqQuestAccept();
            req.questId = "Qu_0001";
            WebReq.Instance.Request(req, delegate(ReqQuestAccept.Res res)
            {
            });
        }*/
        
    }
    
    IEnumerator StartTutorial()
    {
        m_uiLoading.gameObject.SetActive(true);
        Player.instance.transform.localPosition = m_tmPosFromCave.localPosition;
        Player.instance.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        m_npcFox.gameObject.SetActive(false);
        m_uiLoading.gameObject.SetActive(false);
        
        m_npcFox.gameObject.SetActive(true);
        m_npcFox.MoveTo(new Vector2(-2.0f,-11f));
        while (m_npcFox.IS_MOVING)
        {
            yield return null;
        }
        m_npcFox.MoveTo(new Vector2(-2.0f,-11.1f));
        Player.instance.gameObject.SetActive(true);
        Player.instance.SetInputPos(Vector2.up);
        yield return new WaitForSeconds(0.75f);
        Player.instance.SetInputPos(new Vector2(0,0f));
        yield return new WaitForSeconds(0.1f);
        
        bool bTutorialStep = false;
        m_uiManager.PlayDialogForce("Dl_0000_05", delegate
        {
            bTutorialStep = true;
            m_npcFox.SetQuestionMark();
            m_uiManager.SetPrologScene();
        });
        while (!bTutorialStep)
            yield return null;

        yield return null;
        /*bool bReqComplete = false;
        
        if (GameData.QuestDatas.ContainsKey("Qu_0001") && GameData.QuestDatas["Qu_0001"].GetState() ==
            GameData.QuestData.eState.UNAVAILABLE)
        {
            var _req = new ReqQuestAccept();
            _req.questId = "Qu_0001";
            WebReq.Instance.Request(_req, delegate(ReqQuestAccept.Res res)
            {
                bReqComplete = true;
            });
        }
        
        while (!bReqComplete)
        {
            yield return null;
        }
         */       
        m_uiManager.Init();
        
    }
    
    public override void DelFunc()
    {

    }
}
