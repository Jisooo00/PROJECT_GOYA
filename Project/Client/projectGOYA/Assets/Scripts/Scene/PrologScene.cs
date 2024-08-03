using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public class Prolog : BaseScene
{

    protected override void InitScene()
    {
        base.InitScene();
        m_eSceneType = GameData.eScene.PrologScene;
        SetUIManager();
        m_uiManager.SetPrologScene();
    }
    
    public override void Clear(Action del)
    {
        Debug.Log("PrologScene is clear.");
        del();
    }
    
    public override void DelFunc()
    {
        
    }
    
    public Image m_imgFade;
    public Button m_btnSkip;
    public Monster_np0003 m_npc;
    
    void Start()
    {
        m_imgFade.gameObject.SetActive(true);
        m_btnSkip.onClick.AddListener(delegate
        {
            PopupManager.Instance.OpenPopupNotice("튜토리얼을 스킵하시겠습니까?", delegate
            {
                StartCoroutine(RequestTutorialClear());
            },bNoBtn:true);
        });
        Player.instance.SetSleep(true);
        
        StartCoroutine(StartAfter());
        
    }
    IEnumerator StartAfter()
    {/*
        if(GameData.GetQuestData("Qu_0000").GetState() == GameData.QuestData.eState.UNAVAILABLE)
        {
            var req = new ReqQuestAccept();
            req.questId = "Qu_0000";
            WebReq.Instance.Request(req, delegate(ReqQuestAccept.Res res)
            {
            });
        }*/
        
        if (m_imgFade != null)
        {
            float alpha = 1;
            var c = m_imgFade.color;
            c.a = alpha;
            while (alpha > 0)
            {
                alpha -= Time.deltaTime;
                c.a = alpha;
                m_imgFade.color = c;
                yield return null;
            }
        }
        
        m_npc.MoveTo(new Vector2(1.2f,0f));
        while (m_npc.IS_MOVING)
        {
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);
        Player.instance.SetSleep(false);
        
    }

    IEnumerator SkipAfter()
    {
        m_npc.MoveTo(new Vector2(0f,3.5f));
        yield return new WaitForSeconds(0.5f);
        Player.instance.SetSleep(false);
    }

    IEnumerator RequestTutorialClear()
    {
        yield return null;
        bool bReqComplete = false;
        
        var req = new ReqQuestClear();
        req.questId = "Qu_0000";
        WebReq.Instance.Request(req, delegate(ReqQuestClear.Res res)
        {
            bReqComplete = true;
        });

        while (!bReqComplete)
        {
            yield return null;
        }
        
        bReqComplete = false;
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
        
        GameManager.Instance.Scene.LoadSceneByID(GameData.myData.cur_map);
        
    }
}