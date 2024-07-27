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
        //m_uiManager.Init(m_eSceneType);
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
        StartCoroutine(StartAfter());
    }
    IEnumerator StartAfter()
    {
        if(GameData.GetQuestData("Qu_0000").GetState() == GameData.QuestData.eState.UNAVAILABLE)
        {
            var req = new ReqQuestAccept();
            req.questId = "Qu_0000";
            WebReq.Instance.Request(req, delegate(ReqQuestAccept.Res res)
            {
            });
        }
        
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