using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public class PrologScene : BaseScene
{
    protected override void InitScene()
    {
        base.InitScene();
        m_eSceneType = GameData.eScene.PrologScene;
        SetUIManager(delegate { m_uiManager.SetPrologScene();});
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
    
    public GameObject m_gobTuto;
    public GameObject m_gobTutoWeb;
    public GameObject m_gobTutoBase;
    private bool bTutorial1 = false;
    
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
        m_npc.gameObject.SetActive(false);
        m_gobTuto.SetActive(false);
        StartCoroutine(StartAfter());

    }
    
    private void Update()
    {
        if (bTutorial1)
        {
            if (Input.anyKey || Input.touchCount > 0)
            {
                m_gobTuto.SetActive(false);
                bTutorial1 = false;
            }
        }
    }

    IEnumerator StartAfter()
    {
        /*if(GameData.GetQuestData("Qu_0000").GetState() == GameData.QuestData.eState.UNAVAILABLE)
        {
            var req = new ReqQuestAccept();
            req.questId = "Qu_0000";
            WebReq.Instance.Request(req, delegate(ReqQuestAccept.Res res)
            {
            });
        }*/
        
        Player.instance.bIgnoreInput = true;
        
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
        AudioManager.Instance.PlayBgm();
        yield return new WaitForSeconds(0.5f);

        bool bTutorialStep = false;
        m_uiManager.PlayDialogForce("Dl_0000_01", delegate
        {
            bTutorialStep = true;
            m_uiManager.SetPrologScene();
        });
        while (!bTutorialStep)
            yield return null;
            
        m_npc.gameObject.SetActive(true);
        m_npc.MoveTo(new Vector2(1f,0.25f));
        while (m_npc.IS_MOVING)
        {
            yield return null;
        }
        m_npc.rb.isKinematic = true;
        
        bTutorialStep = false;
        m_uiManager.PlayDialogForce("Dl_0000_02", delegate
        {
            bTutorialStep = true;
            m_uiManager.SetPrologScene();
        });
        while (!bTutorialStep)
            yield return null;
        
        Player.instance.SetSleep(false);
        yield return new WaitForSeconds(0.25f);
        
        bTutorialStep = false;
        m_uiManager.PlayDialogForce("Dl_0000_03", delegate
        {
            bTutorialStep = true;
            m_uiManager.SetPrologScene();
        });
        while (!bTutorialStep)
            yield return null;
        
        m_npc.MoveTo(new Vector2(0f,1f));
        while (m_npc.IS_MOVING)
        {
            yield return null;
        }
        m_npc.MoveTo(new Vector2(0f,0.99f));
        
        bTutorial1 = true;
        m_gobTuto.SetActive(true);
        
        #if !UNITY_WEBGL
        m_gobTutoWeb.SetActive(false);
        var pos = m_gobTutoBase.transform.localPosition;
        m_gobTutoBase.transform.localPosition = new Vector3(pos.x, pos.y - 125f);
        #endif
        
        while (bTutorial1)
        {
            yield return null;
        }
        Player.instance.bIgnoreInput = false;
        
        m_uiManager.SetDelTuto(delegate
        {
            m_uiManager.PlayDialogForce("Dl_0000_04", delegate
            {
                bTutorialStep = true;
                m_uiManager.SetPrologScene();
                m_btnSkip.gameObject.SetActive(false);
            });
        });
        m_uiManager.SetTutorialAction();
               
        bTutorialStep = false;

        while (!bTutorialStep)
            yield return null;
        
        m_npc.MoveTo(new Vector2(0f,3.5f));
        while (m_npc.IS_MOVING)
        {
            yield return null;
        }
        m_npc.gameObject.SetActive(false);
        Player.instance.ForceMoveTo(new Vector2(0f,3.5f));
        yield return new WaitForSeconds(0.75f);
        GameManager.Instance.Scene.LoadSceneByID(GameData.myData.cur_map);
        
    }


    IEnumerator RequestTutorialClear()
    {
        yield return null;
        GameData.SetDialogPlayed("np_0003","Dl_0000_01");
        GameData.SetDialogPlayed("np_0003","Dl_0000_02");
        GameData.SetDialogPlayed("np_0003","Dl_0000_03");
        GameData.SetDialogPlayed("np_0003","Dl_0000_04");
        GameData.SetDialogPlayed("np_0003","Dl_0000_05");
        
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
        /*
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
        }*/
        GameManager.Instance.Scene.LoadSceneByID(GameData.myData.cur_map);
        
    }
}