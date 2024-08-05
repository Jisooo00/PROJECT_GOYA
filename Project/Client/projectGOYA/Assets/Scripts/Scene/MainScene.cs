
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class MainScene : BaseScene
{
    public GameObject SceneMoveBox;
    public GameObject m_uiLoading;
    public Transform m_tmPosFromCave;
    public Transform m_tmPosFromSanyeah;
    protected override void InitScene()
    {
        base.InitScene();
        m_eSceneType = GameData.eScene.MainScene;
        WebReq.Instance.Request(new ReqMapEnter(), delegate(ReqMapEnter.Res res) { });
        SetUIManager();
        AudioManager.Instance.StopBgm();
        AudioManager.Instance.PlayBgm();
        //Player.instance.PlayEffect("Chara_APPEAR");
    }

    public void Start()
    {
        StartCoroutine(StartAfter());
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
        
        if (GameManager.Instance.Scene.prevScene == GameData.eScene.PrologScene)
        {
            Player.instance.transform.localPosition = m_tmPosFromCave.localPosition;
            moveFirst = Vector2.up;
        }
        
        if (GameManager.Instance.Scene.prevScene == GameData.eScene.SanyeahScene)
        {
            Player.instance.transform.localPosition = m_tmPosFromSanyeah.localPosition;
            moveFirst = Vector2.right;
        }
        yield return new WaitForSeconds(0.5f);
        m_uiLoading.gameObject.SetActive(false);

        if (moveFirst != Vector2.zero)
        {
            Player.instance.SetInputPos(moveFirst);
            yield return new WaitForSeconds(0.5f);
            Player.instance.SetInputPos(new Vector2(0,0f));
        }

        
        if (GameData.QuestDatas.ContainsKey("Qu_0001") && GameData.QuestDatas["Qu_0001"].GetState() ==
            GameData.QuestData.eState.UNAVAILABLE)
        {
            var req = new ReqQuestAccept();
            req.questId = "Qu_0001";
            WebReq.Instance.Request(req, delegate(ReqQuestAccept.Res res)
            {
            });
        }
        
    }
    
    public override void DelFunc()
    {

    }
}
